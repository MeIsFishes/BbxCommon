using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BbxCommon
{
    public enum EMessageType
    {
        // For there is only one uint in MessageTubes, it can support only 32 types of message currently.
        None,
        GameSucceed,
        GameFail,
        TaskMoveToFinished,
        CollectionChanged,
        ProjectileChanged,
        GetMedicament,
        PlayerHPChanged,
        TaskFadeToFinished,
        CollectionFinished,
        BeAttacked,
        PlayerDead,
        PlayerProcessBegin,
        PlayerProcessEnd,
        CraftEnd,
        CreatResult,
        MonsterArriveDestination,
        ShowChat,
        PlayerJump,
        PlayerSecJump,
        MixBegin,
        BottleBreak,
        AddBuff,
        BossCome,
    }

    // MessageTubes signs which types of message the object is listening.
    public struct MessageTubes
    {
        // Consider uint as a 32-bits bool array.
        private uint m_Tubes;

        public void OpenAllTubes()
        {
            m_Tubes = uint.MaxValue;
        }

        public void CloseAllTubes()
        {
            m_Tubes = 0;
        }

        public void OpenTube(EMessageType messageType)
        {
            m_Tubes |= (uint)1 << (int)messageType;
        }

        public void CloseTube(EMessageType messageType)
        {
            var num = (uint)1 << (int)messageType;
            m_Tubes -= m_Tubes & num;
        }

        public bool IsTubeOpen(EMessageType messageType)
        {
            var num = (uint)1 << (int)messageType;
            return (m_Tubes & num) != 0;
        }

        public bool IsTypeMatch(uint messageType)
        {
            return (m_Tubes & messageType) == messageType;
        }
    }
}
