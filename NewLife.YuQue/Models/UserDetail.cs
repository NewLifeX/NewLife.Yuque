using System.Runtime.Serialization;

namespace NewLife.YuQue.Models
{
    /// <summary>用户信息</summary>
    public class UserDetail
    {
        /// <summary>编号</summary>
        public Int32 Id { get; set; }

        /// <summary>名称</summary>
        public String Name { get; set; }

        /// <summary>类型</summary>
        public String Type { get; set; }

        /// <summary>登录名</summary>
        public String Login { get; set; }

        /// <summary>描述</summary>
        public String Description { get; set; }

        /// <summary>头像</summary>
        [DataMember(Name = "avatar_url")]
        public String Avatar { get; set; }

        /// <summary>创建时间</summary>
        [DataMember(Name = "created_at")]
        public DateTime CreateTime { get; set; }

        /// <summary>更新时间</summary>
        [DataMember(Name = "updated_at")]
        public DateTime UpdateTime { get; set; }
    }
}