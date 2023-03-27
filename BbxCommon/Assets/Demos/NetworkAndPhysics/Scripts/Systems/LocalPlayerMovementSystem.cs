using Unity.Entities;
using BbxCommon.Framework;

namespace Nnp
{
    [DisableAutoCreation]
    public partial class LocalPlayerMovementSystem : EcsMixSystemBase
    {
        protected override void OnUpdate()
        {
            ForeachRawAspect<LocalPlayerMovementRawAspect>(
                (LocalPlayerMovementRawAspect aspect) =>
                {
                    aspect.CharacterController.SimpleMove(aspect.DesiredDirection * aspect.WalkSpeed);
                    if (aspect.DesiredDirection.magnitude > 1e-4)
                    {
                        aspect.CurrentState = PlayerRawComponent.EPlayerState.Walk;
                        aspect.Forward = aspect.DesiredDirection;
                    }
                    else
                        aspect.CurrentState = PlayerRawComponent.EPlayerState.Idle;
                });
        }
    }
}
