using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg
{
    public enum EMapRoomType
    {
        None,
        MapEvent,
        MapBattle,
        MapReward,
    }
    public class MapSingletonRawComponent : EcsSingletonRawComponent,IListenable
    {
        public enum EUiEvent
        {
            MapInit,
        }
        
        /// <summary>
        /// 第一维对应房第几层,第二维对应这一层的房间
        /// </summary>
        public List<List<Entity>> MapRooms;

        private MessageHandler<int> m_MessageHandler = new();
        IMessageDispatcher<int> IListenable.MessageDispatcher => m_MessageHandler;

        /// <summary>
        /// 临时填充数据,后面改成从配置中读取
        /// </summary>
        public void InitByConfig()
        {
            MapRooms = new List<List<Entity>>();
            //构造一个212的三层结构
            List<Entity> rooms = new();
            for (int j = 0; j < 2; j++)
            {
                var ent = EcsApi.CreateEntity();
                ent.AddRawComponent<MapRoomRawComponent>();
                MapRoom room = new();
                room.NextRoomLayers = new List<int>();
                room.NextRoomLayers.Add(1);
                room.NextRoomIndices = new List<int>();
                room.NextRoomIndices.Add(0);
                room.Type = EMapRoomType.MapEvent;
                room.Layer = 0;
                room.Index = j;
                rooms.Add(EntityCreator.CreateMapRoomEntity(room));
            }
            var ent1 = EcsApi.CreateEntity();
            ent1.AddRawComponent<MapRoomRawComponent>();
            MapRoom room21 = new();
            room21.NextRoomLayers = new List<int>();
            room21.NextRoomLayers.Add(2);
            room21.NextRoomIndices = new List<int>();
            room21.NextRoomIndices.Add(0);
            room21.NextRoomLayers = new List<int>();
            room21.NextRoomLayers.Add(2);
            room21.NextRoomIndices = new List<int>();
            room21.NextRoomIndices.Add(1);
            room21.Type = EMapRoomType.MapEvent;
            room21.Layer = 1;
            room21.Index = 0;
            rooms.Add(EntityCreator.CreateMapRoomEntity(room21));
            for (int j = 0; j < 2; j++)
            {
                var ent = EcsApi.CreateEntity();
                ent.AddRawComponent<MapRoomRawComponent>();
                MapRoom room = new();
                room.PreviousRoomLayers = new List<int>();
                room.PreviousRoomLayers.Add(1);
                room.PreviousRoomIndices = new List<int>();
                room.PreviousRoomIndices.Add(0);
                room.Type = EMapRoomType.MapEvent;
                room.Layer = 2;
                room.Index = j;
                rooms.Add(EntityCreator.CreateMapRoomEntity(room));
            }
            MapRooms.Add(rooms);
            // 初始化房间
            DispatchEvent(EUiEvent.MapInit);
        }
        
        public void DispatchEvent(EUiEvent e)
        {
            m_MessageHandler.Dispatch((int)e, this);
        }
    
        protected override void OnCollect()
        {
            m_MessageHandler.RemoveAllListeners();
        }
    }
}

