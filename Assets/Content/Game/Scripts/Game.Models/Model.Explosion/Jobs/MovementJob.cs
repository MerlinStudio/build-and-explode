using Data.Builds.Blocks;
using Data.Explosion.Enums;
using Data.Explosion.Info;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

namespace Model.Explosion.Jobs
{
    [BurstCompile]
    public struct MovementJob : IJobParallelForTransform
    {
        [ReadOnly] public EnvironmentInfo EnvironmentInfo;
        [ReadOnly] public NativeArray<BlockPropertyInfo> BlockPropertyInfo;

        public NativeArray<Vector3> Positions;
        public NativeArray<Vector3> Velocities;
        public NativeArray<Vector3> Rotation;
        public NativeArray<Vector3> LastPosition;
        public NativeArray<EBlockState> CubeState;

        public void Execute(int index, TransformAccess transform)
        {
            if (CubeState[index] == EBlockState.Rest)
            {
                return;
            }
            var position = Positions[index];
            var velocity = Velocities[index];
            var blockInfo = BlockPropertyInfo[index];

            var u = position - EnvironmentInfo.PointOnPlane;
            var d = Vector3.Dot(u, EnvironmentInfo.PlaneNormal);
            var penetration = blockInfo.Radius - d;
     
            // проверка на проникновение
            if (penetration > 0.0f)
            {
                Positions[index] = position + penetration * EnvironmentInfo.PlaneNormal;
                var reflect = Reflect(velocity, EnvironmentInfo.PlaneNormal, blockInfo.Restitution);
                Velocities[index] = reflect - (reflect * 0.5f);

                var lastPosition = LastPosition[index];
                var currentPosition = Positions[index];
                if (Mathf.Round(currentPosition.x * 10) == Mathf.Round(lastPosition.x * 10) &&
                    Mathf.Round(currentPosition.z * 10) == Mathf.Round(lastPosition.z * 10))
                {
                    CubeState[index] = EBlockState.Rest;
                    Velocities[index] = Vector3.zero;
                }
                LastPosition[index] = currentPosition;
            }

            transform.position = Positions[index];
            transform.rotation = Quaternion.Euler(Rotation[index]);
        }
            
        private Vector3 Reflect(Vector3 vec, Vector3 normal, float restitution)
        {
            Vector3 perpendicular = Vector3.Project(vec, normal);
            Vector3 parallel = vec - perpendicular;
            return parallel - restitution * perpendicular;
        }
    }
}