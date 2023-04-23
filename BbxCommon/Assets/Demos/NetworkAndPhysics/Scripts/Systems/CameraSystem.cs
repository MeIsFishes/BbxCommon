using UnityEngine;
using Unity.Entities;
using BbxCommon;

namespace Nnp
{
    [DisableAutoCreation]
    public partial class CameraSystem : EcsMixSystemBase
    {
        protected override void OnUpdate()
        {
            var localPlayerComp = GetSingletonRawComponent<LocalPlayerSingletonRawComponent>();
            if (localPlayerComp == null)
                return;
            var localPlayerTranform = localPlayerComp.GetEntity().GetGameObject().transform;
            ForeachRawComponent(
                (CameraRawComponent cameraComp) =>
                {
                    var transform = cameraComp.GetEntity().GetGameObject().transform;
                    transform.position = new Vector3(0, 10, -8) + localPlayerTranform.position;
                    transform.LookAt(localPlayerTranform.position);
                });
        }
    }
}
