using BbxCommon.Framework;

namespace Nnp
{
    public class NnpGameEngine : GameEngineBase<NnpGameEngine>
    {
        protected override void SetGlobalLoadItems()
        {
            
        }

        protected override void SetGlobalTickItems()
        {
            TickWrapper.AddGlobalUpdateItem<PlayerMovementSystem>();
        }
    }
}
