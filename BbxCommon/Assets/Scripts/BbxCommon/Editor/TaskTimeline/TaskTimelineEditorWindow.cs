using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
namespace BbxCommon.TaskInternal
{
    public class TaskTimelineEditorWindow : EditorWindow
    {
        private string m_DefaultExportDirectory = "Scripts/AutoExported/TaskTimeline/";
        private Vector2 m_ScrollPosition;

        [MenuItem("BbxCommon/TaskTimeline")]
        private static void OpenWindow()
        {
            GetWindow<TaskTimelineEditorWindow>().Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("");
            EditorGUILayout.LabelField("Export File Directory:");
            m_DefaultExportDirectory = EditorGUILayout.TextField(m_DefaultExportDirectory);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Class Name");
            EditorGUILayout.LabelField("Namespace");
            if (GUILayout.Button("Export Scripts For All", GUILayout.Width(200)))
            {

            }
            EditorGUILayout.EndHorizontal();

            m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition, GUILayout.Height(800));
            bool deepColor = false;
            foreach (var task in TaskTimelineEditorDatabase.Tasks)
            {
                var rect = EditorGUILayout.BeginHorizontal();
                if (deepColor)
                    EditorGUI.DrawRect(rect, new Color(0.18f, 0.18f, 0.18f));
                else
                    EditorGUI.DrawRect(rect, new Color(0.45f, 0.45f, 0.45f));
                deepColor = !deepColor;
                EditorGUILayout.LabelField(task.Name);
                EditorGUILayout.LabelField(task.Namespace);
                if (GUILayout.Button("Export Editor Script", GUILayout.Width(200)))
                {

                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }
    }
}
#endif
