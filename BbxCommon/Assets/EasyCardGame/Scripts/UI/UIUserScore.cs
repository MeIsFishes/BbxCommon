using UnityEngine;
using UnityEngine.UI;

namespace CardGame.UI {
    public class UIUserScore : MonoBehaviour {
#pragma warning disable CS0649
        [SerializeField] private Text[] scoreTexts = new Text[2];
        [SerializeField] private Animator[] animators = new Animator[2];
        [SerializeField] private Text resultText;

        [SerializeField] private UIAnimatedPanel localUserSimulatedScore;
        [SerializeField] private Text localUserNewScoreText;
        [SerializeField] private Text localUserAdditionText;
#pragma warning restore CS0649

        private int[] scores = new int[2];

        public void OnLocalUserSimulationClear () {
            localUserSimulatedScore.Close();
        }

        public void OnLocalUserSimulatedScore (int addition) {
            localUserSimulatedScore.Open();

            localUserNewScoreText.text = (scores[0] + addition).ToString();
            localUserAdditionText.text = "(+" + addition.ToString() + ")";
        }

        public void OnUserScore (int user, int score) {
            scores[user] = score;
            scoreTexts[user].text = score.ToString();

            if (animators[user].isActiveAndEnabled) {
                animators[user].SetTrigger("Play");
            }

            if (resultText != null) {
                if (scores[0] == scores[1]) {
                    resultText.text = "DRAW!";
                } else if (scores[0] > scores[1]) {
                    resultText.text = "YOU WON!";
                } else {
                    resultText.text = "YOU LOST!";
                }
            }
        }
    }
}
