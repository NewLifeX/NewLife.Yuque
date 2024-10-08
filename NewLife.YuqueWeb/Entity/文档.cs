﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using NewLife;
using NewLife.Data;
using XCode;
using XCode.Cache;
using XCode.Configuration;
using XCode.DataAccessLayer;

namespace NewLife.YuqueWeb.Entity;

/// <summary>文档。文档内容</summary>
[Serializable]
[DataObject]
[Description("文档。文档内容")]
[BindIndex("IU_Document_BookId_Code", true, "BookId,Code")]
[BindIndex("IX_Document_BookId_Slug", false, "BookId,Slug")]
[BindIndex("IX_Document_BookId_Sort", false, "BookId,Sort")]
[BindIndex("IX_Document_Title", false, "Title")]
[BindIndex("IX_Document_UpdateTime", false, "UpdateTime")]
[BindIndex("IX_Document_SyncTime", false, "SyncTime")]
[BindTable("Document", Description = "文档。文档内容", ConnName = "Yuque", DbType = DatabaseType.None)]
public partial class Document
{
    #region 属性
    private Int32 _Id;
    /// <summary>编号</summary>
    [DisplayName("编号")]
    [Description("编号")]
    [DataObjectField(true, false, false, 0)]
    [BindColumn("Id", "编号", "")]
    public Int32 Id { get => _Id; set { if (OnPropertyChanging("Id", value)) { _Id = value; OnPropertyChanged("Id"); } } }

    private String _Code;
    /// <summary>编码。路径唯一标识，默认取Slug</summary>
    [DisplayName("编码")]
    [Description("编码。路径唯一标识，默认取Slug")]
    [DataObjectField(false, false, true, 50)]
    [BindColumn("Code", "编码。路径唯一标识，默认取Slug", "")]
    public String Code { get => _Code; set { if (OnPropertyChanging("Code", value)) { _Code = value; OnPropertyChanged("Code"); } } }

    private String _Title;
    /// <summary>标题</summary>
    [DisplayName("标题")]
    [Description("标题")]
    [DataObjectField(false, false, false, 200)]
    [BindColumn("Title", "标题", "", Master = true)]
    public String Title { get => _Title; set { if (OnPropertyChanging("Title", value)) { _Title = value; OnPropertyChanged("Title"); } } }

    private Int32 _BookId;
    /// <summary>知识库</summary>
    [DisplayName("知识库")]
    [Description("知识库")]
    [DataObjectField(false, false, false, 0)]
    [BindColumn("BookId", "知识库", "")]
    public Int32 BookId { get => _BookId; set { if (OnPropertyChanging("BookId", value)) { _BookId = value; OnPropertyChanged("BookId"); } } }

    private Boolean _Enable;
    /// <summary>启用</summary>
    [DisplayName("启用")]
    [Description("启用")]
    [DataObjectField(false, false, false, 0)]
    [BindColumn("Enable", "启用", "")]
    public Boolean Enable { get => _Enable; set { if (OnPropertyChanging("Enable", value)) { _Enable = value; OnPropertyChanged("Enable"); } } }

    private Int32 _Sort;
    /// <summary>排序。降序，数字越大越靠前</summary>
    [DisplayName("排序")]
    [Description("排序。降序，数字越大越靠前")]
    [DataObjectField(false, false, false, 0)]
    [BindColumn("Sort", "排序。降序，数字越大越靠前", "")]
    public Int32 Sort { get => _Sort; set { if (OnPropertyChanging("Sort", value)) { _Sort = value; OnPropertyChanged("Sort"); } } }

    private String _UserName;
    /// <summary>用户</summary>
    [DisplayName("用户")]
    [Description("用户")]
    [DataObjectField(false, false, true, 50)]
    [BindColumn("UserName", "用户", "")]
    public String UserName { get => _UserName; set { if (OnPropertyChanging("UserName", value)) { _UserName = value; OnPropertyChanged("UserName"); } } }

