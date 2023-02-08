using UnityEngine;
using UnityEngine.UI;
using System.Text;

namespace CardGame.UI {
    public class CardTooltip : MonoBehaviour {
#pragma warning disable CS0649
        [SerializeField] private UIAnimatedPanel panel;

        [SerializeField] private RectTransform tooltipPanel;
        [SerializeField] private Canvas canvas;
        [SerializeField] private RectTransform holder;

        [SerializeField] private Text nameText;
        [SerializeField] private Text descText;
        [SerializeField] private Text typeText;
        [SerializeField] private Text purposeTypeText;

        [SerializeField] private GameObject pointsInfoHolder;
        [SerializeField] private Text pointsInfoText;

        #region effect ranges.
        [SerializeField] private GameObject healRangeHolder;
        [SerializeField] private Text healRangeText;
        [SerializeField] private Image[] healRangeVisualizations;
        [SerializeField] private GameObject healsBothRowsHolder;

        [SerializeField] private GameObject effectRangeHolder;
        [SerializeField] private Text effectRangeText;
        [SerializeField] private Image[] effectRangeVisualizations;
        [SerializeField] private GameObject bothRowsHolder;

        [SerializeField] private GameObject rangedEffectRangeHolder;
        [SerializeField] private Text rangedEffectRangeText;
        [SerializeField] private Image[] rangedEffectRangeVisualizations;
        [SerializeField] private GameObject rangedEffectBothRowsHolder;
        #endregion

        [SerializeField] private Color effectInRangeColor;
        [SerializeField] private Color effectOutOfRangeColor;

        [SerializeField] private GameObject healsYouHolder;
        [SerializeField] private Text healsYouText;

        [SerializeField] private GameObject attackTypeHolder;
        [SerializeField] private Text attackTypeText;

        [SerializeField] private GameObject attacksHolder;
        [SerializeField] private GameObject resistancesHolder;
        [SerializeField] private GameObject counterHolder;
        [SerializeField] private GameObject rangedAttacksHolder;

        [SerializeField] private Text[] attackTooltipText;
        [SerializeField] private Text[] resistanceTooltipText;
        [SerializeField] private Text[] counterTooltipText;
        [SerializeField] private Text[] rangedAttackTooltipText;
#pragma warning restore CS0649
        /// <summary>
        /// Open / hide tooltip, if value is true, It will find the proper position on UI;
        /// </summary>
        /// <param name="value"></param>
        public void Set (bool value) {
            panel.SetPanel(value);

            if (value) {
                Canvas.ForceUpdateCanvases();

                if (Application.isPlaying) {
                    // check for screen bounds.
                    Vector3 size = new Vector2 (tooltipPanel.sizeDelta.x * holder.localScale.x+ 100, tooltipPanel.sizeDelta.y * holder.localScale.y + 100);

                    Vector3 focusPos = Camera.main.WorldToScreenPoint(transform.position);

                    Vector3 screenBounds = new Vector2(Screen.width, Screen.height);

                    Vector3 endPos = focusPos + size;

                    Vector3 offset = endPos - screenBounds;

                    // clamp.
                    if (offset.x < 0) offset.x = 0;
                    if (offset.y < 0) offset.y = 0;

                    var finalPos = focusPos - offset;
                    finalPos.z = 0;
                    tooltipPanel.position = finalPos;
                } else {
                    // its in editor. make it world space. probably card editor.
                    canvas.renderMode = RenderMode.WorldSpace;
                    canvas.transform.localScale = 0.005f * Vector3.one;
                    holder.anchorMin = Vector2.one / 2;
                    holder.anchorMax = Vector2.one / 2;
                    holder.sizeDelta = Vector3.zero;
                    holder.localPosition = Vector3.zero;
                    tooltipPanel.localPosition = Vector3.zero;
                    tooltipPanel.localRotation = Quaternion.identity;
                }

                Canvas.ForceUpdateCanvases();
            }
        }

        private bool VisualizeRange (int range, Image[] rangeImages, out string rangeText) {
            bool haveEffectRange = range > 0;

            rangeText = "0";

            if (!haveEffectRange) {
                return false;
            } else {
                int childCount = rangeImages.Length;
                int midPoint = childCount / 2;
                for (int i = 0; i < childCount; i++) {
                    if (i >= midPoint - range && i <= midPoint + range) {
                        rangeImages[i].color = effectInRangeColor;
                    } else {
                        rangeImages[i].color = effectOutOfRangeColor;
                    }
                }

                rangeText = (1 + (range * 2)).ToString(); ;
            }

            return haveEffectRange;
        }

