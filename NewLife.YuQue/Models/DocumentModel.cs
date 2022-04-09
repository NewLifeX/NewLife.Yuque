using System.Runtime.Serialization;

namespace NewLife.YuQue.Models;

/// <summary>
/// 文档模型
/// </summary>
public class DocumentModel
{
    /// <summary>标题</summary>
    public String Title { get; set; }

    /// <summary>文档路径</summary>
    public String Slug { get; set; }

    /// <summary>0 - 私密，1 - 公开</summary>
    public Int32 Public { get; set; }

    /// <summary>格式。支持 markdown、lake、html，默认为 markdown</summary>
    public String Format { get; set; }

    /// <summary>format 描述的正文内容，最大允许 5MB</summary>
    public String Body { get; set; }
}

/// <summary>
/// 文档模型
/// </summary>
public class DocumentModel2
{
    /// <summary>标题</summary>
    public String Title { get; set; }

    /// <summary>文档路径</summary>
    public String Slug { get; set; }

    /// <summary>0 - 私密，1 - 公开</summary>
    public Int32 Public { get; set; }

    /// <summary>format 描述的正文内容，最大允许 5MB</summary>
    public String Body { get; set; }

    /// <summary>强制更新</summary>
    /// <remarks>
    /// 如果在页面编辑过文档，那这时文档会转成 lake 格式，如果再用 markdown 无法进行更新，这时需要添加 _force_asl = 1 来确保内容的正确转换。
    /// </remarks>
    [DataMember(Name = "_force_asl")]
    public Int32 ForceAsl { get; set; }
}