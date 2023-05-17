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
        public UnityAction<Interactor, Interactor> OnInteract;
        public UnityAction OnInteractorSleep;

        public void InteractAwake(Interactor awaker) { OnInteractorAwake?.Invoke(awaker); }

        public void Interact(Interactor requester, Interactor responder) { OnInteract?.Invoke(requester, responder); }

        public void InteractSleep() { OnInteractorSleep?.Invoke(); }

        public virtual void OnAddToManager() { AddedToManager = true; }

        public virtual void OnRemoveFromManager() { AddedToManager = false; }

        public void SetInteractFlags(params int[] flags)
        {
            if (AddedToManager)
                InteractorManager.Instance.RemoveInteractor(this);
            InteractFlags.AddArray(flags, clear: true);
            if (AddedToManager)
                InteractorManager.Instance.AddInteractor(this);
        }

        public void SetInteractFlags(List<int> flags)
        {
            if (AddedToManager)
                InteractorManager.Instance.RemoveInteractor(this);
            InteractFlags.AddList(flags, clear: true);
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
