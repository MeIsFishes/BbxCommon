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
        }
    }
}
