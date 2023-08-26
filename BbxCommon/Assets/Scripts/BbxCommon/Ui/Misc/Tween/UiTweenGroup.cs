using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace BbxCommon.Ui
{
    /// <summary>
    /// A group offers you to operate a set of <see cref="UiTweenBase"/>s. Notice that once you add a <see cref="UiTweenBase"/>
    /// to a <see cref="UiTweenGroup"/>, don't operate the single <see cref="UiTweenBase"/> separately any more, or it may cause
    /// a bug!
    /// </summary>
    [Serializable]
    public class UiTweenGroup : MonoBehaviour, IUiPreInit, IUiUpdate
    {
        #region Wrapper
        [HideInInspector]
        public WpData Wrapper;

        [Serializable]
        public struct WpData
        {
            [SerializeField]
            private UiTweenGroup m_Ref;

            public WpData(UiTweenGroup obj) { m_Ref = obj; }

            public bool Finished => m_Ref.Finished;
            public UnityAction OnPlayFinishes { get { return m_Ref.OnPlayFinishes; } set { m_Ref.OnPlayFinishes = value; } }
            public UnityAction OnPlayReverseFinishes { get { return m_Ref.OnPlayReverseFinishes; } set { m_Ref.OnPlayReverseFinishes = value; } }
            public void Play() => m_Ref.Play();
            public void PlayReverse() => m_Ref.PlayReverse();
            public void Stop() => m_Ref.Stop();
            public void Pause() => m_Ref.Pause();
            public void Continue() => m_Ref.Continue();
        }
        #endregion

        #region Common
        private enum EPlay
        {
            None,
            Play,
            PlayReverse,
        }

        public List<UiTweenBase> Tweens = new();
        public bool Finished => m_PlayState == EPlay.None || m_ElapsedTime >= m_Duration;
        public UnityAction OnPlayFinishes;
        public UnityAction OnPlayReverseFinishes;

        private EPlay m_PlayState;
        private float m_ElapsedTime;
        private float m_Duration;
        private bool m_Paused;

        bool IUiPreInit.OnUiPreInit(UiViewBase uiView)
        {
            Wrapper = new WpData(this);
            return true;
        }

        void IUiUpdate.OnUiUpdate(UiControllerBase uiController, float deltaTime)
        {
            if (m_PlayState == EPlay.None || m_Paused)
                return;

            m_ElapsedTime += deltaTime;
            switch (m_PlayState)
            {
                case EPlay.Play:
                    foreach (var tween in Tweens)
                    {
                        tween.ApplyTime(m_ElapsedTime);
                    }
                    break;
                case EPlay.PlayReverse:
                    foreach (var tween in Tweens)
                    {
                        tween.ApplyTime(m_Duration - m_ElapsedTime);
                    }
                    break;
            }
            if (m_ElapsedTime >= m_Duration)
            {
                switch (m_PlayState)
                {
                    case EPlay.Play:
                        OnPlayFinishes?.Invoke();
                        break;
                    case EPlay.PlayReverse:
                        OnPlayReverseFinishes?.Invoke();
                        break;
                }
                m_PlayState = EPlay.None;
            }
        }

        public void Play()
        {
            CalcDuration();
            m_PlayState = EPlay.Play;
            m_ElapsedTime = 0;
            foreach (var tween in Tweens)
            {
                tween.ApplyTime(m_ElapsedTime);
            }
        }

        public void PlayReverse()
        {
            CalcDuration();
            m_PlayState = EPlay.PlayReverse;
            m_ElapsedTime = 0;
            foreach (var tween in Tweens)
            {
                tween.ApplyTime(m_ElapsedTime);
            }
        }

        public void Stop()
        {
            switch (m_PlayState)
            {
                case EPlay.Play:
                    foreach (var tween in Tweens)
                    {
                        tween.ApplyTime(0);
                    }
                    break;
                case EPlay.PlayReverse:
                    foreach (var tween in Tweens)
                    {
                        tween.ApplyTime(m_Duration);
                    }
                    break;
            }
            m_PlayState = EPlay.None;
        }

        public void Continue()
        {
            m_Paused = false;
        }

        public void Pause()
        {
            m_Paused = true;
        }

        private void CalcDuration()
        {
            m_Duration = 0;
            foreach (var tween in Tweens)
            {
                if (tween.Duration > m_Duration)
                    m_Duration = tween.Duration;
            }
        }
        #endregion

        #region Editor
#if UNITY_EDITOR
        [Button]
        private void SelectTweensBelow()
        {
            var tweens = GetComponentsInChildren<UiTweenBase>();
            foreach (var tween in tweens)
            {
                if (Tweens.Contains(tween) == false)
                    Tweens.Add(tween);
            }
        }
#endif
        #endregion
    }
}
