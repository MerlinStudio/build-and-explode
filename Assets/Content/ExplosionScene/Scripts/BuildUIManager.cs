using System;
using System.Collections.Generic;
using Data.Explosion.Configs;
using Dev.Core.Ui.UI.Panels;
using UnityEngine;
using UnityEngine.UI;

public class BuildUIManager : UIPanel
{
    [SerializeField] private List<Buttons> m_buttons;
    [SerializeField] private Button m_boom;

    public event Action<BuildDataConfig> EventBuildCreateClicked;
    public event Action EventExplosionClicked;

    public override void ShowPanel(bool showInstant = false)
    {
        base.ShowPanel(showInstant);
        
        foreach (var button in m_buttons)
        {
            button.Button.onClick.AddListener(()=> OnClickBuildCreate(button.BuildDataConfig));
        }
        m_boom.onClick.AddListener(OnClickBoom);
    }

    private void OnClickBuildCreate(BuildDataConfig buildDataConfig)
    {
        EventBuildCreateClicked?.Invoke(buildDataConfig);
    }

    private void OnClickBoom()
    {
        EventExplosionClicked?.Invoke();
    }
    
    [Serializable]
    public class Buttons
    {
        public Button Button;
        public BuildDataConfig BuildDataConfig;
    }
}
