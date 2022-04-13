using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewLife.YuqueWeb.Services;
using NewLife.YuqueWeb.Entity;
using Xunit;
using NewLife;

namespace XUnitTest
{
    public class BookServiceTests
    {
        [Fact]
        public void ProcessHtml()
        {
            var doc = new Document
            {
                BodyHtml = File.ReadAllText("img.htm".GetFullPath())
            };

            var svc = new BookService(null);
            svc.ProcessHtml(doc);
        }

        [Fact]
        public void ProcessImage()
        {
            var doc = new Document
            {
                BodyHtml = File.ReadAllText("img.htm".GetFullPath())
            };
            var rule = new HtmlRule
            {
                Rule = "*",
            };

            var svc = new BookService(null);
            var html = svc.ProcessImage(doc, rule, doc.BodyHtml);
        }

        [Fact]
        public void ProcessLink()
        {
            var doc = new Document
            {
                BodyHtml = File.ReadAllText("img.htm".GetFullPath())
            };
            var rule = new HtmlRule
            {
                Rule = "https://gitee.com/NewLifeX/*",
                Target = "https://git.newlifex.com/NewLife/$1",
            };

            var html = doc.BodyHtml;
            Assert.Contains(rule.Rule.TrimEnd("*"), html);
            Assert.DoesNotContain(rule.Target.TrimEnd("$1"), html);
            Assert.Contains("<a href=\"https://gitee.com/NewLifeX/Stardust\" data-href=\"https://gitee.com/NewLifeX/Stardust\" target=\"_blank\" class=\"ne-link\"><span class=\"ne-text\">https://gitee.com/NewLifeX/Stardust</span></a>",html);

            var svc = new BookService(null);
            html = svc.ProcessLink(doc, rule, html);

            // 还有一个Url，不在链接里面
            Assert.Contains(rule.Rule.TrimEnd("*"), html);
            //Assert.DoesNotContain(rule.Rule.TrimEnd("*"), html);
            Assert.Contains(rule.Target.TrimEnd("$1"), html);
            Assert.Contains("<a href=\"https://git.newlifex.com/NewLife/Stardust\" data-href=\"https://git.newlifex.com/NewLife/Stardust\" target=\"_blank\" class=\"ne-link\"><span class=\"ne-text\">https://gitee.com/NewLifeX/Stardust</span></a>", html);
        }

        [Fact]
        public void ProcessText()
        {
            var doc = new Document
            {
                BodyHtml = File.ReadAllText("img.htm".GetFullPath())
            };
            var rule = new HtmlRule
            {
                Rule = "星尘代理",
                Target = "神策",
            };

            var html = doc.BodyHtml;
            Assert.Contains(rule.Rule, html);
            Assert.DoesNotContain(rule.Target, html);

            var svc = new BookService(null);
            html = svc.ProcessText(doc, rule, html);

            Assert.DoesNotContain(rule.Rule, html);
            Assert.Contains(rule.Target, html);
        }
    }
}