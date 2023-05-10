using UnityEngine;

namespace BbxCommon
{
    public abstract class GameTimerItem
    {
        public abstract float DeltaTime { get; }
        public abstract float ElapsedTime { get; }
    }

    /// <summary>
    /// Scripts in BbxCommon read time from BbxRawTimer. Set GameTimerItem if you need to control time.
    /// As naming it BbxRawTimer, it means the time don't think of time scale and other factors.
    /// </summary>
    public static class BbxRawTimer
    {
        private static GameTimerItem m_s_GameTimerItem;

        public static void SetGameTimerItem(GameTimerItem item)
        {
            m_s_GameTimerItem = item;
        }

        public static float DeltaTime => m_s_GameTimerItem == null ? Time.deltaTime : m_s_GameTimerItem.DeltaTime;
        public static float GameTime => m_s_GameTimerItem == null ? Time.time : m_s_GameTimerItem.ElapsedTime;
    }
}
