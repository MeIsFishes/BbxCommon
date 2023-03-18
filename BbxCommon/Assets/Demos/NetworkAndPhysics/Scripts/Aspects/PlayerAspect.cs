using Unity.Entities;
using Unity.Mathematics;

namespace Nnp
{
    readonly public partial struct PlayerAspect : IAspect
    {
        readonly RefRW<VelocityComponent> m_VelocityComp;
        readonly RefRO<PosAndRotComponent> m_PosAndRotComp;

        public float3 Velocity { get { return m_VelocityComp.ValueRO.Velocity; } set { m_VelocityComp.ValueRW.Velocity = value; } }
        public float3 Position => m_PosAndRotComp.ValueRO.Position;
        public float4 Rotation => m_PosAndRotComp.ValueRO.Rotation;
        public void SetVelocity(float3 velocity) { Velocity = velocity; }
    }
}
