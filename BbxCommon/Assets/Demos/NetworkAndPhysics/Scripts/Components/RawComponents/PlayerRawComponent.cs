using BbxCommon;

namespace Nnp
{
    public class PlayerRawComponent : EcsRawComponent
    {
        public enum EPlayerState
        {
            Idle,
            Walk,
            Run,
            BeHit,
        }

        public EPlayerState CurrentState;
    }
}
