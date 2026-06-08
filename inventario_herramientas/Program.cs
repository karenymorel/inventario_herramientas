using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using inventario_herramientas.Managers.Managers;
using inventario_herramientas.Managers.Repos;
using Microsoft.Extensions.Configuration;
using inventario_herramientas.Web.Helpers;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Google Auth
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
}).AddCookie()
.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    options.ClientId = builder.Configuration.GetSection("GoogleKeys:ClientId").Value;
    options.ClientSecret = builder.Configuration.GetSection("GoogleKeys:ClientSecret").Value;
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});


builder.Services.AddControllersWithViews();

// Aca se agregan los managers, repos = [inyeccion de dependencias]
builder.Services.AddScoped<IHerramientasManager, HerramientasManager>();
builder.Services.AddScoped<IHerramientasRepositorio>(_ =>
{
    var connectionString = builder.Configuration["Db:ConnectionString"];
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException("Database connection string 'Db:ConnectionString' is not configured.");
    }
    return new HerramientasRepositorio(connectionString);
});

builder.Services.AddScoped<IUsuariosManager, UsuariosManager>();
builder.Services.AddScoped<IUsuariosRepositorio>(_ =>
{
    var connectionString = builder.Configuration["Db:ConnectionString"];
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException("Database connection string 'Db:ConnectionString' is not configured.");
    }
    return new UsuariosRepositorio(connectionString);
});
builder.Services.AddScoped<LogAuditoriaRepositorio>(_ =>
{
    var connectionString = builder.Configuration["Db:ConnectionString"];
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException("Database connection string 'Db:ConnectionString' is not configured.");
    }
    return new LogAuditoriaRepositorio(connectionString);
});


var app = builder.Build();

app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<AuditMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=LoginIndex}/{id?}");


app.Run();