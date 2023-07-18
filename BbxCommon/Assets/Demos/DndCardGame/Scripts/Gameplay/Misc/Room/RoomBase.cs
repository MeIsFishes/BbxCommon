using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dcg
{
    public abstract class RoomBase
    {
        public int Level;

        public void Enter()
        {
            OnEnter();
        }

        protected abstract void OnEnter();
    }
}
