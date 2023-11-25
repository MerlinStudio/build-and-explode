using System;
using System.Collections.Generic;
using System.Linq;
using Data.Builds.Blocks;
using Data.Explosion.Enums;
using Data.Explosion.Info;
using State.Explosion.Jobs;
using UniRx;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using Random = UnityEngine.Random;

namespace State.Explosion.Controllers
{
    public class ExplosionController
    {
        public ExplosionController(
            BlockPropertyInfo[]blockPropertyInfo,
            BlockViewInfo[] blockViewInfo,
            EnvironmentInfo environmentInfo,
            ISubject<Unit> explosionFinished)
        {
            m_blockPropertyInfo = blockPropertyInfo;
            m_blockViewInfo = blockViewInfo;
            m_environmentInfo = environmentInfo;
            m_explosionFinished = explosionFinished;
        }

        private readonly BlockPropertyInfo[] m_blockPropertyInfo;
        private readonly BlockViewInfo[] m_blockViewInfo;
        private readonly EnvironmentInfo m_environmentInfo;
        private readonly ISubject<Unit> m_explosionFinished;
        
        private TransformAccessArray m_transformAccessArray;
        private NativeArray<Vector3> m_position;
        private NativeArray<Vector3> m_velocity;
        private NativeArray<Vector3> m_rotation;
        private NativeArray<Vector3> m_acceleration;
        private NativeArray<Vector3> m_lastPosition;
        private NativeArray<Vector3> m_drag;
        private NativeArray<EBlockState> m_blockState;
        private NativeArray<BlockPropertyInfo> m_nativeBlockPropertyInfo;
        private NativeArray<float> m_randomCoefficient;

        private Dictionary<int, List<int>> m_cubeLayers;
        private ExplosionJob m_explosionJob;
        private MovementJob m_movementJob;
        private PhysicsJob m_physicsJob;
        private ExplosionInfo m_explosionInfo;
        private bool m_isEndExplosion;
        private bool m_isUpdateExplosion;
        private int m_blockCount;
        private int m_explosionsLeft;

        public void Init()
        {
            m_blockCount = m_blockPropertyInfo.Length;
            m_position = new NativeArray<Vector3>(m_blockCount, Allocator.Persistent);
            m_velocity = new NativeArray<Vector3>(m_blockCount, Allocator.Persistent);
            m_rotation = new NativeArray<Vector3>(m_blockCount, Allocator.Persistent);
            m_acceleration = new NativeArray<Vector3>(m_blockCount, Allocator.Persistent);
            m_lastPosition = new NativeArray<Vector3>(m_blockCount, Allocator.Persistent);
            m_drag = new NativeArray<Vector3>(m_blockCount, Allocator.Persistent);
            m_blockState = new NativeArray<EBlockState>(m_blockCount, Allocator.Persistent);
            m_nativeBlockPropertyInfo = new NativeArray<BlockPropertyInfo>(m_blockCount, Allocator.Persistent);
            m_randomCoefficient = new NativeArray<float>(100, Allocator.Persistent);
            m_explosionJob = new ExplosionJob();
            m_movementJob = new MovementJob();
            
            for (int i = 0; i < m_randomCoefficient.Length; i++)
            {
                m_randomCoefficient[i] = Random.Range(-3f, 3f);
            }
        }
        
        private void DeInit()
        {
            m_position.Dispose();
            m_velocity.Dispose();
            m_rotation.Dispose();
            m_lastPosition.Dispose();
            m_acceleration.Dispose();
            m_nativeBlockPropertyInfo.Dispose();
            m_blockState.Dispose();
            m_drag.Dispose();
            m_randomCoefficient.Dispose();
            m_transformAccessArray.Dispose();
        }

        public void SetupTransforms()
        {
            var transforms = new Transform[m_blockCount];
            for (int i = 0; i < m_blockCount; i++)
            {
                transforms[i] = m_blockViewInfo[i].Transform;
                m_nativeBlockPropertyInfo[i] = m_blockPropertyInfo[i];
            }
            m_transformAccessArray = new TransformAccessArray(transforms);
        }

        public void Update()
        {
            if (!m_isUpdateExplosion)
            {
                return;
            }
        
            m_physicsJob.Velocities = m_velocity;
            m_physicsJob.Acceleration = m_acceleration;
            m_physicsJob.BlockPropertyInfo = m_nativeBlockPropertyInfo;
            m_physicsJob.CubeState = m_blockState;
            m_physicsJob.EnvironmentInfo = m_environmentInfo;
            m_physicsJob.Drag = m_drag;

            m_explosionJob.DeltaTime = Time.deltaTime;
            m_explosionJob.Positions = m_position;
            m_explosionJob.Velocities = m_velocity;
            m_explosionJob.Acceleration = m_acceleration;
            m_explosionJob.Rotation = m_rotation;
            m_explosionJob.CubeState = m_blockState;
            m_explosionJob.ExplosionInfo = m_explosionInfo;
            m_explosionJob.RandomCoefficient = m_randomCoefficient;
            m_explosionJob.Drag = m_drag;

            m_movementJob.Positions = m_position;
            m_movementJob.Velocities = m_velocity;
            m_movementJob.Rotation = m_rotation;
            m_movementJob.LastPosition = m_lastPosition;
            m_movementJob.BlockPropertyInfo = m_nativeBlockPropertyInfo;
            m_movementJob.CubeState = m_blockState;
            m_movementJob.EnvironmentInfo = m_environmentInfo;

            var physicsHandel = m_physicsJob.Schedule(m_blockCount, 0);
            var explosionHandle = m_explosionJob.Schedule(m_blockCount, 0, physicsHandel);
            var movementHandle = m_movementJob.Schedule(m_transformAccessArray, explosionHandle);
            movementHandle.Complete();

            m_isEndExplosion = false;
            for (int i = 0; i < m_blockState.Length; i++)
            {
                if (m_blockState[i] != EBlockState.Rest)
                {
                    m_isEndExplosion = true;
                    break;
                }
            }

            if (!m_isEndExplosion && m_explosionsLeft <= 0)
            {
                m_isUpdateExplosion = false;
                DeInit();
                m_explosionFinished.OnNext(Unit.Default);
                Debug.Log("+++++End explosion+++++");
            }
        }

