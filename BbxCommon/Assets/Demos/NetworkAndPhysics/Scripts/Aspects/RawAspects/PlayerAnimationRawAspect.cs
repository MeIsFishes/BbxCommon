using UnityEngine;
using BbxCommon.Framework;

namespace Nnp
{
    public class PlayerAnimationRawAspect : EcsRawAspect
    {
        public PlayerRawComponent.EPlayerState CurrentState => m_PlayerComp.CurrentState;
        public Animator Animator;
        public string IdleAnimation => m_AnimationComp.IdleAnimation;
        public string WalkAnimation => m_AnimationComp.WalkAnimation;
        public string RunAnimation => m_AnimationComp.RunAnimation;
        public string BeHitAnimation => m_AnimationComp.BeHitAnimation;

        private PlayerRawComponent m_PlayerComp;
        private UnitAnimationRawComponent m_AnimationComp;

        protected override void CreateAspect()
        {
            m_PlayerComp = GetRawComponent<PlayerRawComponent>();
            Animator = GetGameObjectComponent<Animator>();
            m_AnimationComp = GetRawComponent<UnitAnimationRawComponent>();
        }
    }
}
