using UnityEngine;

namespace Data.Explosion.Environment
{
    public struct CubeData
    {
        public string Name;
        public int Index;
        public Vector3Int Coord;
        public Transform Transform;
        public float Mass;
        public float Restitution;
    }
}