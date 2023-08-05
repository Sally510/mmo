using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public static class PositionHelpers
    {
        public static readonly Vector2 TILE_SIZE = new(1f, 0.5f);
        public static Vector2 WorldToIso(Vector2 position)
        {
            float x = (position.x / TILE_SIZE.x) - (position.y / TILE_SIZE.y);
            float y = (position.x / TILE_SIZE.x) + (position.y / TILE_SIZE.y);

            return new Vector2(x, -y);
        }

        public static Vector2 IsoToWorld(Vector2 isoPosition)
        {
            float x = isoPosition.x * TILE_SIZE.x * 0.5f - isoPosition.y * TILE_SIZE.x * 0.5f;
            float y = isoPosition.x * TILE_SIZE.y * 0.5f + isoPosition.y * TILE_SIZE.y * 0.5f;

            return new(x, -y);
        }

        public static bool Approximately(Vector2 v1, Vector2 v2, float allowedDifference = 0.001f)
        {
            var dx = v1.x - v2.x;
            if (Mathf.Abs(dx) > allowedDifference)
                return false;

            var dy = v1.y - v2.y;
            if (Mathf.Abs(dy) > allowedDifference)
                return false;

            return true;
        }


        public static Vector2 ToDirectionalVector(float angle)
        {
            return new(Mathf.Cos(angle), Mathf.Sin(angle));
        }

        public static Vector3 FromIsoToWorld(this Vector2 iso)
        {
            Vector3 world = IsoToWorld(iso);
            world.z = 1;

            return world;
        }

        public static Vector3 FromWorldToIso(this Vector2 world)
        {
            Vector3 iso = WorldToIso(world);
            iso.z = 1;

            return iso;
        }
    }
}
