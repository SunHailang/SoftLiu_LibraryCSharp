using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFramework.Save
{
    public abstract class SaveSystem
    {
        protected string m_systemName = string.Empty;

        public string systemName { get { return m_systemName; } }

        protected SaveData m_saveData = null;
        public SaveData saveData
        {
            set { m_saveData = value; }
        }






        public virtual void Reset() { }
        public virtual void Load() { }
        public virtual void Save() { }

    }
}
