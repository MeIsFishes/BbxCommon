using System;
using System.Collections.Generic;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg
{
    /// <summary>
    /// 任务基础属性，DND把力量敏捷这种属性叫做ability
    /// </summary>
    public enum EAbility
    {
        /// <summary>
        /// 力量
        /// </summary>
        Strength,
        /// <summary>
        /// 敏捷
        /// </summary>
        Dexterity,
        /// <summary>
        /// 体质
        /// </summary>
        Consititution,
        /// <summary>
        /// 智力
        /// </summary>
        Intelligence,
        /// <summary>
        /// 感知
        /// </summary>
        Wisdom,
    }

    public class AttributesRawComponent : EcsRawComponent
    {
        public int MaxHp { get { return MaxHpVariable.Value; } set { MaxHpVariable.SetValue(value); } }
        public int CurHp { get { return CurHpVariable.Value; } set { CurHpVariable.SetValue(value); } }

        public List<EDiceType> ArmorClass => ArmorClassVariable.Value;

        public int Strength { get { return StrengthVariable.Value; } set { StrengthVariable.SetValue(value); } }
        public int Dexterity { get { return DexterityVariable.Value; } set { DexterityVariable.SetValue(value); } }
        public int Constitution { get { return ConstitutionVariable.Value; } set { ConstitutionVariable.SetValue(value); } }
        public int Intelligence { get { return IntelligenceVariable.Value; } set { IntelligenceVariable.SetValue(value); } }
        public int Wisdom { get { return WisdomVariable.Value; } set { WisdomVariable.SetValue(value); } }

        public ListenableVariable<int> MaxHpVariable = new();
        public ListenableVariable<int> CurHpVariable = new();
        public ListenableVariable<List<EDiceType>> ArmorClassVariable = new(new());
        public ListenableVariable<int> StrengthVariable = new();
        public ListenableVariable<int> DexterityVariable = new();
        public ListenableVariable<int> ConstitutionVariable = new();
        public ListenableVariable<int> IntelligenceVariable = new();
        public ListenableVariable<int> WisdomVariable = new();
         
        public Action<int> BeforeTakeDamage;
        public Action<int> AfterTakeDamge;

        public void GetModifierDiceList(EAbility attribute, List<Dice> res)
        {
            int attributeValue = 0;
            switch (attribute)
            {
                case EAbility.Strength:
                    attributeValue = Strength;
                    break;
                case EAbility.Dexterity:
                    attributeValue = Dexterity;
                    break;
                case EAbility.Consititution:
                    attributeValue = Constitution;
                    break;
                case EAbility.Intelligence:
                    attributeValue = Intelligence;
                    break;
                case EAbility.Wisdom:
                    attributeValue = Wisdom;
                    break;
            }
            var value = (attributeValue - 1) % 5 + 1;
            for (int i = 0; i < (attributeValue - value) / 5; i++)
            {
                res.Add(Dice.Create(EDiceType.D12));    // 每5点属性填充1个d12
            }
            // 计算剩下的属性
            switch (value)
            {
                case 1:
                    res.Add(Dice.Create(EDiceType.D4));
                    break;
                case 2:
                    res.Add(Dice.Create(EDiceType.D6));
                    break;
                case 3:
                    res.Add(Dice.Create(EDiceType.D8));
                    break;
                case 4:
                    res.Add(Dice.Create(EDiceType.D10));
                    break;
                case 5:
                    res.Add(Dice.Create(EDiceType.D12));
                    break;
            }
        }

        public override void OnCollect()
        {
            MaxHpVariable.MakeInvalid();
            CurHpVariable.MakeInvalid();
            ArmorClassVariable.MakeInvalid();
            StrengthVariable.MakeInvalid();
            DexterityVariable.MakeInvalid();
            ConstitutionVariable.MakeInvalid();
            IntelligenceVariable.MakeInvalid();
            WisdomVariable.MakeInvalid();
            MaxHp = 0;
            CurHp = 0;
            ArmorClass.Clear();
            ArmorClassVariable.SetDirty();
            Strength = 0;
            Dexterity = 0;
            Constitution = 0;
            Intelligence = 0;
            Wisdom = 0;
        }

        public void BindTakeDamage(Action<int> before, Action<int> after)
        {
            if (before != null)
            {
                BeforeTakeDamage += before;
            }
            if (after != null)
            {
                AfterTakeDamge += before;
            }
        }

        public void UnbindTakeDamage(Action<int> before, Action<int> after)
        {
            if (before != null)
            {
                BeforeTakeDamage -= before;
            }
            if (after != null)
            {
                AfterTakeDamge -= before;
            }
        }


        public void OnTakeDamage(int damge)
        {
            BeforeTakeDamage?.Invoke(damge);

            CurHp -= damge;

            AfterTakeDamge?.Invoke(damge);

        }
    }
}
