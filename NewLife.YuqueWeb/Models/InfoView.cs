using NewLife.YuqueWeb.Entity;
using XCode.Membership;

namespace NewLife.YuqueWeb.Models;

/// <summary>信息视图</summary>
public class InfoView
{
    public Document Document { get; set; }

    public IList<NavItem> Navs { get; set; }
}
