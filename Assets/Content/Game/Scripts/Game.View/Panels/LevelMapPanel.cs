using System;
using DanielLochner.Assets.SimpleScrollSnap;
using Dev.Core.Ui.UI.Panels;
using UnityEngine;
using UnityEngine.UI;

namespace Game.View.Panels
{
    public class LevelMapPanel : UIPanel
    {
        public event Action EventSelectLevelIndex;
        
        [SerializeField] private SimpleScrollSnap m_simpleScrollSnap;
        [SerializeField] private Button m_selectLevelButton;

        public SimpleScrollSnap SimpleScrollSnap => m_simpleScrollSnap;

        public override void ShowPanel(bool showInstant = false)
        {
            base.ShowPanel(showInstant);
            m_selectLevelButton.onClick.AddListener(OnSelectLevelButtonClick);
        }

        public override void HidePanel(bool hideInstant = false)
        {
            m_selectLevelButton.onClick.RemoveListener(OnSelectLevelButtonClick);
            base.HidePanel(hideInstant);
        }

        public void SetInteractableButton(bool isInteractable)
        {
            m_selectLevelButton.interactable = isInteractable;
        }

        private void OnSelectLevelButtonClick()
        {
            EventSelectLevelIndex?.Invoke();
        }
    }
}