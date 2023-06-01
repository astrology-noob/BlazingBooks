# 🤷 Система авторизации

Я адаптировала систему авторизации из урока по реализации системы авторизации в Blazor, состоящего из [4 частей](https://chrissainty.com/securing-your-blazor-apps-introduction-to-authentication-with-blazor/), авторства [Chris Sainty](https://chrissainty.com/).

Упрощённо инфраструктура выглядит таким образом:

<figure><img src="../.gitbook/assets/image (20).png" alt=""><figcaption></figcaption></figure>

* **AuthController** добавляет пользователя и шифрует пароль при регистрации, создаёт JWT-токен при авторизации
* **AuthService** работает с токеном: сохраняет или удаляет его из localStorage
* **ApiAuthStateProvider -** каскадный компонент, который позволяет получать доступ к токену в localStorage. Этот компонент наследуется от AuthenticationStateProvider, который используется Blazor-ом для того, чтобы работала авторизация по ролям.

Рассмотрим поэтапно, как работает авторизация:

### Шаг 1

В компоненте с формой авторизации при нажатии на кнопку "Login" мы отправляем запрос в AuthenticationService:

```cshtml
@page "/loginpage"
// ...
@inject AuthService AS

// ...
<form @onsubmit="LoginUser">
	<div class="form-group">
		<input type="text" class="form-control" placeholder="user" @bind-value="username"/>
		<input type="text" class="form-control" placeholder="pass" @bind-value="password" />
		<input type="submit" class="btn btn-primary" value="Login" />
	</div>
</form>
//...

@code {
	public string username;
	public string password;

	public async Task LoginUser()
	{
		UserData userData = new(username, password);
		
		// вызываем метод Login и передаём полученные имя пользователя и пароль
		var result = await AS.Login(userData);
	
		if (result.Successful)
		{
			NavigationManager.NavigateTo("/");
		}
	}
}
```

### Шаг 2

В методе Login полученные данные мы преобразуем в JSON и отправляем POST-запрос на адрес, по которому доступен метод Login AuthController-а.&#x20;

Результат, который возвращает AuthController, преобразуем в объект LoginResult и если статус код "успешный", то полученный в LoginResult токен записывается в localStorage и в ApiAuthStateProvider, а также добавляется в http-заголовок.

{% code title="AuthService.cs" %}
```csharp
// ...
public async Task<LoginResult> Login(UserData userData)
{
    var loginAsJson = JsonSerializer.Serialize(userData);
    var response = await _httpClient.PostAsync("http://localhost:20845/api/Auth/login", new StringContent(loginAsJson, Encoding.UTF8, "application/json"));
    var loginResult = JsonSerializer.Deserialize<LoginResult>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

    if (!response.IsSuccessStatusCode)
    {
        return loginResult;
    }

    await _localStorage.SetItemAsync("authToken", loginResult.Token);
    ((ApiAuthStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(loginResult.Token);
    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", loginResult.Token);

    return loginResult;
}
// ...
```
{% endcode %}

{% code title="AuthController.cs" %}
```csharp
// ...
[HttpPost("login")]
public ActionResult<LoginResult> Login([FromBody] UserData userData)
{
    user = _context.Users.Include(u => u.Roles)
            .FirstOrDefault(u => u.Username == userData.Username);

    UserSameName userSameName = new();
    var userExists = _context.Users.ToList().Contains(user, userSameName);

    if (!userExists)
    {
        return BadRequest(new LoginResult { 
            Successful = false, 
            Error = "User not found" 
        });
    }

    if (!BCrypt.Net.BCrypt.Verify(userData.Password, user.PasswordHash))
    {
        return BadRequest(new LoginResult { 
            Successful = false, 
            Error = "Wrong password" 
        });
    }

    string token = CreateToken(user);

    return Ok(new LoginResult { Successful = true, Token = token });
}

private string CreateToken(User user)
{

    var roles = user.Roles;

    var claims = new List<Claim>()
    {
        new Claim(ClaimTypes.Name, user.Username)
    };

    foreach (var role in roles)
    {
        claims.Add(new Claim(ClaimTypes.Role, role.Name));
    }

    var expiry = DateTime.Now.AddDays(Convert.ToInt32(_configuration["JwtExpiryInDays"]));
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
        _configuration.GetSection("AppSettings:Token").Value!));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

    var token = new JwtSecurityToken(
        _configuration["JwtIssuer"],
        _configuration["JwtAudience"],
        claims,
        expires: expiry,
        signingCredentials: creds
    );

    var jwt = new JwtSecurityTokenHandler().WriteToken(token);

    return jwt;
}
// ...
```
{% endcode %}

{% code title="ApiAuth" %}
```csharp
// ...
public void MarkUserAsAuthenticated(string token)
{
    var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"));
    var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
    NotifyAuthenticationStateChanged(authState);
}
// ...
private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
{
    var claims = new List<Claim>();
    var payload = jwt.Split('.')[1];
    var jsonBytes = ParseBase64WithoutPadding(payload);
    var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

    keyValuePairs.TryGetValue(ClaimTypes.Role, out object roles);

    if (roles != null)
    {
        if (roles.ToString().Trim().StartsWith("["))
        {
            var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString());

            foreach (var parsedRole in parsedRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, parsedRole));
            }
        }
        else
        {
            claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
        }

        keyValuePairs.Remove(ClaimTypes.Role);
    }

    claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));

    return claims;
}
// ...
```
{% endcode %}

### Шаг 3

Для того, чтобы авторизация работала, нужно добавить каскадные компоненты CascadingAuthenticationState и AutorizeRouteView в App.razor:

```cshtml
<CascadingAuthenticationState>
    <Router AppAssembly="@typeof(Program).Assembly" PreferExactMatches="@true">
        <Found Context="routeData">
            <AuthorizeRouteView RouteData="@routeData" DefaultLayout="typeof(MainLayout)"/>
        </Found>
        <NotFound>
            <LayoutView>
                <p>Sorry, there's nothing at this address.</p>
            </LayoutView>
        </NotFound>
    </Router>
</CascadingAuthenticationState>
```

### Шаг 4

Применить свойства авторизации можно 2 способами:

* добавить AuthorizeView в Razor-компонент;
* использовать атрибут Authorize в начале описания Razor-компонента

Рассмотрим первый способ на примере компонента TopBar:

```cshtml
@*...*@
<div class="top-bar">
    <NavLink href=@HomePageURI Match="NavLinkMatch.All" class="logo">
        <img src="img/logo.svg" />
    </NavLink>
    <NavLink href="/genres" Match="NavLinkMatch.Prefix" class="nav-tab">
        <div>Genres</div>
    </NavLink>
    <NavLink href="/authors" Match="NavLinkMatch.Prefix" class="nav-tab">
        <div>Authors</div>
    </NavLink>

    @*Если пользователь авторизован, он увидит кнопку Logout*@
    <AuthorizeView>
        <Authorized>
            <button class="nav-tab" @onclick="Logout">
                Logout
            </button>
            <UserProfileLink/>
        </Authorized>
    @*Иначе, будет создана кнопка Login*@
        <NotAuthorized>
            <NavLink href="/loginpage" Match="NavLinkMatch.Prefix" class="nav-tab">
                <div>Login</div>
            </NavLink>
        </NotAuthorized>
    </AuthorizeView>

    @*Если пользователь является администратором, ему доступна кнопка Admin Panel*@
    <AuthorizeView Roles="Admin">
        <NavLink href="/admin" Match="NavLinkMatch.Prefix" class="nav-tab">
            <div>Admin Panel</div>
        </NavLink>
    </AuthorizeView>
</div>

@*...*@
```

Очевидно, что несмотря на то, что у пользователя нет кнопки с ссылкой на Панель Администратора, это не значит, что он не может теоретически ввести прямой URL на эту страницу.&#x20;

Мы не хотим, чтобы пользователи, которые не являются администраторами, получали доступ к этой странице, поэтому мы используем 2 способ для ограничения доступа пользователя к целому компоненту (странице).

{% code title="AdminPanel.razor" %}
```cshtml
@page "/admin"
@attribute [Authorize(Roles = "Admin")]
@*...*@
```
{% endcode %}
