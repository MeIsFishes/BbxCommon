using System;
using System.Collections.Generic;
using UnityEngine;

namespace BbxCommon
{
    public abstract class InteractorSetterBase<TEnum> : MonoBehaviour where TEnum : Enum
    {
        public List<TEnum> Flags = new List<TEnum>();

        private void Awake()
        {
            GetComponent<Interactor>().SetInteractFlags(Flags);
        }
    }
}
