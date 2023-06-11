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
            ForeachRawAspect((WalkToRawAspect aspect) =>
            {
                if (aspect.Finished)
                    return;

                // 面朝目标
                aspect.Transform.LookAt(aspect.Transform.position + (aspect.Destination - aspect.Transform.position).SetY(0));
                // 向目标移动
                var distance = (aspect.Destination - aspect.Transform.position).magnitude;
                var walkLength = aspect.WalkSpeed * UnityEngine.Time.deltaTime;
                if (distance < walkLength)
                {
                    aspect.Transform.position = aspect.Destination;
                    aspect.Animator.Play("Idle");
                    aspect.Finished = true;
                }
                else
                {
                    aspect.CharacterController.Move((aspect.Destination - aspect.Transform.position).normalized * walkLength);
                    aspect.Animator.Play("Walk");
                }
            });
        }
    }
}
