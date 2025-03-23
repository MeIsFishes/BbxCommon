using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Sirenix.OdinInspector;
using BbxCommon;
using Dcg;

[CreateAssetMenu(fileName = "HitFlyData", menuName = "Dcg/HitFlyData")]
public class HitFlyData : BbxScriptableObject
{
    public float Force;
    public float RandomAngle;

    protected override void OnLoad()
    {
        DataApi.SetData(this);
    }

    protected override void OnUnload()
    {
        DataApi.ReleaseData<HitFlyData>(this);
    }
}
