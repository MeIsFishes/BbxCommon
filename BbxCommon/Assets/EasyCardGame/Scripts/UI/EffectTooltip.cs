using UnityEngine;
using UnityEngine.UI;

namespace CardGame.UI {
    public class EffectTooltip : MonoBehaviour {
#pragma warning disable CS0649
        [SerializeField] private UIAnimatedPanel effectTooltipPanel;
        [SerializeField] private Text effectText;
        [SerializeField] private Color offensiveColor, defensiveColor;
#pragma warning restore CS0649

        public void Set (bool value, int amount = 0, bool isOffensive = false) {
            effectTooltipPanel.transform.position = Camera.main.WorldToScreenPoint(transform.position);

            effectTooltipPanel.SetPanel(value);

            if (value) {
                effectText.text = amount.ToString();
                effectText.color = isOffensive ? offensiveColor : defensiveColor;
            }
        }
    }
}

