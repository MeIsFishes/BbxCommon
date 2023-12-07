using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg.Ui
{
    public class UiTipListItemController : UiControllerBase<UiTipListItemView>
    {
        private string m_Title;
        private string m_Description;

        public void Init(string title, string description)
        {
            m_Title = title;
            m_Description = description;
            m_View.Title.text = title;
        }

        protected override void OnUiInit()
        {
            m_View.Button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            UiApi.GetUiController<UiTipController>().ShowTip(m_Title, m_Description);
        }
    }
}
