using Dev.Core.Ui.Assets;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace Dev.Core.Ui.Commands
{
    public class LoadPanelCommand : PanelCommand
    {
        [Inject] private AssetsProvider m_assetsProvider;
        
        public LoadPanelCommand(string panelGuid)
        {
            PanelId = panelGuid;
        }

        protected override async void Start()
        {
            var handle = m_assetsProvider.LoadByGuid<GameObject>(PanelId);
            await handle.Task;
            
            OnCompleted(handle.Status == AsyncOperationStatus.Succeeded && !Canceled);
        }

        public class Factory : PlaceholderFactory<string, LoadPanelCommand>
        {
            
        }
    }
}