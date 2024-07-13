using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;

namespace Dcg
{
    [DisableAutoCreation]
    public partial class DungeonCameraSystem : EcsMixSystemBase
    {
        protected override void OnUpdate()
        {
            var mainCameraComp = EcsApi.GetSingletonRawComponent<MainCameraSingletonRawComponent>();
            if (mainCameraComp == null)
                return;
            var cameraData = DataApi.GetData<CameraData>();
            if (cameraData == null)
                return;
            var playerComp = EcsApi.GetSingletonRawComponent<LocalPlayerSingletonRawComponent>();
            if (playerComp.DungeonEntities.Count == 0)
                return;
            var character = playerComp.DungeonEntities[0];
            if (character == null)
                return;

            var transform = mainCameraComp.GetEntity().GetGameObject().transform;
            var characterTransform = character.GetGameObject().transform;
            transform.position = characterTransform.position + cameraData.DungeonOffset;
            transform.rotation = Quaternion.LookRotation(characterTransform.position - transform.position);
        }
    }
}
