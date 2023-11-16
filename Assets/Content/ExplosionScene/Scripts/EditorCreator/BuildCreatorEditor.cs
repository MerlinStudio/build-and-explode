using System.Linq;
using Data.Explosion.Configs;
using UnityEditor;
using UnityEngine;

namespace Content.ExplosionScene.Editor
{
#if UNITY_EDITOR
    
    public class BuildCreatorEditor : MonoBehaviour
    {
        [SerializeField] private string m_path = "Assets/Content/ExplosionScene/Configs/";
        [SerializeField] private string m_configName;
        [SerializeField] private Transform m_parent;
        [SerializeField] private CubeEditor m_cubEditorReference;
        [SerializeField] private int m_buildTestX;
        [SerializeField] private int m_buildTestZ;
        [SerializeField] private int m_buildTestAmountCube;

        //[Button]
        private void Init()
        {
            var cubes = m_parent.GetComponentsInChildren<CubeEditor>();
            foreach (var cube in cubes)
            {
                DestroyImmediate(cube.gameObject);
            }
            var cubEditor = Instantiate(m_cubEditorReference, m_parent);
            cubEditor.transform.localPosition = Vector3.zero;
            cubEditor.transform.localEulerAngles = Vector3.zero;
            m_configName = "BuildDataConfig";
        }

        //[Button]
        private void AddBuildConfig()
        {
            var buildDataConfig = ScriptableObject.CreateInstance<BuildDataConfig>();
            AssetDatabase.CreateAsset(buildDataConfig, $"{m_path}{m_configName}.asset");

            var cubeTransforms = m_parent.GetComponentsInChildren<Transform>();
            var cubePosition = cubeTransforms.Select(t => t.position).ToList();
            buildDataConfig.BuildData = cubePosition;
            EditorUtility.SetDirty(buildDataConfig);
        }

        //[Button]
        private void BuildTest()
        {
            var cubes = m_parent.GetComponentsInChildren<CubeEditor>();
            foreach (var cube in cubes)
            {
                DestroyImmediate(cube.gameObject);
            }
            
            var countX = 0;
            var countY = 0;
            var countZ = 0;

            for (int i = 0; i < m_buildTestAmountCube; i++)
            {
                var cubEditor = Instantiate(m_cubEditorReference, m_parent);
                var position = new Vector3(countX, countY, countZ);
                cubEditor.transform.localPosition = position;
                cubEditor.transform.localEulerAngles = Vector3.zero;

                countX++;
                if (countX > m_buildTestX)
                {
                    countX = 0;
                    countZ++;
                }
                if (countZ > m_buildTestZ)
                {
                    countZ = 0;
                    countY++;
                }
            }
        }
    }
#endif

}