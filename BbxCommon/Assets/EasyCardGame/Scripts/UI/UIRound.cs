using UnityEngine;
using UnityEngine.UI;

namespace CardGame.UI {
    public class UIRound : MonoBehaviour {
#pragma warning disable CS0649
        [SerializeField] private Text roundText;
        [SerializeField] private Animator roundAnimator;
#pragma warning restore CS0649

        public void OnUserScore(int round, int max) {
            if (roundAnimator.isActiveAndEnabled) {
                roundAnimator.SetTrigger("Play");
            }

            roundText.text = string.Format("Round {0}/{1}", round, max);
        }
    }
}
