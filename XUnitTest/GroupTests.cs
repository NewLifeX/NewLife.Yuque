using NewLife.YuQue;
using Xunit;

namespace XUnitTest
{
    public class GroupTests
    {
        private readonly YuqueClient _client;
        public GroupTests() => _client = BasicTest.CreateClient();

        [Fact]
        public async void GetGroupById()
        {
            var list = await _client.GetGroups(1144030);
            Assert.NotNull(list);
            Assert.True(list.Length > 0);
        }

        [Fact]
        public async void GetGroupByName()
        {
            var list = await _client.GetGroups("smartstone");
            Assert.NotNull(list);
            Assert.True(list.Length > 0);
        }

        [Fact]
        public async void GetCurrentGroup()
        {
            var list = await _client.GetGroups();
            Assert.NotNull(list);
            Assert.True(list.Length > 0);
        }
    }
}