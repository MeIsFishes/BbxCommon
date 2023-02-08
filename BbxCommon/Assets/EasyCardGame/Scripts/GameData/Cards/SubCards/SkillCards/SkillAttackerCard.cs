using System;
using CardGame.GameData.Interfaces;

namespace CardGame.GameData.Cards {
    [Serializable]
    public class SkillAttackerCard : BaseCard, IAttacker {
        public Effect[] _Attacks;

        public Effect[] Attacks {
            get => _Attacks;
            set => _Attacks = value;
        }
    }
}
