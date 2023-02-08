
namespace CardGame.GameData.Interfaces {
    public interface IVulnerable : ICounterAttack {
        int Health { get; set; }

        /// <summary>
        /// How many points the opponent will receive when this vulnerable is killed?
        /// </summary>
        int Points { get; set; }

        Effect[] Resistances { get; set; }

        /// <summary>
        /// Calculates the amount of damage this can take from the given attacker.
        /// </summary>
        /// <param name="attacker">attacker card</param>
        /// <param name="useRanged">if the attacker can attack both ranged & melee, and attack came from a range. This will be true. And the calculation will use ranged power.</param>
        /// <returns>damage count</returns>
        int CalculateAttack(IAttacker attackerCard, bool useRanged);
    }
}
