using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;

namespace Dcg
{
    public class SpawnRoomShowRawAspect : EcsRawAspect
    {
        public Vector3 Position
        {
            get { return m_Transform.position; }
            set { m_Transform.position = value; }
        }

        public float ElapsedTime
        {
            get { return m_SpawnRoomShowComp.ElapsedTime; }
            set { m_SpawnRoomShowComp.ElapsedTime = value; }
        }

        public AnimationCurve TransitionCurve => m_RoomData.TransitionCurve;
        public float TransitionLength => m_RoomData.TransitionCurve.keys[m_RoomData.TransitionCurve.length - 1].time;

        public Vector3 OriginalPos => m_SpawnRoomShowComp.OriginalPos;
        public Vector3 Offset => m_RoomData.SpawnOffset;

        private static RoomData m_RoomData;

        private SpawnRoomShowRawComponent m_SpawnRoomShowComp;
        private Transform m_Transform;

        protected override void CreateAspect()
        {
            m_RoomData = DataApi.GetData<RoomData>();
            m_SpawnRoomShowComp = GetRawComponent<SpawnRoomShowRawComponent>();
            m_Transform = GetGameObjectComponent<Transform>();
        }
    }
}
