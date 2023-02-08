using UnityEngine;
using UnityEngine.UI;

using CardGame.Layouts;
using CardGame.Animation;
using CardGame.GameData.Cards;
using CardGame.GameData.Interfaces;
using CardGame.UI;
using CardGame.Loaders;
using CardGame.Textures;

using System;

namespace CardGame {
    [ExecuteInEditMode]
    public class Card : MonoBehaviour, IMovable, IColorable, IScalable {
        public const string ShaderMainColor = "_Color";

        public const string ShaderTextureName = "_Avatar";

        const string ShaderEffectColor = "_EffectColor";
        const string ShaderInteractionColor = "_InteractionColor";
        const string ShaderRareColor = "_RareColor";

        const string TargetMarkColor = "_Color";

        const string AnimatorSelectParameterName = "_Select";
        const string AnimatorDeSelectParameterName = "_DeSelect";
        const string AnimatorPlacingParameterName = "_Placing";
        const string AnimatorPlacedParameterName = "_Placed";

        /// <summary>
        /// Card is dead. Needs to go to graveyard.
        /// Card, isDead?
        /// </summary>
        public Action<Card, bool, Layout> OnToGraveyard;

        /// <summary>
        /// Card got hit. User, Damage Points.
        /// </summary>
        public Action<int, int> OnCardHit;

        /// <summary>
        /// Card is killed. User, Points
        /// </summary>
        public Action<int, int> OnCardKilled;

        [HideInInspector] public Layout CurrentLayout;
        [HideInInspector] public int UserId;

        /// <summary>
        /// Tooltip manager of the card.
        /// </summary>
        public CardTooltip cardTooltip;

        /// <summary>
        /// Tooltip of the calculated effect, when marking this card as target.
        /// </summary>
        public EffectTooltip effectTooltip;

        /// <summary>
        /// Current animation query (CardGame.Animation) of the card.
        /// </summary>
        public AnimationQuery CurrentAnimation;

        /// <summary>
        /// Is this card a dummy? Mean is a not real card?, maybe just for animations? 
        /// </summary>
        public bool IsDummy = false;

#pragma warning disable CS0649
        [SerializeField] private Pool hitEffectPool;

        [SerializeField] private Renderer cardRenderer;
        [SerializeField] private Animator cardAnimator;

        [SerializeField] private CardDraggingOffsets cardDraggingOffsets;

        [SerializeField] private Transform cursorAimPoint;
        [SerializeField] private Transform cursorFocusPoint;
        [SerializeField] private Transform hitEffectPoint;

        [SerializeField] private GameObject meleeCardPointer, rangedCardPointer, skillCardPointer, healCardPointer;

        [SerializeField] private GameObject healthHolder;
        [SerializeField] private Text healthValueText;

        [SerializeField] private UIAnimatedPanel cardTypeHolder;

        [SerializeField] private CardColorAnimationData colorAnimationData;

        [Tooltip ("How much time user need to focus on the card to see the tooltip.")]
        [SerializeField] private float tooltipOpenTimeDistance = 0.5f;

        /// <summary>
        /// Activated when card complete the placement.
        /// </summary>
        [SerializeField] private ParticleSystem placementEffect;

        /// <summary>
        /// Activated when card leaves his layout.
        /// </summary>
        [SerializeField] private ParticleSystem placingEffect;

        /// <summary>
        /// Target marking effect.
        /// </summary>
        [SerializeField] private Renderer targetMark;
#pragma warning restore CS0649

        public Vector3 CursorPosition => cursorAimPoint.position;
        public Vector3 CursorFocusPoint => cursorFocusPoint.position;

        public BaseCard baseCard;
        public IVulnerable vulnerableCard;
        public IAttacker attackerCard;
        public IHealer healerCard;
        public IOrganicAttacker organicAttackerCard;

        public int Points => vulnerableCard == null ? 0 : vulnerableCard.Points;
        public bool IsHealer => healerCard != null;
        public bool IsAttacker => attackerCard != null;
        public bool IsOrganic => vulnerableCard != null;

