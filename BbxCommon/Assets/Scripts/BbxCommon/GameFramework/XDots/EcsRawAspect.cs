using UnityEngine;

namespace BbxCommon
{
    public abstract class EcsRawAspect : EcsData
    {
        private GameObject m_GameObject;

        internal void Create()
        {
            m_GameObject = Entity.GetGameObject();
            CreateAspect();
        }

        protected abstract void CreateAspect();

        protected T GetRawComponent<T>() where T : EcsRawComponent
        {
            return Entity.GetRawComponent<T>();
        }

        protected T GetSingletonRawComponent<T>() where T : EcsSingletonRawComponent
        {
            return EcsDataManager.GetSingletonRawComponent<T>();
        }

        protected T GetGameObjectComponent<T>() where T : Component
        {
            return m_GameObject.GetComponent<T>();
        }
    }
}
