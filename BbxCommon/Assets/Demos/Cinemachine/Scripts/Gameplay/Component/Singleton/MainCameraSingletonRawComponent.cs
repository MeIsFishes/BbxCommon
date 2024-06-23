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
        public ListenableVariable<float> PosX = new();
        public ListenableVariable<float> PosY = new();
        public ListenableVariable<float> PosZ = new();
        public ListenableVariable<float> RotX = new();
        public ListenableVariable<float> RotY = new();
        public ListenableVariable<float> RotZ = new();
        public ListenableVariable<float> RotW = new();
    }
}
