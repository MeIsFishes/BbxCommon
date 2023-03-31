using UnityEngine;

namespace BbxCommon.Ui
{
    internal enum EUiGameEngine
    {
        Loading,
        Top,
    }

    internal class UiGameEngineScene : UiSceneBase<EUiGameEngine>
    {
        protected override void OnSceneInit()
        {
            CreateUiGroupRoot(EUiGameEngine.Loading);
            CreateUiGroupRoot(EUiGameEngine.Top);
        }
    }
}
