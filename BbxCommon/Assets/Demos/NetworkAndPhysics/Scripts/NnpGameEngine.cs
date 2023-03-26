using BbxCommon.Framework;

namespace Nnp
{
    public class NnpGameEngine : GameEngineBase<NnpGameEngine>
    {
        protected override void InitSingletonComponents()
        {
            EcsWrapper.AddSingletonRawComponent<InputSingletonRawComponent>();
        }

        protected override void SetGlobalLoadItems()
        {
            
        }

        protected override void SetGlobalTickItems()
        {
            TickWrapper.AddGlobalUpdateSystem<InputSystem>();
            TickWrapper.AddGlobalUpdateSystem<LocalPlayerMovementSystem>();
            TickWrapper.AddGlobalUpdateSystem<PlayerAnimationSystem>();
            TickWrapper.AddGlobalUpdateSystem<CameraSystem>();
        }
    }
}
