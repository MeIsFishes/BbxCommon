using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;

namespace BbxCommon
{
    [DisableAutoCreation]
    public partial class InputSystem : EcsMixSystemBase
    {
        protected override void OnSystemUpdate()
        {
            SimplePool.Alloc(out Dictionary<int, bool> keyStates);
            InputApi.GetKeyStateRequestDic(keyStates);
            foreach (var pair in keyStates)
            {
                if (Input.GetKeyDown((KeyCode)pair.Key))
                {
                    keyStates[pair.Key] = true;
                }
            }
            InputApi.Tick(keyStates);
            keyStates.CollectToPool();
        }
    }
}
