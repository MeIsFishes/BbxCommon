using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BbxCommon
{
    // Classes inherit this interface should use OnAllocate() as initializing, instead of constructor.
    public interface IPooledObject
    {
        void OnAllocate();
        void OnCollect();
    }
}
