using BlazingBooks.Data;
using BlazingBooks.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using ConfigurationManager = System.Configuration.ConfigurationManager;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient();
builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddDbContext<AppDBContext>(options =>
              options.UseSqlServer(
                  builder.Configuration.GetConnectionString("AppDBContext")));
builder.Services.AddScoped<OrderState>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped(sp =>
{
    var dbContext = sp.CreateScope().ServiceProvider.GetRequiredService<AppDBContext>();
    return new BookService(dbContext);
});

builder.Services.AddScoped(sp =>
{
    var dbContext = sp.CreateScope().ServiceProvider.GetRequiredService<AppDBContext>();
    return new UserService(dbContext);
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = true,
        ValidAudience = "domain.com",
        ValidateIssuer = true,
        ValidIssuer = "domain.com",
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("THIS IS THE SECRET KEY"))
    };
});

builder.Services.AddAuthorization(options => {
    var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme);
    defaultAuthorizationPolicyBuilder = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
    options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();
app.UseHttpsRedirection();

app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.MapControllers();
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
//app.Map("/data", [Authorize] (HttpContext context) => $"Hello World!");

// сайт с полезным кодом в конце, jwt по сути просто сохраняется в localstorage
//https://www.prowaretech.com/articles/current/blazor/wasm/jwt-authentication-simple#!
//https://www.programmersought.com/article/49389322410/

// Initialize the database
var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using (var scope = scopeFactory.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDBContext>();
}


app.Run();

