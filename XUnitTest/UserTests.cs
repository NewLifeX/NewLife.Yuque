using NewLife.Yuque;
using Xunit;

namespace XUnitTest
{
    public class UserTests
    {
        private readonly YuqueClient _client;
        public UserTests() => _client = BasicTest.CreateClient();

        [Fact]
        public async void GetUserById()
        {
            var user = await _client.GetUser(1144030);
            Assert.NotNull(user);

            Assert.Equal(1144030, user.Id);
            Assert.Equal("User", user.Type);
            Assert.Equal(0, user.SpaceId);
            Assert.Equal(915593, user.AccountId);
            Assert.Equal("smartstone", user.Login);
            Assert.Equal("大石头", user.Name);

            Assert.NotEmpty(user.Avatar);
            Assert.NotEmpty(user.Description);

            Assert.True(user.Books > 0);
            Assert.True(user.PublicBooks > 0);
            Assert.True(user.Followers > 0);
            Assert.True(user.Following >= 0);
            Assert.True(user.Public > 0);

            Assert.True(user.CreateTime.Year >= 2020);
            Assert.True(user.UpdateTime.Year >= 2022);
        }

        [Fact]
        public async void GetUserByName()
        {
            var user = await _client.GetUser("smartstone");
            Assert.NotNull(user);

            Assert.Equal(1144030, user.Id);
            Assert.Equal("User", user.Type);
            Assert.Equal(0, user.SpaceId);
            Assert.Equal(915593, user.AccountId);
            Assert.Equal("smartstone", user.Login);
            Assert.Equal("大石头", user.Name);

            Assert.NotEmpty(user.Avatar);
            Assert.NotEmpty(user.Description);

            Assert.True(user.Books > 0);
            Assert.True(user.PublicBooks > 0);
            Assert.True(user.Followers > 0);
            Assert.True(user.Following >= 0);
            Assert.True(user.Public > 0);

            Assert.True(user.CreateTime.Year >= 2020);
            Assert.True(user.UpdateTime.Year >= 2022);
        }

        [Fact]
        public async void GetCurrentUser()
        {
            var user = await _client.GetUser();
            Assert.NotNull(user);

            Assert.Equal(1144030, user.Id);
            Assert.Equal("User", user.Type);
            Assert.Equal(0, user.SpaceId);
            Assert.Equal(915593, user.AccountId);
            Assert.Equal("smartstone", user.Login);
            Assert.Equal("大石头", user.Name);

            Assert.NotEmpty(user.Avatar);
            Assert.NotEmpty(user.Description);

            Assert.True(user.Books > 0);
            Assert.True(user.PublicBooks > 0);
            Assert.True(user.Followers > 0);
            Assert.True(user.Following >= 0);
            Assert.True(user.Public > 0);

            Assert.True(user.CreateTime.Year >= 2020);
            Assert.True(user.UpdateTime.Year >= 2022);
        }
    }
}