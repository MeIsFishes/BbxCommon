using UnityEngine;
using Unity.Entities;
using BbxCommon;

namespace Nnp
{
    public class PlayerAuthoring : MonoBehaviour
    {
        
    }

    public class PlayerBaker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            AddComponent(new PosAndRotComponent
            {
                Position = authoring.transform.position,
                Rotation = authoring.transform.rotation.AsFloat4(),
            });
            AddComponent(new VelocityComponent
            {
                Velocity = authoring.GetComponent<CharacterController>().velocity,
            });
            AddComponent<PlayerComponent>();
        }
    }
}
