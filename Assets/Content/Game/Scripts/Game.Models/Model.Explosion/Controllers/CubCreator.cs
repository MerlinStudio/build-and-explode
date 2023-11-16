using System.Collections;
using System.Collections.Generic;
using Data.Explosion.Configs;
using Data.Explosion.Environment;
using UnityEngine;

namespace Model.Explosion.Controllers
{
    public class CubCreator : MonoBehaviour
    {
        [SerializeField] private Transform m_cubesReference;
        [SerializeField] private int m_cubePoolCount;

        [SerializeField] private EnvironmentData m_environmentData;
        [SerializeField] private Bomb m_bombReference;
        [SerializeField] private int m_bombCount = 1;

        private List<CubeData> m_cubeData;
        private List<Bomb> m_bombs;
        private Transform[] m_cubePool;
        private Coroutine m_createCubeCoroutine;

        private void Start()
        {
            m_cubeData = new List<CubeData>();
            m_bombs = new List<Bomb>();
            m_cubePool = new Transform[m_cubePoolCount];
        }

        public void Build(BuildDataConfig buildDataConfig)
        {
            if (m_createCubeCoroutine != null)
            {
                StopCoroutine(m_createCubeCoroutine);
            }
            m_createCubeCoroutine = StartCoroutine(CreateCubeCoroutine(buildDataConfig));
        }
    
        private IEnumerator CreateCubeCoroutine(BuildDataConfig buildDataConfig)
        {
            if (m_bombs.Count > 0)
            {
                m_bombs.ForEach(b => b.Reset());
            }

            for (int i = 0; i < m_cubeData.Count; i++)
            {
                var cubeData = m_cubeData[i];
                cubeData.Transform.gameObject.SetActive(false);
                cubeData.Transform.localEulerAngles = Vector3.zero;
            }

            var bombCount = 0;
            var bombNumber = Random.Range(0, buildDataConfig.BuildData.Count);
            for (int j = 0; j < buildDataConfig.BuildData.Count; j++)
            {
                var cubePosition = buildDataConfig.BuildData[j];
                if (bombNumber == j)
                {
                    if (m_bombs.Count < m_bombCount)
                    {
                        var bomb = Instantiate(m_bombReference, transform);
                        m_bombs.Add(bomb);
                    }
                    m_bombs[bombCount].transform.localPosition = cubePosition;
                    bombCount++;
                    continue;
                }
                Transform cubeTransform;
                if (m_cubeData.Count > j)
                {
                    cubeTransform = m_cubeData[j].Transform;
                    cubeTransform.gameObject.SetActive(true);
                }
                else
                {
                    if (j % m_cubePoolCount == 0)
                    {
                        var cubes = Instantiate(m_cubesReference, null);

                        int t = 0;
                        foreach (Transform cube in cubes)
                        {
                            m_cubePool[t] = cube;
                            t++;
                        }
                        cubes.gameObject.SetActive(false);
                    
                        yield return new WaitForEndOfFrame();
                    }
                
                    cubeTransform = m_cubePool[j % m_cubePoolCount];
                    cubeTransform.transform.SetParent(transform);
                
                    var cubeData = new CubeData
                    {
                        Name = $"cube_{cubePosition.x}_{cubePosition.y}_{cubePosition.z}",
                        Index = j,
                        Coord = new Vector3Int((int)cubePosition.x, (int)cubePosition.y, (int)cubePosition.z),
                        Transform = cubeTransform,
                        Mass = 1, // todo зависимость от типа куба: кирпич, дерево и тд
                        Restitution = 0.5f // todo зависимость от типа куба: кирпич, дерево и тд
                    };
                    m_cubeData.Add(cubeData);
                }
                cubeTransform.localPosition = cubePosition;
            }
            m_bombs.ForEach(b => b.Init(m_cubeData, m_environmentData));
        }

        public void Boom()
        {
            m_bombs.ForEach(b => b.Boom());
        }
    }
}
