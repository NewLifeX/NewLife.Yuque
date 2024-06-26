﻿using Microsoft.AspNetCore.Mvc;
using NewLife.Cube;
using NewLife.Cube.Extensions;
using NewLife.Web;
using NewLife.YuqueWeb.Entity;
using NewLife.YuqueWeb.Services;
using XCode.Membership;

namespace NewLife.YuqueWeb.Areas.Yuque.Controllers;

/// <summary>
/// 知识库管理
/// </summary>
[YuqueArea]
[Menu(90, true, Icon = "fa-tachometer")]
public class BookController : EntityController<Book>
{
    static BookController()
    {
        LogOnChange = true;

        ListFields.RemoveCreateField();
        ListFields.RemoveUpdateField();

        ListFields.RemoveField("Namespace");
        ListFields.TraceUrl();

        {
            var df = ListFields.AddListField("documents", "Sort");
            df.DisplayName = "文档列表";
            df.Url = "document?bookId={Id}";
        }
        {
            var df = ListFields.AddListField("documents2", "Sort");
            df.DisplayName = "前台列表";
            df.Url = "/{Code}";
            df.DataVisible = e => (e as Book).Enable;
        }
        {
            var df = ListFields.AddListField("origin", "Sort");
            df.DisplayName = "原文";
            df.Url = "https://www.yuque.com/{Group.Code}/{Slug}";
        }
    }

    private readonly BookService _bookService;

    /// <summary>
    /// 实例化知识库管理
    /// </summary>
    /// <param name="bookService"></param>
    public BookController(BookService bookService) => _bookService = bookService;

    /// <summary>
    /// 搜索
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    protected override IEnumerable<Book> Search(Pager p)
    {
        var id = p["id"].ToInt(-1);
        if (id > 0)
        {
            var entity = Book.FindById(id);
            if (entity != null) return [entity];
        }

        var groupId = p["groupId"].ToInt(-1);
        var enable = p["enable"]?.ToBoolean();

        var start = p["dtStart"].ToDateTime();
        var end = p["dtEnd"].ToDateTime();

        p.RetrieveState = true;

        return Book.Search(groupId, enable, start, end, p["Q"], p);
    }

    /// <summary>同步知识库</summary>
    /// <returns></returns>
    [EntityAuthorize(PermissionFlags.Update)]
    public async Task<ActionResult> SyncRepo()
    {
        var count = 0;
        var ids = GetRequest("keys").SplitAsInt();
        foreach (var id in ids.OrderBy(e => e))
        {
            count += await _bookService.SyncBook(id);
        }

        return JsonRefresh($"共刷新[{count}]篇文章");
    }

    /// <summary>扫描全部知识库</summary>
    /// <returns></returns>
    [EntityAuthorize(PermissionFlags.Insert)]
    public async Task<ActionResult> ScanAll()
    {
        var count = await _bookService.ScanAll();

        return JsonRefresh($"共扫描[{count}]个知识库");
    }
}