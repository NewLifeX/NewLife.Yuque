using System.Runtime.Serialization;

namespace NewLife.Yuque.Models
{
    /// <summary>组织用户</summary>
    public class GroupUser
    {
        /// <summary>编号</summary>
        public Int32 Id { get; set; }

        /// <summary>组织</summary>
        [DataMember(Name = "group_id")]
        public Int32 GroupId { get; set; }

        /// <summary>用户</summary>
        [DataMember(Name = "user_id")]
        public Int32 UserId { get; set; }

        /// <summary>用户</summary>
        public UserDetail User { get; set; }

        /// <summary>角色</summary>
        public Int32 Role { get; set; }

        /// <summary>可见</summary>
        public Int32 Visibility { get; set; }

        /// <summary>状态</summary>
        public Int32 Status { get; set; }

        /// <summary>组织</summary>
        public GroupDetail Group { get; set; }

        /// <summary>创建时间</summary>
        [DataMember(Name = "created_at")]
        public DateTime CreateTime { get; set; }

        /// <summary>更新时间</summary>
        [DataMember(Name = "updated_at")]
        public DateTime UpdateTime { get; set; }

        /// <summary>已重载。友好显示</summary>
        /// <returns></returns>
        public override String ToString() => $"{User}";
    }
}