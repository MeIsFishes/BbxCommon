using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg.Ui
{
    public class UiEndTurnController : UiControllerBase<UiEndTurnView>
    {
        private Entity m_Entity;

        public void Bind(Entity entity)
        {
            m_Entity = entity;
        }

        protected override void OnUiInit()
        {
            m_View.Button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            m_Entity.GetRawComponent<CombatTurnRawComponent>().RequestEnd = true;
        }
    }
}
