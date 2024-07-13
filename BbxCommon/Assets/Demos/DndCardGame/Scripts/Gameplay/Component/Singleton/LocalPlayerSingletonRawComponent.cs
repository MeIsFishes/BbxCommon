using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;

namespace Dcg
{
    public class LocalPlayerSingletonRawComponent : EcsSingletonRawComponent
    {
        public List<Entity> DungeonEntities = new();
        public List<Entity> CombatEntities = new();

        public void AddDungeonCharacter(Entity entity)
        {
            if (DungeonEntities.Contains(entity))
            {
                Debug.Log("The character has existed in the player's list!");
                return;
            }
            DungeonEntities.Add(entity);
        }

        public void AddCombatCharacter(Entity entity)
        {
            if (CombatEntities.Contains(entity))
            {
                Debug.Log("The character has existed in the player's list!");
                return;
            }
            CombatEntities.Add(entity);
        }

        public void DestroyCharacterEntities()
        {
            foreach (var entity in DungeonEntities)
            {
                entity.Destroy();
            }
            foreach (var entity in CombatEntities)
            {
                entity.Destroy();
            }
            DungeonEntities.Clear();
        }

        public override void OnCollect()
        {
            DungeonEntities.Clear();
        }
    }
}
