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
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    m_FoldoutDic[stage.StageName + "Load Items"] = EditorGUILayout.Foldout(m_FoldoutDic.GetOrAdd(stage.StageName + "Load Items"), "Load Items");
                    GUILayout.EndHorizontal();
                    if (m_FoldoutDic[stage.StageName + "Load Items"])
                    {
                        foreach (var pair in loadingTimeData.GetStageItemDic(stage.StageName))
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Space(40);
                            GUILayout.Label(pair.Key, GUILayout.Width(350));
                            GUILayout.Label((pair.Value / 1000f).ToString() + "ms", GUILayout.Width(150));
                            GUILayout.EndHorizontal();
                        }
                    }

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    m_FoldoutDic[stage.StageName + "System"] = EditorGUILayout.Foldout(m_FoldoutDic.GetOrAdd(stage.StageName + "System"), "System");
                    GUILayout.EndHorizontal();
                    if (m_FoldoutDic[stage.StageName + "System"])
                    {
                        for (int i = 0; i < stage.SystemTypes.Count; i++)
                        {
                            var systemType = stage.SystemTypes[i];
                            GUILayout.BeginHorizontal();
                            GUILayout.Space(40);
                            GUILayout.Label(systemType.Name, GUILayout.Width(350));
                            GUILayout.Label((DebugApi.GetProfilerTimeUs(systemType.Name) / 1000f).ToString() + "ms", GUILayout.Width(150));
                            GUILayout.EndHorizontal();
                        }
                    }
                }
            }
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }
    }
}
