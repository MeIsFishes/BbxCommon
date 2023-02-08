using UnityEngine;

namespace CardGame {
    /// <summary>
    /// Main settings of the game.
    /// </summary>
    [CreateAssetMenu(fileName = "GameSettings", menuName = "CardGame/GameSettings", order = 1)]
    public class GameSettings : ScriptableObject {
        [Tooltip ("How many cards per round?")]
        public int CardsPerRound;

        [Tooltip("How many rounds?")]
        public int RoundCount = 2;

        public int GameCardPoolSize => CardsPerRound * 2 * RoundCount;
    }
}

