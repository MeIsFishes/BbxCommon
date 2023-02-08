using System;
using CardGame.GameData.Interfaces;

namespace CardGame.GameData.Cards {
    [Serializable]
    public class OrganicAttackerCard : OrganicCard, IAttacker, IOrganicAttacker {
        public Effect[] _Attacks;
        public AttackTypes _AttackType;
        public string _RangedEffectParticle;
        public int _RangedEffectRange;
        public bool _RangedEffectBothRows;

        public string RangedEffectParticle {
            get => _RangedEffectParticle;
            set => _RangedEffectParticle = value;
        }

        public int RangedEffectRange {
            get => _RangedEffectRange;
            set => _RangedEffectRange = value;
        }

        public bool RangedEffectBothRows {
            get => _RangedEffectBothRows;
            set => _RangedEffectBothRows = value;
        }

        public Effect[] Attacks {
            get => _Attacks;
            set => _Attacks = value;
        }

        public AttackTypes AttackType { 
            get => _AttackType; 
            set => _AttackType = value; 
        }
    }
}
