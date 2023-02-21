using System.Collections.Generic;
using UnityEngine;

namespace BbxCommon
{
    public class InteractiveObject : MonoBehaviour
    {
        public List<int> InteractFlag;

        public virtual void OnInteractAwake(InteractiveObject item) { }

        public virtual void OnInteract(InteractiveObject item) { }

        public virtual void OnInteractSleep() { }
    }
}