        /// <summary>
        /// Null organic attack means this is a skill card. Skill cards always have ranged attacks.
        /// </summary>
        public bool IsRangedAttacker =>  IsAttacker && (organicAttackerCard == null || organicAttackerCard.AttackType == AttackTypes.Both || organicAttackerCard.AttackType == AttackTypes.Ranged);

        /// <summary>
        /// Is this card attack ranged only?
        /// </summary>
        public bool IsRangedOnly => IsAttacker && organicAttackerCard != null && organicAttackerCard.AttackType == AttackTypes.Ranged;
        
        public bool IsSelected {
            private set;
            get;
        }

        /// <summary>
        /// Is pointer over?
        /// </summary>
        private bool isInteracted;

        /// <summary>
        /// Is the card dead?
        /// </summary>
        private bool isInGraveyard;

        private AnimationQuery tooltipTimerAnimation;
        private AnimationQuery interactColorAnimation;

        /// <summary>
        /// Set base card. cardData as JSON data.
        /// </summary>
        /// <param name="baseCard">Base card from game data.</param>
        public void SetCardData (string cardData) {
            new GameData.CardLoader(cardData, out baseCard, out vulnerableCard, out attackerCard, out healerCard, out organicAttackerCard);
            if (!TextureCollectionReader.Readers["Avatars"].Textures.ContainsKey(baseCard.Avatar)) {
                Debug.LogErrorFormat ("Target texture " + baseCard.Avatar + " is not found on avatars pool.", this);
            } else {
                TextureCollectionReader.Readers["Avatars"].Textures[baseCard.Avatar].SetMaterial(ShaderTextureName, cardRenderer);
            }

            MarkAsTarget(false);

            Placing(false);

            healthHolder.SetActive(false); // close first.
            healthHolder.SetActive(IsOrganic); // then decide.

            meleeCardPointer.SetActive(!IsHealer && IsAttacker && IsOrganic && (organicAttackerCard.AttackType == AttackTypes.Melee || organicAttackerCard.AttackType == AttackTypes.Both));
            rangedCardPointer.SetActive(!IsHealer && IsRangedAttacker && IsOrganic);
            skillCardPointer.SetActive(!IsOrganic);
            healCardPointer.SetActive(IsHealer);

            cardTypeHolder.Open();

            if (IsOrganic) {
                UpdateHealth();
            }

            // load card tooltip.
            cardTooltip.UpdateTooltip(this);

            ResetColor();

            isInGraveyard = false;

        }

        private void ResetColor () {
            SetColor(ShaderMainColor, Color.white);
            SetColor(ShaderEffectColor, Color.black);
            SetColor(ShaderRareColor, baseCard.RareColor);
        }

        public Vector3 GetPosition() {
            return transform.position;
        }

        public Quaternion GetRotation() {
            return transform.rotation;
        }

        public void SetPosition(Vector3 targetPosition) {
            transform.position = targetPosition;
        }

        /// <summary>
        /// This card being dragged on the given position.
        /// </summary>
        /// <param name="position"></param>
        public void SetDragging (Vector3 position) {
            SetPosition(position + cardDraggingOffsets.PositionOffset);
            SetRotation(Quaternion.Euler (cardDraggingOffsets.EulerAngles));
        }

        public void SetRotation(Quaternion targetRotation) {
            transform.rotation = targetRotation;
        }

        public void SetColor(string colorName, Color color) {
            if (Application.isPlaying) {
                cardRenderer.material.SetVector(colorName, color);
            } else {
                cardRenderer.sharedMaterial.SetVector(colorName, color);
            }
        }

        public void SetMaterial (Material material) {
            cardRenderer.sharedMaterial = material;
            healthHolder.SetActive(false);
        }

        public Vector3 GetScale() {
            return transform.localScale;
        }

        public void SetScale(Vector3 scale) {
            transform.localScale = scale;
        }

