using BbxCommon;
using UnityEngine;

namespace Dcg
{
    [CreateAssetMenu(fileName = "FeatData", menuName = "Dcg/FeatData")]
    public class FeatData : BbxScriptableObject
    {
        public int Id;
        public string FeatName;
        public string FeatDesc;
        public string Icon;
        protected override void OnLoad()
        {
            DataApi.SetData(Id,this);
        }
    }
}