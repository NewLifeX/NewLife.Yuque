using NewLife.Cube;
using NewLife.Cube.WebMiddleware;
using NewLife.Log;
using NewLife.YuqueWeb;

XTrace.UseConsole();

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

var star = services.AddStardust(null);
TracerMiddleware.Tracer = star.Tracer;

// 启用接口响应压缩
services.AddResponseCompression();

services.AddControllersWithViews();
services.AddYuque();
services.AddCube();

var app = builder.Build();
app.UseStaticFiles();

//app.UseStardust();
app.UseCube(builder.Environment);
app.UseYuque(true);

app.UseAuthorization();
app.UseResponseCompression();
app.MapControllerRoute(name: "default", pattern: "{controller=Index}/{action=Index}/{id?}");
app.MapControllerRoute(name: "default2", pattern: "{area=Admin}/{controller=Index}/{action=Index}/{id?}");

app.Run();