    private String _Slug;
    /// <summary>路径</summary>
    [DisplayName("路径")]
    [Description("路径")]
    [DataObjectField(false, false, true, 50)]
    [BindColumn("Slug", "路径", "")]
    public String Slug { get => _Slug; set { if (OnPropertyChanging("Slug", value)) { _Slug = value; OnPropertyChanged("Slug"); } } }

    private String _Format;
    /// <summary>格式</summary>
    [DisplayName("格式")]
    [Description("格式")]
    [DataObjectField(false, false, true, 50)]
    [BindColumn("Format", "格式", "")]
    public String Format { get => _Format; set { if (OnPropertyChanging("Format", value)) { _Format = value; OnPropertyChanged("Format"); } } }

    private Boolean _Public;
    /// <summary>公开。公开或私密</summary>
    [DisplayName("公开")]
    [Description("公开。公开或私密")]
    [DataObjectField(false, false, false, 0)]
    [BindColumn("Public", "公开。公开或私密", "")]
    public Boolean Public { get => _Public; set { if (OnPropertyChanging("Public", value)) { _Public = value; OnPropertyChanged("Public"); } } }

    private Boolean _Status;
    /// <summary>正式。正式或草稿</summary>
    [DisplayName("正式")]
    [Description("正式。正式或草稿")]
    [DataObjectField(false, false, false, 0)]
    [BindColumn("Status", "正式。正式或草稿", "")]
    public Boolean Status { get => _Status; set { if (OnPropertyChanging("Status", value)) { _Status = value; OnPropertyChanged("Status"); } } }

    private Int32 _DraftVersion;
    /// <summary>版本。草案版本</summary>
    [DisplayName("版本")]
    [Description("版本。草案版本")]
    [DataObjectField(false, false, false, 0)]
    [BindColumn("DraftVersion", "版本。草案版本", "")]
    public Int32 DraftVersion { get => _DraftVersion; set { if (OnPropertyChanging("DraftVersion", value)) { _DraftVersion = value; OnPropertyChanged("DraftVersion"); } } }

    private Int32 _Hits;
    /// <summary>点击量</summary>
    [DisplayName("点击量")]
    [Description("点击量")]
    [DataObjectField(false, false, false, 0)]
    [BindColumn("Hits", "点击量", "")]
    public Int32 Hits { get => _Hits; set { if (OnPropertyChanging("Hits", value)) { _Hits = value; OnPropertyChanged("Hits"); } } }

    private Int32 _Likes;
    /// <summary>点赞数</summary>
    [DisplayName("点赞数")]
    [Description("点赞数")]
    [DataObjectField(false, false, false, 0)]
    [BindColumn("Likes", "点赞数", "")]
    public Int32 Likes { get => _Likes; set { if (OnPropertyChanging("Likes", value)) { _Likes = value; OnPropertyChanged("Likes"); } } }

    private Int32 _Reads;
    /// <summary>阅读数</summary>
    [DisplayName("阅读数")]
    [Description("阅读数")]
    [DataObjectField(false, false, false, 0)]
    [BindColumn("Reads", "阅读数", "")]
    public Int32 Reads { get => _Reads; set { if (OnPropertyChanging("Reads", value)) { _Reads = value; OnPropertyChanged("Reads"); } } }

    private Int32 _Comments;
    /// <summary>评论数</summary>
    [DisplayName("评论数")]
    [Description("评论数")]
    [DataObjectField(false, false, false, 0)]
    [BindColumn("Comments", "评论数", "")]
    public Int32 Comments { get => _Comments; set { if (OnPropertyChanging("Comments", value)) { _Comments = value; OnPropertyChanged("Comments"); } } }

    private Int32 _LocalHits;
    /// <summary>本地点击</summary>
    [DisplayName("本地点击")]
    [Description("本地点击")]
    [DataObjectField(false, false, false, 0)]
    [BindColumn("LocalHits", "本地点击", "")]
    public Int32 LocalHits { get => _LocalHits; set { if (OnPropertyChanging("LocalHits", value)) { _LocalHits = value; OnPropertyChanged("LocalHits"); } } }

