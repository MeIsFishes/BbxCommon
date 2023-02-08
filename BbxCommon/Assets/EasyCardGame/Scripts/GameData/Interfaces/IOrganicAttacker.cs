using CardGame.GameData.Cards;

namespace CardGame.GameData.Interfaces {
    public interface IOrganicAttacker {
        AttackTypes AttackType { get; set; }

        /// <summary>
        /// On organic card, there is a possibility to make melee & ranged both.
        /// But in ranged attack, this will override the EffectParticle of BaseCard.
        /// </summary>
        string RangedEffectParticle { get; set; }

        /// <summary>
        /// Range of ranged attacks. 0 means target only, 1 means left & right
        /// </summary>
        int RangedEffectRange { get; set; }

        bool RangedEffectBothRows { get; set; }
    }
}
