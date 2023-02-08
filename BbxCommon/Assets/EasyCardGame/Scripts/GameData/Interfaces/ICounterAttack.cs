
namespace CardGame.GameData.Interfaces {
    /// <summary>
    /// When the unit receives damage, it will reflect the power.
    /// </summary>
    public interface ICounterAttack {
        public bool CounterEnabled { get; set; }
        public Effect[] CounterEffects { get; set; }
        public string CounterEffectParticle { get; set; }
    }
}