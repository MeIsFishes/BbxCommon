using BbxCommon;
using System.Collections.Generic;
using System.Text;

namespace Dcg
{
    public class TestCsvData : CsvDataBase<TestCsvData>
    {
        public enum ETestEnum
        {
            Test,
            CeShi,
        }

        public int IntValue;
        public float FloatValue;
        public ETestEnum EnumValue;
        public List<bool> BoolArrayValue;

        public override EDataLoad GetDataLoadType() => EDataLoad.Addition;
        public override string[] GetTableNames() => new string[] { "TestCsvData" };

        protected override void ReadLine()
        {
            IntValue = ParseIntFromKey("IntValue");
            FloatValue = ParseFloatFromKey("FloatValue");
            EnumValue = ParseEnumFromKey<ETestEnum>("EnumValue");
            var bools = ParseBoolArrayFromKey("BoolArrayValue");
            BoolArrayValue = new List<bool>();
            if (bools != null)
            {
                for (int i = 0; i < bools.Length; i++)
                {
                    BoolArrayValue.Add(bools[i]);
                }
            }
            DebugApi.Log(this.ToString());
            DataApi.SetData<TestCsvData>(this);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("IntValue: ");
            sb.Append(IntValue.ToString());
            sb.Append("\nFloatValue: ");
            sb.Append(FloatValue.ToString());
            sb.Append("\nEnumValue: ");
            sb.Append(EnumValue.ToString());
            sb.Append("\nBoolArrayValue: ");
            for (int i = 0; i < BoolArrayValue.Count; i++)
            {
                sb.Append(BoolArrayValue[i].ToString());
                if (i != BoolArrayValue.Count - 1)
                    sb.Append(", ");
            }
            return sb.ToString();
        }
    }
}
