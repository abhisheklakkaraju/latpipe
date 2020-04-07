using System;
using System.Collections.Generic;

namespace MvcTemplate.Components.Extensions
{
    public class MvcTreeNode
    {
        public Int64? Id { get; set; }
        public String Title { get; set; }
        public List<MvcTreeNode> Children { get; set; }

        public MvcTreeNode(Int64 id, String title)
            : this(title)
        {
            Id = id;
        }
        public MvcTreeNode(String title)
        {
            Title = title;
            Children = new List<MvcTreeNode>();
        }
    }
}
