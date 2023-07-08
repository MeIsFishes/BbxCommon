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
        private GameStage m_DungeonWalkStage;
        private GameStage m_DungeonStage;
        private GameStage m_CombatStage;

        protected override void InitSingletonComponents()
        {
            
        }

        protected override void OnAwake()
        {
            UiScene = UiSceneWrapper.CreateUiScene<UiDungeonScene>();
            m_GlobalStage = GlobalStage.CreateStage();
            m_DungeonStage = DungeonStage.CreateStage();
            m_DungeonWalkStage = DungeonWalkStage.CreateStage();
            m_CombatStage = CombatStage.CreateStage();

            StageWrapper.LoadStage(m_GlobalStage);
            StageWrapper.LoadStage(m_DungeonStage);
            StageWrapper.LoadStage(m_DungeonWalkStage);
        }
    }
}
