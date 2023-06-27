using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;
using Cin.Ui;

namespace Cin
{
    public class MainCameraBaker : EcsBaker
    {
        protected override void Bake()
        {
            AddRawComponent<MainCameraSingletonRawComponent>();

            UiApi.GetUiController<UiPosXDataShowerController>().OnTargetInited();
            UiApi.GetUiController<UiPosYDataShowerController>().OnTargetInited();
            UiApi.GetUiController<UiPosZDataShowerController>().OnTargetInited();
            UiApi.GetUiController<UiRotXDataShowerController>().OnTargetInited();
            UiApi.GetUiController<UiRotYDataShowerController>().OnTargetInited();
            UiApi.GetUiController<UiRotZDataShowerController>().OnTargetInited();
            UiApi.GetUiController<UiRotWDataShowerController>().OnTargetInited();
        }
    }
}
