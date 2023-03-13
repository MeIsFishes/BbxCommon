using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace BbxCommon.Ui
{
    public abstract class UiViewBase : MonoBehaviour
    {
        public bool DefaultOpen;

        public abstract string GetResourcePath();

        public abstract Type GetControllerType();

        public abstract int GetUiGroup();
    }
}
