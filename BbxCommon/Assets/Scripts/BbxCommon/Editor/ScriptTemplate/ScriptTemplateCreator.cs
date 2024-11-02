using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEditor.Build.Content;

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

        [MenuItem("Assets/Create/BbxCommon/Script/StageListener", false)]
        public static void CreateStageListener()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                new SimpleScriptReplacer(), "TestListener.cs",
                EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D,
                "Assets/Scripts/BbxCommon/Editor/ScriptTemplate/StageListenerTemplate.txt");
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

        [MenuItem("Assets/Create/BbxCommon/Script/Config/ScriptableObject", false)]
        public static void CreateScriptableObject()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                new SimpleScriptReplacer(), "TestData.cs",
                EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D,
                "Assets/Scripts/BbxCommon/Editor/ScriptTemplate/ScriptableObjectTemplate.txt");
        }

        [MenuItem("Assets/Create/BbxCommon/Script/Config/CsvData", false)]
        public static void CreateCsvData()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                new SimpleScriptReplacer(), "TestData.cs",
                EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D,
                "Assets/Scripts/BbxCommon/Editor/ScriptTemplate/CsvDataTemplate.txt");
        }

        [MenuItem("Assets/Create/BbxCommon/Script/Task/TaskNode", false)]
        public static void CreateTaskNode()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                new SimpleScriptReplacer(), "TaskNodeTest.cs",
                EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D,
                "Assets/Scripts/BbxCommon/Editor/ScriptTemplate/TaskNodeTemplate.txt");
        }

        [MenuItem("Assets/Create/BbxCommon/Script/Task/TaskCondition", false)]
        public static void CreateTaskCondition()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                new SimpleScriptReplacer(), "TaskConditionTest.cs",
                EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D,
                "Assets/Scripts/BbxCommon/Editor/ScriptTemplate/TaskConditionTemplate.txt");
        }

        [MenuItem("Assets/Create/BbxCommon/Script/Task/TaskContext", false)]
        public static void CreateTaskContext()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                new SimpleScriptReplacer(), "TaskContextTest.cs",
                EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D,
                "Assets/Scripts/BbxCommon/Editor/ScriptTemplate/TaskContextTemplate.txt");
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
            if (pathName.EndsWith("Controller.cs"))
            {
                DoProcess(pathName, "Assets/Scripts/BbxCommon/Editor/ScriptTemplate/UiControllerTemplate.txt");
                DoProcess(pathName.TryRemoveEnd("Controller.cs") + "View.cs", "Assets/Scripts/BbxCommon/Editor/ScriptTemplate/UiViewTemplate.txt");
            }
            else if (pathName.EndsWith("View.cs"))
            {
                DoProcess(pathName, "Assets/Scripts/BbxCommon/Editor/ScriptTemplate/UiViewTemplate.txt");
                DoProcess(pathName.TryRemoveEnd("View.cs") + "Controller.cs", "Assets/Scripts/BbxCommon/Editor/ScriptTemplate/UiControllerTemplate.txt");
            }
            if (pathName.EndsWith("ControllerBase.cs"))
            {
                DoProcess(pathName, "Assets/Scripts/BbxCommon/Editor/ScriptTemplate/UiControllerTemplate.txt");
                DoProcess(pathName.TryRemoveEnd("ControllerBase.cs") + "ViewBase.cs", "Assets/Scripts/BbxCommon/Editor/ScriptTemplate/UiViewTemplate.txt");
            }
            else if (pathName.EndsWith("ViewBase.cs"))
            {
                DoProcess(pathName, "Assets/Scripts/BbxCommon/Editor/ScriptTemplate/UiViewTemplate.txt");
                DoProcess(pathName.TryRemoveEnd("ViewBase.cs") + "ControllerBase.cs", "Assets/Scripts/BbxCommon/Editor/ScriptTemplate/UiControllerTemplate.txt");
            }
        }

        private void DoProcess(string pathName, string resourceFile)
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
            if (pathName.EndsWith("Controller.cs"))
            {
                DoProcess(pathName, "Assets/Scripts/BbxCommon/Editor/ScriptTemplate/HudControllerTemplate.txt");
                DoProcess(pathName.TryRemoveEnd("Controller.cs") + "View.cs", "Assets/Scripts/BbxCommon/Editor/ScriptTemplate/HudViewTemplate.txt");
            }
            else if (pathName.EndsWith("View.cs"))
            {
                DoProcess(pathName, "Assets/Scripts/BbxCommon/Editor/ScriptTemplate/HudViewTemplate.txt");
                DoProcess(pathName.TryRemoveEnd("View.cs") + "Controller.cs", "Assets/Scripts/BbxCommon/Editor/ScriptTemplate/HudControllerTemplate.txt");
            }
            if (pathName.EndsWith("ControllerBase.cs"))
            {
                DoProcess(pathName, "Assets/Scripts/BbxCommon/Editor/ScriptTemplate/HudControllerTemplate.txt");
                DoProcess(pathName.TryRemoveEnd("ControllerBase.cs") + "ViewBase.cs", "Assets/Scripts/BbxCommon/Editor/ScriptTemplate/HudViewTemplate.txt");
            }
            else if (pathName.EndsWith("ViewBase.cs"))
            {
                DoProcess(pathName, "Assets/Scripts/BbxCommon/Editor/ScriptTemplate/HudViewTemplate.txt");
                DoProcess(pathName.TryRemoveEnd("ViewBase.cs") + "ControllerBase.cs", "Assets/Scripts/BbxCommon/Editor/ScriptTemplate/HudControllerTemplate.txt");
            }
        }

        private void DoProcess(string pathName, string resourceFile)
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
