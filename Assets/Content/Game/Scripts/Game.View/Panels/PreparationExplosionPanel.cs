using System;
using Cysharp.Threading.Tasks;
using Dev.Core.Ui.UI.Panels;
using Game.View.Widgets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.View.Panels
{
    public class PreparationExplosionPanel : UIPanel
    {
        public event Action EventExplosionButtonClick;
        public event Action<int, int, float> EventAddBombButtonClick;
        public event Action EventBuyBombButtonClick;
        public event Action<float> EventBuildingLayerChange;

        [SerializeField] private Button m_explosionButton;
        [SerializeField] private Button m_addBombButton;
        [SerializeField] private Button m_buyBombButton;
        [SerializeField] private Slider m_radiusSlider;
        [SerializeField] private Slider m_forceSlider;
        [SerializeField] private Slider m_delaySlider;
        [SerializeField] private Slider m_buildingLayerSlider;
        [SerializeField] private TextMeshProUGUI m_radiusText;
        [SerializeField] private TextMeshProUGUI m_forceText;
        [SerializeField] private TextMeshProUGUI m_delayText;
        [SerializeField] private TextMeshProUGUI m_amountBombsText;
        [SerializeField] private TextMeshProUGUI m_buyAmountBombText;
        [SerializeField] private Transform m_bombSettingsContent;
        [SerializeField] private CountdownExplosionWidget m_countdownExplosionWidget;
        [SerializeField] private AnimationWidget m_bombSettingAnimationWidget;
        [SerializeField] private AnimationWidget m_sliderAnimationWidget;

        private int m_maxRadius = 30;
        private int m_maxForce = 15;
        private int m_maxDelay = 3;

        public override async void ShowPanel(bool showInstant = false)
        {
            base.ShowPanel(showInstant);
            VisibilityBombSettings(false);
            ActiveAddBombButton(true);
            ActiveExplosionButton(false);
            SetDefaultSliderValue();
            
            m_countdownExplosionWidget.Hide();
            var task1 = m_bombSettingAnimationWidget.ShowUniTask();
            var task2 = m_sliderAnimationWidget.ShowUniTask();
            await UniTask.WhenAll(task1, task2);
            m_explosionButton.onClick.AddListener(OnExplosionButtonClick);
            m_addBombButton.onClick.AddListener(OnAddBombButtonClick);
            m_buyBombButton.onClick.AddListener(OnBuyBombButtonClick);
            m_radiusSlider.onValueChanged.AddListener(OnRadiusChange);
            m_forceSlider.onValueChanged.AddListener(OnForceChange);
            m_delaySlider.onValueChanged.AddListener(OnDelayChange);
            m_buildingLayerSlider.onValueChanged.AddListener(OnBuildingLayerChange);
        }

        public override void HidePanel(bool hideInstant = false)
        {
            m_explosionButton.onClick.RemoveListener(OnExplosionButtonClick);
            m_addBombButton.onClick.RemoveListener(OnAddBombButtonClick);
            m_buyBombButton.onClick.RemoveListener(OnBuyBombButtonClick);
            m_radiusSlider.onValueChanged.RemoveListener(OnRadiusChange);
            m_forceSlider.onValueChanged.RemoveListener(OnForceChange);
            m_delaySlider.onValueChanged.RemoveListener(OnDelayChange);
            m_buildingLayerSlider.onValueChanged.RemoveListener(OnBuildingLayerChange);
            base.HidePanel(hideInstant);
        }

        public void VisibilityBombSettings(bool isVisible)
        {
            m_bombSettingsContent.gameObject.SetActive(isVisible);
        }
        
        public void ActiveAddBombButton(bool isActive)
        {
            m_addBombButton.gameObject.SetActive(isActive);
            m_buyBombButton.gameObject.SetActive(!isActive);
        }
        
        public void SetAmountBombs(int amountBomb)
        {
            m_amountBombsText.text = $"{amountBomb}";
            // todo добавить картинки на каждую бомбу
        }

        public void SetBuyAmountBombText(int amountBomb)
        {
            m_buyAmountBombText.text = $"+{amountBomb}";
        }

        public async UniTask PlayCountdownExplosion()
        {
            m_countdownExplosionWidget.Show();
            await m_countdownExplosionWidget.PlayCountdownExplosion();
            m_countdownExplosionWidget.Hide();
            await UniTask.Delay(200);
        }

        private void ActiveExplosionButton(bool isActive)
        {
            if (m_explosionButton.gameObject.activeSelf == isActive)
            {
                return;
            }
            m_explosionButton.gameObject.SetActive(isActive);
        }

        private void OnExplosionButtonClick()
        {
            m_explosionButton.onClick.RemoveListener(OnExplosionButtonClick);
            m_bombSettingAnimationWidget.Hide();
            m_sliderAnimationWidget.Hide();
            EventExplosionButtonClick?.Invoke();
        }
        
        private void OnAddBombButtonClick()
        {
            var radius = CalculateIntValue(m_radiusSlider.value, m_maxRadius);
            var force = CalculateIntValue(m_forceSlider.value, m_maxForce);
            var delay = CalculateIntValue(m_forceSlider.value, m_maxDelay);
            EventAddBombButtonClick?.Invoke(radius, force, delay);
            ActiveExplosionButton(true);
        }
        
        private void OnBuyBombButtonClick()
        {
            EventBuyBombButtonClick?.Invoke();
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

        private void SetDefaultSliderValue()
        {
            m_radiusSlider.value = 0.5f;
            m_forceSlider.value = 0.5f;
            m_delaySlider.value = 0;
            m_buildingLayerSlider.value = 1;

            OnRadiusChange(m_radiusSlider.value);
            OnForceChange(m_forceSlider.value);
            OnDelayChange(m_delaySlider.value);
        }
    }
}