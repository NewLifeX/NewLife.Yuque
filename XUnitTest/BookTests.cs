using NewLife.YuQue;
using Xunit;

namespace XUnitTest
{
    public class BookTests
    {
        private readonly YuqueClient _client;
        public BookTests() => _client = BasicTest.CreateClient();

        [Fact]
        public async void GetRepos()
        {
            var list = await _client.GetRepos(1144030);
            Assert.NotNull(list);
            Assert.True(list.Length > 0);
        }

        [Fact]
        public async void GetRepos2()
        {
            var list = await _client.GetGroupRepos("newlifex");
            Assert.NotNull(list);
            Assert.True(list.Length > 0);
        }
    }
}