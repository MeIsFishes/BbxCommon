using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace BbxCommon.Ui
{
    /// <summary>
    /// A group offers you to operate a set of <see cref="UiTweenBase"/>s. Notice that once you add a <see cref="UiTweenBase"/>
    /// to a <see cref="UiTweenGroup"/>, don't operate the single <see cref="UiTweenBase"/> separately any more, or it may cause
    /// a bug!
    /// </summary>
    [Serializable]
    public class UiTweenGroup : PooledObject
    {
        public List<UiTweenBase> Tweens = new();
        public bool Finished => m_LongestTween != null ? m_LongestTween.Finished : true;

        private UiTweenBase m_LongestTween;

        public void Play()
        {
            m_LongestTween = null;
            foreach (var tween in Tweens)
            {
                tween.Play();
                if (m_LongestTween == null || tween.Duration > m_LongestTween.Duration)
                    m_LongestTween = tween;
            }
        }

        public void Stop()
        {
            foreach (var tween in Tweens)
            {
                tween.Stop();
            }
        }

        public void Continue()
        {
            foreach (var tween in Tweens)
            {
                tween.Continue();
            }
        }

        public void Pause()
        {
            foreach (var tween in Tweens)
            {
                tween.Pause();
            }
        }

        public override void OnCollect()
        {
            Tweens.Clear();
        }
    }
}