    private Int32 _TotalHits;
    /// <summary>总点击</summary>
    [DisplayName("总点击")]
    [Description("总点击")]
    [DataObjectField(false, false, false, 0)]
    [BindColumn("TotalHits", "总点击", "")]
    public Int32 TotalHits { get => _TotalHits; set { if (OnPropertyChanging("TotalHits", value)) { _TotalHits = value; OnPropertyChanged("TotalHits"); } } }

    private Int32 _WordCount;
    /// <summary>单词数</summary>
    [DisplayName("单词数")]
    [Description("单词数")]
    [DataObjectField(false, false, false, 0)]
    [BindColumn("WordCount", "单词数", "")]
    public Int32 WordCount { get => _WordCount; set { if (OnPropertyChanging("WordCount", value)) { _WordCount = value; OnPropertyChanged("WordCount"); } } }

    private String _Cover;
    /// <summary>封面</summary>
    [DisplayName("封面")]
    [Description("封面")]
    [DataObjectField(false, false, true, 200)]
    [BindColumn("Cover", "封面", "")]
    public String Cover { get => _Cover; set { if (OnPropertyChanging("Cover", value)) { _Cover = value; OnPropertyChanged("Cover"); } } }

    private String _Summary;
    /// <summary>摘要</summary>
    [DisplayName("摘要")]
    [Description("摘要")]
    [DataObjectField(false, false, true, 500)]
    [BindColumn("Summary", "摘要", "")]
    public String Summary { get => _Summary; set { if (OnPropertyChanging("Summary", value)) { _Summary = value; OnPropertyChanged("Summary"); } } }

    private String _Body;
    /// <summary>正文。Markdown格式</summary>
    [DisplayName("正文")]
    [Description("正文。Markdown格式")]
    [DataObjectField(false, false, true, -1)]
    [BindColumn("Body", "正文。Markdown格式", "")]
    public String Body { get => _Body; set { if (OnPropertyChanging("Body", value)) { _Body = value; OnPropertyChanged("Body"); } } }

    private String _BodyHtml;
    /// <summary>HTML正文</summary>
    [DisplayName("HTML正文")]
    [Description("HTML正文")]
    [DataObjectField(false, false, true, -1)]
    [BindColumn("BodyHtml", "HTML正文", "")]
    public String BodyHtml { get => _BodyHtml; set { if (OnPropertyChanging("BodyHtml", value)) { _BodyHtml = value; OnPropertyChanged("BodyHtml"); } } }

    private String _Html;
    /// <summary>HTML。处理后的HTML内容，用于前端展示</summary>
    [DisplayName("HTML")]
    [Description("HTML。处理后的HTML内容，用于前端展示")]
    [DataObjectField(false, false, true, -1)]
    [BindColumn("Html", "HTML。处理后的HTML内容，用于前端展示", "")]
    public String Html { get => _Html; set { if (OnPropertyChanging("Html", value)) { _Html = value; OnPropertyChanged("Html"); } } }

    private DateTime _ContentUpdateTime;
    /// <summary>内容更新时间</summary>
    [DisplayName("内容更新时间")]
    [Description("内容更新时间")]
    [DataObjectField(false, false, true, 0)]
    [BindColumn("ContentUpdateTime", "内容更新时间", "")]
    public DateTime ContentUpdateTime { get => _ContentUpdateTime; set { if (OnPropertyChanging("ContentUpdateTime", value)) { _ContentUpdateTime = value; OnPropertyChanged("ContentUpdateTime"); } } }

    private DateTime _PublishTime;
    /// <summary>发布时间</summary>
    [DisplayName("发布时间")]
    [Description("发布时间")]
    [DataObjectField(false, false, true, 0)]
    [BindColumn("PublishTime", "发布时间", "")]
    public DateTime PublishTime { get => _PublishTime; set { if (OnPropertyChanging("PublishTime", value)) { _PublishTime = value; OnPropertyChanged("PublishTime"); } } }

