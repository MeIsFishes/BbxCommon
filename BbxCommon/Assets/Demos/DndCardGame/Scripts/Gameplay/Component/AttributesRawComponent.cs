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

        public UiModelVariable<int> MaxHpVariable = new();
        public UiModelVariable<int> CurHpVariable = new();
        public UiModelVariable<List<EDiceType>> ArmorClassVariable = new();
        public UiModelVariable<int> StrengthVariable = new();
        public UiModelVariable<int> DexterityVariable = new();
        public UiModelVariable<int> ConstitutionVariable = new();
        public UiModelVariable<int> IntelligenceVariable = new();
        public UiModelVariable<int> WisdomVariable = new();

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
            while (attributeValue > 0)
            {
                var value = (attributeValue - 1) % 5 + 1;
                attributeValue -= 5;
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
        }

        public override void OnAllocate()
        {
            ArmorClassVariable = ObjectPool<UiModelVariable<List<EDiceType>>>.Alloc();
            ArmorClassVariable.SetValue(SimplePool<List<EDiceType>>.Alloc());
        }

        public override void OnCollect()
        {
            MaxHp = 0;
            CurHp = 0;
            ArmorClass.Clear();
            ArmorClassVariable.SetDirty();
            ArmorClassVariable.CollectToPool();
            Strength = 0;
            Dexterity = 0;
            Constitution = 0;
            Intelligence = 0;
            Wisdom = 0;
        }
    }
}
