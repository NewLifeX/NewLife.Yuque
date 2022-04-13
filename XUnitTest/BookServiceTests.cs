using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewLife.YuqueWeb.Services;
using NewLife.YuQueWeb.Entity;
using Xunit;

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
    }
}