using UnityEngine;
using Unity.Entities;
using BbxCommon.Framework;

namespace Nnp
{
    [DisableAutoCreation]
    public partial class LocalPlayerMovementSystem : EcsMixSystemBase
    {
        protected override void OnUpdate()
        {
            var inputComp = GetSingletonRawComponent<InputSingletonRawComponent>();
            var localPlayerComp = GetSingletonRawComponent<LocalPlayerSingletonRawComponent>();
            var gameObject = localPlayerComp.Entity.GetRawComponent<GameObjectRawComponent>().GameObject;
            gameObject.GetComponent<CharacterController>().SimpleMove((inputComp.MovementDirection + Vector3.down) * 5);
        }
    }
}
