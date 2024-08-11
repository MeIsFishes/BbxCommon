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
    
    public class Singleton<T> : ISingleton where T : Singleton<T>, new()
    {
        private static T m_SingleInstance;

        public static T Instance
        {
            get
            {
                if (m_SingleInstance == null)
                {
                    m_SingleInstance = new T();
                    SingletonManager.Instance.AddSingleton(m_SingleInstance);
                }
                return m_SingleInstance;
            }
        }

        public void InitSingleton()
        {
            OnSingletonInit();
        }

        protected virtual void OnSingletonInit() { }

        void ISingleton.DestroySingleton()
        {
            OnSingletonDestroy();
            m_SingleInstance = null;
        }

        protected virtual void OnSingletonDestroy() { }
    }
}
