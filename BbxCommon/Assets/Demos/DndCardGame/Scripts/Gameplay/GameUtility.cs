using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;
using Dcg.Ui;

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
            public static void ShowTurnUi(ECombatTurn combatTurn)
            {
                switch (combatTurn)
                {
                    case ECombatTurn.PlayerTurn:
                        UiApi.GetUiController<UiAttackController>().Show();
                        UiApi.GetUiController<UiPlayerTurnController>().Show();
                        break;
                    case ECombatTurn.EnemyTurn:
                        UiApi.GetUiController<UiAttackController>().Hide();
                        UiApi.GetUiController<UiPlayerTurnController>().Hide();
                        break;
                }
            }
        }
        #endregion
    }
}
