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
        /// ����������������
        /// </summary>
        public DiceGroup RollingDice;
        /// <summary>
        /// ���Լ�ֵ
        /// </summary>
        public DiceGroup AttributesModifier;
        /// <summary>
        /// buff��ֵ
        /// </summary>
        public DiceGroup BuffModifier;
    }
}