        public void MarkAsTarget (bool value, int amount = 0, bool isOffensive = false) {
            targetMark.gameObject.SetActive(value);
            effectTooltip.Set(value, amount, isOffensive);

            Material material;
            if (Application.isPlaying) {
                material = targetMark.material;
            } else {
                material = targetMark.sharedMaterial;
            }

            material.SetColor(TargetMarkColor, isOffensive ?
                (colorAnimationData.targetMarkColorOffensive * colorAnimationData.targetMarkColorOffensiveIntensity) :
                (colorAnimationData.targetMarkColorDefensive * colorAnimationData.targetMarkColorDefensiveIntensity)
                );
        }

        /// <summary>
        /// User is interesting on this card.
        /// </summary>
        /// <param name="value"></param>
        public void Interested (bool value, bool animateColor = true) {
            if (isInGraveyard) {
                value = false;
            }

            // tooltip timer.
            if (tooltipTimerAnimation != null) {
                tooltipTimerAnimation.Stop();
            }

            if (value) {
                tooltipTimerAnimation = new AnimationQuery();
                tooltipTimerAnimation.AddToQuery(new TimerAction(tooltipOpenTimeDistance));
                tooltipTimerAnimation.Start(this, () => {
                    if (isInteracted) {
                        // still interacted, show tooltip.
                        tooltipTimerAnimation = null;
                        cardTooltip.Set(true);
                    }
                });
            } else {
                cardTooltip.Set(false);
            }

            isInteracted = value;

            if (animateColor) {
                // color animation.
                if (interactColorAnimation != null) {
                    interactColorAnimation.StopWithInstantFinish();
                }

                interactColorAnimation = new AnimationQuery();
                interactColorAnimation.AddToQuery(new ColorAction(this,
                    ShaderInteractionColor,
                    colorAnimationData.interactionColorSpeed,
                    value ? colorAnimationData.interactionGradient : colorAnimationData.interactionEndGradient,
                    colorAnimationData.interactionColorIntensity));

                interactColorAnimation.Start(this, () => {
                    interactColorAnimation = null;
                });
            }
        }

        public void Selected(bool value, bool playEffect = true) {
            if (isInGraveyard)
                return;

            IsSelected = value;

            if (playEffect) {
                cardAnimator.SetTrigger(value ? AnimatorSelectParameterName : AnimatorDeSelectParameterName);
            }
        }

        public void Placing (bool value) {
            cardAnimator.SetBool(AnimatorPlacingParameterName, value);
            if (value) {
                placingEffect.Play();
            }
        }

        public void Placed (bool value) {
            cardAnimator.SetTrigger (AnimatorPlacedParameterName);
            
            if (value) {
                placementEffect.Play();
            }
        }

        #region card actions
        private void UpdateHealth () {
            Debug.Assert(IsOrganic, "UpdateHealth () => This card is not vulnerable.", this);
            healthValueText.text = vulnerableCard.Health.ToString ();
        }

        /// <summary>
        /// Calculate attack point for target card.
        /// </summary>
        /// <param name="card"></param>
        /// <param name="isRangedAttack"></param>
        /// <returns></returns>
        public int CalculateAttack (Card card, bool isRangedAttack, out bool isKilled) {
            var rangedEffect = isRangedAttack && organicAttackerCard != null && organicAttackerCard.AttackType == AttackTypes.Both;

            int amount = card.vulnerableCard.CalculateAttack(attackerCard, rangedEffect);

            isKilled = amount >= card.vulnerableCard.Health;
            if (isKilled) {
                amount = card.vulnerableCard.Health;
            }

            return amount;
        }

        /// <summary>
        /// Attack to a vulnerable.
        /// </summary>
        /// <param name="card"></param>
        public void Attack(Card card, bool rangedAttack = false) {
            Debug.Assert(IsAttacker, "Attack () => This card is not attacker.", this);
            Debug.Assert(card.IsOrganic, "Attack () => Target card is not vulnerable.", this);

            var damageCount = CalculateAttack(card, rangedAttack, out bool _);

            card.Hit(damageCount);
        }

