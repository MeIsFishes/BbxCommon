using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BbxCommon
{
    public class Interactor : MonoBehaviour
    {
        /// <summary>
        /// Set flags by calling SetInteractFlags(). Don't modify its value manually!
        /// </summary>
        public List<int> InteractFlags;
        public bool AddedToManager { get; protected set; }

        public UnityAction<Interactor> OnInteractorAwake;
        public UnityAction<Interactor> OnInteractWith;
        public UnityAction OnInteractorSleep;

        public void InteractAwake(Interactor awaker) { OnInteractorAwake?.Invoke(awaker); }

        public void InteractWith(Interactor item) { OnInteractWith?.Invoke(item); }

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
    }
}
