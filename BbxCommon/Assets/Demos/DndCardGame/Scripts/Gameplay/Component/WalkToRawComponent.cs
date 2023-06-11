using UnityEngine;
using BbxCommon;

namespace Dcg
{
    public class WalkToRawComponent : EcsRawComponent
    {
        public struct RequestData
        {
            public bool Finished;
            public Vector3 Destination;
        }

        public RequestData Request = new() { Finished = true };

        public void AddRequest(Vector3 destination)
        {
            Request.Finished = false;
            Request.Destination = destination;
        }
    }
}