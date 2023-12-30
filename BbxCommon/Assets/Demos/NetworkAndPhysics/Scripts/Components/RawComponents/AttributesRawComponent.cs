using BbxCommon;

namespace Nnp
{
    public class AttributesRawComponent : EcsRawComponent
    {
        public int MaxHp;
        public int Attack;
        public float WalkSpeed;
        public float RunSpeed;

        public override void OnCollect()
        {
            MaxHp = 0;
            Attack = 0;
            WalkSpeed = 0;
            RunSpeed = 0;
        }
    }
}
