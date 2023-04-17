using BbxCommon.Framework;

namespace Oxd
{
    public class OxdGameEngine : GameEngineBase<OxdGameEngine>
    {
        protected override void InitSingletonComponents()
        {
            
        }

        protected override void OnAwake()
        {
            var stage = StageWrapper.CreateStage("Main Stage");

            stage.AddUpdateSystem<GetComponentTSyetem>();
            stage.AddUpdateSystem<EmptySystem>();
            stage.AddUpdateSystem<GetComponentSystem>();
            stage.AddUpdateSystem<GetAndCastSystem>();

            StageWrapper.LoadStage(stage);
        }
    }
}
