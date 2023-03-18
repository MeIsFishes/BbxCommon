using Unity.Entities;
using Unity.Mathematics;
using BbxCommon;
using BbxCommon.Framework;

namespace Nnp
{
    public partial class PlayerMovementSystem : EcsHpSystemBase
    {
        protected override void OnUpdate()
        {
            foreach (var player in SystemAPI.Query<PlayerAspect>().WithAll<PlayerComponent>())
            {
                player.SetVelocity(player.Velocity.AddY(-10 * BbxRawTimer.DeltaTime));
            }
        }
    }
}
