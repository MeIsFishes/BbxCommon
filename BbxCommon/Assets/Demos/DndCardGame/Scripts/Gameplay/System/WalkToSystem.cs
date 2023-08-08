using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;

namespace Dcg
{
    [DisableAutoCreation]
    public partial class WalkToSystem : EcsMixSystemBase
    {
        protected override void OnUpdate()
        {
            foreach (var aspect in GetEnumerator<WalkToRawAspect>())
            {
                if (aspect.Finished)
                    continue;

                // 面朝目标
                aspect.Rotation = Quaternion.LookRotation(aspect.Destination.SetY(aspect.Position.y) - aspect.Position);
                // 向目标移动
                var deltaMove = aspect.WalkSpeed * UnityEngine.Time.deltaTime;
                if (Mathf.Abs(aspect.Destination.x - aspect.Position.x) < deltaMove &&
                    Mathf.Abs(aspect.Destination.z - aspect.Position.z) < deltaMove &&
                    Mathf.Abs(aspect.Destination.y - aspect.Position.y) < 0.5f)
                {
                    aspect.CharacterController.Move(aspect.Destination - aspect.Position);
                    aspect.Animator.Play("Idle");
                    aspect.Finished = true;
                }
                else
                {
                    aspect.CharacterController.Move((aspect.Destination - aspect.Position).SetY(0).normalized * deltaMove);
                    aspect.CharacterController.Move(new Vector3(0, -10, 0) * deltaMove);    // 贴地
                    aspect.Animator.Play("Walk");
                }
            }
        }
    }
}
