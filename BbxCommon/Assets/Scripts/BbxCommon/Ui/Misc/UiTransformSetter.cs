using System;
using System.Collections.Generic;
using UnityEngine;

namespace BbxCommon.Ui
{
    /// <summary>
    /// <para>
    /// Sometimes there will be multiple items try to modify <see cref="Transform"/> variables and one's
    /// request may conflict others'. So the <see cref="UiTransformSetter"/> is for setting variables in
    /// <see cref="Transform"/> by check the requests' priority of those ones reach in a same frame.
    /// </para><para>
    /// Modifying request will exist until be removed or you can call the functions like <see cref="SetPositionOnce(Vector3, int)"/>
    /// to add a one-frame request. That is for the cases, for example, I'm dragging a UI item which is
    /// in a <see cref="UiList"/>, and once I release, it will turn back to the position that the
    /// <see cref="UiList"/> set it.
    /// </para>
    /// </summary>
    public class UiTransformSetter : MonoBehaviour, IUiPreInitRemove, IUiPreInit, IUiUpdate, IUiOpen, IUiClose
    {
        #region Wrapper
        [HideInInspector]
        public PosWpData PosWrapper;
        [HideInInspector]
        public RotWpData RotWrapper;
        [HideInInspector]
        public ScaleWpData ScaleWrapper;

        [Serializable]
        public struct PosWpData
        {
            [SerializeField]
            private UiTransformSetter m_Ref;

            public PosWpData(UiTransformSetter obj) { m_Ref = obj; }
            /// <summary>
            /// Add a request which will exist until be removed.
            /// </summary>
            public void AddPositionRequest(Vector3 position, int priority) => m_Ref.AddPositionRequest(position, priority);
            /// <summary>
            /// Add a request which will exist until be removed.
            /// </summary>
            public void AddPositionRequest<T>(Vector3 position, T priority) where T : Enum => m_Ref.AddPositionRequest(position, priority.GetHashCode());
            /// <summary>
            /// Add a request which will exist until be removed.
            /// </summary>
            internal void AddPositionRequest(Vector3 position, EPosPriority priority) => m_Ref.AddPositionRequest(position, (int)priority);
            public void RemovePositionRequest(int priority) => m_Ref.RemovePositionRequest(priority);
            public void RemovePositionRequest<T>(T priority) where T : Enum => m_Ref.RemovePositionRequest(priority.GetHashCode());
            internal void RemovePositionRequest(EPosPriority priority) => m_Ref.RemovePositionRequest((int)priority);
            /// <summary>
            /// Add a request which exists one frame only.
            /// </summary>
            public void SetPositionOnce(Vector3 position, int priority) => m_Ref.SetPositionOnce(position, priority);
            /// <summary>
            /// Add a request which exists one frame only.
            /// </summary>
            public void SetPositionOnce<T>(Vector3 position, T priority) where T : Enum => m_Ref.SetPositionOnce(position, priority.GetHashCode());
            /// <summary>
            /// Add a request which exists one frame only.
            /// </summary>
            internal void SetPositionOnce(Vector3 position, EPosPriority priority) => m_Ref.SetPositionOnce(position, (int)priority);
        }

        public struct RotWpData
        {
            [SerializeField]
            private UiTransformSetter m_Ref;

            public RotWpData(UiTransformSetter obj) { m_Ref = obj; }
            /// <summary>
            /// Add a request which will exist until be removed.
            /// </summary>
            public void AddRotationRequest(Quaternion rotation, int priority) => m_Ref.AddRotationRequest(rotation, priority);
            /// <summary>
            /// Add a request which will exist until be removed.
            /// </summary>
            public void AddRotationRequest<T>(Quaternion rotation, T priority) where T : Enum => m_Ref.AddRotationRequest(rotation, priority.GetHashCode());
            /// <summary>
            /// Add a request which will exist until be removed.
            /// </summary>
            internal void AddRotationRequest(Quaternion rotation, EScalePriority priority) => m_Ref.AddRotationRequest(rotation, (int)priority);
            public void RemoveRotationRequest(int priority) => m_Ref.RemoveRotationRequest(priority);
            public void RemoveRotationRequest<T>(T priority) where T : Enum => m_Ref.RemoveRotationRequest(priority.GetHashCode());
            internal void RemoveRotationRequest(EScalePriority priority) => m_Ref.RemoveRotationRequest((int)priority);
            /// <summary>
            /// Add a request which exists one frame only.
            /// </summary>
            public void SetRotationOnce(Quaternion rotation, int priority) => m_Ref.SetRotationOnce(rotation, priority);
            /// <summary>
            /// Add a request which exists one frame only.
            /// </summary>
            public void SetRotationOnce<T>(Quaternion rotation, T priority) where T : Enum => m_Ref.SetRotationOnce(rotation, priority.GetHashCode());
            /// <summary>
            /// Add a request which exists one frame only.
            /// </summary>
            internal void SetRotationOnce(Quaternion rotation, EScalePriority priority) => m_Ref.SetRotationOnce(rotation, (int)priority);
        }

