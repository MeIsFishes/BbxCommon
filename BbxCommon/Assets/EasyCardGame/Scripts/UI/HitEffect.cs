using UnityEngine;
using UnityEngine.UI;

namespace CardGame.UI {
    [RequireComponent (typeof (Animator))]
    public class HitEffect : MonoBehaviour {
#pragma warning disable CS0649
        [SerializeField] private Animator animator;
        [SerializeField] private Text text;
#pragma warning restore CS0649

        private void OnValidate() {
            animator = GetComponent<Animator>();
        }

        public void Play (Vector3 position, object amount, Color color) {
            transform.position = position;
            text.text = amount.ToString();
            text.color = color;

            animator.SetTrigger("Play");
        }
    }
}