    private DateTime _FirstPublishTime;
    /// <summary>首次发布</summary>
    [DisplayName("首次发布")]
    [Description("首次发布")]
    [DataObjectField(false, false, true, 0)]
    [BindColumn("FirstPublishTime", "首次发布", "")]
    public DateTime FirstPublishTime { get => _FirstPublishTime; set { if (OnPropertyChanging("FirstPublishTime", value)) { _FirstPublishTime = value; OnPropertyChanged("FirstPublishTime"); } } }

    private Boolean _Sync;
    /// <summary>同步。是否自动同步远程内容</summary>
    [DisplayName("同步")]
    [Description("同步。是否自动同步远程内容")]
    [DataObjectField(false, false, false, 0)]
    [BindColumn("Sync", "同步。是否自动同步远程内容", "")]
    public Boolean Sync { get => _Sync; set { if (OnPropertyChanging("Sync", value)) { _Sync = value; OnPropertyChanged("Sync"); } } }

    private DateTime _SyncTime;
    /// <summary>同步时间。最后一次同步数据的时间</summary>
    [DisplayName("同步时间")]
    [Description("同步时间。最后一次同步数据的时间")]
    [DataObjectField(false, false, true, 0)]
    [BindColumn("SyncTime", "同步时间。最后一次同步数据的时间", "")]
    public DateTime SyncTime { get => _SyncTime; set { if (OnPropertyChanging("SyncTime", value)) { _SyncTime = value; OnPropertyChanged("SyncTime"); } } }

    private String _TraceId;
    /// <summary>追踪。用于记录调用链追踪标识，在星尘APM查找调用链</summary>
    [Category("扩展")]
    [DisplayName("追踪")]
    [Description("追踪。用于记录调用链追踪标识，在星尘APM查找调用链")]
    [DataObjectField(false, false, true, 50)]
    [BindColumn("TraceId", "追踪。用于记录调用链追踪标识，在星尘APM查找调用链", "")]
    public String TraceId { get => _TraceId; set { if (OnPropertyChanging("TraceId", value)) { _TraceId = value; OnPropertyChanged("TraceId"); } } }

    private String _CreateUser;
    /// <summary>创建者</summary>
    [Category("扩展")]
    [DisplayName("创建者")]
    [Description("创建者")]
    [DataObjectField(false, false, true, 50)]
    [BindColumn("CreateUser", "创建者", "")]
    public String CreateUser { get => _CreateUser; set { if (OnPropertyChanging("CreateUser", value)) { _CreateUser = value; OnPropertyChanged("CreateUser"); } } }

    private Int32 _CreateUserID;
    /// <summary>创建人</summary>
    [Category("扩展")]
    [DisplayName("创建人")]
    [Description("创建人")]
    [DataObjectField(false, false, false, 0)]
    [BindColumn("CreateUserID", "创建人", "")]
    public Int32 CreateUserID { get => _CreateUserID; set { if (OnPropertyChanging("CreateUserID", value)) { _CreateUserID = value; OnPropertyChanged("CreateUserID"); } } }

    private String _CreateIP;
    /// <summary>创建地址</summary>
    [Category("扩展")]
    [DisplayName("创建地址")]
    [Description("创建地址")]
    [DataObjectField(false, false, true, 50)]
    [BindColumn("CreateIP", "创建地址", "")]
    public String CreateIP { get => _CreateIP; set { if (OnPropertyChanging("CreateIP", value)) { _CreateIP = value; OnPropertyChanged("CreateIP"); } } }

    private DateTime _CreateTime;
    /// <summary>创建时间</summary>
    [Category("扩展")]
    [DisplayName("创建时间")]
    [Description("创建时间")]
    [DataObjectField(false, false, true, 0)]
    [BindColumn("CreateTime", "创建时间", "")]
    public DateTime CreateTime { get => _CreateTime; set { if (OnPropertyChanging("CreateTime", value)) { _CreateTime = value; OnPropertyChanged("CreateTime"); } } }

    private String _UpdateUser;
    /// <summary>更新者</summary>
    [Category("扩展")]
    [DisplayName("更新者")]
    [Description("更新者")]
    [DataObjectField(false, false, true, 50)]
    [BindColumn("UpdateUser", "更新者", "")]
    public String UpdateUser { get => _UpdateUser; set { if (OnPropertyChanging("UpdateUser", value)) { _UpdateUser = value; OnPropertyChanged("UpdateUser"); } } }

