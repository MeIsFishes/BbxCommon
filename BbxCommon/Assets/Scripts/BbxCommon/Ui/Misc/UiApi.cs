
namespace BbxCommon.Ui
{
    public static class UiApi
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
}
