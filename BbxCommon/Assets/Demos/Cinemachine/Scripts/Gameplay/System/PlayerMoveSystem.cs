using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;

namespace Cin
{
    [DisableAutoCreation]
    public partial class PlayerMoveSystem : EcsMixSystemBase
    {
        protected override void OnUpdate()
        {
            // hasn't been baken
            var mainCameraComp = EcsApi.GetSingletonRawComponent<MainCameraSingletonRawComponent>();
            if (mainCameraComp == null)
                return;

            // get forward
            var camera = mainCameraComp.GetEntity().GetGameObject();
            var forward = camera.transform.forward.SetY(0).normalized;
            if (forward == Vector3.zero)
                forward = new Vector3(0, 0, 1);
            var right = Quaternion.AngleAxis(90, Vector3.up) * forward;

            // calculate direction
            Vector3 dir = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
                dir += forward;
            if (Input.GetKey(KeyCode.S))
                dir -= forward;
            if (Input.GetKey(KeyCode.A))
                dir -= right;
            if (Input.GetKey(KeyCode.D))
                dir += right;

            // move
            var playerComp = EcsApi.GetSingletonRawComponent<PlayerSingletonRawComponent>();
            var player = playerComp.GetEntity().GetGameObject();
            player.GetComponent<CharacterController>().Move((dir * playerComp.Speed).AddY(-10) * UnityEngine.Time.deltaTime);
        }
    }
}
