using System;
using UnityEngine;

namespace Data.Builds.Blocks
{
    [Serializable]
    public struct BlockPropertyInfo
    {
        [HideInInspector] public int Index;
        public string Id;
        public float Mass;
        public float Radius;
        public float Restitution;
    }
}