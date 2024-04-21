using System.Runtime.Serialization;

namespace NewLife.YuqueWeb.Models;

/// <summary>导航项</summary>
public class NavItem
{
    public String Id { get; set; }

    public Int32 Level { get; set; }

    public String Title { get; set; }

    //[IgnoreDataMember]
    //public NavItem Parent { get; set; }

    public IList<NavItem> Children { get; set; } = [];
}
