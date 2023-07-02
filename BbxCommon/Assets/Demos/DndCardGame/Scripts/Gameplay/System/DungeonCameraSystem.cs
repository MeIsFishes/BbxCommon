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
            var gameObject = mainCameraComp.GetEntity().GetGameObject();
            if (gameObject == null)
                return;
            var cameraData = DataApi.GetData<CameraData>();
            if (cameraData == null)
                return;
            var playerComp = EcsApi.GetSingletonRawComponent<PlayerSingletonRawComponent>();
            if (playerComp.Characters.Count == 0)
                return;
            var characterGo = playerComp.Characters[0].GetGameObject();
            if (characterGo == null)
                return;

            gameObject.transform.position = characterGo.transform.position + cameraData.DungeonOffset;
            gameObject.transform.LookAt(characterGo.transform.position);
        }
    }
}
