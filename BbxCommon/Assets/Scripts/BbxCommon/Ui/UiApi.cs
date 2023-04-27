using System.Collections.Generic;

namespace BbxCommon.Ui
{
    public static class UiApi
    {


        #region internal
        internal static void SetUiGameEngineScene(UiGameEngineScene scene)
        {
            UiControllerManager.SetUiGameEngineScene(scene);
        }

        internal static UiGameEngineScene GetUiGameEngineScene()
        {
            return UiControllerManager.GetUiGameEngineScene();
        }
        #endregion
    }
}