        public struct ScaleWpData
        {
            [SerializeField]
            private UiTransformSetter m_Ref;

            public ScaleWpData(UiTransformSetter obj) { m_Ref = obj; }
            /// <summary>
            /// Add a request which will exist until be removed.
            /// </summary>
            public void AddScaleRequest(Vector3 scale, int priority) => m_Ref.AddScaleRequest(scale, priority);
            /// <summary>
            /// Add a request which will exist until be removed.
            /// </summary>
            public void AddScaleRequest<T>(Vector3 scale, T priority) where T : Enum => m_Ref.AddScaleRequest(scale, priority.GetHashCode());
            /// <summary>
            /// Add a request which will exist until be removed.
            /// </summary>
            internal void AddScaleRequest(Vector3 scale, EScalePriority priority) => m_Ref.AddScaleRequest(scale, (int)priority);
            public void RemoveScaleRequest(int priority) => m_Ref.RemoveScaleRequest(priority);
            public void RemoveScaleRequest<T>(T priority) where T : Enum => m_Ref.RemoveScaleRequest(priority.GetHashCode());
            internal void RemoveScaleRequest(EScalePriority priority) => m_Ref.RemoveScaleRequest((int)priority);
            /// <summary>
            /// Add a request which exists one frame only.
            /// </summary>
            public void SetScaleOnce(Vector3 scale, int priority) => m_Ref.SetScaleOnce(scale, priority);
            /// <summary>
            /// Add a request which exists one frame only.
            /// </summary>
            public void SetScaleOnce<T>(Vector3 scale, T priority) where T : Enum => m_Ref.SetScaleOnce(scale, priority.GetHashCode());
            /// <summary>
            /// Add a request which exists one frame only.
            /// </summary>
            internal void SetScaleOnce(Vector3 scale, EScalePriority priority) => m_Ref.SetScaleOnce(scale, (int)priority);
        }
        #endregion

        #region Common
        [SerializeField]
        [Tooltip("To avoid redundant UiTransformSetters, all UiTransformSetters willbe removed by <see cref=\"UiViewBase.PreUiInit\"/>," +
            "unless you check DontRemove.")]
        internal bool DontRemove;
        bool IUiPreInitRemove.DontRemove => DontRemove;

        bool IUiPreInit.OnUiPreInit(UiViewBase uiView)
        {
            PosWrapper = new PosWpData(this);
            RotWrapper = new RotWpData(this);
            ScaleWrapper = new ScaleWpData(this);
            return true;
        }

        void IUiUpdate.OnUiUpdate(UiControllerBase uiController, float deltaTime)
        {
            OnUiUpdatePos();
            OnUiUpdateRot();
            OnUiUpdateScale();
        }

        void IUiOpen.OnUiOpen(UiControllerBase uiController)
        {
            OnUiOpenPos();
            OnUiOpenRot();
            OnUiOpenScale();
        }

        void IUiClose.OnUiClose(UiControllerBase uiController)
        {
            OnUiClosePos();
            OnUiCloseRot();
            OnUiCloseScale();
        }
        #endregion

        #region Position
        /// <summary>
        /// Only provide to the BbxCommon library items.
        /// We don't encourage calling internal functions and types from high-level.
        /// If you want to use <see cref="UiTransformSetter"/>, sign priority with int or other enums insdead.
        /// And there has reserved interval scope in the enum <see cref="EPosPriority"/> for extension.
        /// Higher priority will be accepted.
        /// </summary>
        internal enum EPosPriority
        {
            Normal = 0,
            Tween = 1000,
            Drag = 2000,
        }

