using Data.Explosion.Enums;
using Data.Explosion.Jobs;
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
        [ReadOnly] public JobBombData JobBombData;
        [ReadOnly] public NativeArray<float> RandomCoefficient;
        [ReadOnly] public NativeArray<Vector3> Drag;
        [ReadOnly] public NativeArray<Vector3> Acceleration;

        public NativeArray<ECubeState> CubeState;
        public NativeArray<Vector3> Positions;
        public NativeArray<Vector3> Velocities;
        public NativeArray<Vector3> Rotation;

        private int RandomCoefficientCount => RandomCoefficient.Length;
        
        public void Execute(int index)
        {
            if (CubeState[index] != ECubeState.Explosion)
            {
                return;
            }

            var duration = Positions[index] - JobBombData.Center;
            Velocities[index] += (Acceleration[index] + duration.normalized) * DeltaTime;
            Positions[index] += Velocities[index] * DeltaTime;
            Rotation[index] += (Drag[index] + Velocities[index].normalized) * (10 * RandomCoefficient[index % RandomCoefficientCount]) * DeltaTime;
        }
    }
}