using UnityEngine;
using BbxCommon;

namespace Dcg
{
    [CreateAssetMenu(fileName = "ModelAttributesData", menuName = "Dcg/ModelAttributesData")]
    public class ModelAttributesData : BbxScriptableObject
    {
        public float WalkSpeed;

        protected override void OnLoad()
        {
            DataApi.SetData(this);
        }
    }
}
