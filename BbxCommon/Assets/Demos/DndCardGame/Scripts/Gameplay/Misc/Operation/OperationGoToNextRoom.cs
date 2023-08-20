using BbxCommon;
using UnityEngine;

namespace Dcg
{
    public class OperationGoToNextRoom : BlockedOperationBase
    {
        // 暂时用hard code写死，后面需要做timeline的editor然后转移过去
        private float m_ElapsedTime = 0;
        private float m_FinishTime = 3.5f;

        private float m_WalkTime = 0.5f;
        private bool m_RequestedWalk = false;

        private float m_EnterCombatTime = 3.0f;
        private bool m_RequestedCombat = false;

        protected override void OnEnter()
        {
            EcsApi.GetSingletonRawComponent<DungeonRoomSingletonRawComponent>().RequestSpawnRoom = true;
        }

        protected override EOperationState OnUpdate(float deltaTime)
        {
            m_ElapsedTime += deltaTime;
            // 走到下一个房间
            if (m_ElapsedTime > m_WalkTime && m_RequestedWalk == false)
            {
                var dungeonRoomComp = EcsApi.GetSingletonRawComponent<DungeonRoomSingletonRawComponent>();
                var playerComp = EcsApi.GetSingletonRawComponent<PlayerSingletonRawComponent>();
                var roomData = DataApi.GetData<RoomData>();
                foreach (var character in playerComp.Characters)
                {
                    var walkToComp = character.GetRawComponent<WalkToRawComponent>();
                    walkToComp.AddRequest(dungeonRoomComp.CurRoom.GetGameObject().transform.position + roomData.CharacterOffset);
                    m_RequestedWalk = true;
                }
            }
            // 进入战斗
            if (m_ElapsedTime > m_EnterCombatTime && m_RequestedCombat == false)
            {
                DcgGameEngine.Instance.EnterCombat();
                m_RequestedCombat = true;
            }
            if (m_ElapsedTime > m_FinishTime)
                return EOperationState.Finished;
            return EOperationState.Running;
        }

        protected override void OnExit()
        {
            
        }

        public override void OnCollect()
        {
            base.OnCollect();
            m_ElapsedTime = 0;
            m_RequestedWalk = false;
            m_RequestedCombat = false;
        }
    }
}
