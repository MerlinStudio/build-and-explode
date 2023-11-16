using Dev.Core.Interfaces;
using UnityEngine;
using Zenject;

namespace Dev.Core.Main
{
    public class GameCoreInitializer : MonoBehaviour, IInitializable
    {
        [Inject]
        private IPlayerLoopFacade playerLoopFacade;
        
        public void Initialize()
        {
            playerLoopFacade.Initialize();
        }
    }
}