        private struct PosRequest
        {
            public int Priority;
            public Vector3 Position;

            public PosRequest(int priority, Vector3 position)
            {
                Priority = priority;
                Position = position;
            }
        }

        // the requests which will exist until be removed
        private List<PosRequest> m_PosRequests = new();
        // the requests which will be removed at the end of frame
        private List<PosRequest> m_TempPosRequest = new();

        private void OnUiOpenPos()
        {
            SimplePool.Alloc(out m_PosRequests);
            SimplePool.Alloc(out m_TempPosRequest);
        }

        private void OnUiUpdatePos()
        {
            int priority = int.MinValue;
            for (int i = 0; i < m_PosRequests.Count; i++)
            {
                if (m_PosRequests[i].Priority >= priority)
                {
                    priority = m_PosRequests[i].Priority;
                    transform.localPosition = m_PosRequests[i].Position;
                }
            }
            // if there is a temporary request with the same priority, apply the temporary one first
            for (int i = 0; i < m_TempPosRequest.Count; i++)
            {
                if (m_TempPosRequest[i].Priority >= priority)
                {
                    priority = m_TempPosRequest[i].Priority;
                    transform.localPosition = m_TempPosRequest[i].Position;
                }
            }
            m_TempPosRequest.Clear();
        }

        private void OnUiClosePos()
        {
            m_PosRequests.CollectToPool();
            m_TempPosRequest.CollectToPool();
        }

        public void AddPositionRequest(Vector3 position, int priority)
        {
            for (int i = 0; i < m_PosRequests.Count; i++)
            {
                if (m_PosRequests[i].Priority == priority)
                {
                    m_PosRequests[i] = new PosRequest(priority, position);
                    return;
                }
            }
            m_PosRequests.Add(new PosRequest(priority, position));
        }

        public void RemovePositionRequest(int priority)
        {
            for (int i = 0; i < m_PosRequests.Count; i++)
            {
                if (m_PosRequests[i].Priority == priority)
                {
                    m_PosRequests.RemoveAt(i);
                    return;
                }
            }
        }

        public void SetPositionOnce(Vector3 position, int priority)
        {
            for (int i = 0; i < m_TempPosRequest.Count; i++)
            {
                if (m_TempPosRequest[i].Priority == priority)
                {
                    m_TempPosRequest[i] = new PosRequest(priority, position);
                    return;
                }
            }
            m_TempPosRequest.Add(new PosRequest(priority, position));
        }
        #endregion

        #region Rotation
        /// <summary>
        /// Only provide to the BbxCommon library items.
        /// We don't encourage calling internal functions and types from high-level.
        /// If you want to use <see cref="UiTransformSetter"/>, sign priority with int or other enums insdead.
        /// And there has reserved interval scope in the enum <see cref="ERotPriority"/> for extension.
        /// Higher priority will be accepted.
        /// </summary>
        internal enum ERotPriority
        {
            Normal = 0,
            Tween = 1000,
        }

        private struct RotRequest
        {
            public int Priority;
            public Quaternion Rotation;

            public RotRequest(int priority, Quaternion rotation)
            {
                Priority = priority;
                Rotation = rotation;
            }
        }

        // the requests which will exist until be removed
        private List<RotRequest> m_RotRequests = new();
        // the requests which will be removed at the end of frame
        private List<RotRequest> m_TempRotRequest = new();

        private void OnUiOpenRot()
        {
            SimplePool.Alloc(out m_RotRequests);
            SimplePool.Alloc(out m_TempPosRequest);
        }

        private void OnUiUpdateRot()
        {
            int priority = int.MinValue;
            for (int i = 0; i < m_RotRequests.Count; i++)
            {
                if (m_RotRequests[i].Priority >= priority)
                {
                    priority = m_RotRequests[i].Priority;
                    transform.localRotation = m_RotRequests[i].Rotation;
                }
            }
            // if there is a temporary request with the same priority, apply the temporary one first
            for (int i = 0; i < m_TempRotRequest.Count; i++)
            {
                if (m_TempRotRequest[i].Priority >= priority)
                {
                    priority = m_TempRotRequest[i].Priority;
                    transform.localRotation = m_TempRotRequest[i].Rotation;
                }
            }
            m_TempRotRequest.Clear();
        }

