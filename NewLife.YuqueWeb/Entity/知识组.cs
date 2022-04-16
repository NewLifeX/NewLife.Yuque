using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using XCode;
using XCode.Configuration;
using XCode.DataAccessLayer;

namespace NewLife.YuqueWeb.Entity
{
    /// <summary>知识组。管理用户或团队的令牌等</summary>
    [Serializable]
    [DataObject]
    [Description("知识组。管理用户或团队的令牌等")]
    [BindIndex("IU_Group_Code", true, "Code")]
    [BindIndex("IX_Group_Name", false, "Name")]
    [BindTable("Group", Description = "知识组。管理用户或团队的令牌等", ConnName = "Yuque", DbType = DatabaseType.None)]
    public partial class Group
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

        private Int32 _Books;
        /// <summary>知识库数</summary>
        [DisplayName("知识库数")]
        [Description("知识库数")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("Books", "知识库数", "")]
        public Int32 Books { get => _Books; set { if (OnPropertyChanging("Books", value)) { _Books = value; OnPropertyChanged("Books"); } } }

        private Int32 _Topics;
        /// <summary>主题数</summary>
        [DisplayName("主题数")]
        [Description("主题数")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("Topics", "主题数", "")]
        public Int32 Topics { get => _Topics; set { if (OnPropertyChanging("Topics", value)) { _Topics = value; OnPropertyChanged("Topics"); } } }

        private Int32 _Members;
        /// <summary>成员数</summary>
        [DisplayName("成员数")]
        [Description("成员数")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("Members", "成员数", "")]
        public Int32 Members { get => _Members; set { if (OnPropertyChanging("Members", value)) { _Members = value; OnPropertyChanged("Members"); } } }

        private Boolean _Public;
        /// <summary>公开。公开或私密</summary>
        [DisplayName("公开")]
        [Description("公开。公开或私密")]
        [DataObjectField(false, false, false, 0)]
        [BindColumn("Public", "公开。公开或私密", "")]
        public Boolean Public { get => _Public; set { if (OnPropertyChanging("Public", value)) { _Public = value; OnPropertyChanged("Public"); } } }

        private String _Token;
        /// <summary>令牌</summary>
        [DisplayName("令牌")]
        [Description("令牌")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn("Token", "令牌", "")]
        public String Token { get => _Token; set { if (OnPropertyChanging("Token", value)) { _Token = value; OnPropertyChanged("Token"); } } }

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
                    case "Books": return _Books;
                    case "Topics": return _Topics;
                    case "Members": return _Members;
                    case "Public": return _Public;
                    case "Token": return _Token;
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
                    case "Books": _Books = value.ToInt(); break;
                    case "Topics": _Topics = value.ToInt(); break;
                    case "Members": _Members = value.ToInt(); break;
                    case "Public": _Public = value.ToBoolean(); break;
                    case "Token": _Token = Convert.ToString(value); break;
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
        /// <summary>取得知识组字段信息的快捷方式</summary>
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

            /// <summary>知识库数</summary>
            public static readonly Field Books = FindByName("Books");

            /// <summary>主题数</summary>
            public static readonly Field Topics = FindByName("Topics");

            /// <summary>成员数</summary>
            public static readonly Field Members = FindByName("Members");

            /// <summary>公开。公开或私密</summary>
            public static readonly Field Public = FindByName("Public");

            /// <summary>令牌</summary>
            public static readonly Field Token = FindByName("Token");

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

        /// <summary>取得知识组字段名称的快捷方式</summary>
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

            /// <summary>知识库数</summary>
            public const String Books = "Books";

            /// <summary>主题数</summary>
            public const String Topics = "Topics";

            /// <summary>成员数</summary>
            public const String Members = "Members";

            /// <summary>公开。公开或私密</summary>
            public const String Public = "Public";

            /// <summary>令牌</summary>
            public const String Token = "Token";

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