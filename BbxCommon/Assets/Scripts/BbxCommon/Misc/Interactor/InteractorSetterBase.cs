using System;
using System.Collections.Generic;
using UnityEngine;

namespace BbxCommon
{
    /// <summary>
    /// For classes in the assembly BbxCommon cannot get enums in trunk, we offer another class
    /// <see cref="InteractorSetterBase{TEnum}"/> to auto set interacting flags to <see cref="Interactor"/>.
    /// You can inherit this class and set the enum as generic type.
    /// </summary>
    public abstract class InteractorSetterBase<TEnum> : MonoBehaviour where TEnum : Enum
    {
        public List<TEnum> Flags = new List<TEnum>();

        private void Awake()
        {
            GetComponent<Interactor>().SetInteractFlags(Flags);
        }
    }
}
