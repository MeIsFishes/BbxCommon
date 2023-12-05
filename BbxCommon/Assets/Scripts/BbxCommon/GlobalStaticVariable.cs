
namespace BbxCommon
{
    public static class GlobalStaticVariable
    {
        /// <summary>
        /// Relative path of <see cref="PreLoadUiData"/> in Resources folder.
        /// </summary>
        public static string ExportPreLoadUiPathInResources = "BbxCommon/Ui/PreLoadUiData";
        /// <summary>
        /// Relative path of <see cref="ScriptableObjectAssets"/> in Resources folder.
        /// </summary>
        public static string ExportScriptableObjectPathInResource = "BbxCommon/ScriptableObjectAssets";

        public static int SimplePoolLimit = 100;
        public static int ObjectPoolLimit = 100;
    }
}
