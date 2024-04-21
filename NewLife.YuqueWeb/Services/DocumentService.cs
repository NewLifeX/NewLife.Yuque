using System.Text.RegularExpressions;
using NewLife.YuqueWeb.Models;

namespace NewLife.YuqueWeb.Services;

public partial class DocumentService
{
    [GeneratedRegex("""<h(\d)\s+id="(\w+)">(.*?)</h\d>""", RegexOptions.Compiled)]
    private static partial Regex NavReg();
    public IList<NavItem> BuildNavs(String html)
    {
        var list = new List<NavItem>();
        if (html.IsNullOrEmpty()) return list;

        foreach (var match in NavReg().Matches(html).Cast<Match>())
        {
            var nav = new NavItem
            {
                Level = match.Groups[1].Value.ToInt(),
                Id = match.Groups[2].Value,
                Title = match.Groups[3].Value,
            };

            // 去掉标题中的标签
            var p = nav.Title.IndexOf('>');
            if (p > 0)
            {
                var p2 = nav.Title.IndexOf('<', p);
                if (p2 > 0)
                {
                    nav.Title = nav.Title.Substring(p + 1, p2 - p - 1);
                }
            }

            list.Add(nav);
        }

        // 构建树状结构
        var tree = BuildTree(list);

        return tree;
    }

    public IList<NavItem> BuildTree(IList<NavItem> list)
    {
        // 找到最高层级，该层级将作为根层级
        var max = list.Min(e => e.Level);

        var tree = new List<NavItem>();

        // 遍历所有导航项，构建根层级的树，递归构建子树
        for (var i = 0; i < list.Count; i++)
        {
            var nav = list[i];
            if (nav.Level == max)
            {
                tree.Add(nav);

                // 找到后续小于该层级的项
                var childs = new List<NavItem>();
                for (i++; i < list.Count; i++)
                {
                    // 任意小于等于当前层级的项，都要退出
                    if (list[i].Level <= max) break;

                    childs.Add(list[i]);
                }
                i--;

                if (childs.Count > 0)
                    nav.Children = BuildTree(childs);
            }
        }

        return tree;
    }
}