    private Int32 _UpdateUserID;
    /// <summary>更新人</summary>
    [Category("扩展")]
    [DisplayName("更新人")]
    [Description("更新人")]
    [DataObjectField(false, false, false, 0)]
    [BindColumn("UpdateUserID", "更新人", "")]
    public Int32 UpdateUserID { get => _UpdateUserID; set { if (OnPropertyChanging("UpdateUserID", value)) { _UpdateUserID = value; OnPropertyChanged("UpdateUserID"); } } }

    private String _UpdateIP;
    /// <summary>更新地址</summary>
    [Category("扩展")]
    [DisplayName("更新地址")]
    [Description("更新地址")]
    [DataObjectField(false, false, true, 50)]
    [BindColumn("UpdateIP", "更新地址", "")]
    public String UpdateIP { get => _UpdateIP; set { if (OnPropertyChanging("UpdateIP", value)) { _UpdateIP = value; OnPropertyChanged("UpdateIP"); } } }

    private DateTime _UpdateTime;
    /// <summary>更新时间</summary>
    [Category("扩展")]
    [DisplayName("更新时间")]
    [Description("更新时间")]
    [DataObjectField(false, false, true, 0)]
    [BindColumn("UpdateTime", "更新时间", "")]
    public DateTime UpdateTime { get => _UpdateTime; set { if (OnPropertyChanging("UpdateTime", value)) { _UpdateTime = value; OnPropertyChanged("UpdateTime"); } } }

    private String _Remark;
    /// <summary>备注</summary>
    [Category("扩展")]
    [DisplayName("备注")]
    [Description("备注")]
    [DataObjectField(false, false, true, 500)]
    [BindColumn("Remark", "备注", "")]
    public String Remark { get => _Remark; set { if (OnPropertyChanging("Remark", value)) { _Remark = value; OnPropertyChanged("Remark"); } } }
    #endregion

