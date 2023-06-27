using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BbxCommon;
using BbxCommon.Ui;

namespace Cin.Ui
{
    public abstract class UiDataShowerViewBase : UiViewBase
    {
        public GameObject PointPrefab;
        public GameObject DataPanel;
        public TextMeshProUGUI MinValueText;
        public TextMeshProUGUI MaxValueText;

        public float TimeRange = 5f;
    }
}
