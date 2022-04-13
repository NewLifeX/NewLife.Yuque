using NewLife.Yuque;
using Xunit;

namespace XUnitTest
{
    public class DocumentTests
    {
        private readonly YuqueClient _client;
        public DocumentTests() => _client = BasicTest.CreateClient();

        [Fact]
        public async void GetDocuments()
        {
            var list = await _client.GetDocuments(895145);
            Assert.NotNull(list);
            Assert.True(list.Length > 0);
        }

        [Fact]
        public async void GetDocuments2()
        {
            var list = await _client.GetDocuments("smartstone/nx");
            Assert.NotNull(list);
            Assert.True(list.Length > 0);
        }

        [Fact]
        public async void GetDocument()
        {
            var doc = await _client.GetDocument("smartstone/nx", "redis");
            Assert.NotNull(doc);
        }
    }
}