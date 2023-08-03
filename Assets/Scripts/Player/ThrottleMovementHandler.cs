using System.Threading;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public static class ThrottleMovementHandler
    {
        private const float MOVEMENT_TICK_SECONDS = 0.1f;
        private static int MOVEMENT_TICK_COUNTER = 1;

        private static float _timer = 0;
        private static float? _angle = null;
        private static bool _stopOnNextUpdate = false;

        public static float? Angle { get => _angle; }
        public static bool IsInMovement { get => _angle.HasValue && !_stopOnNextUpdate; }

        public static void Start(float angle)
        {
            _angle = angle;
            _stopOnNextUpdate = false;
        }

        public static void Stop()
        {
            _stopOnNextUpdate = true;
        }

        public static void Restart()
        {
            _timer = 0;
            _angle = null;
            _stopOnNextUpdate = false;
        }

        public static bool PollPacket(float elapsedMillis, Vector2 isoPosition, out MovementPacket packet)
        {
            packet = null;
            bool newPacket = false;

            if (_angle.HasValue)
            {
                _timer = Mathf.Round((_timer + elapsedMillis) * 100f) / 100f;

                newPacket = _stopOnNextUpdate || _timer >= MOVEMENT_TICK_SECONDS;

                if (newPacket)
                {
                    int id = Interlocked.Increment(ref MOVEMENT_TICK_COUNTER);
                    packet = new MovementPacket(id, _angle.Value - math.PI, _timer, isoPosition);
                    _timer = 0;
                    _angle = null;
                }
            }

            _stopOnNextUpdate = false;

            return newPacket;
        }

        public class MovementPacket
        {

            public int Id { get; set; }
            public float Angle { get; set; }
            public float ElapsedSeconds { get; set; }
            public Vector2 PredictedIsoPosition { get; set; }

            public MovementPacket(int id, float angle, float elapsedSeconds, Vector2 predictedIsoPosition)
            {
                Id = id;
                Angle = angle;
                ElapsedSeconds = elapsedSeconds;
                PredictedIsoPosition = predictedIsoPosition;
            }
        }
    }
}
