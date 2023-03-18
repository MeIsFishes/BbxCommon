using UnityEngine;
using Unity.Entities;
using BbxCommon;

namespace Nnp
{
    public class CameraAuthoring : MonoBehaviour
    {
        
    }

    public class CameraBaker : Baker<CameraAuthoring>
    {
        public override void Bake(CameraAuthoring authoring)
        {
            AddComponent(new PosAndRotComponent
            {
                Position = authoring.transform.position,
                Rotation = authoring.transform.rotation.AsFloat4(),
            });
            AddComponent<CameraComponent>();
        }
    }
}
