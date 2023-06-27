using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;

namespace Cin
{
    public class MainCameraSingletonRawComponent : EcsSingletonRawComponent
    {
        public UiModelVariable<float> PosX = new();
        public UiModelVariable<float> PosY = new();
        public UiModelVariable<float> PosZ = new();
        public UiModelVariable<float> RotX = new();
        public UiModelVariable<float> RotY = new();
        public UiModelVariable<float> RotZ = new();
        public UiModelVariable<float> RotW = new();
    }
}
