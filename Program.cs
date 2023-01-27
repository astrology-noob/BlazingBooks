using BlazingBooks.Data;
using BlazingBooks.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using ConfigurationManager = System.Configuration.ConfigurationManager;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient();
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

