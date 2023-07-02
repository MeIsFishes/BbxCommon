using BbxCommon;

namespace Dcg
{
    public class OperationGoToNextRoom : OperationBase
    {
        // ��ʱ��hard codeд����������Ҫ��timeline��editorȻ��ת�ƹ�ȥ
        private float m_ElapsedTime = 0;
        private float m_FinishTime = 2.5f;
        private float m_WalkTime = 0.5f;
        private bool m_RequestWalk = false;

        protected override void OnEnter()
        {
            EcsApi.GetSingletonRawComponent<DungeonRoomSingletonRawComponent>().RequestSpawnRoom = true;
        }

        protected override EOperationState OnUpdate(float deltaTime)
        {
            m_ElapsedTime += deltaTime;
            // �ߵ���һ������
            if (m_ElapsedTime > m_WalkTime && m_RequestWalk == false)
            {
                var roomComp = EcsApi.GetSingletonRawComponent<DungeonRoomSingletonRawComponent>();
                var playerComp = EcsApi.GetSingletonRawComponent<PlayerSingletonRawComponent>();
                var roomData = DataApi.GetData<RoomData>();
                foreach (var character in playerComp.Characters)
                {
                    var walkToComp = character.GetRawComponent<WalkToRawComponent>();
                    walkToComp.AddRequest(roomComp.CurRoom.transform.position + roomData.CharacterOffset);
                    m_RequestWalk = true;
                }
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
            m_RequestWalk = false;
        }
    }
}
