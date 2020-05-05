using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFramework.TreeStruct
{
    public class TreeNode
    {
        int data;
        TreeNode firstchild;
        TreeNode rightsib;
    }

    public abstract class Tree
    {
        protected Dictionary<string, TreeNode> m_treeNodeDic = new Dictionary<string, TreeNode>();

        public virtual void InitTree() { }

        public virtual void DestroyTree() { }

        public virtual void ClearTree() { }

        public virtual void TreeIsEmpty() { }

        public virtual int TreeDepth() { return 0; }

        public virtual void Value() { }
    }
}
