using System.Runtime.Serialization;
using NewLife.Yuque.Models;

namespace NewLife.YuqueWeb.Models;

public class WebHookModel : DocumentDetail
{
    public String Path { get; set; }

    [DataMember(Name = "action_type")]
    public String ActionType { get; set; }

    public Boolean Publish { get; set; }

    //public DocumentDetail Data { get; set; }
}
