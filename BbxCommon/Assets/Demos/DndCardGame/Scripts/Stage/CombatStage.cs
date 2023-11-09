using UnityEngine;
using BbxCommon;
using BbxCommon.Ui;
using Dcg.Ui;

namespace Dcg
{
    public class CombatStage
    {
        public static GameStage CreateStage()
        {
            var stage = DcgGameEngine.Instance.StageWrapper.CreateStage("Combat Stage");

            stage.SetUiScene(DcgGameEngine.Instance.UiScene, Resources.Load<UiSceneAsset>("DndCardGame/Config/UiScene/UiCombatScene"));

            stage.AddLoadItem(new CombatStageInitPlayerData());
            stage.AddLoadItem(new CombatStageInitCombatInfo());

            stage.AddLateLoadItem(new CombatStageBindUi());

            stage.AddUpdateSystem<CauseDamageSystem>();
            stage.AddUpdateSystem<TakeDamageSystem>();
            stage.AddUpdateSystem<CombatRoundSystem>();
            stage.AddUpdateSystem<CombatTurnSystem>();
            stage.AddUpdateSystem<MonsterTurnSystem>();

            return stage;
        }

        private class CombatStageInitPlayerData : IStageLoad
        {
            void IStageLoad.Load(GameStage stage)
            {
                var playerComp = EcsApi.GetSingletonRawComponent<PlayerSingletonRawComponent>();
                var charcterDeckComp = playerComp.Characters[0].GetRawComponent<CharacterDeckRawComponent>();
                var combatDeckComp = playerComp.Characters[0].AddRawComponent<CombatDeckRawComponent>();
                combatDeckComp.DicesInDeck.Clear();
                combatDeckComp.DicesInDeck.AddList(charcterDeckComp.Dices);
                combatDeckComp.DicesInDeck.Shuffle();
                combatDeckComp.DrawDice(5);    // 初始抽牌（临时）
            }

            void IStageLoad.Unload(GameStage stage)
            {
                var playerEntity = EcsApi.GetSingletonRawComponent<PlayerSingletonRawComponent>().GetEntity();
                playerEntity.RemoveRawComponent<CombatDeckRawComponent>();
            }
        }

        private class CombatStageInitCombatInfo : IStageLoad
        {
            void IStageLoad.Load(GameStage stage)
            {
                // 创建怪物
                var dungeonRoomComp = EcsApi.GetSingletonRawComponent<DungeonRoomSingletonRawComponent>();
                var roomData = DataApi.GetData<RoomData>();
                var roomPos = dungeonRoomComp.CurRoom.GetGameObject().transform.position;
                var spawnPos = roomPos + roomData.MonsterOffset;
                var monster = EntityCreator.CreateMonsterEntity(DataApi.GetData<MonsterData>(10010001), spawnPos, Quaternion.LookRotation((roomPos - spawnPos).SetY(0)));

                // 初始化战场信息
                var combatInfoComp = EcsApi.AddSingletonRawComponent<CombatInfoSingletonRawComponent>();
                var playerComp = EcsApi.GetSingletonRawComponent<PlayerSingletonRawComponent>();
                combatInfoComp.Character = playerComp.Characters[0];
                combatInfoComp.Monster = monster;
            }

            void IStageLoad.Unload(GameStage stage)
            {
                
            }
        }

        /// <summary>
        /// 为CombatStage的UI绑定信息
        /// </summary>
        private class CombatStageBindUi : IStageLoad
        {
            void IStageLoad.Load(GameStage stage)
            {
                var uiController = UiApi.GetUiController<UiDicesInHandController>();
                uiController.Bind(EcsApi.GetSingletonRawComponent<PlayerSingletonRawComponent>().Characters[0]);
            }

            void IStageLoad.Unload(GameStage stage) { }
        }
    }
}
