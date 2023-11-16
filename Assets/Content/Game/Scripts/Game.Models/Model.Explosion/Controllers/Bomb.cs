using System.Collections.Generic;
using System.Linq;
using Data.Explosion.Enums;
using Data.Explosion.Environment;
using Data.Explosion.Jobs;
using Model.Explosion.Jobs;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

namespace Model.Explosion.Controllers
{
    public class Bomb : MonoBehaviour
    {
        [SerializeField] private int m_explosionForce = 2500;
        [SerializeField] private int m_explosionRadius = 10;
        [SerializeField] private float m_airResistance = 0.1f;
        [SerializeField] private Vector3 m_gravity = new Vector3(0, -9.81f, 0);

        private TransformAccessArray m_transformAccessArray;
        private NativeArray<Vector3> m_position;
        private NativeArray<Vector3> m_velocity;
        private NativeArray<Vector3> m_rotation;
        private NativeArray<Vector3> m_acceleration;
        private NativeArray<Vector3> m_lastPosition;
        private NativeArray<Vector3> m_drag;
        private NativeArray<ECubeState> m_cubeState;
        private NativeArray<JobCubeData> m_jobCubeData;
        private NativeArray<float> m_randomCoefficient;
        private JobEnvironmentData m_jobEnvironmentData;
        private JobBombData m_jobBombData;

        private ExplosionJob m_explosionJob;
        private MovementJob m_movementJob;
        private PhysicsJob m_physicsJob;

        private List<CubeData> m_cubeData;
        private EnvironmentData m_environmentData;
        private bool m_isExplosion;
        private int m_cubeCount = 0;
        private Dictionary<int, List<int>> m_cubeLayers; 

        public void Init(List<CubeData> cubeData, EnvironmentData environmentData)
        {
            m_cubeData = cubeData;
            m_environmentData = environmentData;
        }

        private void Update()
        {
            if (!m_isExplosion)
            {
                return;
            }
        
            m_physicsJob.Velocities = m_velocity;
            m_physicsJob.Acceleration = m_acceleration;
            m_physicsJob.JobCubeData = m_jobCubeData;
            m_physicsJob.CubeState = m_cubeState;
            m_physicsJob.JobEnvironmentData = m_jobEnvironmentData;
            m_physicsJob.Drag = m_drag;

            m_explosionJob.DeltaTime = Time.deltaTime;
            m_explosionJob.Positions = m_position;
            m_explosionJob.Velocities = m_velocity;
            m_explosionJob.Acceleration = m_acceleration;
            m_explosionJob.Rotation = m_rotation;
            m_explosionJob.CubeState = m_cubeState;
            m_explosionJob.JobBombData = m_jobBombData;
            m_explosionJob.RandomCoefficient = m_randomCoefficient;
            m_explosionJob.Drag = m_drag;

            m_movementJob.Positions = m_position;
            m_movementJob.Velocities = m_velocity;
            m_movementJob.Rotation = m_rotation;
            m_movementJob.LastPosition = m_lastPosition;
            m_movementJob.JobCubeData = m_jobCubeData;
            m_movementJob.CubeState = m_cubeState;
            m_movementJob.JobEnvironmentData = m_jobEnvironmentData;

            var physicsHandel = m_physicsJob.Schedule(m_cubeCount, 0);
            var explosionHandle = m_explosionJob.Schedule(m_cubeCount, 0, physicsHandel);
            var movementHandle = m_movementJob.Schedule(m_transformAccessArray, explosionHandle);
            movementHandle.Complete();

            m_isExplosion = false;
            for (int i = 0; i < m_cubeState.Length; i++)
            {
                if (m_cubeState[i] == ECubeState.Rest)
                {
                    m_isExplosion = true;
                    break;
                }
            }

            if (!m_isExplosion)
            {
                OnDestroy();
            }
        }

        public void Reset()
        {
            if (!m_isExplosion)
            {
                return;
            }
            m_isExplosion = false;
            OnDestroy();
        }


        public void Boom()
        {
            Debug.Log("Boom");

            InitExplosionJob();
            m_isExplosion = true;
        }

        private void InitExplosionJob()
        {
            m_cubeCount = m_cubeData.Count;
            m_position = new NativeArray<Vector3>(m_cubeCount, Allocator.Persistent);
            m_velocity = new NativeArray<Vector3>(m_cubeCount, Allocator.Persistent);
            m_rotation = new NativeArray<Vector3>(m_cubeCount, Allocator.Persistent);
            m_acceleration = new NativeArray<Vector3>(m_cubeCount, Allocator.Persistent);
            m_lastPosition = new NativeArray<Vector3>(m_cubeCount, Allocator.Persistent);
            m_drag = new NativeArray<Vector3>(m_cubeCount, Allocator.Persistent);
            m_cubeState = new NativeArray<ECubeState>(m_cubeCount, Allocator.Persistent);
            m_jobCubeData = new NativeArray<JobCubeData>(m_cubeCount, Allocator.Persistent);
            m_randomCoefficient = new NativeArray<float>(100, Allocator.Persistent);
            m_explosionJob = new ExplosionJob();
            m_movementJob = new MovementJob();

            for (int i = 0; i < m_randomCoefficient.Length; i++)
            {
                m_randomCoefficient[i] = Random.Range(-3f, 3f);
            }
            var transforms = new Transform[m_cubeCount];
            for (int i = 0; i < m_cubeCount; i++)
            {
                transforms[i] = m_cubeData[i].Transform;
                var position = m_cubeData[i].Transform.position;

                m_position[i] = position;
                m_lastPosition[i] = position;
                m_velocity[i] = CalculateVelocity(position, i);
                m_cubeState[i] = CalculateCubeState(position);
                m_jobCubeData[i] = new JobCubeData
                {
                    Index = m_cubeData[i].Index,
                    Mass = m_cubeData[i].Mass,
                    SphereRadius = 0.5f,
                    Restitution = m_cubeData[i].Restitution
                };
            }
            m_transformAccessArray = new TransformAccessArray(transforms);
            m_jobEnvironmentData = new JobEnvironmentData
            {
                PlaneNormal = m_environmentData.PlaneNormal,
                PointOnPlane = m_environmentData.PointOnPlane,
                Gravity = m_gravity,
                AirResistance = m_airResistance
            };
            m_jobBombData = new JobBombData
            {
                Center = transform.position,
                Force = m_explosionForce,
                Radius = m_explosionRadius
            };
        }
    
