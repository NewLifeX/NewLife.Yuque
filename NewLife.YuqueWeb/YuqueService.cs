using System.Reflection;
using NewLife.Cube;
using NewLife.Log;
using NewLife.YuqueWeb.Areas.Yuque;

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

        XTrace.WriteLine("{0} End   配置语雀 {0}", new String('=', 32));

        return services;
    }

    /// <summary>使用语雀</summary>
    /// <param name="app"></param>
    /// <param name="env"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseYuque(this IApplicationBuilder app, IWebHostEnvironment env = null)
    {
        using var span = DefaultTracer.Instance?.NewSpan(nameof(UseYuque));

        XTrace.WriteLine("{0} Start 初始化语雀 {0}", new String('=', 32));

        // 自动检查并添加菜单
        AreaBase.RegisterArea<YuqueArea>();

        XTrace.WriteLine("{0} End   初始化语雀 {0}", new String('=', 32));

        return app;
    }
}