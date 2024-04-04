using System.ComponentModel;
using NewLife.Configuration;
using XCode.Configuration;

namespace NewLife.YuqueWeb;

[Config("YuqueSync")]
public class YuqueSyncSetting : Config<YuqueSyncSetting>
{
    static YuqueSyncSetting() => Provider = new DbConfigProvider { UserId = 0, Category = "YuqueSync" };

    /// <summary>同步知识库周期。默认3600秒</summary>
    [Description("同步知识库周期。默认3600秒")]
    public Int32 SyncBookPeriod { get; set; } = 3600;

    /// <summary>同步文档周期。默认3600秒</summary>
    [Description("同步文档周期。默认3600秒")]
    public Int32 SyncDocumentPeriod { get; set; } = 3600;
}
