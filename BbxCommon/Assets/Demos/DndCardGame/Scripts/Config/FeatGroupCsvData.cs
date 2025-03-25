using System.Collections.Generic;
using BbxCommon;

namespace Dcg
{
    public class FeatGroupCsvData: CsvDataBase<FeatGroupCsvData>
    {
        public int GroupId;
        public string GroupName;
        public string GroupDesc;
        public int[] FeatList;
        
        public override string[] GetTableNames() => new string[] { "FeatGroupData" };

        protected override void ReadLine()
        {
            GroupId = ParseIntFromKey("GroupId");
            GroupName = GetStringFromKey("GroupName");
            GroupDesc = GetStringFromKey("GroupDesc");
            FeatList = ParseIntArrayFromKey("FeatList");
            
            DataApi.SetData<FeatGroupCsvData>(this);
        }
    }
}