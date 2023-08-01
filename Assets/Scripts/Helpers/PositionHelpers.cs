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


        public static Vector2 ToDirectionalVector(float angle)
        {
            return new(Mathf.Cos(angle), Mathf.Sin(angle));
        }
    }
}
