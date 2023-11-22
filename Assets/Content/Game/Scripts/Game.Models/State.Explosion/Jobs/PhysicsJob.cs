using Data.Builds.Blocks;
using Data.Explosion.Enums;
using Data.Explosion.Info;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace State.Explosion.Jobs
{
    [BurstCompile]

    public struct PhysicsJob : IJobParallelFor
    {
        [ReadOnly] public EnvironmentInfo EnvironmentInfo;
        [ReadOnly] public NativeArray<BlockPropertyInfo> BlockPropertyInfo;
        [ReadOnly] public NativeArray<Vector3> Velocities;
        [ReadOnly] public NativeArray<EBlockState> CubeState;

        public NativeArray<Vector3> Acceleration;
        public NativeArray<Vector3> Drag;
        
        public void Execute(int index)
        {
            if (CubeState[index] == EBlockState.Rest)
            {
                return;
            }
            
            Drag[index] = -Velocities[index].normalized * EnvironmentInfo.AirResistance * Velocities[index].sqrMagnitude;
            Acceleration[index] = (Drag[index] / BlockPropertyInfo[index].Mass) + EnvironmentInfo.Gravity;
        }
    }
}