        public void UpdateTooltip (Card card) {
            nameText.text = card.baseCard.Name;
            descText.text = card.baseCard.Desc;
            typeText.text = card.baseCard.CardInteractionType.ToString ();
            purposeTypeText.text = card.baseCard.CardType.ToString();

            bool isAttacker = card.IsAttacker;
            bool isOrganicAttacker = isAttacker && card.IsOrganic;
            bool isHealer = card.IsHealer;
            bool showRangedAttack = card.IsOrganic && card.IsRangedAttacker && !card.IsRangedOnly;

            attackTypeHolder.SetActive(isOrganicAttacker);

            if (isOrganicAttacker) {
                if (card.organicAttackerCard.AttackType != GameData.Cards.AttackTypes.Both) {
                    attackTypeText.text = card.organicAttackerCard.AttackType.ToString();
                } else {
                    attackTypeText.text = "Ranged & Melee";
                }
            }

            healsYouHolder.SetActive(isHealer);

            if (isHealer) {
                healsYouText.text = card.healerCard.Heal.ToString();

                healsBothRowsHolder.SetActive(card.baseCard.EffectBothRows);

                // show heal range.
                bool value = VisualizeRange(card.baseCard.EffectRange, healRangeVisualizations, out string head);
                healRangeHolder.SetActive(value);
                if (value) {
                    healRangeText.text = head;
                }
                //
            }

            if (!isAttacker) {
                attacksHolder.SetActive(false);
                rangedAttacksHolder.SetActive(false);
                effectRangeHolder.SetActive(false);
                rangedEffectRangeHolder.SetActive(false);
            } else {
                // this is an attacker card. draw attacks.
                bool isAttackTooltipsAvailable = GetTooltip(card.attackerCard.Attacks, out string headers, out string values);
                
                attacksHolder.SetActive(isAttackTooltipsAvailable);
                if (isAttackTooltipsAvailable) {
                    attackTooltipText[0].text = headers;
                    attackTooltipText[1].text = values;
                }

                bothRowsHolder.SetActive(card.baseCard.EffectBothRows);

                // show attack range.
                bool value = VisualizeRange(card.baseCard.EffectRange, effectRangeVisualizations, out string head);
                effectRangeHolder.SetActive(value);
                if (value) {
                    effectRangeText.text = head;
                }
                //
            }

            var counterAvailable = card.IsOrganic && card.vulnerableCard.CounterEnabled;
            counterHolder.SetActive(counterAvailable);

            if (card.IsOrganic) {
                // draw resistances.
                bool isResistanceTooltipsAvailable = GetTooltip(card.vulnerableCard.Resistances, out string headers, out string values);

                resistancesHolder.SetActive(isResistanceTooltipsAvailable);
                if (isResistanceTooltipsAvailable) {
                    resistanceTooltipText[0].text = headers;
                    resistanceTooltipText[1].text = values;
                }

                // draw counter attack.
                if (counterAvailable) {
                    GetTooltip(card.vulnerableCard.CounterEffects, out string counterHeaders, out string counterValues);
                
                    counterTooltipText[0].text = counterHeaders;
                    counterTooltipText[1].text = counterValues;
                }
            } else {
                resistancesHolder.SetActive(false);
            }

            //draw ranged attacks.
            rangedAttacksHolder.SetActive(showRangedAttack);

            if (showRangedAttack) {
                // create ranged attacks for tooltip.
                int length = card.attackerCard.Attacks.Length;
                GameData.Effect[] rangedAttacks = new GameData.Effect[length];
                for (int i = 0; i < length; i++) {
                    rangedAttacks[i] = new GameData.Effect(card.attackerCard.Attacks[i].EffectType);
                    rangedAttacks[i].Power = card.attackerCard.Attacks[i].RangedPower;
                }

                rangedEffectBothRowsHolder.SetActive(card.organicAttackerCard.RangedEffectBothRows);

                // draw rangeds.
                bool isRangedTooltipsAvailable = GetTooltip(rangedAttacks, out string headers, out string values);
                
                if (isRangedTooltipsAvailable) {
                    rangedAttackTooltipText[0].text = headers;
                    rangedAttackTooltipText[1].text = values;
                } else {
                    rangedAttackTooltipText[0].text = "No damage";
                    rangedAttackTooltipText[1].text = "";
                }

                // show ranged attack range.
                bool value = VisualizeRange(card.organicAttackerCard.RangedEffectRange, rangedEffectRangeVisualizations, out string head);
                rangedEffectRangeHolder.SetActive(value);
                if (value) {
                    rangedEffectRangeText.text = head;
                }
                //
            }

            // show how many points on kill as reward.
            bool showRewardPoints = card.IsOrganic;
            pointsInfoHolder.SetActive(showRewardPoints);
            if (showRewardPoints) {
                if (Scores.Instance == null) {
                    new Scores();
                }

                pointsInfoText.text = Scores.Instance.HowManyPointsFromDeath(card.vulnerableCard.Points).ToString ();
            }
        }

        /// <summary>
        /// Creates a tooltip for effects array. If rangedModifier is not -1, It will multiply the values by rangedModifier for rangedAttacks.
        /// </summary>
        /// <param name="effects"></param>
        /// <param name="rangedModifier"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        private bool GetTooltip (GameData.Effect[] effects, out string headers, out string values) {
            StringBuilder header = new StringBuilder();
            StringBuilder value = new StringBuilder();

            int length = effects.Length;

            bool found = false;

            for (int i=0; i<length; i++) {
                if (effects[i].Power > 0) {
                    var infoText = effects[i].EffectType.ToString();

                    header.Append(infoText);

                    value.Append(effects[i].Power.ToString());

                    if (i < length - 1) {
                        header.Append("\r\n");
                        value.Append("\r\n");
                    }

                    found = true;
                }
            }

            headers = header.ToString();
            values = value.ToString();

            return found;
        }
    }
}

