using UnityEngine;
using BbxCommon.Ui;

namespace BbxCommon
{
    public class GameTimer
    {
        public virtual float DeltaTime { get; }
        public virtual float GameTime { get; }
    }

    /// <summary>
    /// UI items implemented <see cref="IBbxUiItem"/> read time from <see cref="UiTimer"/>. Call <see cref="SetTimer(GameTimer)"/> if you need.
    /// </summary>
    public static class UiTimer
    {
        private static GameTimer m_GameTimer;

        public static void SetTimer(GameTimer item)
        {
            m_GameTimer = item;
        }

        public static float DeltaTime => m_GameTimer == null ? Time.deltaTime : m_GameTimer.DeltaTime;
        public static float GameTime => m_GameTimer == null ? Time.time : m_GameTimer.GameTime;
    }
}
