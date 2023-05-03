using BlazingBooks.Data;
using BlazingBooks.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddHttpClient();

builder.Services.AddDbContext<AppDBContext>(options =>
              options.UseSqlServer(
                  builder.Configuration.GetConnectionString("AppDBContext")).EnableSensitiveDataLogging());


//builder.Services.AddDefaultIdentity<IdentityUser>()
//    .AddEntityFrameworkStores<AppDBContext>()
//    .AddRoles<IdentityRole>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<OrderState>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthStateProvider>();

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

builder.Services.AddScoped(sp =>
{
    var dbContext = sp.CreateScope().ServiceProvider.GetRequiredService<AppDBContext>();
    return new GenreService(dbContext);
});

builder.Services.AddScoped(sp =>
{
    var dbContext = sp.CreateScope().ServiceProvider.GetRequiredService<AppDBContext>();
    return new AuthorService(dbContext);
});


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["JwtIssuer"],   
                ValidAudience = builder.Configuration["JwtAudience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSecurityKey"]))
            };
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

app.UseStaticFiles();
app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.MapControllers();
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

// Initialize the database
var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using (var scope = scopeFactory.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDBContext>();
}


app.Run();

