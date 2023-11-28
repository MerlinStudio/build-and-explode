using System.Collections.Generic;
using System.Linq;
using Common.Saves.Controllers;
using Common.Saves.Interfaces;
using Cysharp.Threading.Tasks;
using Data.Builds.Blocks;
using Dev.Core.Ui.UI.Manager;
using Game.Models.Camera.Components;
using Game.Models.Camera.Interfaces;
using Game.Models.Common.Subject;
using Game.View.Panels;
using State.LevelLoader.Interfaces;
using UniRx;
using UnityEngine;

namespace Game.Models.Camera.Controllers
{
    public class CameraController : ICameraController, ICameraProvider
    {
        public CameraController(
            UiManager uiManager,
            TargetGroupCamera targetGroupCamera,
            ILevelProvider levelProvider,
            ISavesProvider savesProvider,
            ISubjectBinder<BlockViewInfo> onBlockCreated,
            ISubjectBinder<int> onBuildLayerHeight)
        {
            m_uiManager = uiManager;
            m_targetGroupCamera = targetGroupCamera;
            m_levelProvider = levelProvider;
            m_savesProvider = savesProvider;
            m_onBlockCreated = onBlockCreated;
            m_onBuildLayerHeight = onBuildLayerHeight;
        }

        private readonly UiManager m_uiManager;
        private readonly TargetGroupCamera m_targetGroupCamera;
        private readonly ILevelProvider m_levelProvider;
        private readonly ISavesProvider m_savesProvider;
        private readonly ISubjectBinder<BlockViewInfo> m_onBlockCreated;
        private readonly ISubjectBinder<int> m_onBuildLayerHeight;

        private CompositeDisposable m_compositeDisposable;
        private CameraControllerPanel m_cameraControllerPanel;
        private int m_currentHeightConstruction;
        private bool m_isActiveRotate;

        public async void Init()
        {
            m_compositeDisposable = new CompositeDisposable();
            await SetCameraControllerPanel();
            SetTargetGroupPositions();
            SubscribeOnChangeHeight();
        }

        public void DeInit()
        {
            m_compositeDisposable.Dispose();
        }
        
        public void SetActiveRotateCamera(bool isActive)
        {
            m_isActiveRotate = isActive;
            m_targetGroupCamera.ActiveRotator(m_isActiveRotate);
            m_cameraControllerPanel.SetActiveRotateButtonImage(m_isActiveRotate);
        }

        private async UniTask SetCameraControllerPanel()
        {
            m_cameraControllerPanel = await m_uiManager.ShowPanelAsync<CameraControllerPanel>();
            m_cameraControllerPanel.OnActiveRotateCamera.Subscribe(OnActiveRotationCamera).AddTo(m_compositeDisposable);
            SetActiveRotateCamera(false);
        }

        private void SetTargetGroupPositions()
        {
            var buildDataConfig = m_levelProvider.GetCurrentBuildDataConfig();
            var fireworkPositions = buildDataConfig.FireworkPositions;
            
            var x = (int)fireworkPositions.Average(p => p.x);
            var z = (int)fireworkPositions.Average(p => p.z);
            var blockIndex = m_savesProvider.GetSavesData<LastNumberBlockSaves>();
            var y = blockIndex >= buildDataConfig.BlockData.Count
                ? buildDataConfig.BlockData.Last().Coord.y
                : buildDataConfig.BlockData[blockIndex].Coord.y;
            var targetGroupTopPosition = new Vector3(x, y, z);
            
            var maxX = (int)fireworkPositions.Max(p => p.x);
            var maxZ = (int)fireworkPositions.Max(p => p.z);
            var t = maxX > maxZ ? maxX : maxZ;
            var d = (t / 3f) * 2;
            var newPositions = new List<Vector3>()
            {
                new Vector3(x + t, 0, z), new Vector3(x, 0, z + t),
                new Vector3(x - t, 0, z), new Vector3(x, 0, z - t),
                new Vector3(x + d, 0, z + d), new Vector3(x + d, 0, z - d),
                new Vector3(x - d, 0, z + d), new Vector3(x - d, 0, z - d),
            };

            m_targetGroupCamera.Init(newPositions, targetGroupTopPosition);
        }

        private void SubscribeOnChangeHeight()
        {
            m_onBlockCreated.Subject.Subscribe(OnUpdateTargetGroupTopHeight).AddTo(m_compositeDisposable);
            m_onBuildLayerHeight.Subject.Subscribe(OnUpdateTargetGroupTopHeight).AddTo(m_compositeDisposable);
        }

        private void OnUpdateTargetGroupTopHeight(BlockViewInfo blockViewInfo)
        {
            UpdateTargetGroupTopHeight(blockViewInfo.Coord.y);
        }

        private void OnUpdateTargetGroupTopHeight(int height)
        {
            UpdateTargetGroupTopHeight(height);
        }
        
        private void UpdateTargetGroupTopHeight(int height)
        {
            if (m_currentHeightConstruction == height)
            {
                return;
            }

            m_currentHeightConstruction = height;
            m_targetGroupCamera.UpdateTargetGroupTopHeight(m_currentHeightConstruction);
        }

        private void OnActiveRotationCamera(Unit unit)
        {
            m_isActiveRotate = !m_isActiveRotate;
            SetActiveRotateCamera(m_isActiveRotate);
        }
    }
}