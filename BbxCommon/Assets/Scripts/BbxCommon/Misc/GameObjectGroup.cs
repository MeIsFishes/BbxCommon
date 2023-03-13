using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BbxCommon
{
    /// <summary>
    /// <see cref="GameObjectGroup"/> is for managing a set of <see cref="GameObject"/>s.
    /// You can easily set all of them active or inactive, or call a function to them.
    /// <see cref="GameObjectGroup"/> can even include other <see cref="GameObjectGroup"/>s, in that way, take care of circular reference.
    /// </summary>
    public class GameObjectGroup : MonoBehaviour
    {
        public struct GameObjectGroupWrapper
        {
            private GameObjectGroup m_Ref;

            public GameObjectGroupWrapper(GameObjectGroup gameObjectGroup) { m_Ref = gameObjectGroup; }

            public UnityAction<GameObject> OnSetActive { get { return m_Ref.OnSetActive; } set { m_Ref.OnSetActive = value; } }
            public UnityAction<GameObject> OnSetInactive { get { return m_Ref.OnSetInactive; } set { m_Ref.OnSetInactive = value; } }
            public void SetActive() => m_Ref.SetActive();
            public void SetInactive() => m_Ref.SetInactive();
            /// <summary>
            /// Call a function to each <see cref="GameObject"/> stored in the group.
            /// </summary>
            /// <param name="action"></param>
            public void Do(UnityAction<GameObject> action) => m_Ref.Do(action);
        }

        [SerializeField]
        private List<GameObject> m_GameObjects = new List<GameObject>();
        [SerializeField]
        private List<GameObjectGroup> m_Groups = new List<GameObjectGroup>();

        public UnityAction<GameObject> OnSetActive;
        public UnityAction<GameObject> OnSetInactive;

        public GameObjectGroupWrapper Wrapper;

        protected void Awake()
        {
            Wrapper = new GameObjectGroupWrapper(this);
        }

        public void SetActive()
        {
            if (OnSetActive == null)
            {
                foreach (var go in m_GameObjects)
                {
                    go.SetActive(true);
                }
            }
            else
            {
                foreach (var go in m_GameObjects)
                {
                    go.SetActive(true);
                    OnSetActive(go);
                }
            }
            foreach (var group in m_Groups)
            {
                group.SetActive();
            }
        }

        public void SetInactive()
        {
            if (OnSetInactive == null)
            {
                foreach (var go in m_GameObjects)
                {
                    go.SetActive(false);
                }
            }
            else
            {
                foreach (var go in m_GameObjects)
                {
                    go.SetActive(false);
                    OnSetInactive(go);
                }
            }
            foreach (var group in m_Groups)
            {
                group.SetInactive();
            }
        }

        public void Do(UnityAction<GameObject> action)
        {
            foreach (var go in m_GameObjects)
            {
                action(go);
            }
            foreach (var group in m_Groups)
            {
                group.Do(action);
            }
        }
    }
}
