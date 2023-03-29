using Unity.Entities;
using BbxCommon.Framework;

namespace Nnp
{
    [DisableAutoCreation, UpdateAfter(typeof(LocalPlayerMovementSystem))]
    public partial class PlayerAnimationSystem : EcsMixSystemBase
    {
        protected override void OnUpdate()
        {
            ForeachRawAspect(
                (PlayerAnimationRawAspect aspect) =>
                {
                    switch (aspect.CurrentState)
                    {
                        case PlayerRawComponent.EPlayerState.Idle:
                            aspect.Animator.Play(aspect.IdleAnimation);
                            break;
                        case PlayerRawComponent.EPlayerState.Walk:
                            aspect.Animator.Play(aspect.WalkAnimation);
                            break;
                        case PlayerRawComponent.EPlayerState.Run:
                            aspect.Animator.Play(aspect.RunAnimation);
                            break;
                        case PlayerRawComponent.EPlayerState.BeHit:
                            aspect.Animator.Play(aspect.BeHitAnimation);
                            break;
                    }
                });
        }
    }
}
