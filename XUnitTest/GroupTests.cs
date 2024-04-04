using NewLife.Yuque;
using Xunit;

namespace XUnitTest;

public class GroupTests
{
    private readonly YuqueClient _client;
    public GroupTests() => _client = BasicTest.CreateClient();

    [Fact]
    public async void GetUserGroupById()
    {
        var list = await _client.GetUserGroups(1144030);
        Assert.NotNull(list);
        Assert.True(list.Length > 0);
    }

    [Fact]
    public async void GetUserGroupByName()
    {
        var list = await _client.GetUserGroups("smartstone");
        Assert.NotNull(list);
        Assert.True(list.Length > 0);
    }

    //[Fact]
    //public async void GetCurrentGroup()
    //{
    //    var list = await _client.GetGroups();
    //    Assert.NotNull(list);
    //    Assert.True(list.Length > 0);
    //}

    //[Fact]
    //public async void GetGroupById()
    //{
    //    var group = await _client.GetGroup(1144035);
    //    Assert.NotNull(group);

    //    Assert.Equal("新生命", group.Name);
    //}

    //[Fact]
    //public async void GetGroupByName()
    //{
    //    var group = await _client.GetGroup("newlifex");
    //    Assert.NotNull(group);

    //    Assert.Equal("新生命", group.Name);
    //}

    //[Fact(Skip = "不修改数据")]
    //public async void CreateGroup()
    //{
    //    var group = await _client.CreateGroup("新生命", "newlifex", null);
    //    Assert.NotNull(group);

    //    Assert.Equal("新生命", group.Name);
    //}

    [Fact]
    public async void GetGroupUsers()
    {
        var list = await _client.GetGroupUsers("newlifex");
        Assert.NotNull(list);
    }
}