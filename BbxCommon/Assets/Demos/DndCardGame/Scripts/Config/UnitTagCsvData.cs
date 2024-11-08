using BbxCommon;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Dcg
{
    public class UnitTagCsvData : CsvDataBase<UnitTagCsvData>
    {
        public string Tag;
        public float Slashing;
        public float Piercing;
        public float Bludgeoning;
        public float Exploding;
        public float Fire;
        public float Cold;
        public float Lightning;
        public float Force;
        public float Radiant;
        public float Poison;
        public float Psychic;
        public float Healing;

        public override EDataLoad GetDataLoadType() => EDataLoad.Addition; 
        public override string[] GetTableNames() => new string[] { "UnitTagData" };

        protected override void ReadLine()
        {
            Tag = GetStringFromKey("Tag");
            Slashing = ParseFloatFromKey("Slashing");
            Piercing = ParseFloatFromKey("Piercing");
            Bludgeoning = ParseFloatFromKey("Bludgeoning");
            Exploding = ParseFloatFromKey("Exploding");
            Fire = ParseFloatFromKey("Fire");
            Cold = ParseFloatFromKey("Cold");
            Lightning = ParseFloatFromKey("Lightning");
            Force = ParseFloatFromKey("Force");
            Radiant = ParseFloatFromKey("Radiant");
            Poison = ParseFloatFromKey("Poison");
            Psychic = ParseFloatFromKey("Psychic");
            Healing = ParseFloatFromKey("Healing");

            if (DataApi.GetData<UnitTagCsvData>(Tag) == null)
                DataApi.SetData<UnitTagCsvData>(Tag, this);
        }

        public float GetCoefficientByDamageType(DamageType damageType)
        {
            switch (damageType)
            {
                case DamageType.Slash:
                    return Slashing;
                case DamageType.Piercing:
                    return Piercing;
                case DamageType.Bludgeoning:
                    return Bludgeoning;
                case DamageType.Exploding:
                    return Exploding;
                case DamageType.Fire:
                    return Fire;
                case DamageType.Cold:
                    return Cold;
                case DamageType.Lightning:
                    return Lightning;
                case DamageType.Force:
                    return Force;
                case DamageType.Radiant:
                    return Radiant;
                case DamageType.Poison:
                    return Poison;
                case DamageType.Psychic:
                    return Psychic;
                case DamageType.Healing:
                    return Healing;
                default:
                    return 1f;
            }
        }
    }
}