        public void Heal (Card card) {
            Debug.Assert(IsHealer, "Heal () => This card is not attacker.", this);
            Debug.Assert(card.IsOrganic, "Heal () => Target card is not vulnerable.", this);

            card.Healed (healerCard.Heal);
        }

        public void Destroy (Action onCompleted) {
            cardTypeHolder.Close();

            var myLayout = CurrentLayout;
            CurrentLayout.Remove(this);
            CurrentLayout = null;

            var card = this;
            var dieAnimation = new AnimationQuery();
            dieAnimation.AddToQuery(new ColorAction(this, ShaderMainColor, colorAnimationData.destroyEffectSpeed, colorAnimationData.destroyGradient, colorAnimationData.destroyColorIntensity));
            dieAnimation.Start(this, ()=> {
                Debug.Log("[Card] Destroyed.");
                OnToGraveyard?.Invoke(card, false, myLayout);
                onCompleted?.Invoke();
            });
        }

        private AnimationQuery m_hitTimer;
        private int m_hitAmount = 0;
        private void Hit(int amount) {
            Debug.Assert(IsOrganic, "Hit () => This card is not vulnerable.", this);
            Debug.Assert(OnToGraveyard != null, "No listener found to take this card back to graveyard.");

            if (vulnerableCard.Health <= 0) {
                return;
            }

            Interested(false);

            OnCardHit?.Invoke(UserId, amount);

            vulnerableCard.Health = vulnerableCard.Health - amount;

            UpdateHealth();

            m_hitAmount += amount;

            if (m_hitTimer != null) {
                m_hitTimer.Stop();
            }
            
            m_hitTimer = new AnimationQuery();
            m_hitTimer.AddToQuery(new TimerAction(0.2f));
            m_hitTimer.Start(this, hitEffect);

            void hitEffect () {
                PlayTextEffect(m_hitAmount, colorAnimationData.hitEffectTextColorForDamage);
                
                m_hitAmount = 0;

                var damageAnimation = new AnimationQuery();
                damageAnimation.AddToQuery(new ColorAction(this, ShaderEffectColor, colorAnimationData.damageEffectSpeed, colorAnimationData.damageGradient, colorAnimationData.damageColorIntensity));
                damageAnimation.Start(this, null);

                if (vulnerableCard.Health <= 0) {
                    cardTypeHolder.Close();

                    isInGraveyard = true;

                    var myLayout = CurrentLayout;

                    // remove from layout.
                    CurrentLayout.Remove(this);
                    CurrentLayout = null;

                    OnCardKilled?.Invoke(UserId, vulnerableCard.Points);

                    var card = this;
                    var dieAnimation = new AnimationQuery();
                    dieAnimation.AddToQuery(new ColorAction(this, ShaderMainColor, colorAnimationData.dieEffectSpeed, colorAnimationData.dieGradient, colorAnimationData.dieColorIntensity));
                    dieAnimation.Start(this, () => {
                        Debug.Log("[Card] Died.");
                        card.OnToGraveyard?.Invoke(card, true, myLayout);
                    });
                }
            }
        }

        public void PlayTextEffect (object obj, Color color) {
            hitEffectPool.GetRandom<HitEffect>().Play(hitEffectPoint.position, obj, color);
        }
        
        private void Healed(int amount) {
            Debug.Assert(IsOrganic, "Healed () => This card is not vulnerable.", this);

            Interested(false);

            vulnerableCard.Health += amount;

            UpdateHealth();

            PlayTextEffect(amount, colorAnimationData.hitEffectTextColorForHealing);

            var healAnimation = new AnimationQuery();
            healAnimation.AddToQuery(new ColorAction(this, ShaderEffectColor, colorAnimationData.healEffectSpeed, colorAnimationData.healGradient, colorAnimationData.healColorIntensity));
            healAnimation.Start(this, null);
        }
        #endregion
    }
}
