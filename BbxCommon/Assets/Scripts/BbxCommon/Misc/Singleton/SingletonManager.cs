using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BbxCommon
{
    public class SingletonManager : Singleton<SingletonManager>
    {
        private List<IDestroySingleton> m_Singletons = new List<IDestroySingleton>();

        public void AddSingleton(IDestroySingleton singleton)
        {
            m_Singletons.Add(singleton);
        }

        public void ClearSingleton()
        {
            foreach (var singleton in m_Singletons)
            {
                singleton.DestroyStaticSingleton();
            }
            m_Singletons.Clear();
        }
    }
}
