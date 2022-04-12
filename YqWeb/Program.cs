using NewLife.Cube;
using NewLife.Log;
using NewLife.YuQueWeb;

XTrace.UseConsole();

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

var star = services.AddStardust(null);

services.AddControllersWithViews();
services.AddYuque();
services.AddCube();

var app = builder.Build();
app.UseCube(builder.Environment);
app.UseYuque(true);
app.UseAuthorization();
app.MapControllerRoute(name: "default", pattern: "{controller=Index}/{action=Index}/{id?}");
app.MapControllerRoute(name: "default2", pattern: "{area=Admin}/{controller=Index}/{action=Index}/{id?}");

app.Run();