        private void OnUiCloseRot()
        {
            m_PosRequests.CollectToPool();
            m_TempPosRequest.CollectToPool();
        }

        public void AddRotationRequest(Quaternion rotation, int priority)
        {
            for (int i = 0; i < m_RotRequests.Count; i++)
            {
                if (m_RotRequests[i].Priority == priority)
                {
                    m_RotRequests[i] = new RotRequest(priority, rotation);
                    return;
                }
            }
            m_RotRequests.Add(new RotRequest(priority, rotation));
        }

        public void RemoveRotationRequest(int priority)
        {
            for (int i = 0; i < m_RotRequests.Count; i++)
            {
                if (m_RotRequests[i].Priority == priority)
                {
                    m_RotRequests.RemoveAt(i);
                    return;
                }
            }
        }

        public void SetRotationOnce(Quaternion rotation, int priority)
        {
            for (int i = 0; i < m_TempRotRequest.Count; i++)
            {
                if (m_TempRotRequest[i].Priority == priority)
                {
                    m_TempRotRequest[i] = new RotRequest(priority, rotation);
                    return;
                }
            }
            m_TempRotRequest.Add(new RotRequest(priority, rotation));
        }
        #endregion

        #region Scale
        /// <summary>
        /// Only provide to the BbxCommon library items.
        /// We don't encourage calling internal functions and types from high-level.
        /// If you want to use <see cref="UiTransformSetter"/>, sign priority with int or other enums insdead.
        /// And there has reserved interval scope in the enum <see cref="EScalePriority"/> for extension.
        /// Higher priority will be accepted.
        /// </summary>
        internal enum EScalePriority
        {
            Normal = 0,
            Tween = 1000,
        }

        private struct ScaleRequest
        {
            public int Priority;
            public Vector3 Scale;

            public ScaleRequest(int priority, Vector3 scale)
            {
                Priority = priority;
                Scale = scale;
            }
        }

        // the requests which will exist until be removed
        private List<ScaleRequest> m_ScaleRequests = new();
        // the requests which will be removed at the end of frame
        private List<ScaleRequest> m_TempScaleRequest = new();

        private void OnUiOpenScale()
        {
            SimplePool.Alloc(out m_ScaleRequests);
            SimplePool.Alloc(out m_TempScaleRequest);
        }

        private void OnUiUpdateScale()
        {
            int priority = int.MinValue;
            for (int i = 0; i < m_ScaleRequests.Count; i++)
            {
                if (m_ScaleRequests[i].Priority >= priority)
                {
                    priority = m_ScaleRequests[i].Priority;
                    transform.localScale = m_ScaleRequests[i].Scale;
                }
            }
            // if there is a temporary request with the same priority, apply the temporary one first
            for (int i = 0; i < m_TempScaleRequest.Count; i++)
            {
                if (m_TempScaleRequest[i].Priority >= priority)
                {
                    priority = m_TempScaleRequest[i].Priority;
                    transform.localScale = m_TempScaleRequest[i].Scale;
                }
            }
            m_TempScaleRequest.Clear();
        }

        private void OnUiCloseScale()
        {
            m_ScaleRequests.CollectToPool();
            m_TempScaleRequest.CollectToPool();
        }

        public void AddScaleRequest(Vector3 scale, int priority)
        {
            for (int i = 0; i < m_ScaleRequests.Count; i++)
            {
                if (m_ScaleRequests[i].Priority == priority)
                {
                    m_ScaleRequests[i] = new ScaleRequest(priority, scale);
                    return;
                }
            }
            m_ScaleRequests.Add(new ScaleRequest(priority, scale));
        }

        public void RemoveScaleRequest(int priority)
        {
            for (int i = 0; i < m_ScaleRequests.Count; i++)
            {
                if (m_ScaleRequests[i].Priority == priority)
                {
                    m_ScaleRequests.RemoveAt(i);
                    return;
                }
            }
        }

        public void SetScaleOnce(Vector3 scale, int priority)
        {
            for (int i = 0; i < m_TempScaleRequest.Count; i++)
            {
                if (m_TempScaleRequest[i].Priority == priority)
                {
                    m_TempScaleRequest[i] = new ScaleRequest(priority, scale);
                    return;
                }
            }
            m_TempScaleRequest.Add(new ScaleRequest(priority, scale));
        }
        #endregion
    }
}
