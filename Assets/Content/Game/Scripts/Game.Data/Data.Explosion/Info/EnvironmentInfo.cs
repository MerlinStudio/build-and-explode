using System;
using UnityEngine;

namespace Data.Explosion.Info
{
    [Serializable]
    public struct EnvironmentInfo
    {
        public Vector3 PlaneNormal;
        public Vector3 PointOnPlane;
        public Vector3 Gravity;
        public float AirResistance;
    }
}