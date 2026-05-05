using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using inventario_herramientas.Managers.Managers;
using inventario_herramientas.Managers.Repos;
using Microsoft.Extensions.Configuration;
using inventario_herramientas.Web.Helpers;

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

builder.Services.AddControllersWithViews();

// Aca se agregan los managers, repos = [inyeccion de dependencias]
builder.Services.AddScoped<IHerramientasManager, HerramientasManager>();
builder.Services.AddScoped<IHerramientasRepositorio>(
    _ => new HerramientasRepositorio(builder.Configuration["Db:ConnectionString"]));

builder.Services.AddScoped<IUsuariosManager, UsuariosManager>();
builder.Services.AddScoped<IUsuariosRepositorio>(_ =>
    new UsuariosRepositorio(builder.Configuration["Db:ConnectionString"]));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=LoginIndex}/{id?}");


app.Run();