        private Vector3 CalculateVelocity(Vector3 position, int index)
        {
            var randomCoefficient = m_randomCoefficient[index % m_randomCoefficient.Length];
            Vector3 direction = (position - transform.position) + (Vector3.one * randomCoefficient);
            float distance = Vector3.Distance(position, transform.position);
            return direction.normalized * ((m_explosionForce * Mathf.Abs(randomCoefficient)) * (1f - distance / m_explosionRadius));
        }

        private ECubeState CalculateCubeState(Vector3 position)
        {
            var explosionDistance = position - transform.position;
            return explosionDistance.magnitude <= m_explosionRadius ? ECubeState.Explosion : ECubeState.Rest;
        }

        private async void Gravity() // не работает
        {
            var maxHigh = m_cubeData.Max(v => v.Coord.y);
            m_cubeLayers = new Dictionary<int, List<int>>();
            for (int i = 0; i <= maxHigh; i++)
            {
                m_cubeLayers[i] = new List<int>();
            }
            for (int i = 0; i < m_cubeData.Count; i++)
            {
                var cubeData = m_cubeData[i];
                m_cubeLayers[cubeData.Coord.y].Add(cubeData.Index);
            }

            for (int i = 0; i < m_cubeLayers.Count; i++)
            {
                if (i + 1 >= m_cubeLayers.Count)
                {
                    break;
                }

                var botLayer = m_cubeLayers[i];
                var targetLayer = m_cubeLayers[i + 1];
            
                if (botLayer.Count <= 0)
                {
                    continue;
                }

                for (int j = 0; j < targetLayer.Count; j++)
                {
                    var tIndex = targetLayer[j] - 1;
                    if (botLayer.Any(bIndex => m_cubeData[bIndex].Coord.x == m_cubeData[tIndex].Coord.x 
                                               && m_cubeData[bIndex].Coord.z == m_cubeData[tIndex].Coord.z 
                                               && m_cubeState[bIndex] == ECubeState.Rest))
                    {
                        continue;
                    }

                    var nearestSecondLayerCubes = targetLayer.Where(bIndex =>
                        m_cubeState[bIndex] == ECubeState.Rest &&
                        ((m_cubeData[bIndex].Coord.x == m_cubeData[tIndex].Coord.x + 1 && m_cubeData[bIndex].Coord.z == m_cubeData[tIndex].Coord.z) ||
                         (m_cubeData[bIndex].Coord.x == m_cubeData[tIndex].Coord.x - 1 && m_cubeData[bIndex].Coord.z == m_cubeData[tIndex].Coord.z) ||
                         (m_cubeData[bIndex].Coord.x == m_cubeData[tIndex].Coord.z + 1 && m_cubeData[bIndex].Coord.x == m_cubeData[tIndex].Coord.x) ||
                         (m_cubeData[bIndex].Coord.x == m_cubeData[tIndex].Coord.z - 1 && m_cubeData[bIndex].Coord.x == m_cubeData[tIndex].Coord.x))).ToList();

                    if (nearestSecondLayerCubes.Count <= 0)
                    {
                        m_cubeState[tIndex] = ECubeState.Loose;
                        continue;
                    }

                    var isLooseCub = true;
                    for (int k = 0; k < nearestSecondLayerCubes.Count; k++)
                    {
                        var nearestCubeIndex = nearestSecondLayerCubes[k];
                        if (botLayer.Any(bIndex => m_cubeData[bIndex].Coord.x == m_cubeData[nearestCubeIndex].Coord.x 
                                                   && m_cubeData[bIndex].Coord.z == m_cubeData[nearestCubeIndex].Coord.z
                                                   && m_cubeState[bIndex] == ECubeState.Rest))
                        {
                            isLooseCub = false;
                            break;
                        }
                    }

                    if (isLooseCub)
                    {
                        m_cubeState[tIndex] = ECubeState.Loose;
                    }
                }
            }
        }

        private void OnDestroy()
        {
            m_position.Dispose();
            m_velocity.Dispose();
            m_rotation.Dispose();
            m_lastPosition.Dispose();
            m_acceleration.Dispose();
            m_jobCubeData.Dispose();
            m_cubeState.Dispose();
            m_drag.Dispose();
            m_randomCoefficient.Dispose();
            m_transformAccessArray.Dispose();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, m_explosionRadius);
        }
    }
}
