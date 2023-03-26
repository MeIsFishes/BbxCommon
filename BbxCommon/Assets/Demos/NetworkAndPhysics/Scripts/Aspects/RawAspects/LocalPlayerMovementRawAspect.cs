using UnityEngine;
using BbxCommon.Framework;

namespace Nnp
{
    public class LocalPlayerMovementRawAspect : EcsRawAspect
    {
        public Vector3 DesiredDirection => m_InputComp.MovementDirection;
        public Vector3 Forward
        {
            get { return m_Transform.forward; }
            set { m_Transform.forward = value; }
        }
        public float WalkSpeed => m_AttributesComp.WalkSpeed;
        public float RunSpeed => m_AttributesComp.RunSpeed;
        public PlayerRawComponent.EPlayerState CurrentState
        {
            get { return m_PlayerComp.CurrentState; }
            set { m_PlayerComp.CurrentState = value; }
        }
        public CharacterController CharacterController;

        private InputSingletonRawComponent m_InputComp;
        private Transform m_Transform;
        private AttributesRawComponent m_AttributesComp;
        private PlayerRawComponent m_PlayerComp;

        protected override void CreateAspect()
        {
            m_InputComp = GetSingletonRawComponent<InputSingletonRawComponent>();
            m_Transform = GetGameObjectComponent<Transform>();
            m_AttributesComp = GetRawComponent<AttributesRawComponent>();
            m_PlayerComp = GetRawComponent<PlayerRawComponent>();
            CharacterController = GetGameObjectComponent<CharacterController>();
        }
    }
}
