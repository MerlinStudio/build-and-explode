using Common.Configs;
using Cysharp.Threading.Tasks;
using Dev.Core.Ui.UI.Manager;
using Game.Utils.Extantions;
using Game.View.Panels;
using Game.View.Widgets;
using State.Creator.Interfaces;
using UnityEngine;
using Zenject;

namespace Common.Creators
{
    public class UiEffectCreator : MonoBehaviour, ICreator
    {
        [Inject] private UiManager m_uiManager;
        [Inject] private EnvironmentInfoConfig m_environmentInfoConfig;

        private Transform m_effectParent;
        private AddBlockEffectWidget m_addBlockEffectWidgetReference;

        public void Init()
        {
        }

        public void DeInit()
        {
        }

        public async UniTask<T> Create<T>(string id) where T : CreatedItem
        {
            if (m_addBlockEffectWidgetReference == null)
            {
                var result  = await AssetReferenceExtension.LoadAssetReferenceAsync(m_environmentInfoConfig.AddBlockEffectWidgetReference);
                m_addBlockEffectWidgetReference = result.GetComponent<AddBlockEffectWidget>();
            }

            if (m_effectParent == null)
            {
                var buildClickerPanel = m_uiManager.TryGetPanel<BuildClickerPanel>();
                m_effectParent = buildClickerPanel.EffectParent;
            }

            var addBlockEffectWidget = Instantiate(m_addBlockEffectWidgetReference, m_effectParent);
            var addBlockEffect = new AddBlockEffect { AddBlockEffectWidget = addBlockEffectWidget };
            return addBlockEffect as T;
        }
    }

    public class AddBlockEffect : CreatedItem
    {
        public AddBlockEffectWidget AddBlockEffectWidget;
    }
}