    #region 获取/设置 字段值
    /// <summary>获取/设置 字段值</summary>
    /// <param name="name">字段名</param>
    /// <returns></returns>
    public override Object this[String name]
    {
        get => name switch
        {
            "Id" => _Id,
            "Code" => _Code,
            "Title" => _Title,
            "BookId" => _BookId,
            "Enable" => _Enable,
            "Sort" => _Sort,
            "UserName" => _UserName,
            "Slug" => _Slug,
            "Format" => _Format,
            "Public" => _Public,
            "Status" => _Status,
            "DraftVersion" => _DraftVersion,
            "Hits" => _Hits,
            "Likes" => _Likes,
            "Reads" => _Reads,
            "Comments" => _Comments,
            "LocalHits" => _LocalHits,
            "TotalHits" => _TotalHits,
            "WordCount" => _WordCount,
            "Cover" => _Cover,
            "Summary" => _Summary,
            "Body" => _Body,
            "BodyHtml" => _BodyHtml,
            "Html" => _Html,
            "ContentUpdateTime" => _ContentUpdateTime,
            "PublishTime" => _PublishTime,
            "FirstPublishTime" => _FirstPublishTime,
            "Sync" => _Sync,
            "SyncTime" => _SyncTime,
            "TraceId" => _TraceId,
            "CreateUser" => _CreateUser,
            "CreateUserID" => _CreateUserID,
            "CreateIP" => _CreateIP,
            "CreateTime" => _CreateTime,
            "UpdateUser" => _UpdateUser,
            "UpdateUserID" => _UpdateUserID,
            "UpdateIP" => _UpdateIP,
            "UpdateTime" => _UpdateTime,
            "Remark" => _Remark,
            _ => base[name]
        };
        set
        {
            switch (name)
            {
                case "Id": _Id = value.ToInt(); break;
                case "Code": _Code = Convert.ToString(value); break;
                case "Title": _Title = Convert.ToString(value); break;
                case "BookId": _BookId = value.ToInt(); break;
                case "Enable": _Enable = value.ToBoolean(); break;
                case "Sort": _Sort = value.ToInt(); break;
                case "UserName": _UserName = Convert.ToString(value); break;
                case "Slug": _Slug = Convert.ToString(value); break;
                case "Format": _Format = Convert.ToString(value); break;
                case "Public": _Public = value.ToBoolean(); break;
                case "Status": _Status = value.ToBoolean(); break;
                case "DraftVersion": _DraftVersion = value.ToInt(); break;
                case "Hits": _Hits = value.ToInt(); break;
                case "Likes": _Likes = value.ToInt(); break;
                case "Reads": _Reads = value.ToInt(); break;
                case "Comments": _Comments = value.ToInt(); break;
                case "LocalHits": _LocalHits = value.ToInt(); break;
                case "TotalHits": _TotalHits = value.ToInt(); break;
                case "WordCount": _WordCount = value.ToInt(); break;
                case "Cover": _Cover = Convert.ToString(value); break;
                case "Summary": _Summary = Convert.ToString(value); break;
                case "Body": _Body = Convert.ToString(value); break;
                case "BodyHtml": _BodyHtml = Convert.ToString(value); break;
                case "Html": _Html = Convert.ToString(value); break;
                case "ContentUpdateTime": _ContentUpdateTime = value.ToDateTime(); break;
                case "PublishTime": _PublishTime = value.ToDateTime(); break;
                case "FirstPublishTime": _FirstPublishTime = value.ToDateTime(); break;
                case "Sync": _Sync = value.ToBoolean(); break;
                case "SyncTime": _SyncTime = value.ToDateTime(); break;
                case "TraceId": _TraceId = Convert.ToString(value); break;
                case "CreateUser": _CreateUser = Convert.ToString(value); break;
                case "CreateUserID": _CreateUserID = value.ToInt(); break;
                case "CreateIP": _CreateIP = Convert.ToString(value); break;
                case "CreateTime": _CreateTime = value.ToDateTime(); break;
                case "UpdateUser": _UpdateUser = Convert.ToString(value); break;
                case "UpdateUserID": _UpdateUserID = value.ToInt(); break;
                case "UpdateIP": _UpdateIP = Convert.ToString(value); break;
                case "UpdateTime": _UpdateTime = value.ToDateTime(); break;
                case "Remark": _Remark = Convert.ToString(value); break;
                default: base[name] = value; break;
            }
        }
    }
    #endregion

    #region 关联映射
    #endregion

    #region 扩展查询
    /// <summary>根据知识库、编码查找</summary>
    /// <param name="bookId">知识库</param>
    /// <param name="code">编码</param>
    /// <returns>实体对象</returns>
    public static Document FindByBookIdAndCode(Int32 bookId, String code)
    {
        if (bookId < 0) return null;
        if (code.IsNullOrEmpty()) return null;

        // 实体缓存
        if (Meta.Session.Count < 1000) return Meta.Cache.Find(e => e.BookId == bookId && e.Code.EqualIgnoreCase(code));

        return Find(_.BookId == bookId & _.Code == code);
    }

    /// <summary>根据知识库、路径查找</summary>
    /// <param name="bookId">知识库</param>
    /// <param name="slug">路径</param>
    /// <returns>实体列表</returns>
    public static IList<Document> FindAllByBookIdAndSlug(Int32 bookId, String slug)
    {
        if (bookId < 0) return [];
        if (slug.IsNullOrEmpty()) return [];

        // 实体缓存
        if (Meta.Session.Count < 1000) return Meta.Cache.FindAll(e => e.BookId == bookId && e.Slug.EqualIgnoreCase(slug));

        return FindAll(_.BookId == bookId & _.Slug == slug);
    }

