using NewLife.YuQue;
using Xunit;

namespace XUnitTest
{
    public class UserTests
    {
        private readonly YuqueClient _client;
        public UserTests() => _client = BasicTest.CreateClient();

        [Fact]
        public async void Test1()
        {
            {
                var user = await _client.GetUser("smartstone");
                Assert.NotNull(user);
            }
            {
                var user = await _client.GetUser(1234);
                Assert.NotNull(user);
            }
        }
    }
}