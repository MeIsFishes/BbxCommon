using CardGame.Animation;
using CardGame.Layouts;
using CardGame.Effects;
using CardGame.GameEvents;

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace CardGame.GameFunctions {
    public class UserTargetedCard : BaseFunction {
        private int cardLayoutIndex;
        private int cardIndex;
        private int targetLayoutIndex;
        private int targetCardIndex;

        private List<SkillEffect> currentEffects = new List<SkillEffect>();

        public UserTargetedCard(int cardLayoutIndex, int cardIndex, int targetLayoutIndex, int targetCardIndex, Game game) : base(game) {
            this.cardLayoutIndex = cardLayoutIndex;
            this.cardIndex         = cardIndex;
            this.targetLayoutIndex = targetLayoutIndex;
            this.targetCardIndex   = targetCardIndex;

            Debug.LogFormat("[UserTargetedCard] cardLayoutIndex {0} cardIndex {1} targetLayoutIndex {2} targetCardIndex {3}",
                            this.cardLayoutIndex,
                            this.cardIndex,
                            this.targetLayoutIndex,
                            this.targetCardIndex
                );
        }

        public override void Clear() {
            base.Clear();

            int count = currentEffects.Count;
            for (int i=0; i<count; i++) {
                currentEffects[i].Stop();
            }

            currentEffects.Clear();
        }

        private void PlayEffect (string particleId, Card selectedCard, Vector3 targetPosition, int effectRange, Action onEffectCompleted) {
            Debug.LogFormat("[PlayEffect] {0}, time: {1}", particleId, Time.time);
            // create effect.
            var createdEffect = game.effectsData.Get<SkillEffect>(particleId);
            if (createdEffect == null) {
                Debug.LogErrorFormat("[UserTargetedCard] Target effect {0} is not found on Resources folder, please fix the card named {1}", particleId, selectedCard.baseCard.Name);
                onEffectCompleted?.Invoke ();
                return;
            }

            currentEffects.Add(createdEffect);

            //play main effect.
            createdEffect.Play(cardLayoutIndex == 0 ? selectedCard.CursorPosition : selectedCard.CursorFocusPoint, targetPosition, effectRange, (hitEffectId) => {
                Debug.Log("Effect done, looking for hit effect.");
                
                currentEffects.Remove(createdEffect);

                if (!string.IsNullOrEmpty(hitEffectId)) {
                    var createdHitEffect = game.effectsData.Get<SkillEffect>(hitEffectId);

                    Debug.LogFormat("[UserTargetedCard] Playing hit effect {0}", hitEffectId);

                    if (createdHitEffect == null) {
                        Debug.LogErrorFormat("[UserTargetedCard] Hit effect with Id => {0} is null on effect => {1}, please fix this issue by finding {1} on Resources folder", hitEffectId, particleId);
                        onEffectCompleted?.Invoke();
                    } else {
                        currentEffects.Add(createdHitEffect);
                        createdHitEffect.Play(targetPosition, targetPosition, 0, (_) => {
                            completed();
                            currentEffects.Remove(createdHitEffect);
                        });
                    }
                } else {
                    completed();
                }
            });

            void completed () {
                onEffectCompleted?.Invoke();
            }
        }

        public override void Trigger(Action onCompleted) {
            EventUserTargetedCard.OnTriggered?.Invoke(game.currentTurn);

            game.Decks[game.currentTurn].enabled = false; // no interaction.

            Card selectedCard;
            Card targetCard;

            // target cards by effect range of the selected card.
            Card[] targetCards;

            if (cardLayoutIndex == 0) {
                // this card is from deck.
                selectedCard = game.Decks[game.currentTurn].Get(cardIndex);
            } else {
                // this card is from ground.
                if (game.currentTurn == 0) {
                    selectedCard = game.UserTables[cardLayoutIndex - 1].Get(cardIndex);
                } else {
                    selectedCard = game.OpponentTables[cardLayoutIndex - 1].Get(cardIndex);
                }
            }

            bool isRangedAttack = Mathf.Abs(cardLayoutIndex - targetLayoutIndex) >= 1;
            int effectRange = selectedCard.baseCard.EffectRange;

            var particleId = selectedCard.baseCard.EffectParticle;
            if (isRangedAttack && selectedCard.IsOrganic && selectedCard.IsAttacker && selectedCard.organicAttackerCard.AttackType == GameData.Cards.AttackTypes.Both) {
                // this attack will use Ranged Effect & Range
                particleId = selectedCard.organicAttackerCard.RangedEffectParticle;
                effectRange = selectedCard.organicAttackerCard.RangedEffectRange;
            }

            Debug.LogFormat ("Checking for target table with => {0} {1} {2} {3}", cardLayoutIndex, cardIndex, targetLayoutIndex, targetCardIndex);

            Layout targetTable;
            Layout otherTable;

            if (game.currentTurn == 0) {
                if (selectedCard.IsHealer) {
                    targetTable = game.UserTables[targetLayoutIndex];
                    otherTable = game.UserTables[targetLayoutIndex == 0 ? 1 : 0];
                } else {
                    var tIndex = 3 - targetLayoutIndex;
                    targetTable = game.OpponentTables[tIndex];
                    otherTable = game.OpponentTables[tIndex == 0 ? 1 : 0];
                }
            } else {
                if (selectedCard.IsHealer) {
                    targetTable = game.OpponentTables[targetLayoutIndex];
                    otherTable = game.OpponentTables[targetLayoutIndex == 0 ? 1 : 0];
                } else {
                    var tIndex = 3 - targetLayoutIndex;
                    targetTable = game.UserTables[tIndex];
                    otherTable = game.UserTables[tIndex == 0 ? 1 : 0];
                }
            }

            targetCard = targetTable.Get(targetCardIndex); 

            /// get all cards as target in the effect range of selected card.
            targetCards = targetTable.GetCardsInRange(targetCard, effectRange, new List<Card>() { selectedCard });

            var rangedEffect = isRangedAttack && selectedCard.organicAttackerCard != null && selectedCard.organicAttackerCard.AttackType == GameData.Cards.AttackTypes.Both;
            
            if ( (rangedEffect && selectedCard.organicAttackerCard.RangedEffectBothRows) ||
                selectedCard.baseCard.EffectBothRows) {

                var otherTargets = otherTable.GetCardsInRange(targetCardIndex, effectRange, new List<Card>() { selectedCard });

                var asList = targetCards.ToList();
                asList.AddRange(otherTargets);
                targetCards = asList.ToArray();
            }

            if (cardLayoutIndex == 0) {
                // played from deck show the card first.
                if (game.currentTurn == 1) { // opponent plays from his hand. show card then result.
                    game.ShowCard(selectedCard, result);
                } else {
                    // local user plays from his hand, go to result.
                    result();
                }
            } else {
                // user plays from table, go to result.
                result();
            }

            void result() {
                Debug.Log("result ()");
                bool didHit = false;
                bool isDone = false;

                if (string.IsNullOrEmpty (particleId)) {
                    Debug.LogErrorFormat("[UserTargetedCard] Effect Id is null or empty, please fix the card named {0}", selectedCard.baseCard.Name);
                    onEffectCompleted();
                    return;
                }

                for (int i = 0, length = targetCards.Length; i < length;  i++) {
                    if (targetCards[i] == null) {
                        continue;
                    }

                    Vector3 targetPosition = targetCards[i].CursorFocusPoint;

                    PlayEffect(particleId, selectedCard, targetPosition, effectRange, onEffectCompleted);
                }

                void onEffectCompleted() {
                    if (didHit) {
                        return;
                    }

                    didHit = true;

                    Debug.Log("ATTACK RANGE => " + Mathf.Abs(cardLayoutIndex - targetLayoutIndex));

                    if (selectedCard.IsAttacker) {
                        int totalCounters = 0;

                        // attacker card will attack the target cards.
                        for (int i = 0, length = targetCards.Length; i < length; i++) {
                            if (targetCards[i] != null) {
                                var targetCard = targetCards[i];

                                selectedCard.Attack(targetCard, isRangedAttack);

                                if (selectedCard.IsOrganic) {
                                    // check counter attack & done.
                                    if (targetCard.vulnerableCard.Health > 0 && targetCard.vulnerableCard.CounterEnabled) {
                                        // play counter text effect.

                                        totalCounters++;

                                        var timer = new AnimationQuery();
                                        timer.AddToQuery(new TimerAction(1));
                                        timer.Start(game, playCounter);

                                        void playCounter () {
                                            targetCard.PlayTextEffect("Counter!", Color.white);

                                            var attacker = targetCard;
                                            var receiver = selectedCard;
                                            // play effect & damage to the selected card.
                                            PlayEffect(targetCard.vulnerableCard.CounterEffectParticle,
                                                targetCard,
                                                selectedCard.CursorFocusPoint,
                                                1,
                                                () => {
                                                    if (selectedCard.vulnerableCard.Health > 0) {
                                                    // replace with counter.
                                                    var attacks = attacker.attackerCard.Attacks;
                                                        attacker.attackerCard.Attacks = attacker.vulnerableCard.CounterEffects;

                                                        attacker.Attack(receiver);

                                                    // restore.
                                                    attacker.attackerCard.Attacks = attacks;
                                                    }

                                                    totalCounters--;
                                                    if (totalCounters == 0) {
                                                        if (!isDone) {
                                                            var timer = new AnimationQuery();
                                                            timer.AddToQuery(new TimerAction(1f));
                                                            timer.Start(game, done);
                                                        }
                                                    }
                                                }
                                            );
                                        }
                                    }
                                }
                            }
                        }

                        if (totalCounters == 0) {
                            done();
                        }
                    } else if (selectedCard.IsHealer) {
                        // healer card will heal the target cards.
                        for (int i = 0, length = targetCards.Length; i < length; i++) {
                            if (targetCards[i] != null) {
                                selectedCard.Heal(targetCards[i]);
                            }
                        }

                        done();
                    }

                    void done () {
                        if (isDone) {
                            return;
                        }

                        Debug.Log("[UserTargetedCard] Targeting is done.");

                        isDone = true;

                        if (cardLayoutIndex == 0) {
                            // this is a hand card. destroy selected.
                            Debug.Log("[Game] Destroying card from hand.");
                            selectedCard.Destroy(() => {
                                Debug.Log("Destroyed...");
                                game.Decks[game.currentTurn].Remove(selectedCard);
                                game.Decks[game.currentTurn].ReOrder();
                                game.Decks[game.currentTurn].Refresh(finish);
                            });
                        } else {
                            finish();
                        }

                        void finish() {
                            // wait 1 seconds and next turn.
                            currentAnimation = new AnimationQuery();
                            currentAnimation.AddToQuery(new TimerAction(1f));
                            currentAnimation.Start(game, () => {
                                currentAnimation = null;
                                game.NextTurn();
                                onCompleted?.Invoke();
                            });
                        }
                    }
                }
            }
        }
    }
}
