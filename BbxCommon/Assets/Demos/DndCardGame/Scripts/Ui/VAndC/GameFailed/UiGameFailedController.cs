using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;
using UnityEngine.EventSystems;

namespace Dcg.Ui
{
    public class UiGameFailedController : UiControllerBase<UiGameFailedView>
    {
        protected override void OnUiInit()
        {
            m_View.UiEventListener.OnPointerClick += (PointerEventData eventData) => { m_View.TweenGroup.Play(); };
            m_View.TweenGroup.Wrapper.OnPlayingFinishes += () => { Hide(); };
        }
    }
}
