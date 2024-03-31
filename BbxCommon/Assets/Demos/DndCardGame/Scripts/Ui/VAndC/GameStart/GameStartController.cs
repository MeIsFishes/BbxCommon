using BbxCommon.Ui;
using UnityEngine;

namespace Dcg.Ui
{
    public class GameStartController: UiControllerBase<GameStartView>
    {
        protected override void OnUiInit()
        {
            m_View.Button.onClick.AddListener(DcgGameEngine.Instance.StartGame);
        }

        protected override void OnUiOpen()
        {
            m_View.Rect.anchorMin = new Vector2(0.5f, 0.5f);
            m_View.Rect.anchorMax = new Vector2(0.5f, 0.5f);
            m_View.Rect.sizeDelta = new Vector2(Screen.width, Screen.height);
        }
    }
}