using System.Collections.Generic;
using System.Linq;
using Data.Builds.Configs;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
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

        [Button]
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

        [Button]
        private void AddBuildConfig()
        {
            var buildDataConfig = ScriptableObject.CreateInstance<BuildDataConfig>();
            AssetDatabase.CreateAsset(buildDataConfig, $"{m_path}{m_configName}.asset");
            
            var blockTransforms = m_parent.GetComponentsInChildren<CubeEditor>();
            var blockDatas = new List<BlockData>();

            var sortBlockTransforms = blockTransforms.OrderBy(b => b.transform.position.y).ToList();
            for (int i = 0; i < sortBlockTransforms.Count; i++)
            {
                var block = sortBlockTransforms[i];
                var blockData = new BlockData
                {
                    Id = block.Id,
                    Coord = Vector3Int.CeilToInt(block.transform.position)
                };
                blockDatas.Add(blockData);
            }

            var maxBlock = 5000;
            var roundBlocks = Mathf.Clamp(blockDatas.Count, 0, maxBlock);
            var t = roundBlocks / maxBlock;
            var amountBomb = (int)Mathf.Lerp(1, 5, t);

            var offset = 3;
            var maxX = blockDatas.Max(b => b.Coord.x) + offset;
            var minX = blockDatas.Min(b => b.Coord.x) - offset;
            var maxZ = blockDatas.Max(b => b.Coord.z) + offset;
            var minZ = blockDatas.Min(b => b.Coord.z) - offset;
            var fireworkPositions = new List<Vector3>
            {
                new Vector3(maxX, -0.5f, maxZ),
                new Vector3(maxX, -0.5f, minZ),
                new Vector3(minX, -0.5f, maxZ),
                new Vector3(minX, -0.5f, minZ)
            };
            
            buildDataConfig.BlockData = blockDatas;
            buildDataConfig.FireworkPositions = fireworkPositions;
            buildDataConfig.AmountBomb = amountBomb;
            EditorUtility.SetDirty(buildDataConfig);
        }

        [Button]
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