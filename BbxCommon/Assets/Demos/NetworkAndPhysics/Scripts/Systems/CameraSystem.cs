using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using BbxCommon.Framework;

namespace Nnp
{
    [DisableAutoCreation]
    public partial class CameraSystem : EcsMixSystemBase
    {
        protected override void OnUpdate()
        {
            var localPlayerComp = GetSingletonRawComponent<LocalPlayerSingletonRawComponent>();
            var localPlayerTranform = localPlayerComp.Entity.GetRawComponent<GameObjectRawComponent>().GameObject.transform;
            ForeachRawComponent<CameraRawComponent>(
                (CameraRawComponent cameraComp) =>
                {
                    var transform = cameraComp.Entity.GetRawComponent<GameObjectRawComponent>().GameObject.transform;
                    transform.position = new Vector3(0, 10, -8) + localPlayerTranform.position;
                    transform.LookAt(localPlayerTranform.position);
                });
        }
    }
}
