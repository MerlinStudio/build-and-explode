using System;
using System.Text;
using TMPro;
using UnityEngine;

namespace Content.ExplosionScene.Scripts
{
    public class FPS : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_text;
        
        private float m_fps;
        private float m_count;

        private void Start()
        {
            m_count = 1;
        }

        private void Update()
        {
            if (m_count > 0)
            {
                m_count -= 2f * Time.deltaTime;
                return;
            }
            m_fps = 1.0f / Time.deltaTime;
            m_text.text = $"{(int)m_fps}";
            m_count = 1;
        }
    }
}