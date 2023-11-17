using Data.Explosion.Enums;
using Data.Explosion.Info;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Model.Explosion.Jobs
{
    [BurstCompile]
    public struct ExplosionJob : IJobParallelFor
    {
        [ReadOnly] public float DeltaTime;
        [ReadOnly] public ExplosionInfo ExplosionInfo;
        [ReadOnly] public NativeArray<float> RandomCoefficient;
        [ReadOnly] public NativeArray<Vector3> Drag;
        [ReadOnly] public NativeArray<Vector3> Acceleration;

        public NativeArray<EBlockState> CubeState;
        public NativeArray<Vector3> Positions;
        public NativeArray<Vector3> Velocities;
        public NativeArray<Vector3> Rotation;

        private int RandomCoefficientCount => RandomCoefficient.Length;
        
        public void Execute(int index)
        {
            if (CubeState[index] != EBlockState.Explosion)
            {
                return;
            }

            var duration = Positions[index] - ExplosionInfo.Center;
            Velocities[index] += (Acceleration[index] + duration.normalized) * DeltaTime;
            Positions[index] += Velocities[index] * DeltaTime;
            Rotation[index] += (Drag[index] + Velocities[index].normalized) * (10 * RandomCoefficient[index % RandomCoefficientCount]) * DeltaTime;
        }
    }
}