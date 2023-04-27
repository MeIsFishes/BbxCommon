using System.Collections.Generic;

namespace BbxCommon.Ui
{
    internal static class UiControllerManager
    {
        private static UiGameEngineScene m_UiGameEngineScene;

        internal static void SetUiGameEngineScene(UiGameEngineScene scene)
        {
            m_UiGameEngineScene = scene;
        }

        internal static UiGameEngineScene GetUiGameEngineScene()
        {
            return m_UiGameEngineScene;
        }
    }

    internal static class UiCollection<T> where T : UiControllerBase
    {
        private static List<T> m_UiControllers = new();
        private static List<T> m_PooledControllers = new();
    }
}
