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
                aspect.Transform.LookAt(aspect.Transform.position + (aspect.Destination - aspect.Transform.position).SetY(0));
                // 向目标移动
                var deltaMove = aspect.WalkSpeed * UnityEngine.Time.deltaTime;
                if (Mathf.Abs(aspect.Destination.x - aspect.Transform.position.x) < deltaMove &&
                    Mathf.Abs(aspect.Destination.z - aspect.Transform.position.z) < deltaMove &&
                    Mathf.Abs(aspect.Destination.y - aspect.Transform.position.y) < 0.5f)
                {
                    aspect.CharacterController.Move(aspect.Destination - aspect.Transform.position);
                    aspect.Animator.Play("Idle");
                    aspect.Finished = true;
                }
                else
                {
                    aspect.CharacterController.Move((aspect.Destination - aspect.Transform.position).SetY(0).normalized * deltaMove);
                    aspect.CharacterController.Move(new Vector3(0, -10, 0) * deltaMove);    // 贴地
                    aspect.Animator.Play("Walk");
                }
            }
        }
    }
}
