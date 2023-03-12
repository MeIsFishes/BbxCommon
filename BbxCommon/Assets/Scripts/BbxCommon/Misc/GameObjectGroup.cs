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
        public List<GameObject> GameObjects = new List<GameObject>();
        public List<GameObjectGroup> Groups = new List<GameObjectGroup>();

        public UnityAction<GameObject> OnSetActive;
        public UnityAction<GameObject> OnSetInactive;

        public void SetActive()
        {
            if (OnSetActive == null)
            {
                foreach (var go in GameObjects)
                {
                    go.SetActive(true);
                }
            }
            else
            {
                foreach (var go in GameObjects)
                {
                    go.SetActive(true);
                    OnSetActive(go);
                }
            }
            foreach (var group in Groups)
            {
                group.SetActive();
            }
        }

        public void SetInactive()
        {
            if (OnSetInactive == null)
            {
                foreach (var go in GameObjects)
                {
                    go.SetActive(false);
                }
            }
            else
            {
                foreach (var go in GameObjects)
                {
                    go.SetActive(false);
                    OnSetInactive(go);
                }
            }
            foreach (var group in Groups)
            {
                group.SetInactive();
            }
        }

        public void Do(UnityAction<GameObject> func)
        {
            foreach (var go in GameObjects)
            {
                func(go);
            }
            foreach (var group in Groups)
            {
                group.Do(func);
            }
        }
    }
}
