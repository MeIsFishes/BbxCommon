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
                new SimpleScriptReplacer(), "EcsRawComponentTemplate.cs",
                EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D,
                "Assets/Scripts/BbxCommon/Editor/ScriptTemplate/EcsRawComponentTemplate.txt");
        }

        [MenuItem("Assets/Create/BbxCommon/Script/Ecs/EcsSingletonRawComponent", false)]
        public static void CreateEcsSingletonRawComponent()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                new SimpleScriptReplacer(), "EcsSingletonRawComponentTemplate.cs",
                EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D,
                "Assets/Scripts/BbxCommon/Editor/ScriptTemplate/EcsSingletonRawComponentTemplate.txt");
        }

        [MenuItem("Assets/Create/BbxCommon/Script/Ecs/EcsRawAspect", false)]
        public static void CreateEcsRawAspect()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                new SimpleScriptReplacer(), "EcsRawAspectTemplate.cs",
                EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D,
                "Assets/Scripts/BbxCommon/Editor/ScriptTemplate/EcsRawAspectTemplate.txt");
        }

        [MenuItem("Assets/Create/BbxCommon/Script/Ecs/EcsMixSystem", false)]
        public static void CreateEcsMixSystem()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                new SimpleScriptReplacer(), "EcsMixSystemTemplate.cs",
                EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D,
                "Assets/Scripts/BbxCommon/Editor/ScriptTemplate/EcsMixSystemTemplate.txt");
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
            string fileNameWithOutExtension = Path.GetFileNameWithoutExtension(pathName);
            txt = Regex.Replace(txt, "#SCRIPT_NAME#", fileNameWithOutExtension);
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
