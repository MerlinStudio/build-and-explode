using System;
using Dev.Core.Ui.UI.Panels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Panels
{
    public class PreparationExplosionPanel : UIPanel
    {
        public event Action EventExplosion;
        public event Action<Transform, int, int, float> EventAddBomb;

        [SerializeField] private Button m_explosionButton;
        [SerializeField] private Button m_addBombButton;
        [SerializeField] private Slider m_radiusSlider;
        [SerializeField] private Slider m_forceSlider;
        [SerializeField] private Slider m_delaySlider;
        [SerializeField] private TextMeshProUGUI m_radiusText;
        [SerializeField] private TextMeshProUGUI m_forceText;
        [SerializeField] private TextMeshProUGUI m_delayText;

        private int m_radiusX = 5;
        private int m_forceX = 15;
        private int m_delayX = 2;

        public override void ShowPanel(bool showInstant = false)
        {
            base.ShowPanel(showInstant);
            m_explosionButton.onClick.AddListener(OnExplosionButtonClick);
            m_addBombButton.onClick.AddListener(OnAddBombButtonClick);
            m_radiusSlider.onValueChanged.AddListener(OnRadiusChange);
            m_forceSlider.onValueChanged.AddListener(OnForceChange);
            m_delaySlider.onValueChanged.AddListener(OnForceChange);
        }

        public override void HidePanel(bool hideInstant = false)
        {
            m_explosionButton.onClick.RemoveListener(OnExplosionButtonClick);
            m_addBombButton.onClick.RemoveListener(OnAddBombButtonClick);
            m_radiusSlider.onValueChanged.RemoveListener(OnRadiusChange);
            m_forceSlider.onValueChanged.RemoveListener(OnForceChange);
            m_delaySlider.onValueChanged.RemoveListener(OnForceChange);
            base.HidePanel(hideInstant);
        }

        private void OnExplosionButtonClick()
        {
            EventExplosion?.Invoke();
        }
        
        private void OnAddBombButtonClick()
        {
            var blockTransform = transform;
            var radius = (int)CalculateValue(m_radiusSlider.value, m_radiusX);
            var force = (int)CalculateValue(m_forceSlider.value, m_forceX);
            var delay = (int)CalculateValue(m_forceSlider.value, m_delayX);
            EventAddBomb?.Invoke(blockTransform, radius, force, delay);
        }

        private void OnRadiusChange(float value)
        {
            var radius = (int)CalculateValue(value, m_radiusX);
            m_radiusText.text = $"{radius}";
        }
        
        private void OnForceChange(float value)
        {
            var force = (int)CalculateValue(value, m_forceX);
            m_forceText.text = $"{force}";
        }
        
        private void OnDelayChange(float value)
        {
            var delay = CalculateValue(value, m_delayX);
            m_delayText.text = $"{delay}";
        }

        private float CalculateValue(float value, int x)
        {
            var roundValue = Math.Round(value, 1);
            return (float)(roundValue * 10 + x);
        }
    }
}