    /// <summary>根据知识库、排序查找</summary>
    /// <param name="bookId">知识库</param>
    /// <param name="sort">排序</param>
    /// <returns>实体列表</returns>
    public static IList<Document> FindAllByBookIdAndSort(Int32 bookId, Int32 sort)
    {
        if (bookId < 0) return [];
        if (sort < 0) return [];

        // 实体缓存
        if (Meta.Session.Count < 1000) return Meta.Cache.FindAll(e => e.BookId == bookId && e.Sort == sort);

        return FindAll(_.BookId == bookId & _.Sort == sort);
    }
    #endregion

    #region 字段名
    /// <summary>取得文档字段信息的快捷方式</summary>
    public partial class _
    {
        /// <summary>编号</summary>
        public static readonly Field Id = FindByName("Id");

        /// <summary>编码。路径唯一标识，默认取Slug</summary>
        public static readonly Field Code = FindByName("Code");

        /// <summary>标题</summary>
        public static readonly Field Title = FindByName("Title");

        /// <summary>知识库</summary>
        public static readonly Field BookId = FindByName("BookId");

        /// <summary>启用</summary>
        public static readonly Field Enable = FindByName("Enable");

        /// <summary>排序。降序，数字越大越靠前</summary>
        public static readonly Field Sort = FindByName("Sort");

        /// <summary>用户</summary>
        public static readonly Field UserName = FindByName("UserName");

        /// <summary>路径</summary>
        public static readonly Field Slug = FindByName("Slug");

        /// <summary>格式</summary>
        public static readonly Field Format = FindByName("Format");

        /// <summary>公开。公开或私密</summary>
        public static readonly Field Public = FindByName("Public");

        /// <summary>正式。正式或草稿</summary>
        public static readonly Field Status = FindByName("Status");

        /// <summary>版本。草案版本</summary>
        public static readonly Field DraftVersion = FindByName("DraftVersion");

        /// <summary>点击量</summary>
        public static readonly Field Hits = FindByName("Hits");

        /// <summary>点赞数</summary>
        public static readonly Field Likes = FindByName("Likes");

        /// <summary>阅读数</summary>
        public static readonly Field Reads = FindByName("Reads");

        /// <summary>评论数</summary>
        public static readonly Field Comments = FindByName("Comments");

        /// <summary>本地点击</summary>
        public static readonly Field LocalHits = FindByName("LocalHits");

        /// <summary>总点击</summary>
        public static readonly Field TotalHits = FindByName("TotalHits");

        /// <summary>单词数</summary>
        public static readonly Field WordCount = FindByName("WordCount");

        /// <summary>封面</summary>
        public static readonly Field Cover = FindByName("Cover");

        /// <summary>摘要</summary>
        public static readonly Field Summary = FindByName("Summary");

        /// <summary>正文。Markdown格式</summary>
        public static readonly Field Body = FindByName("Body");

        /// <summary>HTML正文</summary>
        public static readonly Field BodyHtml = FindByName("BodyHtml");

        /// <summary>HTML。处理后的HTML内容，用于前端展示</summary>
        public static readonly Field Html = FindByName("Html");

        /// <summary>内容更新时间</summary>
        public static readonly Field ContentUpdateTime = FindByName("ContentUpdateTime");

        /// <summary>发布时间</summary>
        public static readonly Field PublishTime = FindByName("PublishTime");

        /// <summary>首次发布</summary>
        public static readonly Field FirstPublishTime = FindByName("FirstPublishTime");

        /// <summary>同步。是否自动同步远程内容</summary>
        public static readonly Field Sync = FindByName("Sync");

        /// <summary>同步时间。最后一次同步数据的时间</summary>
        public static readonly Field SyncTime = FindByName("SyncTime");

        /// <summary>追踪。用于记录调用链追踪标识，在星尘APM查找调用链</summary>
        public static readonly Field TraceId = FindByName("TraceId");

        /// <summary>创建者</summary>
        public static readonly Field CreateUser = FindByName("CreateUser");

        /// <summary>创建人</summary>
        public static readonly Field CreateUserID = FindByName("CreateUserID");

        /// <summary>创建地址</summary>
        public static readonly Field CreateIP = FindByName("CreateIP");

        /// <summary>创建时间</summary>
        public static readonly Field CreateTime = FindByName("CreateTime");

