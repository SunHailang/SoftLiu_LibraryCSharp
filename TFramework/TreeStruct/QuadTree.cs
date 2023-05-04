using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFramework.TreeStruct
{
    public class QuadTree<T> : IDisposable
    {
        private int m_capacity = 0;
        private HashSet<T> values = null;

        private bool m_divided = false;

        private QuadTree<T> m_northeastTree;
        private QuadTree<T> m_northwestTree;
        private QuadTree<T> m_southeastTree;
        private QuadTree<T> m_southwestTree;

        public QuadTree(int count = 4)
        {
            m_capacity = count;
            this.values = new HashSet<T>(m_capacity);
        }

        public void Inster(T point)
        {

        }

        public void Dispose()
        {

        }

    }
}
