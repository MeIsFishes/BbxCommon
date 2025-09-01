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
            m_ScrollPos = GUILayout.BeginScrollView(m_ScrollPos, GUILayout.ExpandHeight(true));
            GUILayout.BeginVertical();
            // title
            GUILayout.Label(CurGameEngine.GetType().Name);
            // game stages
            foreach (var stage in CurGameEngine.GetEnabledGameStage())
            {
                m_FoldoutDic.GetOrAdd(stage.StageName, out bool isFoldout);
                m_FoldoutDic[stage.StageName] = EditorGUILayout.Foldout(isFoldout, stage.StageName);
                if (m_FoldoutDic[stage.StageName])
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    m_FoldoutDic.GetOrAdd(stage.StageName + "Load Items", out isFoldout);
                    m_FoldoutDic[stage.StageName + "Load Items"] = EditorGUILayout.Foldout(isFoldout, "Load Items");
                    GUILayout.EndHorizontal();
                    if (m_FoldoutDic[stage.StageName + "Load Items"])
                    {
                        foreach (var pair in loadingTimeData.GetStageItemDic(stage.StageName))
                        {
                            var strs = pair.Key.Split('.');
                            var key = pair.Key.TryRemoveStart(strs[0] + ".");
                            GUILayout.BeginHorizontal();
                            GUILayout.Space(40);
                            GUILayout.Label(key, GUILayout.Width(350));
                            GUILayout.Label((pair.Value / 1000000f).ToString() + "ms", GUILayout.Width(150));
                            GUILayout.EndHorizontal();
                        }
                    }

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    m_FoldoutDic.GetOrAdd(stage.StageName + "System", out isFoldout);
                    m_FoldoutDic[stage.StageName + "System"] = EditorGUILayout.Foldout(isFoldout, "System");
                    GUILayout.EndHorizontal();
                    if (m_FoldoutDic[stage.StageName + "System"])
                    {
                        for (int i = 0; i < stage.SystemTypes.Count; i++)
                        {
                            var systemType = stage.SystemTypes[i];
                            GUILayout.BeginHorizontal();
                            GUILayout.Space(40);
                            GUILayout.Label(systemType.Name, GUILayout.Width(350));
                            GUILayout.Label((DebugApi.GetProfilerTimeNs(systemType.Name) / 1000000f).ToString() + "ms", GUILayout.Width(150));
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
