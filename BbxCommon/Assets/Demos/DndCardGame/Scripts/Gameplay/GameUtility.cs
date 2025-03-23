using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;
using Dcg.Ui;
using System.Text;

namespace Dcg
{
    public static class GameUtility
    {
        #region Room
        public static class Room
        {
            public static void SpawnRoomStart(Entity entity, Vector3 originalPos)
            {
                var spawnRoomShowComp = entity.GetRawComponent<SpawnRoomShowRawComponent>();
                spawnRoomShowComp.IsSpawning = true;
                spawnRoomShowComp.ElapsedTime = 0;
                spawnRoomShowComp.OriginalPos = originalPos;
                entity.ActivateRawAspect<SpawnRoomShowRawAspect>();
            }

            public static void SpawnRoomEnd(Entity entity)
            {
                entity.GetRawComponent<SpawnRoomShowRawComponent>().IsSpawning = false;
                entity.DeactiveRawComponent<SpawnRoomShowRawComponent>();
                entity.DeactiveRawAspect<SpawnRoomShowRawAspect>();
            }
        }
        #endregion

        #region Combat
        public static class CombatTurn
        {
            public static void UpdateUiWhenTurnPass(ECombatTurn combatTurn, Entity entity)
            {
                switch (combatTurn)
                {
                    case ECombatTurn.PlayerTurn:
                        // ��ʾ/����UI
                        UiApi.GetUiController<UiAttackController>().Show();
                        UiApi.GetUiController<UiEndTurnController>().Show();
                        UiApi.GetUiController<UiPlayerTurnController>().Show();
                        UiApi.GetUiController<UiEnemyTurnController>().Hide();
                        // ��UI
                        UiApi.GetUiController<UiDicesInHandController>().Bind(entity);
                        UiApi.GetUiController<UiEndTurnController>().Bind(entity);
                        break;
                    case ECombatTurn.EnemyTurn:
                        // ��ʾ/����UI
                        UiApi.GetUiController<UiAttackController>().Hide();
                        UiApi.GetUiController<UiEndTurnController>().Hide();
                        UiApi.GetUiController<UiPlayerTurnController>().Hide();
                        UiApi.GetUiController<UiEnemyTurnController>().Show();
                        UiApi.GetUiController<UiWildDiceListController>().Hide();
                        break;
                }
            }
        }
        #endregion

        #region Dice
        public static class DiceFunctions
        {
            /// <summary>
            /// ��һ����������ת��������8d6+2d4���ַ���
            /// </summary>
            public static string ConvertDicesToString(List<Dice> dices)
            {
                List<EDiceType> diceTypes = SimplePool.Alloc(out diceTypes);
                var res = ConvertDicesToString(diceTypes);
                diceTypes.CollectToPool();
                return res;
            }

            /// <summary>
            /// ��һ����������ת��������8d6+2d4���ַ���
            /// </summary>
            public static string ConvertDicesToString(List<EDiceType> dices)
            {
                StringBuilder sb = SimplePool.Alloc(out sb);
                List<int> diceCount = SimplePool.Alloc(out diceCount);
                diceCount.ModifyCount((int)EDiceType.D20 + 1);
                for (int i = 0; i < dices.Count; i++)
                {
                    diceCount[(int)dices[i]]++;
                }

                bool hasDice = false;
                if (diceCount[(int)EDiceType.D20] > 0)
                {
                    sb.Append(diceCount[(int)EDiceType.D20]);
                    sb.Append("d20");
                    hasDice = true;
                }
                if (diceCount[(int)EDiceType.D12] > 0)
                {
                    if (hasDice)
                        sb.Append("+");
                    sb.Append(diceCount[(int)EDiceType.D12]);
                    sb.Append("d12");
                    hasDice = true;
                }
                if (diceCount[(int)EDiceType.D10] > 0)
                {
                    if (hasDice)
                        sb.Append("+");
                    sb.Append(diceCount[(int)EDiceType.D10]);
                    sb.Append("d10");
                    hasDice = true;
                }
                if (diceCount[(int)EDiceType.D8] > 0)
                {
                    if (hasDice)
                        sb.Append("+");
                    sb.Append(diceCount[(int)EDiceType.D8]);
                    sb.Append("d8");
                    hasDice = true;
                }
                if (diceCount[(int)EDiceType.D6] > 0)
                {
                    if (hasDice)
                        sb.Append("+");
                    sb.Append(diceCount[(int)EDiceType.D6]);
                    sb.Append("d6");
                    hasDice = true;
                }
                if (diceCount[(int)EDiceType.D4] > 0)
                {
                    if (hasDice)
                        sb.Append("+");
                    sb.Append(diceCount[(int)EDiceType.D4]);
                    sb.Append("d4");
                }

                var res = sb.ToString();
                sb.CollectToPool();
                diceCount.CollectToPool();
                return res;
            }
        }
        #endregion

        #region RandomPool
        public static class RandomPool
        {
            #region MonsterPool
            private static RandomPool<MonsterData> m_MonsterDataPool;

            public static void BuildMonsterDataPool()
            {
                ObjectPool.Alloc(out m_MonsterDataPool);
                foreach (var data in DataApi.GetEnumerator<MonsterData>())
                {
                    m_MonsterDataPool.SetWeight(data, 1);
                }
            }

            public static MonsterData GetRandomMonster()
            {
                return m_MonsterDataPool.Rand();
            }
            #endregion

            #region DicePool
            private static RandomPool<EDiceType> m_RewardDiceTypePool;

            public static void BuildDicePool()
            {
                ObjectPool.Alloc(out m_RewardDiceTypePool);
                var diceData = DataApi.GetData<DiceData>();
                m_RewardDiceTypePool.SetWeight(EDiceType.D4, diceData.RewardD4Weight);
                m_RewardDiceTypePool.SetWeight(EDiceType.D6, diceData.RewardD6Weight);
                m_RewardDiceTypePool.SetWeight(EDiceType.D8, diceData.RewardD8Weight);
                m_RewardDiceTypePool.SetWeight(EDiceType.D10, diceData.RewardD10Weight);
                m_RewardDiceTypePool.SetWeight(EDiceType.D12, diceData.RewardD12Weight);
                m_RewardDiceTypePool.SetWeight(EDiceType.D20, diceData.RewardD20Weight);
            }

            public static EDiceType GetRandomRewardDiceType()
            {
                return m_RewardDiceTypePool.Rand();
            }
            #endregion
        }
        #endregion

        #region Hitfly

        private static HitFlyData m_HitFlyData;

        public static Vector3 GetHitFlyForce(Vector3 attackSourcePos, Vector3 targetPos, int finalDamage, int targetMaxHealth)
        {
            if (m_HitFlyData == null)
            {
                m_HitFlyData = DataApi.GetData<HitFlyData>();
            }

            //��������
            float force = (finalDamage / (float)targetMaxHealth) * m_HitFlyData.Force;

            //���ɷ���
            Vector3 direction = (targetPos - attackSourcePos).normalized;

            //����Ƕ�
            float angleRandomness = m_HitFlyData.RandomAngle;

            direction = Quaternion.Euler(
                Random.Range(-angleRandomness, angleRandomness), 
                Random.Range(-angleRandomness, angleRandomness), 
                Random.Range(-angleRandomness, angleRandomness)) * direction;

            return direction * force;
        }

        #endregion
    }
}
