using NewLife.Cube;
using NewLife.Log;
using NewLife.YuqueWeb;

XTrace.UseConsole();

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddStardust();

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