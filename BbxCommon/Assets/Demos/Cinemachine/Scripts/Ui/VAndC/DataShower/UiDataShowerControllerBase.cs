using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BbxCommon;
using BbxCommon.Ui;

namespace Cin.Ui
{
    /// <summary>
    /// A base UI item for showing datas on the panel based on their times and values.
    /// </summary>
    public abstract class UiDataShowerControllerBase<T> : UiControllerBase<T> where T : UiDataShowerViewBase
    {
        #region Show Point
        public struct PointData
        {
            public float Time;
            public float Value;

            public PointData(float time, float value)
            {
                Time = time;
                Value = value;
            }
        }

        private bool m_AddedPoint;
        private List<PointData> m_Points = new();
        private List<GameObject> m_PointObjs = new();
        private float m_MaxValue;
        private float m_MinValue;
        private float m_MinTime;
        private float m_MaxTime;

        protected override void OnUpdate()
        {
            foreach (var pointObj in m_PointObjs)
            {
                Destroy(pointObj);
            }
            m_PointObjs.Clear();

            m_View.MaxValueText.text = m_MaxValue.ToString();
            m_View.MinValueText.text = m_MinValue.ToString();

            m_MaxTime = Time.realtimeSinceStartup;
            m_MinTime = m_MaxTime - m_View.TimeRange;

            // remove out-of-time points
            while (TryRemoveFront()) ;

            // use the last value if it is not dirty
            if (m_AddedPoint == false && m_Points.Count > 0)
                AddPoint(new PointData(Time.realtimeSinceStartup, m_Points[m_Points.Count - 1].Value));
            m_AddedPoint = false;

            ShowPoints();
        }

        private void ShowPoints()
        {
            // keep point objects' count equals to the point datas'
            while (m_PointObjs.Count > m_Points.Count)
            {
                Destroy(m_PointObjs.GetBack());
                m_PointObjs.RemoveBack();
            }
            while (m_PointObjs.Count < m_Points.Count)
            {
                var pointObj = Instantiate(m_View.PointPrefab, this.transform);
                m_PointObjs.Add(pointObj);
            }

            var rectTransform = (RectTransform)m_View.DataPanel.transform;
            float xMin = rectTransform.rect.xMin + rectTransform.localPosition.x;
            float xMax = rectTransform.rect.xMax + rectTransform.localPosition.x;
            float yMin = rectTransform.rect.yMin + rectTransform.localPosition.y;
            float yMax = rectTransform.rect.yMax + rectTransform.localPosition.y;
            for (int i = 0; i < m_Points.Count; i++)
            {
                var point = m_Points[i];
                float x = xMin;
                if (m_MaxTime - m_MinTime != 0)
                    x = xMin + ((point.Time - m_MinTime) / (m_MaxTime - m_MinTime)) * (xMax - xMin);
                float y = yMin;
                if (m_MaxValue - m_MinValue != 0)
                    y = yMin + ((point.Value - m_MinValue) / (m_MaxValue - m_MinValue)) * (yMax - yMin);
                ((RectTransform)m_PointObjs[i].transform).localPosition = new Vector3(x, y, 0);
            }
        }

        private void AddPoint(PointData point)
        {
            m_Points.Add(point);
            if (point.Value > m_MaxValue)
                m_MaxValue = point.Value;
            if (point.Value < m_MinValue)
                m_MinValue = point.Value;
        }

        private bool TryRemoveFront()
        {
            if (m_Points.Count == 0)
                return false;
            if (m_Points[0].Time >= m_MinTime)
                return false;

            var value = m_Points[0].Value;
            m_Points.RemoveFront();
            if (m_MaxValue == value)
                m_MaxValue = GetMaxValue();
            if (m_MinValue == value)
                m_MinValue = GetMinValue();
            return true;
        }

        private float GetMaxValue()
        {
            if (m_Points.Count == 0)
                return 0;

            var maxValue = m_MinValue;
            foreach (var point in m_Points)
            {
                if (point.Value > maxValue)
                    maxValue = point.Value;
            }
            return maxValue;
        }

        private float GetMinValue()
        {
            if (m_Points.Count == 0)
                return 0;

            var minValue = m_MaxValue;
            foreach (var point in m_Points)
            {
                if (point.Value < minValue)
                    minValue = point.Value;
            }
            return minValue;
        }
        #endregion

        #region Register Listener
        public void RegisterDataShowerListener(IUiModelItem target)
        {
            AddUiModelVariableListener(EControllerLifeCycle.Open, target, EUiModelVariableEvent.Dirty, OnListeningTargetDirty);
        }

        private void OnListeningTargetDirty(MessageDataBase messageData)
        {
            var value = ((VariableDirtyMessageData<float>)messageData).CurValue;
            AddPoint(new PointData(Time.realtimeSinceStartup, value));
            m_AddedPoint = true;
        }
        #endregion

        #region Callback
        public abstract void OnTargetInited();
        #endregion
    }
}
