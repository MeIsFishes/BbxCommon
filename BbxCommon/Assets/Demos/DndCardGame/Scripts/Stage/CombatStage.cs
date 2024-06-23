using UnityEngine;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;
using Dcg.Ui;
using System.Collections.Generic;

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

            stage.AddStageListener<CombatWinListener>();
            stage.AddStageListener<CombatDefeatedListener>();

            return stage;
        }

        private class CombatStageInitPlayerData : IStageLoad
        {
            private Entity combatEntity;
            void IStageLoad.Load(GameStage stage)
            {
                var playerComp = EcsApi.GetSingletonRawComponent<LocalPlayerSingletonRawComponent>();
                combatEntity = EntityCreator.CreateCombatEntity();
                ConvertCharacterToCombatEntity(playerComp.Characters[0],combatEntity);
                playerComp.DestroyCharacterEntities();
                var combatInfoComp = EcsApi.AddSingletonRawComponent<CombatInfoSingletonRawComponent>();
                combatInfoComp.Character = combatEntity;
            }

            void IStageLoad.Unload(GameStage stage)
            {
                var playerComp = EcsApi.GetSingletonRawComponent<LocalPlayerSingletonRawComponent>();
                var character = EntityCreator.CreateCharacterEntity();
                ConvertCombatEntityToCharacter(combatEntity, character);
                playerComp.AddCharacter(character);
                DestroyCombatEntity(combatEntity);
            }

            void ConvertCharacterToCombatEntity(Entity character,Entity combatEntity)
            {
                var charObj=character.GetGameObject();
                var comBatObj= combatEntity.GetGameObject();
                comBatObj.transform.localPosition = charObj.transform.localPosition;
                // 创建初始卡组
                var combatDeckComp = combatEntity.GetRawComponent<CombatDeckRawComponent>();
                var characterDeckComp = character.GetRawComponent<CharacterDeckRawComponent>();
                combatDeckComp.DicesInDeck.Clear();
                foreach (var dice in characterDeckComp.Dices)
                {
                    combatDeckComp.DicesInDeck.Add(Dice.Create(dice.DiceType));
                }
                combatDeckComp.DicesInDeck.Shuffle();
            }

            void ConvertCombatEntityToCharacter(Entity combatEntity,Entity character)
            {
                var charObj = character.GetGameObject();
                var comBatObj = combatEntity.GetGameObject();
                charObj.transform.localPosition = comBatObj.transform.localPosition;
                // 创建初始卡组
                var combatDeckComp = combatEntity.GetRawComponent<CombatDeckRawComponent>();
                var characterDeckComp = character.GetRawComponent<CharacterDeckRawComponent>();
                characterDeckComp.Dices.Clear();
                foreach (var dice in combatDeckComp.DicesInDeck)
                {
                    characterDeckComp.Dices.Add(Dice.Create(dice.DiceType));
                }
                characterDeckComp.Dices.Shuffle();
            }

            void DestroyCombatEntity(Entity combatEntity)
            {
                if (combatEntity!=null)
                {
                    var combatDeckComp = combatEntity.GetRawComponent<CombatDeckRawComponent>();
                    combatDeckComp.DicesInDeck = null;
                    combatEntity.UnbindHud<HudCharacterStatusController>();
                    combatEntity.Destroy();
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
