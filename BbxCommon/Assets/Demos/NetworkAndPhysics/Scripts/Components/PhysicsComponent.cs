using Unity.Entities;
using Unity.Mathematics;

namespace Nnp
{
    public struct PosAndRotComponent : IComponentData
    {
        public float3 Position;
        public float4 Rotation;
    }

    public struct VelocityComponent : IComponentData
    {
        public float3 Velocity;
    }
}
