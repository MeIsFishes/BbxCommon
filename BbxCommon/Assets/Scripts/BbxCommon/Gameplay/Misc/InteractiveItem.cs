using UnityEngine;

namespace BbxCommon
{
    public class InteractiveItem : MonoBehaviour
    {
        public int InteractFlag;

        public virtual void OnInteractAwake(InteractiveItem item) { }

        public virtual void OnInteract(InteractiveItem item) { }

        public virtual void OnInteractSleep() { }
    }
}
