using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;

namespace Cin
{
    public class PlayerBaker : EcsBaker
    {
        public float Speed;

        protected override void Bake()
        {
            var playerComp = AddRawComponent<PlayerSingletonRawComponent>();
            playerComp.Speed = Speed;
            playerComp.SpawnPosition = gameObject.transform.position;
        }
    }
}
