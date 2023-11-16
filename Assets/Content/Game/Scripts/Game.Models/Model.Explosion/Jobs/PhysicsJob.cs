using Data.Explosion.Enums;
using Data.Explosion.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Model.Explosion.Jobs
{
    [BurstCompile]

    public struct PhysicsJob : IJobParallelFor
    {
        [ReadOnly] public JobEnvironmentData JobEnvironmentData;
        [ReadOnly] public NativeArray<JobCubeData> JobCubeData;
        [ReadOnly] public NativeArray<Vector3> Velocities;
        [ReadOnly] public NativeArray<ECubeState> CubeState;

        public NativeArray<Vector3> Acceleration;
        public NativeArray<Vector3> Drag;
        
        public void Execute(int index)
        {
            if (CubeState[index] == ECubeState.Rest)
            {
                return;
            }
            
            Drag[index] = -Velocities[index].normalized * JobEnvironmentData.AirResistance * Velocities[index].sqrMagnitude;
            Acceleration[index] = (Drag[index] / JobCubeData[index].Mass) + JobEnvironmentData.Gravity;
        }
    }
}