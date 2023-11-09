using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BbxCommon
{
    public abstract class Interactor : MonoBehaviour
    {
        /// <summary>
        /// Set flags by calling SetInteractFlags(). Don't modify its value manually!
        /// </summary>
        public List<int> InteractFlags;
        public bool AddedToManager { get; protected set; }

        public UnityAction<Interactor> OnInteractorAwake;
        /// <summary>
        /// The first parameter is requester, and the second one is responder.
        /// </summary>
        public UnityAction<Interactor, Interactor> OnInteract;
        /// <summary>
        /// A syntax sugar. If the current <see cref="Interactor"/> is requester, than pass the responder as parameter, and so do
        /// the opposite case, to free users from thinking of who I am.
        /// </summary>
        public UnityAction<Interactor> OnInteractWith;
        public UnityAction OnInteractorSleep;

        /// <summary>
        /// Object for storing interacting information.
        /// </summary>
        public object ExtraInfo
        {
            get
            {
                return m_ExtraInfo;
            }
            set
            {
                if (m_ExtraInfo != null && m_ExtraInfo is PooledObject pooled)
                    pooled.CollectToPool();
                m_ExtraInfo = value;
            }
        }
        protected object m_ExtraInfo;

        public void InteractAwake(Interactor awaker) { OnInteractorAwake?.Invoke(awaker); }

        public void Interact(Interactor requester, Interactor responder)
        {
            OnInteract?.Invoke(requester, responder);
            if (OnInteractWith != null)
            {
                if (this == requester) OnInteractWith(responder);
                if (this == responder) OnInteractWith(requester);
            }
        }

        public void InteractSleep() { OnInteractorSleep?.Invoke(); }

        public virtual void OnAddToManager() { AddedToManager = true; }

        public virtual void OnRemoveFromManager() { AddedToManager = false; }

        public void SetInteractFlags(params int[] flags)
        {
            if (AddedToManager)
                InteractorManager.Instance.RemoveInteractor(this);
            InteractFlags.Clear();
            InteractFlags.AddArray(flags);
            if (AddedToManager)
                InteractorManager.Instance.AddInteractor(this);
        }

        public void SetInteractFlags(List<int> flags)
        {
            if (AddedToManager)
                InteractorManager.Instance.RemoveInteractor(this);
            InteractFlags.Clear();
            InteractFlags.AddList(flags);
            if (AddedToManager)
                InteractorManager.Instance.AddInteractor(this);
        }


        public void SetInteractFlags<TEnum>(params TEnum[] flags) where TEnum : Enum
        {
            if (AddedToManager)
                InteractorManager.Instance.RemoveInteractor(this);
            foreach (var flag in flags)
            {
                InteractFlags.Add(flag.GetHashCode());
            }
            if (AddedToManager)
                InteractorManager.Instance.AddInteractor(this);
        }

        public void SetInteractFlags<TEnum>(List<TEnum> flags) where TEnum : Enum
        {
            if (AddedToManager)
                InteractorManager.Instance.RemoveInteractor(this);
            foreach (var flag in flags)
            {
                InteractFlags.Add(flag.GetHashCode());
            }
            if (AddedToManager)
                InteractorManager.Instance.AddInteractor(this);
        }

        public bool HasFlag(int flag)
        {
            return InteractFlags.Contains(flag);
        }

        public bool HasFlag<TEnum>(TEnum flag) where TEnum : Enum
        {
            return InteractFlags.Contains(flag.GetHashCode());
        }
    }
}
