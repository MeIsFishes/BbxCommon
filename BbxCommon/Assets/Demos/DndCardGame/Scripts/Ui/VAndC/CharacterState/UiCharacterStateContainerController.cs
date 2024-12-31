using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;
using Dcg;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using UnityEngine.Profiling.Memory.Experimental;

public class UiCharacterStateContainerController : UiControllerBase<UiCharacterStateContainerView>
{
    public enum ECharacterType
    {
        Combat,
        DungeonWalk
    }

    protected override void OnUiInit()
    {

    }

    public void Refresh(ECharacterType type)
    {
        switch (type)
        {
            case ECharacterType.Combat:
                SetData(EcsApi.GetSingletonRawComponent<LocalPlayerSingletonRawComponent>().CombatEntities);
                break;
            case ECharacterType.DungeonWalk:
                SetData(EcsApi.GetSingletonRawComponent<LocalPlayerSingletonRawComponent>().DungeonEntities);
                break;
        }
    }

    private void SetData(List<Entity> entities)
    {
        m_View.UiList.ItemWrapper.ModifyCount<UiCharacterStateItemController>(entities.Count);
        for (int i = 0; i < entities.Count; i++)
        {
            Entity combateEntity = entities[i];
            var item = m_View.UiList.ItemWrapper.GetItem<UiCharacterStateItemController>(i);
            item.SetEntity(combateEntity);
        }
    }

}
