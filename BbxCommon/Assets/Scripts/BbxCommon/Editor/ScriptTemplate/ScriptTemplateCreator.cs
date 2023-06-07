using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;

namespace BbxCommon.Editor
{
    public static class ScriptTemplateCreator
    {
        [MenuItem("Assets/Create/BbxCommon/Script/Ecs/EcsRawComponent", false)]
        public static void CreateEcsRawComponent()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                new SimpleScriptReplacer(), "TestRawComponent.cs",
                EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D,
                "Assets/Scripts/BbxCommon/Editor/ScriptTemplate/EcsRawComponentTemplate.txt");
        }

        [MenuItem("Assets/Create/BbxCommon/Script/Ecs/EcsSingletonRawComponent", false)]
        public static void CreateEcsSingletonRawComponent()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                new SimpleScriptReplacer(), "TestSingletonRawComponent.cs",
                EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D,
                "Assets/Scripts/BbxCommon/Editor/ScriptTemplate/EcsSingletonRawComponentTemplate.txt");
        }

        [MenuItem("Assets/Create/BbxCommon/Script/Ecs/EcsRawAspect", false)]
        public static void CreateEcsRawAspect()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                new SimpleScriptReplacer(), "TestRawAspect.cs",
                EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D,
                "Assets/Scripts/BbxCommon/Editor/ScriptTemplate/EcsRawAspectTemplate.txt");
        }

        [MenuItem("Assets/Create/BbxCommon/Script/Ecs/EcsMixSystem", false)]
        public static void CreateEcsMixSystem()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                new SimpleScriptReplacer(), "TestSystem.cs",
                EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D,
                "Assets/Scripts/BbxCommon/Editor/ScriptTemplate/EcsMixSystemTemplate.txt");
        }

        [MenuItem("Assets/Create/BbxCommon/Script/Ui/UiModel", false)]
        public static void CreateUiModel()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                new SimpleScriptReplacer(), "UiTestModel.cs",
                EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D,
                "Assets/Scripts/BbxCommon/Editor/ScriptTemplate/UiModelTemplate.txt");
        }

        [MenuItem("Assets/Create/BbxCommon/Script/Ui/UiView", false)]
        public static void CreateUiView()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                new UiScriptReplacer(), "UiTestView.cs",
                EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D,
                "Assets/Scripts/BbxCommon/Editor/ScriptTemplate/UiViewTemplate.txt");
        }

        [MenuItem("Assets/Create/BbxCommon/Script/Ui/UiController", false)]
        public static void CreateUiController()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                new UiScriptReplacer(), "UiTestController.cs",
                EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D,
                "Assets/Scripts/BbxCommon/Editor/ScriptTemplate/UiControllerTemplate.txt");
        }
    }

    internal class SimpleScriptReplacer : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            string fullPath = Path.GetFullPath(pathName);
            StreamReader streamReader = new StreamReader(resourceFile);
            string txt = streamReader.ReadToEnd();
            streamReader.Close();
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathName);
            txt = Regex.Replace(txt, "#SCRIPT_NAME#", fileNameWithoutExtension);
            bool encoderShouldEmitUTF8Identifier = true;
            bool throwOnInvalidBytes = false;
            UTF8Encoding encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier, throwOnInvalidBytes);
            bool append = false;
            StreamWriter streamWriter = new StreamWriter(fullPath, append, encoding);
            streamWriter.Write(txt);
            streamWriter.Close();
            AssetDatabase.ImportAsset(txt);
            var obj = AssetDatabase.LoadAssetAtPath(pathName, typeof(Object));
            ProjectWindowUtil.ShowCreatedAsset(obj);
            AssetDatabase.Refresh();
        }
    }

    internal class UiScriptReplacer : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            string fullPath = Path.GetFullPath(pathName);
            StreamReader streamReader = new StreamReader(resourceFile);
            string txt = streamReader.ReadToEnd();
            streamReader.Close();
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathName);
            var uiName = fileNameWithoutExtension;
            if (uiName.EndsWith("Controller"))
                uiName = uiName.Remove(uiName.Length - "Controller".Length - 1, "Controller".Length);
            else if (uiName.EndsWith("View"))
                uiName = uiName.Remove(uiName.Length - "View".Length - 1, "View".Length);
            uiName = uiName.StartsWith("Ui") || uiName.StartsWith("UI") ? uiName.Remove(0, 2) : uiName;
            txt = Regex.Replace(txt, "#UI_NAME#", uiName);
            bool encoderShouldEmitUTF8Identifier = true;
            bool throwOnInvalidBytes = false;
            UTF8Encoding encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier, throwOnInvalidBytes);
            bool append = false;
            StreamWriter streamWriter = new StreamWriter(fullPath, append, encoding);
            streamWriter.Write(txt);
            streamWriter.Close();
            AssetDatabase.ImportAsset(txt);
            var obj = AssetDatabase.LoadAssetAtPath(pathName, typeof(Object));
            ProjectWindowUtil.ShowCreatedAsset(obj);
            AssetDatabase.Refresh();
        }
    }
}
