using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using XCode;
using XCode.Configuration;
using XCode.DataAccessLayer;

namespace NewLife.YuQueWeb.Entity
{
    /// <summary>知识库。管理知识库</summary>
    [Serializable]
    [DataObject]
    [Description("知识库。管理知识库")]
    [BindIndex("IU_Book_Code", true, "Code")]
    [BindIndex("IX_Book_Name", false, "Name")]
    [BindTable("Book", Description = "知识库。管理知识库", ConnName = "YuQue", DbType = DatabaseType.None)]
    public partial class Book
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

        private String _Name;
        /// <summary>名称</summary>
        [DisplayName("名称")]
        [Description("名称")]
        [DataObjectField(false, false, false, 50)]
        [BindColumn("Name", "名称", "", Master = true)]
        public String Name { get => _Name; set { if (OnPropertyChanging("Name", value)) { _Name = value; OnPropertyChanged("Name"); } } }

        private String _Type;
        /// <summary>类型</summary>
        [DisplayName("类型")]
        [Description("类型")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn("Type", "类型", "")]
        public String Type { get => _Type; set { if (OnPropertyChanging("Type", value)) { _Type = value; OnPropertyChanged("Type"); } } }

        private Boolean _Enable;
        /// <summary>启用</summary>
        [DisplayName("启用")]
        [Description("启用")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("Enable", "启用", "")]
        public Boolean Enable { get => _Enable; set { if (OnPropertyChanging("Enable", value)) { _Enable = value; OnPropertyChanged("Enable"); } } }

        private String _UserName;
        /// <summary>用户</summary>
        [DisplayName("用户")]
        [Description("用户")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn("UserName", "用户", "")]
        public String UserName { get => _UserName; set { if (OnPropertyChanging("UserName", value)) { _UserName = value; OnPropertyChanged("UserName"); } } }

        private Int32 _Docs;
        /// <summary>文章数</summary>
        [DisplayName("文章数")]
        [Description("文章数")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("Docs", "文章数", "")]
        public Int32 Docs { get => _Docs; set { if (OnPropertyChanging("Docs", value)) { _Docs = value; OnPropertyChanged("Docs"); } } }

        private Int32 _Likes;
        /// <summary>点赞数</summary>
        [DisplayName("点赞数")]
        [Description("点赞数")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("Likes", "点赞数", "")]
        public Int32 Likes { get => _Likes; set { if (OnPropertyChanging("Likes", value)) { _Likes = value; OnPropertyChanged("Likes"); } } }

        private Int32 _Watches;
        /// <summary>订阅数</summary>
        [DisplayName("订阅数")]
        [Description("订阅数")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("Watches", "订阅数", "")]
        public Int32 Watches { get => _Watches; set { if (OnPropertyChanging("Watches", value)) { _Watches = value; OnPropertyChanged("Watches"); } } }

        private Boolean _Sync;
        /// <summary>同步。是否自动同步远程内容</summary>
        [DisplayName("同步")]
        [Description("同步。是否自动同步远程内容")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("Sync", "同步。是否自动同步远程内容", "")]
        public Boolean Sync { get => _Sync; set { if (OnPropertyChanging("Sync", value)) { _Sync = value; OnPropertyChanged("Sync"); } } }

        private String _Slug;
        /// <summary>路径</summary>
        [DisplayName("路径")]
        [Description("路径")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn("Slug", "路径", "")]
        public String Slug { get => _Slug; set { if (OnPropertyChanging("Slug", value)) { _Slug = value; OnPropertyChanged("Slug"); } } }

        private String _Namespace;
        /// <summary>全路径</summary>
        [DisplayName("全路径")]
        [Description("全路径")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn("Namespace", "全路径", "")]
        public String Namespace { get => _Namespace; set { if (OnPropertyChanging("Namespace", value)) { _Namespace = value; OnPropertyChanged("Namespace"); } } }

