using System;
using System.Collections.Generic;
using System.Text;

namespace NewLife.YuQue.Models
{
    /// <summary>
    /// 知识库模型
    /// </summary>
    public class BookModel
    {
        /// <summary>名称</summary>
        public String Name { get; set; }

        /// <summary>文档路径</summary>
        public String Slug { get; set; }

        /// <summary>描述</summary>
        public String Description { get; set; }

        /// <summary>‘Book’ 文库, ‘Design’ 画板, 请注意大小写。仅用于新建</summary>
        public String Type { get; set; }

        /// <summary>0私密，1公开，2空间成员可见，3空间所有人（含外部联系人）可见，4知识库成员可见</summary>
        public Int32 Public { get; set; }

        /// <summary>更新文档仓库的目录信息。仅用于更新</summary>
        public String Toc { get; set; }
    }
}