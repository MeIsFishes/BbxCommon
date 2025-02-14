﻿using Unity.Entities;
using BbxCommon;

namespace Dcg
{
    [DisableAutoCreation]
    public partial class CauseDamageSystem : EcsMixSystemBase
    {
        protected override void OnSystemUpdate()
        {
            foreach (var attackableComp in GetEnumerator<AttackableRawComponent>())
            {
                // 将每个damageRequest写到目标Entity的AttackableRawComponent.TakeDamageRequests下
                foreach(var damageRequest in attackableComp.CauseDamageRequests)
                {
                    // 处理增益逻辑
                    attackableComp.OnCauseDamage?.Invoke(damageRequest);
                    damageRequest.Target.GetRawComponent<AttackableRawComponent>().AddTakeDamageRequest(damageRequest);
                }
                // 清空CauseDamageRequests
                attackableComp.CauseDamageRequests.Clear();
            }
        }
    }
}