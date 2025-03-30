using System.Collections.Generic;
using BbxCommon;
using UnityEngine;

namespace Dcg
{
    [CreateAssetMenu(fileName = "SubClassData", menuName = "Dcg/ClassData/SubClassData")]
    public class SubClassData: BbxScriptableObject
    {
        public int Id;
        public int MainClassId;
        public string Name;
        public string SubClassDesc;
        public GameObject SubClassModel;
        public int InitialFeatId;
        public List<int> LevelFeatGroupId;
        
        protected override void OnLoad()
        {
            DataApi.SetData(Id,this);
        }
    }
}