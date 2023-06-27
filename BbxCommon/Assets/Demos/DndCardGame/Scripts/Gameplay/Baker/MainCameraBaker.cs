using UnityEngine;
using BbxCommon;

namespace Dcg
{
    public class MainCameraBaker : EcsBaker
    {
        protected override void Bake()
        {
            AddRawComponent<MainCameraSingletonRawComponent>();


        }
    }
}
