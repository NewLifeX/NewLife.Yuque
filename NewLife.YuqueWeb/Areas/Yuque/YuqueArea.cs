using System.ComponentModel;
using NewLife.Cube;

namespace NewLife.YuqueWeb.Areas.Yuque;

/// <summary>语雀管理区域注册</summary>
[DisplayName("语雀管理")]
[Menu(-2, true, Icon = "fa-tachometer")]
public class YuqueArea : AreaBase
{
    /// <inheritdoc />
    public YuqueArea() : base(nameof(YuqueArea).TrimEnd("Area")) { }
}