        private DateTime _SyncTime;
        /// <summary>同步时间。最后一次同步数据的时间</summary>
        [DisplayName("同步时间")]
        [Description("同步时间。最后一次同步数据的时间")]
        [DataObjectField(false, false, true, 0)]
        [BindColumn("SyncTime", "同步时间。最后一次同步数据的时间", "")]
        public DateTime SyncTime { get => _SyncTime; set { if (OnPropertyChanging("SyncTime", value)) { _SyncTime = value; OnPropertyChanged("SyncTime"); } } }

        private String _CreateUser;
        /// <summary>创建者</summary>
        [DisplayName("创建者")]
        [Description("创建者")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn("CreateUser", "创建者", "")]
        public String CreateUser { get => _CreateUser; set { if (OnPropertyChanging("CreateUser", value)) { _CreateUser = value; OnPropertyChanged("CreateUser"); } } }

        private Int32 _CreateUserID;
        /// <summary>创建人</summary>
        [DisplayName("创建人")]
        [Description("创建人")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("CreateUserID", "创建人", "")]
        public Int32 CreateUserID { get => _CreateUserID; set { if (OnPropertyChanging("CreateUserID", value)) { _CreateUserID = value; OnPropertyChanged("CreateUserID"); } } }

        private String _CreateIP;
        /// <summary>创建地址</summary>
        [DisplayName("创建地址")]
        [Description("创建地址")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn("CreateIP", "创建地址", "")]
        public String CreateIP { get => _CreateIP; set { if (OnPropertyChanging("CreateIP", value)) { _CreateIP = value; OnPropertyChanged("CreateIP"); } } }

        private DateTime _CreateTime;
        /// <summary>创建时间</summary>
        [DisplayName("创建时间")]
        [Description("创建时间")]
        [DataObjectField(false, false, true, 0)]
        [BindColumn("CreateTime", "创建时间", "")]
        public DateTime CreateTime { get => _CreateTime; set { if (OnPropertyChanging("CreateTime", value)) { _CreateTime = value; OnPropertyChanged("CreateTime"); } } }

        private String _UpdateUser;
        /// <summary>更新者</summary>
        [DisplayName("更新者")]
        [Description("更新者")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn("UpdateUser", "更新者", "")]
        public String UpdateUser { get => _UpdateUser; set { if (OnPropertyChanging("UpdateUser", value)) { _UpdateUser = value; OnPropertyChanged("UpdateUser"); } } }

        private Int32 _UpdateUserID;
        /// <summary>更新人</summary>
        [DisplayName("更新人")]
        [Description("更新人")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("UpdateUserID", "更新人", "")]
        public Int32 UpdateUserID { get => _UpdateUserID; set { if (OnPropertyChanging("UpdateUserID", value)) { _UpdateUserID = value; OnPropertyChanged("UpdateUserID"); } } }

        private String _UpdateIP;
        /// <summary>更新地址</summary>
        [DisplayName("更新地址")]
        [Description("更新地址")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn("UpdateIP", "更新地址", "")]
        public String UpdateIP { get => _UpdateIP; set { if (OnPropertyChanging("UpdateIP", value)) { _UpdateIP = value; OnPropertyChanged("UpdateIP"); } } }

        private DateTime _UpdateTime;
        /// <summary>更新时间</summary>
        [DisplayName("更新时间")]
        [Description("更新时间")]
        [DataObjectField(false, false, true, 0)]
        [BindColumn("UpdateTime", "更新时间", "")]
        public DateTime UpdateTime { get => _UpdateTime; set { if (OnPropertyChanging("UpdateTime", value)) { _UpdateTime = value; OnPropertyChanged("UpdateTime"); } } }

