using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;

namespace Dcg
{
    public class LocalPlayerSingletonRawComponent : EcsSingletonRawComponent
    {
        public List<Entity> Characters = new();

        public void AddCharacter(Entity entity)
        {
            if (Characters.Contains(entity))
            {
                Debug.Log("The character has existed in the player's list!");
                return;
            }
            Characters.Add(entity);
        }

        public void DestroyCharacterEntities()
        {
            foreach (var entity in Characters)
            {
                entity.Destroy();
            }
        }

        public override void OnCollect()
        {
            Characters.Clear();
        }
    }
}
