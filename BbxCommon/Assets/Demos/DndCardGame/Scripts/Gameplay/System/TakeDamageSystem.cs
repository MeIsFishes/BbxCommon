using UnityEngine;
using Unity.Entities;
using BbxCommon;

namespace Dcg
{
    [DisableAutoCreation, UpdateAfter(typeof(CauseDamageSystem))]
    public partial class TakeDamageSystem : EcsMixSystemBase
    {
        protected override void OnSystemUpdate()
        {
            foreach (var attackableComp in GetEnumerator<AttackableRawComponent>())
            {
                // 没有伤害请求，返回
                if (attackableComp.TakeDamageRequests.Count < 1)
                {
                    continue;
                }

                var attributesRawComponent = attackableComp.GetEntity().GetRawComponent<AttributesRawComponent>();

                // 取出伤害请求
                var damageRequest = attackableComp.TakeDamageRequests[0];
                attackableComp.TakeDamageRequests.RemoveAt(0);
                // 处理造成伤害时的增益
                attackableComp.OnTakeDamage?.Invoke(damageRequest);

                //处理护甲和类型tag的伤害修正
                ProcessDamageByArmorTag(damageRequest);

                // 伤害和治疗公用逻辑
                attributesRawComponent.CurHp -= damageRequest.Damage;
                if (attributesRawComponent.CurHp > attributesRawComponent.MaxHp)
                {
                    attributesRawComponent.CurHp = attributesRawComponent.MaxHp;
                }
                if (attributesRawComponent.CurHp <= 0)
                {
                    attributesRawComponent.CurHp = 0;
                 
                    //最后一击击飞
                    DoHitFly(damageRequest);
                }
                attackableComp.DispatchEvent(AttackableRawComponent.EEvent.DamageRequestProcessed, damageRequest);

                // 回收
                damageRequest.CollectToPool();
            }
        }

        private void ProcessDamageByArmorTag(DamageRequest damageRequest)
        {
            //根据伤害类型DamageType与受伤者标签Tag类型，修正伤害
            var rawComponent = damageRequest.Target.GetRawComponent<AttributesRawComponent>();
            float damageCoefficient = 1f;
            if (rawComponent != null && rawComponent.UnitTags != null)
            {
                foreach (var tag in rawComponent.UnitTags)
                {
                    var coefficient = DataApi.GetData<UnitTagCsvData>(tag);
                    if (coefficient != null)
                    {
                        damageCoefficient *= coefficient.GetCoefficientByDamageType(damageRequest.DamageType);
                    }
                }
            }
            damageRequest.Damage = (int)(damageRequest.Damage * damageCoefficient);
        }

        private void DoHitFly(DamageRequest damageRequest)
        {
            var force = GameUtility.GetHitFlyForce(damageRequest.DamagePos,
                      damageRequest.Target.GetGameObject().transform.position,
                      damageRequest.Damage,
                      damageRequest.Target.GetRawComponent<AttributesRawComponent>().MaxHp);
            Debug.Log("击飞direction：" + force);

            var gameObject = damageRequest.Target.GetGameObject();
            if (!gameObject.TryGetComponent<Rigidbody>(out var rigidbody))
            {
                rigidbody = gameObject.AddComponent<Rigidbody>();
            }

            rigidbody.AddForce(force, ForceMode.Impulse);
        }
    }
}