        private String _Remark;
        /// <summary>备注</summary>
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
            get
            {
                switch (name)
                {
                    case "Id": return _Id;
                    case "Code": return _Code;
                    case "Name": return _Name;
                    case "Type": return _Type;
                    case "Enable": return _Enable;
                    case "UserName": return _UserName;
                    case "Docs": return _Docs;
                    case "Likes": return _Likes;
                    case "Watches": return _Watches;
                    case "Sync": return _Sync;
                    case "Slug": return _Slug;
                    case "Namespace": return _Namespace;
                    case "SyncTime": return _SyncTime;
                    case "CreateUser": return _CreateUser;
                    case "CreateUserID": return _CreateUserID;
                    case "CreateIP": return _CreateIP;
                    case "CreateTime": return _CreateTime;
                    case "UpdateUser": return _UpdateUser;
                    case "UpdateUserID": return _UpdateUserID;
                    case "UpdateIP": return _UpdateIP;
                    case "UpdateTime": return _UpdateTime;
                    case "Remark": return _Remark;
                    default: return base[name];
                }
            }
            set
            {
                switch (name)
                {
                    case "Id": _Id = value.ToInt(); break;
                    case "Code": _Code = Convert.ToString(value); break;
                    case "Name": _Name = Convert.ToString(value); break;
                    case "Type": _Type = Convert.ToString(value); break;
                    case "Enable": _Enable = value.ToBoolean(); break;
                    case "UserName": _UserName = Convert.ToString(value); break;
                    case "Docs": _Docs = value.ToInt(); break;
                    case "Likes": _Likes = value.ToInt(); break;
                    case "Watches": _Watches = value.ToInt(); break;
                    case "Sync": _Sync = value.ToBoolean(); break;
                    case "Slug": _Slug = Convert.ToString(value); break;
                    case "Namespace": _Namespace = Convert.ToString(value); break;
                    case "SyncTime": _SyncTime = value.ToDateTime(); break;
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

        #region 字段名
        /// <summary>取得知识库字段信息的快捷方式</summary>
        public partial class _
        {
            /// <summary>编号</summary>
            public static readonly Field Id = FindByName("Id");

            /// <summary>编码。路径唯一标识，默认取Slug</summary>
            public static readonly Field Code = FindByName("Code");

            /// <summary>名称</summary>
            public static readonly Field Name = FindByName("Name");

            /// <summary>类型</summary>
            public static readonly Field Type = FindByName("Type");

            /// <summary>启用</summary>
            public static readonly Field Enable = FindByName("Enable");

            /// <summary>用户</summary>
            public static readonly Field UserName = FindByName("UserName");

            /// <summary>文章数</summary>
            public static readonly Field Docs = FindByName("Docs");

            /// <summary>点赞数</summary>
            public static readonly Field Likes = FindByName("Likes");

            /// <summary>订阅数</summary>
            public static readonly Field Watches = FindByName("Watches");

            /// <summary>同步。是否自动同步远程内容</summary>
            public static readonly Field Sync = FindByName("Sync");

            /// <summary>路径</summary>
            public static readonly Field Slug = FindByName("Slug");

            /// <summary>全路径</summary>
            public static readonly Field Namespace = FindByName("Namespace");

            /// <summary>同步时间。最后一次同步数据的时间</summary>
            public static readonly Field SyncTime = FindByName("SyncTime");

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

        /// <summary>取得知识库字段名称的快捷方式</summary>
        public partial class __
        {
            /// <summary>编号</summary>
            public const String Id = "Id";

            /// <summary>编码。路径唯一标识，默认取Slug</summary>
            public const String Code = "Code";

            /// <summary>名称</summary>
            public const String Name = "Name";

            /// <summary>类型</summary>
            public const String Type = "Type";

            /// <summary>启用</summary>
            public const String Enable = "Enable";

            /// <summary>用户</summary>
            public const String UserName = "UserName";

            /// <summary>文章数</summary>
            public const String Docs = "Docs";

            /// <summary>点赞数</summary>
            public const String Likes = "Likes";

            /// <summary>订阅数</summary>
            public const String Watches = "Watches";

            /// <summary>同步。是否自动同步远程内容</summary>
            public const String Sync = "Sync";

            /// <summary>路径</summary>
            public const String Slug = "Slug";

            /// <summary>全路径</summary>
            public const String Namespace = "Namespace";

            /// <summary>同步时间。最后一次同步数据的时间</summary>
            public const String SyncTime = "SyncTime";

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
}