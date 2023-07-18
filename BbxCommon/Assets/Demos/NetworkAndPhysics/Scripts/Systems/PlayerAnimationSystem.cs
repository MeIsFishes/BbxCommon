using Unity.Entities;
using BbxCommon;

namespace Nnp
{
    [DisableAutoCreation, UpdateAfter(typeof(LocalPlayerMovementSystem))]
    public partial class PlayerAnimationSystem : EcsMixSystemBase
    {
        protected override void OnUpdate()
        {
            foreach (var aspect in GetEnumerator<PlayerAnimationRawAspect>())
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
            }
        }
    }
}
