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
                        // 显示/隐藏UI
                        UiApi.GetUiController<UiAttackController>().Show();
                        UiApi.GetUiController<UiEndTurnController>().Show();
                        UiApi.GetUiController<UiPlayerTurnController>().Show();
                        UiApi.GetUiController<UiEnemyTurnController>().Hide();
                        // 绑定UI
                        UiApi.GetUiController<UiDicesInHandController>().Bind(entity);
                        UiApi.GetUiController<UiEndTurnController>().Bind(entity);
                        break;
                    case ECombatTurn.EnemyTurn:
                        // 显示/隐藏UI
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
            /// 把一串骰子数据转化成形如8d6+2d4的字符串
            /// </summary>
            public static string ConvertDicesToString(List<Dice> dices)
            {
                List<EDiceType> diceTypes = SimplePool.Alloc(out diceTypes);
                var res = ConvertDicesToString(diceTypes);
                diceTypes.CollectToPool();
                return res;
            }

            /// <summary>
            /// 把一串骰子数据转化成形如8d6+2d4的字符串
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
        }
        #endregion
    }
}
