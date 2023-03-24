using UnityEngine;
using BbxCommon.Framework;

namespace Nnp
{
    public class LocalPlayerBaker : EcsBaker
    {
        protected override void Bake()
        {
            AddRawComponent<PlayerRawComponent>();
            AddRawComponent<LocalPlayerSingletonRawComponent>();
        }
    }
}
