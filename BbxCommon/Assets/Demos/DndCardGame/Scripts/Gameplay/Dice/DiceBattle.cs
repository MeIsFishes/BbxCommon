using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dcg
{
    public class DiceBattle
    {
        public DiceCamp Camp1;
        public DiceCamp Camp2;
    }

    public class DiceCamp
    {
        /// <summary>
        /// 玩家主动拖入的骰子
        /// </summary>
        public DiceGroup RollingDice;
        /// <summary>
        /// 属性加值
        /// </summary>
        public DiceGroup AttributesModifier;
        /// <summary>
        /// buff加值
        /// </summary>
        public DiceGroup BuffModifier;
    }
}
