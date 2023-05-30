using UnityEngine;
using Unity.Entities;
using BbxCommon;

namespace Nnp
{
    [DisableAutoCreation]
    public partial class InputSystem : EcsMixSystemBase
    {
        protected override void OnSystemUpdate()
        {
            var inputComp = GetSingletonRawComponent<InputSingletonRawComponent>();
            inputComp.MovementDirection = new Vector3();
            if (Input.GetKey(KeyCode.W)) inputComp.MovementDirection += new Vector3(0, 0, 1);
            if (Input.GetKey(KeyCode.S)) inputComp.MovementDirection += new Vector3(0, 0, -1);
            if (Input.GetKey(KeyCode.A)) inputComp.MovementDirection += new Vector3(-1, 0, 0);
            if (Input.GetKey(KeyCode.D)) inputComp.MovementDirection += new Vector3(1, 0, 0);
        }
    }
}
