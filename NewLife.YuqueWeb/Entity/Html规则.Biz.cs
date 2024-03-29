﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using NewLife;
using NewLife.Data;
using NewLife.Log;
using NewLife.Model;
using NewLife.Reflection;
using NewLife.Threading;
using NewLife.Web;
using XCode;
using XCode.Cache;
using XCode.Configuration;
using XCode.DataAccessLayer;
using XCode.Membership;
using XCode.Shards;

namespace NewLife.YuqueWeb.Entity
{
    /// <summary>Html规则。用于替换Html中的连接或字符串</summary>
    public partial class HtmlRule : Entity<HtmlRule>
    {
        #region 对象操作
        static HtmlRule()
        {
            // 累加字段，生成 Update xx Set Count=Count+1234 Where xxx
            //var df = Meta.Factory.AdditionalFields;
            //df.Add(nameof(Kind));

            // 过滤器 UserModule、TimeModule、IPModule
            Meta.Modules.Add<UserModule>();
            Meta.Modules.Add<TimeModule>();
            Meta.Modules.Add<IPModule>();
        }

        /// <summary>验证并修补数据，通过抛出异常的方式提示验证失败。</summary>
        /// <param name="isNew">是否插入</param>
        public override void Valid(Boolean isNew)
        {
            // 如果没有脏数据，则不需要进行任何处理
            if (!HasDirty) return;

            // 建议先调用基类方法，基类方法会做一些统一处理
            base.Valid(isNew);

            // 在新插入数据或者修改了指定字段时进行修正
            // 处理当前已登录用户信息，可以由UserModule过滤器代劳
            /*var user = ManageProvider.User;
            if (user != null)
            {
                if (isNew && !Dirtys[nameof(CreateUserID)]) CreateUserID = user.ID;
                if (!Dirtys[nameof(UpdateUserID)]) UpdateUserID = user.ID;
            }*/
            //if (isNew && !Dirtys[nameof(CreateTime)]) CreateTime = DateTime.Now;
            //if (!Dirtys[nameof(UpdateTime)]) UpdateTime = DateTime.Now;
            //if (isNew && !Dirtys[nameof(CreateIP)]) CreateIP = ManageProvider.UserHost;
            //if (!Dirtys[nameof(UpdateIP)]) UpdateIP = ManageProvider.UserHost;
        }

        /// <summary>首次连接数据库时初始化数据，仅用于实体类重载，用户不应该调用该方法</summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override void InitData()
        {
            // InitData一般用于当数据表没有数据时添加一些默认数据，该实体类的任何第一次数据库操作都会触发该方法，默认异步调用
            if (Meta.Session.Count > 0) return;

            if (XTrace.Debug) XTrace.WriteLine("开始初始化HtmlRule[Html规则]数据……");

            var entity = new HtmlRule
            {
                Kind = RuleKinds.图片,
                Rule = "*",
                Enable = true,
                Remark = "抓取所有域名的图片到本地",
            };
            entity.Insert();

            entity = new HtmlRule
            {
                Kind = RuleKinds.图片,
                Rule = "cdn.nlark.com",
                Enable = false,
                Remark = "抓取指定域名的图片到本地",
            };
            entity.Insert();

            entity = new HtmlRule
            {
                Kind = RuleKinds.超链接,
                Rule = "https://www.yuque.com/newlife/*",
                Target = "https://www.newlifex.com/$1",
                Enable = true,
                Remark = "替换所有连接字符串到本地",
            };
            entity.Insert();

            entity = new HtmlRule
            {
                Kind = RuleKinds.文本,
                Rule = "语雀文档",
                Target = "文档",
                Enable = true,
                Remark = "替换所有文本",
            };
            entity.Insert();

            if (XTrace.Debug) XTrace.WriteLine("完成初始化HtmlRule[Html规则]数据！");
        }
        #endregion

        #region 扩展属性
        #endregion

        #region 扩展查询
        /// <summary>根据编号查找</summary>
        /// <param name="id">编号</param>
        /// <returns>实体对象</returns>
        public static HtmlRule FindById(Int32 id)
        {
            if (id <= 0) return null;

            // 实体缓存
            if (Meta.Session.Count < 1000) return Meta.Cache.Find(e => e.Id == id);

            // 单对象缓存
            return Meta.SingleCache[id];

            //return Find(_.Id == id);
        }

        /// <summary>根据种类查找</summary>
        /// <param name="kind">种类</param>
        /// <returns>实体列表</returns>
        public static IList<HtmlRule> FindAllByKind(RuleKinds kind)
        {
            // 实体缓存
            if (Meta.Session.Count < 1000) return Meta.Cache.FindAll(e => e.Kind == kind);

            return FindAll(_.Kind == kind);
        }

        /// <summary>
        /// 获取有效规则，优先级降序
        /// </summary>
        /// <returns></returns>
        public static IList<HtmlRule> GetValids()=> FindAllWithCache().Where(e => e.Enable).OrderByDescending(e => e.Priority).ToList();
        #endregion

        #region 高级查询
        /// <summary>高级查询</summary>
        /// <param name="kind">种类。图片、链接、文本</param>
        /// <param name="start">更新时间开始</param>
        /// <param name="end">更新时间结束</param>
        /// <param name="key">关键字</param>
        /// <param name="page">分页参数信息。可携带统计和数据权限扩展查询等信息</param>
        /// <returns>实体列表</returns>
        public static IList<HtmlRule> Search(Int32 kind, DateTime start, DateTime end, String key, PageParameter page)
        {
            var exp = new WhereExpression();

            if (kind >= 0) exp &= _.Kind == kind;
            exp &= _.UpdateTime.Between(start, end);
            if (!key.IsNullOrEmpty()) exp &= _.Rule.Contains(key) | _.Target.Contains(key) | _.CreateUser.Contains(key) | _.CreateIP.Contains(key) | _.UpdateUser.Contains(key) | _.UpdateIP.Contains(key) | _.Remark.Contains(key);

            return FindAll(exp, page);
        }
        #endregion

        #region 业务操作
        #endregion
    }
}