        public void Explosion(ExplosionInfo explosionInfo, int explosionsLeft)
        {
            m_explosionInfo = explosionInfo;
            m_explosionsLeft = explosionsLeft;
            for (int i = 0; i < m_blockCount; i++)
            {
                var position = m_blockViewInfo[i].Transform.position;
                m_position[i] = position;
                m_lastPosition[i] = position;
                m_velocity[i] = CalculateVelocity(position, i);
                m_blockState[i] = CalculateCubeState(position);
            }
            m_isUpdateExplosion = true;
        }
    
        private Vector3 CalculateVelocity(Vector3 position, int index)
        {
            var randomCoefficient = m_randomCoefficient[index % m_randomCoefficient.Length];
            Vector3 direction = (position - m_explosionInfo.Center) + (Vector3.one * randomCoefficient);
            float distance = Vector3.Distance(position, m_explosionInfo.Center);
            return direction.normalized * ((m_explosionInfo.Force * Mathf.Abs(randomCoefficient)) * (1f - distance / m_explosionInfo.Radius));
        }

        private EBlockState CalculateCubeState(Vector3 position)
        {
            var explosionDistance = position - m_explosionInfo.Center;
            return explosionDistance.magnitude <= m_explosionInfo.Radius ? EBlockState.Explosion : EBlockState.Rest;
        }

        private async void Gravity() // не работает
        {
            var maxHigh = m_blockViewInfo.Max(v => v.Coord.y);
            m_cubeLayers = new Dictionary<int, List<int>>();
            for (int i = 0; i <= maxHigh; i++)
            {
                m_cubeLayers[i] = new List<int>();
            }
            for (int i = 0; i < m_blockViewInfo.Length; i++)
            {
                var blockViewInfo = m_blockViewInfo[i];
                m_cubeLayers[blockViewInfo.Coord.y].Add(blockViewInfo.Index);
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
                    if (botLayer.Any(bIndex => m_blockViewInfo[bIndex].Coord.x == m_blockViewInfo[tIndex].Coord.x 
                                               && m_blockViewInfo[bIndex].Coord.z == m_blockViewInfo[tIndex].Coord.z 
                                               && m_blockState[bIndex] == EBlockState.Rest))
                    {
                        continue;
                    }

                    var nearestSecondLayerCubes = targetLayer.Where(bIndex =>
                        m_blockState[bIndex] == EBlockState.Rest &&
                        ((m_blockViewInfo[bIndex].Coord.x == m_blockViewInfo[tIndex].Coord.x + 1 && m_blockViewInfo[bIndex].Coord.z == m_blockViewInfo[tIndex].Coord.z) ||
                         (m_blockViewInfo[bIndex].Coord.x == m_blockViewInfo[tIndex].Coord.x - 1 && m_blockViewInfo[bIndex].Coord.z == m_blockViewInfo[tIndex].Coord.z) ||
                         (m_blockViewInfo[bIndex].Coord.x == m_blockViewInfo[tIndex].Coord.z + 1 && m_blockViewInfo[bIndex].Coord.x == m_blockViewInfo[tIndex].Coord.x) ||
                         (m_blockViewInfo[bIndex].Coord.x == m_blockViewInfo[tIndex].Coord.z - 1 && m_blockViewInfo[bIndex].Coord.x == m_blockViewInfo[tIndex].Coord.x))).ToList();

                    if (nearestSecondLayerCubes.Count <= 0)
                    {
                        m_blockState[tIndex] = EBlockState.Loose;
                        continue;
                    }

                    var isLooseCub = true;
                    for (int k = 0; k < nearestSecondLayerCubes.Count; k++)
                    {
                        var nearestCubeIndex = nearestSecondLayerCubes[k];
                        if (botLayer.Any(bIndex => m_blockViewInfo[bIndex].Coord.x == m_blockViewInfo[nearestCubeIndex].Coord.x 
                                                   && m_blockViewInfo[bIndex].Coord.z == m_blockViewInfo[nearestCubeIndex].Coord.z
                                                   && m_blockState[bIndex] == EBlockState.Rest))
                        {
                            isLooseCub = false;
                            break;
                        }
                    }

                    if (isLooseCub)
                    {
                        m_blockState[tIndex] = EBlockState.Loose;
                    }
                }
            }
        }
    }
}
