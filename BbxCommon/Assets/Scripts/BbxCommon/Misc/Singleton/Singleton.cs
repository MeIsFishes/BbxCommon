using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BbxCommon
{
    // You can simply implement a singleton by keeping it inherits this class.
    // But notice that, this class request a new() function, so it cannot refuse
    // creating the second instance by call constructor. In that case, it is an
    // unsafe singleton class.

    // Warning!! If you are writing a singleton for a monobehavior, see MonoSingleton.
    
    public class Singleton<T> : IDestroySingleton where T : Singleton<T>, new()
    {
        protected static T m_s_SingleInstance;

        public static T Instance
        {
            get
            {
                if (m_s_SingleInstance == null)
                {
                    m_s_SingleInstance = new T();
                    SingletonManager.Instance.AddSingleton(m_s_SingleInstance);
                }
                return m_s_SingleInstance;
            }
        }

        void IDestroySingleton.DestroyStaticSingleton()
        {
            OnSingletonDestroy();
            m_s_SingleInstance = null;
        }

        protected virtual void OnSingletonDestroy()
        {

        }
    }
}
