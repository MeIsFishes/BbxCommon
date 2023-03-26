using UnityEngine;
using BbxCommon.Framework;

namespace Nnp
{
    public class LocalPlayerBaker : EcsBaker
    {
        public PlayerSettingAsset PlayerSettingAsset;

        protected override void Bake()
        {
            AddRawComponent<LocalPlayerSingletonRawComponent>();

            var playerComp = AddRawComponent<PlayerRawComponent>();
            playerComp.CurrentState = PlayerRawComponent.EPlayerState.Idle;

            var attributesComp = AddRawComponent<AttributesRawComponent>();
            attributesComp.MaxHp = PlayerSettingAsset.MaxHp;
            attributesComp.Attack = PlayerSettingAsset.Attack;
            attributesComp.WalkSpeed = PlayerSettingAsset.WalkSpeed;
            attributesComp.RunSpeed = PlayerSettingAsset.RunSpeed;

            var animationComp = AddRawComponent<UnitAnimationRawComponent>();
            animationComp.IdleAnimation = PlayerSettingAsset.IdleAnimation;
            animationComp.WalkAnimation = PlayerSettingAsset.WalkAnimation;
            animationComp.RunAnimation = PlayerSettingAsset.RunAnimation;
            animationComp.BeHitAnimation = PlayerSettingAsset.BeHitAnimation;

            CreateRawAspect<LocalPlayerMovementRawAspect>();
            CreateRawAspect<PlayerAnimationRawAspect>();
        }
    }
}
