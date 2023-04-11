using BbxCommon.Framework;

namespace Nnp
{
    public class NnpGameEngine : GameEngineBase<NnpGameEngine>
    {
        protected override void InitSingletonComponents()
        {
            EcsWrapper.AddSingletonRawComponent<InputSingletonRawComponent>();
        }

        protected override void OnAwake()
        {
            var globalStage = StageWrapper.CreateStage("Global Stage");

            globalStage.AddScene("NnpMain");

            globalStage.AddUpdateSystem<InputSystem>();
            globalStage.AddUpdateSystem<LocalPlayerMovementSystem>();
            globalStage.AddUpdateSystem<PlayerAnimationSystem>();
            globalStage.AddUpdateSystem<CameraSystem>();

            StageWrapper.LoadStage(globalStage);
        }
    }
}
