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

        [MenuItem("Assets/Create/BbxCommon/Script/Ecs/EcsBaker", false)]
        public static void CreateEcsBaker()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                new SimpleScriptReplacer(), "TestBaker.cs",
                EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D,
                "Assets/Scripts/BbxCommon/Editor/ScriptTemplate/EcsBakerTemplate.txt");
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

        [MenuItem("Assets/Create/BbxCommon/Script/Ui/HudView", false)]
        public static void CreateHudView()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                new HudScriptReplacer(), "HudTestView.cs",
                EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D,
                "Assets/Scripts/BbxCommon/Editor/ScriptTemplate/HudViewTemplate.txt");
        }

        [MenuItem("Assets/Create/BbxCommon/Script/Ui/HudController", false)]
        public static void CreateHudController()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                new HudScriptReplacer(), "HudTestController.cs",
                EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D,
                "Assets/Scripts/BbxCommon/Editor/ScriptTemplate/HudControllerTemplate.txt");
        }

        [MenuItem("Assets/Create/BbxCommon/Script/ScriptableObject", false)]
        public static void CreateScriptableObject()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                new SimpleScriptReplacer(), "TestData.cs",
                EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D,
                "Assets/Scripts/BbxCommon/Editor/ScriptTemplate/ScriptableObjectTemplate.txt");
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

            if (fileNameWithoutExtension.EndsWith("Base"))
            {
                txt = Regex.Replace(txt, "public partial class", "public abstract partial class");
                txt = Regex.Replace(txt, "public class", "public abstract class");
            }

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
                uiName = uiName.TryRemoveEnd("Controller");
            else if (uiName.EndsWith("View"))
                uiName = uiName.TryRemoveEnd("View");
            else if (uiName.EndsWith("ControllerBase"))
                uiName = uiName.TryRemoveEnd("ControllerBase");
            else if (uiName.EndsWith("ViewBase"))
                uiName = uiName.TryRemoveEnd("ViewBase");
            uiName = uiName.TryRemoveStart("Ui");
            uiName = uiName.TryRemoveStart("UI");
            txt = Regex.Replace(txt, "#UI_NAME#", uiName);

            if (fileNameWithoutExtension.EndsWith("Base"))
            {
                txt = Regex.Replace(txt, "public class", "public abstract class");
                txt = Regex.Replace(txt, "#BASE#", "Base");
            }
            else
                txt = Regex.Replace(txt, "#BASE#", "");

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

    internal class HudScriptReplacer : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            string fullPath = Path.GetFullPath(pathName);
            StreamReader streamReader = new StreamReader(resourceFile);
            string txt = streamReader.ReadToEnd();
            streamReader.Close();
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathName);
            var hudName = fileNameWithoutExtension;

            if (hudName.EndsWith("Controller"))
                hudName = hudName.TryRemoveEnd("Controller");
            else if (hudName.EndsWith("View"))
                hudName = hudName.TryRemoveEnd("View");
            else if (hudName.EndsWith("ControllerBase"))
                hudName = hudName.TryRemoveEnd("ControllerBase");
            else if (hudName.EndsWith("ViewBase"))
                hudName = hudName.TryRemoveEnd("ViewBase");
            hudName = hudName.TryRemoveStart("Hud");
            hudName = hudName.TryRemoveStart("HUD");
            txt = Regex.Replace(txt, "#HUD_NAME#", hudName);

            if (fileNameWithoutExtension.EndsWith("Base"))
            {
                txt = Regex.Replace(txt, "public class", "public abstract class");
                txt = Regex.Replace(txt, "#BASE#", "Base");
            }
            else
                txt = Regex.Replace(txt, "#BASE#", "");

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
