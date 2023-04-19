using NewLife.Log;
using NewLife.Yuque;
using NewLife.YuqueWeb.Entity;
using Group = NewLife.YuqueWeb.Entity.Group;

namespace NewLife.YuqueWeb.Services;

/// <summary>
/// 知识小组服务
/// </summary>
public class GroupService
{
    private readonly ITracer _tracer;

    /// <summary>
    /// 实例化知识库服务
    /// </summary>
    /// <param name="tracer"></param>
    public GroupService(ITracer tracer) => _tracer = tracer;

    /// <summary>
    /// 同步知识组
    /// </summary>
    /// <param name="groupId"></param>
    /// <returns></returns>
    public async Task<Int32> Sync(Int32 groupId)
    {
        using var span = _tracer?.NewSpan("SyncGroup", groupId);

        var group = Group.FindById(groupId);
        if (group == null) return 0;

        var client = new YuqueClient { Token = group.Token, Log = XTrace.Log, Tracer = _tracer };

        // 同步详细
        if (group.Type == "group")
        {
            var gp = groupId > 100 ?
                await client.GetGroup(groupId) :
                await client.GetGroup(group.Code);
            if (gp != null)
            {
                // 需要更新Id
                if (group.Id != gp.Id)
                {
                    Group.Update(Group._.Id == gp.Id, Group._.Id == group.Id);

                    group.Id = gp.Id;
                }

                if (group.Name.IsNullOrEmpty()) group.Name = gp.Name;
                group.Code = gp.Login;
                group.Public = gp.Public > 0;
                group.Books = gp.Books;
                group.Topics = gp.Topics;
                group.Members = gp.Members;
                group.Remark = gp.Description;
                group.CreateTime = gp.CreateTime;
                group.UpdateTime = gp.UpdateTime;
                //group.TraceId = span?.TraceId;

                group.Update();
            }
        }
        else
        {
            var user = groupId > 100 ?
                await client.GetUser(groupId) :
                await client.GetUser(group.Code);
            if (user != null)
            {
                // 需要更新Id
                if (group.Id != user.Id)
                {
                    Group.Update(Group._.Id == user.Id, Group._.Id == group.Id);

                    group.Id = user.Id;
                }

                if (group.Name.IsNullOrEmpty()) group.Name = user.Name;
                group.Code = user.Login;
                group.Public = user.Public > 0;
                group.Books = user.Books;
                //group.Topics = user.Topics;
                //group.Members = user.Members;
                group.Remark = user.Description;
                group.CreateTime = user.CreateTime;
                group.UpdateTime = user.UpdateTime;
                //group.TraceId = span?.TraceId;

                group.Update();
            }
        }

        var count = 0;
        var offset = 0;
        while (true)
        {
            // 分批拉取
            var list = group.Type == "group" ?
                await client.GetGroupRepos(group.Id, null, offset) :
                await client.GetRepos(group.Id, null, offset);
            if (list.Length == 0) break;

            foreach (var repo in list)
            {
                var book = Book.FindById(repo.Id);
                book ??= new Book { Id = repo.Id, Enable = group.Enable, Sync = repo.Public > 0 };

                book.Fill(repo);
                book.GroupId = group.Id;
                book.SyncTime = DateTime.Now;
                //book.TraceId = span?.TraceId;

                book.Save();
            }

            count += list.Length;
            offset += list.Length;
        }

        return count;
    }
}