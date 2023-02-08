using UnityEngine;

namespace BbxCommon
{
    public class MonoSingleton<T> : MonoBehaviour, IDestroySingleton where T : MonoSingleton<T>
    {
        private static MonoSingleton<T> m_s_SingleInstance;

        protected void Awake()
        {
            InitSingleton();
        }

        // Remember to call InitSingleton() in child class.
        protected void InitSingleton()
        {
            if (m_s_SingleInstance != null && m_s_SingleInstance != this)
            {
                Debug.LogWarning("Find the second instance of a singleton!");
                Destroy(this.gameObject);
                SingletonManager.Instance.AddSingleton(m_s_SingleInstance);
                return;
            }
            else if (m_s_SingleInstance == null)
            {
                m_s_SingleInstance = this;
                SingletonManager.Instance.AddSingleton(m_s_SingleInstance);
            }
        }

        public static T Instance
        {
            get
            {
                if (m_s_SingleInstance != null)
                {
                    return (T)m_s_SingleInstance;
                }
                var go = new GameObject(typeof(T).Name);
                m_s_SingleInstance = go.AddComponent<T>();
                SingletonManager.Instance.AddSingleton(m_s_SingleInstance);
                return (T)m_s_SingleInstance;
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
