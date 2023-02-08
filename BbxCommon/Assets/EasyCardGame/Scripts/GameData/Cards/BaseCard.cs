using System;
using UnityEngine;

namespace CardGame.GameData.Cards {
    public enum AttackTypes {
        Melee,
        Ranged,
        Both
    }

    public enum CardInteractionTypes {
        Organic,
        Skill
    }

    public enum CardTypes {
        Attacker,
        Healer
    }

    [Serializable]
    public class BaseCard {
        public void SetBase (BaseCard baseCard) {
            Name = baseCard.Name;
            Desc = baseCard.Desc;
            Avatar = baseCard.Avatar;
            CardInteractionType = baseCard.CardInteractionType;
            CardType = baseCard.CardType;
            EffectParticle = baseCard.EffectParticle;
            EffectRange = baseCard.EffectRange;
            EffectBothRows = baseCard.EffectBothRows;
            RareColor = baseCard.RareColor;
            DropRate = baseCard.DropRate;
        }

        /// <summary>
        /// Name of the card. It can be used as localization Id.
        /// </summary>
        public string Name;

        /// <summary>
        /// Description of the card. It can be used as localication Id. 
        /// </summary>
        public string Desc;

        /// <summary>
        /// Image asset will be showed on the card.
        /// </summary>
        public string Avatar;

        /// <summary>
        /// Rare color of the card, black means no rare.
        /// </summary>
        public Vector4 RareColor;

        /// <summary>
        /// On skill use, this particle will be used.
        /// </summary>
        public string EffectParticle;

        /// <summary>
        /// System will decide how to interact with this card.
        /// </summary>
        public CardInteractionTypes CardInteractionType;

        /// <summary>
        /// What is the purpose of this card? Attacking or healing?
        /// </summary>
        public CardTypes CardType;

        /// <summary>
        /// Range of the effect. 0 means target only, 1 means left & right
        /// </summary>
        public int EffectRange;

        /// <summary>
        /// If this yes, other row also will be effected by effect range.
        /// </summary>
        public bool EffectBothRows;

        /// <summary>
        /// Rate of drop.
        /// </summary>
        public int DropRate;
    }
}

