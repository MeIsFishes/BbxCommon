using UnityEngine;
using Unity.Entities;
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
            stage.AddUpdateSystem<CombatMonsterTurnSystem>();
            stage.AddUpdateSystem<CombatBeginTurnSystem>();
            stage.AddUpdateSystem<CombatEndTurnSystem>();
            stage.AddUpdateSystem<CombatWinSystem>();
            stage.AddUpdateSystem<CombatDefeatedSystem>();

            return stage;
        }

        private class CombatStageInitPlayerData : IStageLoad
        {
            private Entity combatEntity;
            // 目前这种临时加上component的做法暂时是可以的，不过考虑了一下未来可能出现召唤物等情况，
            // 所以最好的做法应该是把战斗中的entity和地图中的entity分为两个entity写在EntityCreator里面
            void IStageLoad.Load(GameStage stage)
            {
                var playerComp = EcsApi.GetSingletonRawComponent<LocalPlayerSingletonRawComponent>();
                // var charcterDeckComp = playerComp.Characters[0].GetRawComponent<CharacterDeckRawComponent>();
                // var combatDeckComp = playerComp.Characters[0].AddRawComponent<CombatDeckRawComponent>();
                // playerComp.Characters[0].AddRawComponent<CombatTurnRawComponent>();
                // combatDeckComp.DicesInDeck.Clear();
                // combatDeckComp.DicesInDeck.AddList(charcterDeckComp.Dices);
                // combatDeckComp.DicesInDeck.Shuffle();
                // playerComp.Characters[0].BindHud<HudCharacterStatusController>();
                combatEntity=EntityCreator.CreateCombatEntity();
                ConvertCharacterToCombatEntity(playerComp.Characters[0],combatEntity);
                var combatInfoComp = EcsApi.AddSingletonRawComponent<CombatInfoSingletonRawComponent>();
                combatInfoComp.Character = combatEntity;
            }

            void IStageLoad.Unload(GameStage stage)
            {
                var playerComp = EcsApi.GetSingletonRawComponent<LocalPlayerSingletonRawComponent>();
                // var characterEntity = playerComp.Characters[0];
                // characterEntity.RemoveRawComponent<CombatDeckRawComponent>();
                // characterEntity.RemoveRawComponent<CombatTurnRawComponent>();
                // characterEntity.UnbindHud<HudCharacterStatusController>();
                
                ConvertCombatEntityToCharacter(combatEntity,playerComp.Characters[0]);
                DestroyCombatEntity(combatEntity);
            }

            void ConvertCharacterToCombatEntity(Entity character,Entity combatEntity)
            {
                // 创建初始卡组
                var combatDeckComp = combatEntity.GetRawComponent<CombatDeckRawComponent>();
                var characterDeckComp = character.GetRawComponent<CharacterDeckRawComponent>();
                combatDeckComp.DicesInDeck=characterDeckComp.Dices;
                combatDeckComp.DicesInDeck.Shuffle();
                // 初始化属性
                var characterAttributesComp = character.GetRawComponent<AttributesRawComponent>();
                var combatAttributesComp = combatEntity.GetRawComponent<AttributesRawComponent>();
            }
            void ConvertCombatEntityToCharacter(Entity combatEntity,Entity character)
            {
                
            }

            void DestroyCombatEntity(Entity combatEntity)
            {
                if (combatEntity!=null)
                {
                    var combatDeckComp = combatEntity.GetRawComponent<CombatDeckRawComponent>();
                    combatDeckComp.DicesInDeck = null;
                    combatEntity.Destroy();
                    combatEntity.UnbindHud<HudCharacterStatusController>();
                }
               
            }
        }

        private class CombatStageInitCombatInfo : IStageLoad
        {
            private Entity m_MonsterEntity;

            void IStageLoad.Load(GameStage stage)
            {
                // 创建怪物
                var dungeonRoomComp = EcsApi.GetSingletonRawComponent<DungeonRoomSingletonRawComponent>();
                var roomData = DataApi.GetData<RoomData>();
                var roomPos = dungeonRoomComp.CurRoom.GetGameObject().transform.position;
                var spawnPos = roomPos + roomData.MonsterOffset;
                m_MonsterEntity = EntityCreator.CreateMonsterEntity(GameUtility.RandomPool.GetRandomMonster(), spawnPos, Quaternion.LookRotation((roomPos - spawnPos).SetY(0)));

                // 初始化战场信息
                var combatInfoComp = EcsApi.GetSingletonRawComponent<CombatInfoSingletonRawComponent>();
                var combatRoundComp = EcsApi.AddSingletonRawComponent<CombatRoundSingletonRawComponent>();
                combatInfoComp.Monster = m_MonsterEntity;
                combatRoundComp.EnterCombat = true;
            }

            void IStageLoad.Unload(GameStage stage)
            {
                EcsApi.RemoveSingletonRawComponent<CombatInfoSingletonRawComponent>();
                EcsApi.RemoveSingletonRawComponent<CombatRoundSingletonRawComponent>();
                m_MonsterEntity.Destroy();
            }
        }

        private class CombatStageBindUi : IStageLoad
        {
            void IStageLoad.Load(GameStage stage)
            {
                var combatInfoComp = EcsApi.GetSingletonRawComponent<CombatInfoSingletonRawComponent>();
                UiApi.GetUiController<UiCharacterInfoController>().Bind(combatInfoComp.Character);
                UiApi.GetUiController<UiMonsterInfoController>().Bind(combatInfoComp.Monster);
            }

            void IStageLoad.Unload(GameStage stage)
            {

            }
        }
    }
}
