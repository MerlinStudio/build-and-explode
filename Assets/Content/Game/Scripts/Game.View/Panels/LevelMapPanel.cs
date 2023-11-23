using System;
using DanielLochner.Assets.SimpleScrollSnap;
using Dev.Core.Ui.UI.Panels;
using UnityEngine;
using UnityEngine.UI;

namespace Game.View.Panels
{
    public class LevelMapPanel : UIPanel
    {
        public event Action<int> EventSelectLevelIndex;
        
        [SerializeField] private SimpleScrollSnap m_simpleScrollSnap;
        [SerializeField] private Button m_selectLevelButton;

        public SimpleScrollSnap SimpleScrollSnap => m_simpleScrollSnap;

        private int m_currentLevelIndex;
        
        public override void ShowPanel(bool showInstant = false)
        {
            base.ShowPanel(showInstant);
            m_selectLevelButton.onClick.AddListener(OnSelectLevelButtonClick);
            m_simpleScrollSnap.OnPanelCentered.AddListener(OnCurrentLevelView);
            m_simpleScrollSnap.EventBeginScrolling += OnBeginScrolling;
        }

        public override void HidePanel(bool hideInstant = false)
        {
            m_selectLevelButton.onClick.RemoveListener(OnSelectLevelButtonClick);
            m_simpleScrollSnap.OnPanelCentered.RemoveListener(OnCurrentLevelView);
            m_simpleScrollSnap.EventBeginScrolling -= OnBeginScrolling;
            base.HidePanel(hideInstant);
        }

        private void OnSelectLevelButtonClick()
        {
            EventSelectLevelIndex?.Invoke(m_currentLevelIndex);
        }
        
        private void OnCurrentLevelView(int current, int next)
        {
            m_currentLevelIndex = current;
            Debug.Log($"{current}");
        }

        private void OnBeginScrolling(bool isBeginScrolling)
        {
            m_selectLevelButton.interactable = !isBeginScrolling;
        }
    }
}