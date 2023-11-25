using Common.Configs;
using Data.Builds.Configs;
using Game.View.Panels;
using State.LevelLoader.Interfaces;
using UnityEngine;

namespace State.Explosion.Controllers
{
    public class InspectorBomb
    {
        public InspectorBomb(
            ILevelProvider levelProvider,
            ShopConfig shopConfig,
            PreparationExplosionPanel preparationExplosionPanel)
        {
            m_levelProvider = levelProvider;
            m_shopConfig = shopConfig;
            m_preparationExplosionPanel = preparationExplosionPanel;
        }

        private readonly ILevelProvider m_levelProvider;
        private readonly ShopConfig m_shopConfig;
        private readonly PreparationExplosionPanel m_preparationExplosionPanel;

        private int m_currentAmountBomb;

        public void Init()
        {
            var buildDataConfig = m_levelProvider.GetCurrentBuildDataConfig();
            m_currentAmountBomb = buildDataConfig.AmountBomb;
            m_preparationExplosionPanel.SetAmountBombs(m_currentAmountBomb);
            m_preparationExplosionPanel.SetBuyAmountBombText(m_shopConfig.BuyBombForAd);
            m_preparationExplosionPanel.EventBuyBombButtonClick += OnBuyBombButtonClick;
            m_preparationExplosionPanel.EventAddBombButtonClick += OnAddBombButtonClick;
        }

        public void DeInit()
        {
            m_preparationExplosionPanel.EventBuyBombButtonClick -= OnBuyBombButtonClick;
            m_preparationExplosionPanel.EventAddBombButtonClick -= OnAddBombButtonClick;
        }

        private void OnAddBombButtonClick(int radius, int force, float delay)
        {
            m_currentAmountBomb--;
            if (m_currentAmountBomb <= 0)
            {
                m_preparationExplosionPanel.ActiveAddBombButton(false);
            }
            m_preparationExplosionPanel.SetAmountBombs(m_currentAmountBomb);
        }

        private void OnBuyBombButtonClick()
        {
            Debug.Log("Show reward video successful");

            m_currentAmountBomb += m_shopConfig.BuyBombForAd;
            m_preparationExplosionPanel.SetAmountBombs(m_currentAmountBomb);
            m_preparationExplosionPanel.ActiveAddBombButton(true);
        }
    }
}