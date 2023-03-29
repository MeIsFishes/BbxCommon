using BbxCommon.Framework;

namespace Nnp
{
    public class NnpGameEngine : GameEngineBase<NnpGameEngine>
    {
        protected override string GetGameMainScene()
        {
            return "NnpMain";
        }

        protected override void InitSingletonComponents()
        {
            EcsWrapper.AddSingletonRawComponent<InputSingletonRawComponent>();
        }

        protected override void SetGlobalLoadItems()
        {
            
        }

        protected override void SetGlobalTickItems()
        {
            GlobalStageWrapper.AddGlobalUpdateSystem<InputSystem>();
            GlobalStageWrapper.AddGlobalUpdateSystem<LocalPlayerMovementSystem>();
            GlobalStageWrapper.AddGlobalUpdateSystem<PlayerAnimationSystem>();
            GlobalStageWrapper.AddGlobalUpdateSystem<CameraSystem>();
        }
    }
}