        /// <summary>更新者</summary>
        public static readonly Field UpdateUser = FindByName("UpdateUser");

        /// <summary>更新人</summary>
        public static readonly Field UpdateUserID = FindByName("UpdateUserID");

        /// <summary>更新地址</summary>
        public static readonly Field UpdateIP = FindByName("UpdateIP");

        /// <summary>更新时间</summary>
        public static readonly Field UpdateTime = FindByName("UpdateTime");

        /// <summary>备注</summary>
        public static readonly Field Remark = FindByName("Remark");

        static Field FindByName(String name) => Meta.Table.FindByName(name);
    }

    /// <summary>取得文档字段名称的快捷方式</summary>
    public partial class __
    {
        /// <summary>编号</summary>
        public const String Id = "Id";

        /// <summary>编码。路径唯一标识，默认取Slug</summary>
        public const String Code = "Code";

        /// <summary>标题</summary>
        public const String Title = "Title";

        /// <summary>知识库</summary>
        public const String BookId = "BookId";

        /// <summary>启用</summary>
        public const String Enable = "Enable";

        /// <summary>排序。降序，数字越大越靠前</summary>
        public const String Sort = "Sort";

        /// <summary>用户</summary>
        public const String UserName = "UserName";

        /// <summary>路径</summary>
        public const String Slug = "Slug";

        /// <summary>格式</summary>
        public const String Format = "Format";

        /// <summary>公开。公开或私密</summary>
        public const String Public = "Public";

        /// <summary>正式。正式或草稿</summary>
        public const String Status = "Status";

        /// <summary>版本。草案版本</summary>
        public const String DraftVersion = "DraftVersion";

        /// <summary>点击量</summary>
        public const String Hits = "Hits";

        /// <summary>点赞数</summary>
        public const String Likes = "Likes";

        /// <summary>阅读数</summary>
        public const String Reads = "Reads";

        /// <summary>评论数</summary>
        public const String Comments = "Comments";

        /// <summary>本地点击</summary>
        public const String LocalHits = "LocalHits";

        /// <summary>总点击</summary>
        public const String TotalHits = "TotalHits";

        /// <summary>单词数</summary>
        public const String WordCount = "WordCount";

        /// <summary>封面</summary>
        public const String Cover = "Cover";

        /// <summary>摘要</summary>
        public const String Summary = "Summary";

        /// <summary>正文。Markdown格式</summary>
        public const String Body = "Body";

        /// <summary>HTML正文</summary>
        public const String BodyHtml = "BodyHtml";

        /// <summary>HTML。处理后的HTML内容，用于前端展示</summary>
        public const String Html = "Html";

        /// <summary>内容更新时间</summary>
        public const String ContentUpdateTime = "ContentUpdateTime";

        /// <summary>发布时间</summary>
        public const String PublishTime = "PublishTime";

        /// <summary>首次发布</summary>
        public const String FirstPublishTime = "FirstPublishTime";

        /// <summary>同步。是否自动同步远程内容</summary>
        public const String Sync = "Sync";

        /// <summary>同步时间。最后一次同步数据的时间</summary>
        public const String SyncTime = "SyncTime";

        /// <summary>追踪。用于记录调用链追踪标识，在星尘APM查找调用链</summary>
        public const String TraceId = "TraceId";

        /// <summary>创建者</summary>
        public const String CreateUser = "CreateUser";

        /// <summary>创建人</summary>
        public const String CreateUserID = "CreateUserID";

        /// <summary>创建地址</summary>
        public const String CreateIP = "CreateIP";

        /// <summary>创建时间</summary>
        public const String CreateTime = "CreateTime";

        /// <summary>更新者</summary>
        public const String UpdateUser = "UpdateUser";

        /// <summary>更新人</summary>
        public const String UpdateUserID = "UpdateUserID";

        /// <summary>更新地址</summary>
        public const String UpdateIP = "UpdateIP";

        /// <summary>更新时间</summary>
        public const String UpdateTime = "UpdateTime";

        /// <summary>备注</summary>
        public const String Remark = "Remark";
    }
    #endregion
}
