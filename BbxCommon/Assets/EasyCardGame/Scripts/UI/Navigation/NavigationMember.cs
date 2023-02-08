using UnityEngine;
using UnityEngine.Events;
using CardGame.Animation;

namespace CardGame.UI {
    [RequireComponent (typeof (RectTransform))]
    public abstract class NavigationMember : MonoBehaviour {
#pragma warning disable CS0649
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private UnityEvent onInteracted;
#pragma warning restore CS0649
        [HideInInspector] public AnimationQuery CurrentAnimation;

        void OnValidate () {
            rectTransform = GetComponent<RectTransform>();
        }

        public RectTransform Rect => rectTransform;

        public virtual void Select () {

        }

        public virtual void DeSelect () {

        }
        public virtual void Interact () {
            onInteracted?.Invoke();
        }

        public virtual void Activate () {

        }

        public virtual void DeActivate () {

        }
    }
}
