using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace VGDevs
{
    public class UIItem : UIItemBase
    {
        [Header("UIItem Settings")] 
        [SerializeField] protected List<UIVisualRoot> m_visualRoots = new List<UIVisualRoot>();
        [SerializeField] protected bool destroyOnHide = true;
        
        protected RectTransform m_rectTransform => transform as RectTransform;

        public List<UIVisualRoot> VisualRoots => m_visualRoots;
        public RectTransform RectTransform => m_rectTransform;

        public event Action OnShow;
        public event Action OnAnimatedShowStart;
        public event Action OnAnimatedHideStart;
        public event Action OnHide;

        private bool m_hidingAnimationActive = false;


        protected override void OnInit()
        {
            if (m_visualRoots.Count == 0)
                throw new Exception($"No visual root found on '{this.name}' gameObject");
        }

        [ContextMenu("AnimatedShow")]
        public virtual void AnimatedShow()
        {
            OnAnimatedShowStart?.Invoke();
            m_visualRoots.Enable();
            m_visualRoots.GetVisualRootsByTriggerType(VisualRootAnimTriggerType.EnterAnimation).StartAnimation(VisualRootAnimTriggerType.EnterAnimation, Show);
        }

        [ContextMenu("AnimatedHide")]
        public virtual void AnimatedHide()
        {
            if (m_hidingAnimationActive)
                return;
            m_hidingAnimationActive = true;
            
            OnAnimatedHideStart?.Invoke();
            m_visualRoots.GetVisualRootsByTriggerType(VisualRootAnimTriggerType.ExitAnimation).StartAnimation(VisualRootAnimTriggerType.ExitAnimation, Hide);
        }

        public virtual void Show()
        {
            m_visualRoots.GetVisualRootsByTriggerType(VisualRootAnimTriggerType.OnEnable).StartAnimation(VisualRootAnimTriggerType.OnEnable);
            m_visualRoots.GetVisualRootsByTriggerType(VisualRootAnimTriggerType.OnEnable).Enable();

            OnShow?.Invoke();
        }

        public virtual void Show(bool includeRoot)
        {
            if (includeRoot)
            {
                gameObject.SetActive(true);
            }

            Show();
        }

        public virtual void Hide()
        {
            m_visualRoots.Disable();

            OnHide?.Invoke();
            m_hidingAnimationActive = false;

            if (destroyOnHide)
            {
                this.SafeDestroy(this.gameObject);
            }
        }
        

        public virtual void Hide(bool includeRoot)
        {
            if (includeRoot)
            {
                gameObject.SetActive(false);
            }

            Hide();
        }

        public virtual void Disable(bool softDisable = false)
        {
            m_visualRoots.Disable(softDisable);
        }

        public virtual void Enable()
        {
            m_visualRoots.Enable();
        }

        protected virtual void OnDestroy()
        {
            m_visualRoots.DOKill();
        }
    }
}
