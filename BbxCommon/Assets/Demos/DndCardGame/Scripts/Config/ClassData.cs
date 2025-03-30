using System.Collections.Generic;
using BbxCommon;
using UnityEngine;

namespace Dcg
{
    [CreateAssetMenu(fileName = "ClassData", menuName = "Dcg/ClassData/ClassData")]
    public class ClassData: BbxScriptableObject
    {
        public int Id;
        public string Name;
        public string ClassDesc;
        public GameObject Model;
        public int InitialFeatId;
        public List<int> LevelFeatGroupId;
        
        
        protected override void OnLoad()
        {
            DataApi.SetData(Id,this);
        }
    }
}