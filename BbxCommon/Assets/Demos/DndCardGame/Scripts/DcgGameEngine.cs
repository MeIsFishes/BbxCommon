using System;
using UnityEngine;
using BbxCommon;
using BbxCommon.Ui;
using Dcg.Ui;

namespace Dcg
{
    public class DcgGameEngine : GameEngineBase<DcgGameEngine>
    {
        [NonSerialized]
        public UiDungeonScene UiScene;

        private GameStage m_GlobalStage;
        private GameStage m_DungeonStage;
        private GameStage m_CombatStage;

        protected override void InitSingletonComponents()
        {
            
        }

        protected override void OnAwake()
        {
            UiScene = UiSceneWrapper.CreateUiScene<UiDungeonScene>();
            m_GlobalStage = CreateGlobalStage();
            m_DungeonStage = CreateDungeonStage();
            m_CombatStage = CreateCombatStage();

            StageWrapper.LoadStage(m_GlobalStage);
            StageWrapper.LoadStage(m_DungeonStage);
            StageWrapper.LoadStage(m_CombatStage);
        }

        #region GlobalStage
        private GameStage CreateGlobalStage()
        {
            var globalStage = StageWrapper.CreateStage("Global Stage");
            globalStage.AddScene("DcgMain");
            globalStage.SetUiScene(UiScene, Resources.Load<UiSceneAsset>("DndCardGame/Configs/UiDungeonScene"));
            globalStage.AddLoadItem(Resources.Load<DcgInteractingDataAsset>("DndCardGame/Configs/DcgInteractingDataAsset"));
            return globalStage;
        }
        #endregion

        #region DungeonStage
        private GameStage CreateDungeonStage()
        {
            var dungeonStage = StageWrapper.CreateStage("Dungeon Stage");
            dungeonStage.AddLoadItem(new CreatePlayerEntity());
            return dungeonStage;
        }

        private class CreatePlayerEntity : IStageLoad
        {
            void IStageLoad.Load(GameStage stage)
            {
                EntityCreator.CreatePlayerEntity();
            }

            void IStageLoad.Unload(GameStage stage)
            {
                var entity = EcsApi.GetSingletonRawComponent<PlayerSingletonRawComponent>().GetEntity();
                entity.Destroy();
            }
        }
        #endregion

        #region CombatStage
        private GameStage CreateCombatStage()
        {
            var combatStage = StageWrapper.CreateStage("Combat Stage");
            combatStage.SetUiScene(UiScene, Resources.Load<UiSceneAsset>("DndCardGame/Configs/UiCombatScene"));
            combatStage.AddLoadItem(new CombatStageInitPlayerData());
            combatStage.AddLateLoadItem(new CombatStageBindUi());
            return combatStage;
        }

        private class CombatStageInitPlayerData : IStageLoad
        {
            void IStageLoad.Load(GameStage stage)
            {
                var playerEntity = EcsApi.GetSingletonRawComponent<PlayerSingletonRawComponent>().GetEntity();
                var charcterDeckComp = playerEntity.GetRawComponent<CharacterDeckRawComponent>();
                var combatDeckComp = playerEntity.AddRawComponent<CombatDeckRawComponent>();
                combatDeckComp.DicesInDeck.Clear();
                combatDeckComp.DicesInDeck.AddList(charcterDeckComp.Dices);
            }

            void IStageLoad.Unload(GameStage stage)
            {
                var playerEntity = EcsApi.GetSingletonRawComponent<PlayerSingletonRawComponent>().GetEntity();
                playerEntity.RemoveRawComponent<CombatDeckRawComponent>();
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
                uiController.Bind(EcsApi.GetSingletonRawComponent<PlayerSingletonRawComponent>().GetEntity());
            }

            void IStageLoad.Unload(GameStage stage) { }
        }
        #endregion
    }
}
