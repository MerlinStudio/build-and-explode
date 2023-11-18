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
        public event Action<int, int, float> EventAddBomb;
        public event Action<float> EventBuildingLayerChange;

        [SerializeField] private Button m_explosionButton;
        [SerializeField] private Button m_addBombButton;
        [SerializeField] private Slider m_radiusSlider;
        [SerializeField] private Slider m_forceSlider;
        [SerializeField] private Slider m_delaySlider;
        [SerializeField] private Slider m_buildingLayerSlider;
        [SerializeField] private TextMeshProUGUI m_radiusText;
        [SerializeField] private TextMeshProUGUI m_forceText;
        [SerializeField] private TextMeshProUGUI m_delayText;

        private int m_maxRadius = 30;
        private int m_maxForce = 15;
        private int m_maxDelay = 3;

        public override void ShowPanel(bool showInstant = false)
        {
            base.ShowPanel(showInstant);
            m_explosionButton.onClick.AddListener(OnExplosionButtonClick);
            m_addBombButton.onClick.AddListener(OnAddBombButtonClick);
            m_radiusSlider.onValueChanged.AddListener(OnRadiusChange);
            m_forceSlider.onValueChanged.AddListener(OnForceChange);
            m_delaySlider.onValueChanged.AddListener(OnDelayChange);
            m_buildingLayerSlider.onValueChanged.AddListener(OnBuildingLayerChange);
            m_radiusSlider.value = 0.5f;
            m_forceSlider.value = 0.5f;
            m_delaySlider.value = 0;
            m_buildingLayerSlider.value = 0;
        }

        public override void HidePanel(bool hideInstant = false)
        {
            m_explosionButton.onClick.RemoveListener(OnExplosionButtonClick);
            m_addBombButton.onClick.RemoveListener(OnAddBombButtonClick);
            m_radiusSlider.onValueChanged.RemoveListener(OnRadiusChange);
            m_forceSlider.onValueChanged.RemoveListener(OnForceChange);
            m_delaySlider.onValueChanged.RemoveListener(OnDelayChange);
            m_buildingLayerSlider.onValueChanged.RemoveListener(OnBuildingLayerChange);
            base.HidePanel(hideInstant);
        }

        private void OnExplosionButtonClick()
        {
            EventExplosion?.Invoke();
        }
        
        private void OnAddBombButtonClick()
        {
            var radius = CalculateIntValue(m_radiusSlider.value, m_maxRadius);
            var force = CalculateIntValue(m_forceSlider.value, m_maxForce);
            var delay = CalculateIntValue(m_forceSlider.value, m_maxDelay);
            EventAddBomb?.Invoke(radius, force, delay);
        }

        private void OnRadiusChange(float value)
        {
            var radius = CalculateIntValue(value, m_maxRadius);
            m_radiusText.text = $"{radius}";
        }
        
        private void OnForceChange(float value)
        {
            var force = CalculateIntValue(value, m_maxForce);
            m_forceText.text = $"{force}";
        }
        
        private void OnDelayChange(float value)
        {
            var delay = CalculateFloatValue(value, m_maxDelay);
            m_delayText.text = $"{delay}";
        }
        
        private void OnBuildingLayerChange(float value)
        {
            EventBuildingLayerChange?.Invoke(value);
        }

        private int CalculateIntValue(float value, int maxValue)
        {
            var noRoundValue = (value / (1f / maxValue));
            return Mathf.RoundToInt(noRoundValue);
        }
        
        private float CalculateFloatValue(float value, int maxValue)
        {
            var noRoundValue =  value / (1f / maxValue);
            return (float)Math.Round(noRoundValue, 1);
        }
    }
}