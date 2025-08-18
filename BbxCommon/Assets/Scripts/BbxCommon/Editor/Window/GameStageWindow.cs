using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

namespace BbxCommon.Editor
{
    internal class GameStageWindow : EditorWindow
    {
        internal static IGameEngine CurGameEngine;

        private Dictionary<string, bool> m_FoldoutDic = new();
        private Vector2 m_ScrollPos;

        [MenuItem("BbxCommon/GameStageWindow")]
        private static void Open()
        {
            var window = GetWindow<GameStageWindow>("Game Stage");
            window.Show();
        }

        private void OnGUI()
        {
            if (CurGameEngine == null)
                return;
            var loadingTimeData = DataApi.GetData<LoadingTimeData>();
            loadingTimeData.Refresh();
            m_ScrollPos = GUILayout.BeginScrollView(m_ScrollPos, GUILayout.ExpandHeight(true));
            GUILayout.BeginVertical();
            // title
            GUILayout.Label(CurGameEngine.GetType().Name);
            // game stages
            foreach (var stage in CurGameEngine.GetEnabledGameStage())
            {
                m_FoldoutDic[stage.StageName] = EditorGUILayout.Foldout(m_FoldoutDic.GetOrAdd(stage.StageName), stage.StageName);
                if (m_FoldoutDic[stage.StageName])
                {
                    foreach (var pair in loadingTimeData.GetStageItemDic(stage.StageName))
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("    " + pair.Key, GUILayout.Width(300));
                        GUILayout.Label((pair.Value / 1000f).ToString() + "ms", GUILayout.Width(150));
                        GUILayout.EndHorizontal();
                    }
                }
            }
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }
    }
}
