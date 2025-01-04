using System;
using UnityEngine;
using BbxCommon;
using BbxCommon.Ui;
using Dcg.Ui;

namespace Dcg
{
    public partial class DcgGameEngine : GameEngineBase<DcgGameEngine>
    {
        [NonSerialized]
        public UiDungeonScene UiScene;

        private GameStage m_GlobalStage;
        private GameStage m_GameStartStage;
        private GameStage m_DungeonWalkStage;
        private GameStage m_DungeonStage;
        private GameStage m_MapStage;
        private GameStage m_CombatStage;
        private GameStage m_RewardStage;

        protected override void InitSingletonComponents()
        {
            
        }

        protected override void OnAwake()
        {
            UiScene = UiSceneWrapper.CreateUiScene<UiDungeonScene>();
            m_GlobalStage = GlobalStage.CreateStage();
            m_GameStartStage = GameStartStage.CreateStage();
            m_DungeonStage = DungeonStage.CreateStage();
            m_CombatStage = CombatStage.CreateStage();
            m_RewardStage = RewardStage.CreateStage();
            m_MapStage = MapStage.CreateStage();

            StageWrapper.LoadStage(m_GlobalStage);
            StageWrapper.LoadStage(m_GameStartStage);
            SetLoadingUi<UiLoadingController>();
        }

        public void OpenMap()
        {
            StageWrapper.UnloadStage(m_GameStartStage);
            StageWrapper.UnloadStage(m_CombatStage);
            StageWrapper.LoadStage(m_MapStage);
        }
        
        
        public void EnterCombat()
        {
            StageWrapper.UnloadStage(m_MapStage);
            StageWrapper.LoadStage(m_DungeonStage);
            StageWrapper.LoadStage(m_CombatStage);
            ClearAllTips();
            
        }

        public void CombatWin()
        {
            StageWrapper.LoadStage(m_RewardStage);
            
        }

        public void ChooseRewardComplete()
        {
            StageWrapper.UnloadStage(m_RewardStage);
            StageWrapper.UnloadStage(m_CombatStage);
            StageWrapper.UnloadStage(m_DungeonStage);
            StageWrapper.LoadStage(m_MapStage);
            
        }

        public void RestartGame()
        {
            StageWrapper.UnloadStage(m_RewardStage);
            StageWrapper.UnloadStage(m_CombatStage);
            StageWrapper.UnloadStage(m_DungeonStage);
            StageWrapper.LoadStage(m_DungeonStage);
            StageWrapper.LoadStage(m_DungeonWalkStage);
            UiApi.GetUiController<UiGameFailedController>().Show();
            ClearAllTips();
            
        }

        public void ClearAllTips()
        {
            UiApi.GetUiController<UiTipController>().ClearTips();
            UiApi.GetUiController<UiPromptController>().Hide();
        }
        
    }
}
