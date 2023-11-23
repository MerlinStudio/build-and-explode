using System;
using UnityEngine;

namespace Dev.Core.Ui.UI.Panels
{
    [Serializable]
    public class UIPanelAnimator
    {
        [Serializable]
        public class Config
        {
            [SerializeField, Min(0.01f)] private float m_speed = 1f;
            [SerializeField] private bool m_useInstance = false;

            public float Speed { get => m_speed; set => m_speed = value; }
            public bool UseInstance { get => m_useInstance; set => m_useInstance = value; }
        }

        [SerializeField] private Animator m_animator = null;
        [SerializeField] private Config m_showConfig = null;
        [SerializeField] private Config m_hideConfig = null;

        public Animator Animator => m_animator;
        public Config ShowConfig => m_showConfig;
        public Config HideConfig => m_hideConfig;

        public virtual void PlayShow(bool showInstant = false)
        {
            if (m_animator == null)
            {
                return;
            }

            m_animator.Update(0);
            m_animator.speed = showInstant ? 0 : m_showConfig.Speed > 0 ? m_showConfig.Speed : 0.01f;
            m_animator.SetBool("use_instance", showInstant || m_showConfig.UseInstance);
            m_animator.ResetTrigger("play_hide");
            m_animator.SetTrigger("play_show");
        }

        public virtual void PlayHide(bool hideInstant = false)
        {
            if (m_animator == null)
            {
                return;
            }

            m_animator.Update(0);
            m_animator.speed = hideInstant ? 0 : m_hideConfig.Speed > 0 ? m_hideConfig.Speed : 0.01f;
            m_animator.SetBool("use_instance", hideInstant || m_hideConfig.UseInstance);
            m_animator.ResetTrigger("play_show");
            m_animator.SetTrigger("play_hide");
        }
    }
}