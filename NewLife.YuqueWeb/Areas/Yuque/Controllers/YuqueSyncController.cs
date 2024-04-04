using System.ComponentModel;
using NewLife.Cube;
using XCode.Membership;

namespace NewLife.YuqueWeb.Areas.Yuque.Controllers;

/// <summary>同步配置</summary>
[YuqueArea]
[DisplayName("同步配置")]
[Menu(10, true, Icon = "fa-tachometer")]
public class YuqueSyncController : ConfigController<YuqueSyncSetting>
{
}