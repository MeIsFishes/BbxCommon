using CardGame.GameEvents;
using UnityEngine;

namespace CardGame {
    /// <summary>
    /// Score manager.
    /// </summary>
    public class Scores {
        public static Scores Instance;

        public Scores () {
            Debug.Assert(Instance == null, "This is not the first time that scores class constructed. Only Game.cs can do that, and only one time.");
            Instance = this;
        }

        private int[] scores = new int[2];

        private int calculatedScore;

        #region score calculators
        /// <summary>
        /// A card hitted to a card with amount of {amount}, how many points?
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public int HowManyPointsFromHit(int amount) {
            return amount;
        }

        /// <summary>
        /// Card killed by user, how many points by its startingHealth?
        /// </summary>
        /// <param name="startingHealth"></param>
        /// <returns></returns>
        public int HowManyPointsFromDeath(int startingHealth) {
            return startingHealth;
        }
        #endregion

        public void ClearScores () {
            for (int i=0; i<2; i++) {
                scores[i] = 0;
            }
        }

        #region score simulations
        public void ClearSimulation() {
            Debug.Log("[Scores] ClearSimulation ()");
            calculatedScore = 0;
            EventLocalUserScoreSimulation.OnCleared?.Invoke();
        }
        public void ScoreSimulationHit(int amount) {
            calculatedScore += HowManyPointsFromHit(amount);
        }
        public void ScoreSimulationDeath(int points) {
            calculatedScore += HowManyPointsFromDeath(points);
        }
        public void ScoreSimulationDone() {
            Debug.Log("[Scores] ScoreSimulationDone ()");
            EventLocalUserScoreSimulation.OnTriggered?.Invoke(calculatedScore);
        }
        #endregion

        /// <summary>
        /// Card got hit. Add score to the hitter.
        /// </summary>
        /// <param name="hitPoints"></param>
        public void CardHit(int user, int hitPoints) {
            FixUser(ref user);

            scores[user] += HowManyPointsFromHit(hitPoints);
            EventUserScore.OnTriggered?.Invoke(user, scores[user]);
        }

        private void FixUser (ref int user) {
            if (user == 1) {
                user = 0;
            } else {
                user = 1;
            }
        }
        /// <summary>
        /// Card is dead. Add score to the killer.
        /// </summary>
        /// <param name="startingHealth"></param>
        public void CardKilled(int user, int startingHealth) {
            FixUser(ref user);

            scores[user] += HowManyPointsFromDeath(startingHealth);
            EventUserScore.OnTriggered?.Invoke(user, scores[user]);
        }

        public void CardPlaced(int user) {
            scores[user] += 1;
            EventUserScore.OnTriggered?.Invoke(user, scores[user]);
        }
    }
}
