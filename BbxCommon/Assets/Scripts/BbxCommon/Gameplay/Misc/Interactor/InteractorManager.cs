using System;
using System.Collections.Generic;
using UnityEngine;

namespace BbxCommon
{
    public class InteractorManager : Singleton<InteractorManager>
    {
        /// <summary>
        /// Store target flags those key flag can interact with.
        /// </summary>
        private Dictionary<int, HashSet<int>> m_InteractingDatas = new Dictionary<int, HashSet<int>>();
        private Dictionary<int, HashSet<Interactor>> m_Interactors = new Dictionary<int, HashSet<Interactor>>();
        private HashSet<Interactor> m_Awaken = new HashSet<Interactor>();

        public void RegisterInteracting(int requestor, params int[] responsers)
        {
            if (m_InteractingDatas.ContainsKey(requestor))
            {
                foreach (var responser in responsers)
                {
                    m_InteractingDatas[requestor].Add(responser);
                }
            }
            else
            {
                m_InteractingDatas[requestor] = new HashSet<int>();
                foreach (var responser in responsers)
                {
                    m_InteractingDatas[requestor].Add(responser);
                }
            }
        }

        public void UnregisterInteracting(int requestor, params int[] responsers)
        {
            if (m_InteractingDatas.TryGetValue(requestor, out var hash))
            {
                foreach (var responser in responsers)
                {
                    hash.Remove(responser);
                }
            }
            else
            {
                Debug.LogWarning("There is no such a requestor existed. It is no need to unregister!");
            }
        }

        public void UnregisterInteracting(int requestor)
        {
            if (m_InteractingDatas.TryGetValue(requestor, out var hash))
                hash.Clear();
            else
                Debug.LogWarning("There is no such a requestor existed. It is no need to unregister!");
        }

        public void AddInteractor(params Interactor[] interactors)
        {
            foreach (var interactor in interactors)
            {
                if (interactor.AddedToManager)
                    continue;
                foreach (var flag in interactor.InteractFlags)
                {
                    HashSet<Interactor> set;
                    if (m_Interactors.TryGetValue(flag, out set))
                        set.Add(interactor);
                    else
                    {
                        set = SimplePool<HashSet<Interactor>>.Alloc();
                        set.Add(interactor);
                        m_Interactors[flag] = set;
                    }
                    interactor.OnAddToManager();
                }
            }
        }

        public void RemoveInteractor(params Interactor[] interactors)
        {
            foreach (var interactor in interactors)
            {
                if (interactor.AddedToManager == false)
                    continue;
                foreach (var flag in interactor.InteractFlags)
                {
                    m_Interactors[flag].Remove(interactor);
                }
                interactor.OnRemoveFromManager();
            }
        }

        public void InteractorAwake(Interactor interactor)
        {
            if (m_Awaken.Count > 0)
            {
                Debug.LogError("There has been awaken interactors. It's invalid to awake multiple objects at a time!");
                return;
            }
            m_Awaken.Add(interactor);
            foreach (var flag in interactor.InteractFlags)
            {
                if (m_Interactors.TryGetValue(flag, out var targets))
                {
                    foreach (var target in targets)
                    {
                        if (m_Awaken.Contains(target) == false)   // if hasn't been awaken, awake
                        {
                            target.InteractAwake(interactor);
                            m_Awaken.Add(target);
                        }
                    }
                }
            }
        }

        public void InteractorSleep()
        {
            foreach (var interactor in m_Awaken)
            {
                interactor.InteractSleep();
            }
        }
    }
}