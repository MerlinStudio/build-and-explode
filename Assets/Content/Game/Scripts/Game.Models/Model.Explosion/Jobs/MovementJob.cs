using Data.Explosion.Enums;
using Data.Explosion.Jobs;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

namespace Model.Explosion.Jobs
{
    [BurstCompile]
    public struct MovementJob : IJobParallelForTransform
    {
        [ReadOnly] public JobEnvironmentData JobEnvironmentData;
        [ReadOnly] public NativeArray<JobCubeData> JobCubeData;

        public NativeArray<Vector3> Positions;
        public NativeArray<Vector3> Velocities;
        public NativeArray<Vector3> Rotation;
        public NativeArray<Vector3> LastPosition;
        public NativeArray<ECubeState> CubeState;

        public void Execute(int index, TransformAccess transform)
        {
            if (CubeState[index] == ECubeState.Rest)
            {
                return;
            }
            var position = Positions[index];
            var velocity = Velocities[index];
            var jobCubeData = JobCubeData[index];

            var u = position - JobEnvironmentData.PointOnPlane;
            var d = Vector3.Dot(u, JobEnvironmentData.PlaneNormal);
            var penetration = jobCubeData.SphereRadius - d;
     
            // проверка на проникновение
            if (penetration > 0.0f)
            {
                Positions[index] = position + penetration * JobEnvironmentData.PlaneNormal;
                var reflect = Reflect(velocity, JobEnvironmentData.PlaneNormal, jobCubeData.Restitution);
                Velocities[index] = reflect - (reflect * 0.5f);

                var lastPosition = LastPosition[index];
                var currentPosition = Positions[index];
                if (Mathf.Round(currentPosition.x * 10) == Mathf.Round(lastPosition.x * 10) &&
                    Mathf.Round(currentPosition.z * 10) == Mathf.Round(lastPosition.z * 10))
                {
                    CubeState[index] = ECubeState.Rest;
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