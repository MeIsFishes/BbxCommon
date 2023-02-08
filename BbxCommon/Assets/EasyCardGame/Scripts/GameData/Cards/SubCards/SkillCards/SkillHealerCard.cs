using System;
using CardGame.GameData.Interfaces;

namespace CardGame.GameData.Cards {
    [Serializable]
    public class SkillHealerCard : SkillCard, IHealer {
        public int _Heal;

        public int Heal {
            get => _Heal;
            set => _Heal = value;
        }
    }
}
