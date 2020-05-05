﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFramework.Singleton;

namespace TFramework.Save
{
    public class SaveFacade : AutoGeneratedSingleton<SaveFacade>
    {

        private Dictionary<string, SaveSystem> m_saveSystemDictionary = null;


        public SaveFacade()
        {
            if (m_saveSystemDictionary == null)
            {
                m_saveSystemDictionary = new Dictionary<string, SaveSystem>();
            }
            m_saveSystemDictionary.Clear();

        }

        public void RegisterSaveSystem(SaveSystem saveSystem)
        {
            if (!m_saveSystemDictionary.ContainsKey(saveSystem.systemName))
            {
                m_saveSystemDictionary.Add(saveSystem.systemName, saveSystem);
            }
            else
            {
                Console.WriteLine("SaveFacade Register Save System Error: System Name has Exist. name: " + saveSystem.systemName);
            }
        }

        public void Save(string systemName = null)
        {
            if (m_saveSystemDictionary.ContainsKey(systemName))
            {
                SaveSystem save = m_saveSystemDictionary[systemName];
                save.Save();
            }
            else
            {
                foreach (KeyValuePair<string, SaveSystem> item in m_saveSystemDictionary)
                {
                    item.Value.Save();
                }
            }
        }
    }
}
