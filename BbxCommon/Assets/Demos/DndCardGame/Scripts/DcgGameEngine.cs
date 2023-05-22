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

        protected override void InitSingletonComponents()
        {
            
        }

        protected override void OnAwake()
        {
            UiScene = UiSceneWrapper.CreateUiScene<UiDungeonScene>();
            m_GlobalStage = CreateGlobalStage();
            m_DungeonStage = CreateDungeonStage();

            StageWrapper.LoadStage(m_GlobalStage);
            StageWrapper.LoadStage(m_DungeonStage);
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
    }
}
