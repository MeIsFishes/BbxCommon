using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using CardGame.Animation;

namespace CardGame.Players {
    public enum AIMode {
        Easy,
        Medium,
        Hard
    }
    public class AIPlayer : Player {
        private int randomizer;

        public AIPlayer(Game game, int layoutIndex, AIMode difficultyMode) : base(game, layoutIndex) {
            switch (difficultyMode) {
                case AIMode.Easy: randomizer = 2; break;
                case AIMode.Medium: randomizer = 5; break;
                default: randomizer = 10; break;
            }
        }

        private AnimationQuery waitingAnimation;

        private Card targetedCard = null;
        private int targetedLayout = 0;

        public override void Clear() {
            base.Clear();

            if (waitingAnimation != null) {
                waitingAnimation.Stop();
                waitingAnimation = null;
                targetedCard = null;
            }
        }

        public override void OnTurn() {
            base.OnTurn();

            Debug.Log("On AI Turn, waiting for AI move.");
            
            targetedCard = null;

            waitingAnimation = new AnimationQuery();
            waitingAnimation.AddToQuery(new TimerAction(Random.Range(1, 3)));
            waitingAnimation.Start(game, AIPlayAlgorithm);
        }

        private void AIPlayAlgorithm () {
            List<Card> checkedCards = new List<Card>();

            var availableCards = myHand.cards.ToList().FindAll(x => x != null);

            int cardsCount = availableCards.Count;

            Card pickCard (List<Card> except) {
                var pickable = availableCards.FindAll(x => !except.Contains(x)).OrderBy (x=>x.Points).ToList ();

                if (Random.Range (0, 10) < randomizer) {
                    // !smart select!
                    int pickableCount = pickable.Count;
                    for (int i = 0; i < pickableCount; i++) {
                        if (!pickable[i].IsOrganic) {
                            // this is a skill card and we will try to use its all power here.
                            int range = pickable[i].baseCard.EffectRange;
                            var possibleTargets = GetValidTargetLayouts(pickable[i]);
                            foreach (var target in possibleTargets) {
                                if (target.ValidCardCount >= range) {
                                    return pickable[i];
                                }
                            }
                        }
                    }

                    return pickable[0];
                } else {
                    // select randomly.
                    return pickable[Random.Range(0, pickable.Count)];
                }
            }

            while (cardsCount > 0) {
                cardsCount--;

                // pick random card.
                var pickedCard = pickCard(checkedCards);

                // add to already checked cards.
                checkedCards.Add(pickedCard);

                if (pickedCard.IsHealer) {
                    var found = mostNeedHealing(pickedCard, out int targetLayout, out int targetIndex);
                    if (found != null) {
                        if (pickedCard.IsOrganic) {
                            // save found & place.
                            targetedCard = found;
                            targetedLayout = targetLayout;

                            //
                            game.UserPlacedCard(myHand.FindIndex(pickedCard), targetLayout, targetIndex);
                        } else {
                            // target directly.
                            game.UserTargetedCard(0, myHand.FindIndex(pickedCard), targetLayout, targetIndex);
                        }

                        return;
                    }
                } else {
                    Debug.Log("[AIPlayer] Selected an attacker card.");

                    var found = mostAttack(0, pickedCard, out int targetLayout, out int targetIndex, out bool isRanged, out int placementLayout);
                    if (found != null) {
                        Debug.LogFormat("[AIPlayer] Found target => {0} isRanged {1}", found.baseCard.Name, isRanged);
                        if (pickedCard.IsOrganic) {
                            int placeRow = placementLayout;

                            Debug.Log("[AIPlayer] Place row => " + placeRow);

                            int placeIndex = myLayouts[placeRow].FindAPlace();

                            if (placeIndex == -1) {
                                Debug.Log("[AIPlayer] No place found, row is full. skipping.");
                                continue;
                            }

                            placeIndex = Random.Range(0, myLayouts[placeRow].Capacity);

                            // save the target.
                            Debug.LogFormat ("[AIPlayer] Saving target, targetLayout {0}, targetIndex {1}", targetLayout, targetIndex);
                            targetedCard = found;
                            targetedLayout = targetLayout;
                            //

                            game.UserPlacedCard(myHand.FindIndex(pickedCard), placeRow, placeIndex);
                        } else {
                            // target directly.
                            game.UserTargetedCard(0, myHand.FindIndex(pickedCard), targetLayout, targetIndex);
                        }

                        return;
                    }
                }
            }

            // no card found. find a best pickable one from hand and place.
            Card picked = null;
            var organicCardsToPlace = myHand.cards.ToList().FindAll(x => x != null && x.IsOrganic).OrderBy(x => x.vulnerableCard.Points).ToList();
            int count = organicCardsToPlace.Count;

            if (count > 0) {
                if (Random.Range(0, 10) <= 8) {
                    // pick the first one.
                    Debug.Log("[AIPlayer] Picking the first one of the list.");
                    picked = organicCardsToPlace[0];
                } else {
                    // pick random.
                    Debug.Log("[AIPlayer] Picking randomly");
                    picked = organicCardsToPlace[Random.Range(0, count)];
                }

                var index = myHand.FindIndex(picked);
                var tLayouts = GetValidPlacementLayouts(picked);

                int length = tLayouts.Length;
                int choosed = Random.Range(0, length);
                var tLayout = tLayouts[choosed];
                for (int l = 0, m_l = myLayouts.Length; l < m_l ; l++) {
                    if (myLayouts[l] == tLayout) {
                        int place = tLayout.FindAPlace();
                        if (place != -1) {
                            // place here.
                            int placeIndex = Random.Range(0, tLayout.Capacity);
                            Debug.LogFormat("[AIPlayer] Placing " + l);
                            game.UserPlacedCard(myHand.FindIndex(picked), l, placeIndex);
                            return;
                        }
                    }
                }
            } else {
                Debug.LogError("[AIPlayer] could not find any move. Passing.");
                game.UserNoExtraMove();
                return;
            }

            Card mostNeedHealing(Card card, out int tLayout, out int tIndex) {
                Debug.LogFormat("[AIPlayer] mostNeedHealing, selected card {0}", card.baseCard.Name);

                // find a place by possible target.
                int mostNeed = -int.MaxValue;
                var layouts = GetValidPlacementLayouts(card);
                int layoutsLength = layouts.Length;

                Card tCard = null;
                tLayout = 0;
                tIndex = 0;

                // find a card needs help most.
                for (int i = 0; i < layoutsLength; i++) {
                    if (!card.IsOrganic || layouts[i].FindAPlace() != -1) {
                        Debug.LogFormat("[AIPlayer] LayoutIndex {0} => Searching for a card in need of healing most.", i);
                        var orderByNeeds = layouts[i].cards.ToList().FindAll(x => x != null).OrderByDescending(x => x.vulnerableCard.Points - x.vulnerableCard.Health).ToList();

                        if (orderByNeeds.Count > 0 && orderByNeeds[0] != null) {
                            var needRate = orderByNeeds[0].vulnerableCard.Points - orderByNeeds[0].vulnerableCard.Health;

                            Debug.LogFormat("[AIPlayer] Card found that needed heal. How much heal it need {0}, cardName {1}",needRate, orderByNeeds[0].baseCard.Name);

                            if (needRate > mostNeed || Random.Range (0, 10) >= randomizer) {
                                Debug.LogFormat ("[AIPlayer] Selected card as a heal target => {0}", orderByNeeds[0].baseCard.Name);
                                tCard = orderByNeeds[0];
                                mostNeed = needRate;

                                targetedLayout = i;

                                for (int l = 0; l < 2; l++) {
                                    if (myLayouts[l] == layouts[i]) {
                                        tLayout = l;
                                        break;
                                    }
                                }

                                if (card.IsOrganic) {
                                    tIndex = Random.Range(0, layouts[i].Capacity);
                                } else {
                                    tIndex = layouts[i].FindIndex(tCard);
                                }
                            }
                        }
                    }
                }

                Debug.LogFormat("[AIPlayer] card needs healing => {0}", tCard);
                return tCard;
            }

            Card mostAttack(int cardLayoutIndex, Card card, out int tLayout, out int tIndex, out bool isRanged, out int placementLayout) {
                Debug.LogFormat("[AIPLayer] Checking for best attack for card {0}", card.baseCard.Name);
                // attacker card.
                var targetLayouts = GetValidTargetLayouts(card);
                int targetLayoutsLength = targetLayouts.Length;

                var placementLayouts = GetValidPlacementLayouts(card);
                int placementLayoutsLength = placementLayouts.Length;

                int effectRange = card.baseCard.EffectRange;

                tLayout = 0;
                tIndex = 0;
                isRanged = false;
                placementLayout = 0;

                Debug.LogFormat("[AIPLayer] Effect range => {0}", effectRange);

                int mostPoint = -1;
                Card finalCard = null;

                // foreach placement layouts, search for each opponent places.
                for (int p = 0; p<placementLayoutsLength; p++) {
                    int thisLayout = 0;
                    for (int m = 0; m < 2; m++) {
                        if (placementLayouts[p] == myLayouts[m]) {
                            thisLayout = m;
                            break;
                        }
                    }

                    for (int t = 0; t < targetLayoutsLength; t++) {

                        int theirLayout = 0;
                        for (int m = 0; m<2; m++) {
                            if (targetLayouts[t] == opponentLayouts[m]) {
                                theirLayout = m;
                            }
                        }

                        int mtLayout = 3 - theirLayout;

                        Debug.Log("thisLayout = " + thisLayout + " theirLayout" + theirLayout);

                        isRanged = Mathf.Abs(mtLayout - thisLayout) > 1;

                        var m_isRanged = isRanged;
                        var orderByPoints = targetLayouts[t].cards.ToList().FindAll(x => x != null).
                            OrderByDescending(x => points(x, m_isRanged)).ThenByDescending (x=>x.vulnerableCard.Points).ToList();

                        if (orderByPoints.Count > 0 && orderByPoints[0] != null) {
                            int attackPoints = points(orderByPoints[0], isRanged);

                            if (attackPoints > mostPoint || Random.Range(0, 10) >= randomizer) {
                                mostPoint = attackPoints;

                                finalCard = orderByPoints[0];
                                Debug.LogFormat("[AIPlayer] attack target found {0}", finalCard.baseCard.Name);
                                tIndex = targetLayouts[t].FindIndex(finalCard);
                                tLayout = mtLayout;
                                placementLayout = thisLayout;
                                Debug.LogFormat("[AIPlayer] tLayout {0}, tIndex {1}", tLayout, tIndex);
                            }
                        }

                        int points(Card x, bool isRangedAttack) {
                            if (x == null) {
                                return 0;
                            }

                            Debug.Log("[AIPlayer] Calculating hit to " + x.baseCard.Name + " isRanged: " + isRangedAttack);

                            int totalPoints = 0;

                            void calculateHitCards (Card[] hitCards) {
                                for (int h = 0, length = hitCards.Length; h < length; h++) {
                                    if (hitCards[h] == null)
                                        continue;

                                    int tpm = card.CalculateAttack(hitCards[h], isRangedAttack, out bool isKilled);
                                    Debug.Log("[AIPlayer] Calculated attack for " + hitCards[h].baseCard.Name);

                                    tpm = Scores.Instance.HowManyPointsFromHit(tpm);
                                    if (isKilled) {
                                        tpm += Scores.Instance.HowManyPointsFromDeath(x.vulnerableCard.Points);
                                    }

                                    totalPoints += tpm;
                                }
                            }

                            var targetCards = targetLayouts[t].GetCardsInRange(x, effectRange);

                            calculateHitCards(targetCards);

                            if ( (isRangedAttack && card.organicAttackerCard != null && card.organicAttackerCard.RangedEffectBothRows) || card.baseCard.EffectBothRows) {
                                var otherLayout = targetLayouts[t] == opponentLayouts[0] ? opponentLayouts[1] : opponentLayouts[0];

                                var otherCards = otherLayout.GetCardsInRange(card.CurrentLayout.FindIndex(card), effectRange);
                                calculateHitCards(otherCards);
                            }

                            Debug.LogFormat("[AIPlayer] Total points calculated for {0} => {1}", x.baseCard.Name, totalPoints);

                            return totalPoints;
                        }
                    }
                }

                return finalCard;
            }
        }

        public override void AimFromCard(int cardLayoutIndex, int cardIndex) {
            Debug.Log("[AIPlayer] Aim from card => card layout index: " + cardLayoutIndex);

            if (targetedCard != null) {
                // find tIndex;
                int targetedIndex;
                if (targetedLayout >= 2) {
                    targetedIndex = opponentLayouts[3 - targetedLayout].FindIndex(targetedCard);
                } else {
                    targetedIndex = myLayouts[targetedLayout].FindIndex(targetedCard);
                }

                game.UserTargetedCard(cardLayoutIndex, cardIndex, targetedLayout, targetedIndex);
            } else {
                game.UserNoExtraMove();
            }
        }
    }
}
