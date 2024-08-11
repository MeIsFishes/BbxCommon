using UnityEngine;

namespace BbxCommon
{
    public class MonoSingleton<T> : MonoBehaviour, ISingleton where T : MonoSingleton<T>
    {
        private static MonoSingleton<T> m_SingleInstance;

        protected virtual void Awake()
        {
            InitSingleton();
        }

        // Remember to call InitSingleton() in child class.
        protected void InitSingleton()
        {
            if (m_SingleInstance != null && m_SingleInstance != this)
            {
                DebugApi.LogWarning("Find the second instance of a singleton!");
                Destroy(this.gameObject);
                SingletonManager.Instance.AddSingleton(m_SingleInstance);
                return;
            }
            else if (m_SingleInstance == null)
            {
                m_SingleInstance = this;
                SingletonManager.Instance.AddSingleton(m_SingleInstance);
            }
        }

        public static T Instance
        {
            get
            {
                if (m_SingleInstance != null)
                {
                    return (T)m_SingleInstance;
                }
                var go = new GameObject(typeof(T).Name);
                m_SingleInstance = go.AddComponent<T>();
                SingletonManager.Instance.AddSingleton(m_SingleInstance);
                return (T)m_SingleInstance;
            }
        }

        void ISingleton.InitSingleton()
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
