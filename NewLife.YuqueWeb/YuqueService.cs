using System.Reflection;
using Microsoft.Extensions.FileProviders;
using NewLife.Cube;
using NewLife.Cube.Extensions;
using NewLife.Log;
using NewLife.YuqueWeb.Areas.Yuque;
using NewLife.YuqueWeb.Services;
using NewLife.YuQueWeb.Entity;

namespace NewLife.YuQueWeb;

/// <summary>语雀服务</summary>
public static class YuqueService
{
    /// <summary>添加语雀</summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddYuque(this IServiceCollection services)
    {
        using var span = DefaultTracer.Instance?.NewSpan(nameof(AddYuque));

        XTrace.WriteLine("{0} Start 配置语雀 {0}", new String('=', 32));
        Assembly.GetExecutingAssembly().WriteVersion();

        services.AddSingleton<BookService>();

        services.AddHostedService<SyncService>();

        XTrace.WriteLine("{0} End   配置语雀 {0}", new String('=', 32));

        return services;
    }

    /// <summary>使用语雀</summary>
    /// <param name="app"></param>
    /// <param name="useHomeIndex"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseYuque(this IApplicationBuilder app, Boolean useHomeIndex)
    {
        using var span = DefaultTracer.Instance?.NewSpan(nameof(UseYuque));

        XTrace.WriteLine("{0} Start 初始化语雀 {0}", new String('=', 32));

        // 独立静态文件设置，魔方自己的静态资源内嵌在程序集里面
        var options = new StaticFileOptions();
        {
            var env = app.ApplicationServices.GetService<IWebHostEnvironment>();
            var embeddedProvider = new CubeEmbeddedFileProvider(Assembly.GetExecutingAssembly(), "NewLife.YuqueWeb.wwwroot");
            if (!env.WebRootPath.IsNullOrEmpty() && Directory.Exists(env.WebRootPath))
                options.FileProvider = new CompositeFileProvider(new PhysicalFileProvider(env.WebRootPath), embeddedProvider);
            else
                options.FileProvider = embeddedProvider;
        }
        app.UseStaticFiles(options);

        app.UseRouter(endpoints => RegisterRoute(endpoints, useHomeIndex));

        // 自动检查并添加菜单
        AreaBase.RegisterArea<YuqueArea>();

        XTrace.WriteLine("{0} End   初始化语雀 {0}", new String('=', 32));

        return app;
    }

    /// <summary>
    /// 注册路由
    /// </summary>
    /// <param name="endpoints"></param>
    /// <param name="useHomeIndex"></param>
    public static void RegisterRoute(IEndpointRouteBuilder endpoints, Boolean useHomeIndex)
    {
        if (useHomeIndex)
        {
            endpoints.MapControllerRoute(
                name: "Yuque_Index",
                pattern: "/",
                defaults: new { controller = "Yuque", action = "Index" }
            );
        }

        #region 类别
        endpoints.MapControllerRoute(
            name: "Yuque_Category",
            pattern: "{categoryCode}",
            defaults: new { controller = "Yuque", action = "List" },
            constraints: new { categoryCode = new CategoryUrlConstraint() }
        );

        endpoints.MapControllerRoute(
            name: "Yuque_Category_Page",
            pattern: "{categoryCode}-{pageIndex}",
            defaults: new { controller = "Yuque", action = "List" },
            constraints: new { categoryCode = new CategoryUrlConstraint(), pageIndex = "[\\d]+" }
        );
        #endregion

        #region 信息
        endpoints.MapControllerRoute(
            name: "Yuque_Info",
            pattern: "{categoryCode}/{infoCode}",
            defaults: new { controller = "Yuque", action = "Detail" },
            constraints: new { categoryCode = new CategoryUrlConstraint(), infoCode = new InfoUrlConstraint() }
        );
        #endregion

        #region  搜索
        endpoints.MapControllerRoute(
            name: "Yuque_Search",
            pattern: "search-{key}-{pageIndex}",
            defaults: new { controller = "Yuque", action = "Search" },
            constraints: null
        );

        endpoints.MapControllerRoute(
            name: "Yuque_Search2",
            pattern: "{categoryCode}-{key}-{pageIndex}",
            defaults: new { controller = "Yuque", action = "Search" },
            constraints: new { categoryCode = new CategoryUrlConstraint() }
        );
        #endregion
    }
}

class CategoryUrlConstraint : IRouteConstraint
{
    public Boolean Match(HttpContext httpContext, IRouter route, String parameterName, RouteValueDictionary values, RouteDirection routeDirection)
    {
        var name = values[parameterName] + "";
        if (name.IsNullOrEmpty()) return false;

        var book = Book.FindByCode(name);
        return book != null && book.Enable;
    }
}

/// <summary>信息路径适配</summary>
class InfoUrlConstraint : IRouteConstraint
{
    public Boolean Match(HttpContext httpContext, IRouter route, String parameterName, RouteValueDictionary values, RouteDirection routeDirection)
    {
        var book = Book.FindByCode(values["categoryCode"] + "");
        if (book == null) return false;

        var infoCode = values[parameterName] + "";
        if (infoCode.IsNullOrEmpty() || infoCode.ToInt() > 0) return false;

        if (Document.FindByBookAndCode(book.Id, infoCode) != null) return true;

        return false;
    }
}