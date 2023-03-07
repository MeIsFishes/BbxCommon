using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BbxCommon
{
    public interface ISingleton
    {
        void InitSingleton();
        void DestroySingleton();
    }

    public class SingletonManager : Singleton<SingletonManager>
    {
        private List<ISingleton> m_Singletons = new List<ISingleton>();

        public void AddSingleton(ISingleton singleton)
        {
            m_Singletons.Add(singleton);
            singleton.InitSingleton();
        }

        public void ClearSingleton()
        {
            foreach (var singleton in m_Singletons)
            {
                singleton.DestroySingleton();
            }
            m_Singletons.Clear();
        }
    }
}
