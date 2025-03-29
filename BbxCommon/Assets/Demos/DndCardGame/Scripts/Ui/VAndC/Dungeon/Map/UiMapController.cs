using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg.Ui
{
    public class UiMapController : UiControllerBase<UiMapView>
    {
        
        private ListenableItemListener m_DicesInHandRefreshListener;

        protected override void OnUiInit()
        {
            m_DicesInHandRefreshListener = ModelWrapper.CreateListener(EControllerLifeCycle.Open, (int)MapSingletonRawComponent.EUiEvent.MapInit, InitMap);
            InitMap();
        }

        protected override void OnUiClose()
        {
            m_View.MapRoomList.ClearItems();
        }
        
        private void InitMap()
        {
            var mapData = EcsApi.GetSingletonRawComponent<MapSingletonRawComponent>();
            m_View.MapRoomList.ItemWrapper.ClearItems();
            foreach (var layerRooms in mapData.MapRooms)
            {
                foreach (var room in layerRooms)
                {
                    
                    var diceController = m_View.MapRoomList.ItemWrapper.AddItem<UiMapRoomController>();
                }
            }
        }
    }
}
