using System;
using CardGame.GameData.Interfaces;
using System.Linq;

namespace CardGame.GameData.Cards {
    /// <summary>
    /// Organic cards have life & resistances. Can be attacked.
    /// </summary>
    [Serializable]
    public class OrganicCard : BaseCard, IVulnerable {
        public int _Health;
        public int _Points;
        public Effect[] _Resistances;

        public bool _CounterEnabled;
        public string _CounterEffectParticle;
        public Effect[] _CounterEffects;

        public int Health { get => _Health; set => _Health = value; }
        public int Points { get => _Points; set => _Points = value; }
        public Effect[] Resistances { get => _Resistances; set => _Resistances = value; }

        public bool CounterEnabled { get => _CounterEnabled; set => _CounterEnabled = value; }
        public string CounterEffectParticle { get => _CounterEffectParticle; set => _CounterEffectParticle = value; }
        public Effect[] CounterEffects { get => _CounterEffects; set => _CounterEffects = value; }

        public void SetOrganicCard (OrganicCard card) {
            Health = card.Health;
            Points = card.Points;
            Resistances = card.Resistances;

            CounterEnabled = card.CounterEnabled;
            CounterEffectParticle = card.CounterEffectParticle;
            CounterEffects = card.CounterEffects;
        }

        public int CalculateAttack (IAttacker attacker, bool useRanged) {
            int totalDamage = 0;

            var resistanceList = _Resistances.ToList();

            int attackCount = attacker.Attacks.Length;
            for (int i=0; i<attackCount; i++) {
                var resistance = resistanceList.Find(x => x.EffectType == attacker.Attacks[i].EffectType);
                int damage = useRanged ? attacker.Attacks[i].RangedPower : attacker.Attacks[i].Power;
                if (resistance != null) {
                    damage -= resistance.Power;
                    if (damage < 0) // clamp.
                        damage = 0;
                }

                totalDamage += damage;
            }

            return totalDamage;
        }
    }
}
