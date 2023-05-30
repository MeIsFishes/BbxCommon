using Unity.Entities;
using BbxCommon;

namespace Dcg
{
    [DisableAutoCreation]
    public class CombatRoundSystem : EcsMixSystemBase
    {
        protected override void OnUpdate()
        {
            EcsApi.GetSingletonRawComponent<CombatRoundSingletonRawComponent>();
        }
    }
}
