using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class AutoMovementManager
    {
        private readonly DateTime _utcStart;
        private readonly TimeSpan _duration;
        private readonly List<Vector2> _moves;
        private readonly List<float> _distances;
        private readonly float _totalDistance;
        private float _timerSeconds;
        private bool _isLast;

        public bool IsDone { get => _isLast; }

        public AutoMovementManager(DateTime utcStart, TimeSpan duration, List<Vector2> moves)
        {
            if (moves == null)
                throw new ArgumentNullException(nameof(moves));

            if (moves.Count < 2)
            {
                throw new ArgumentException("Atleast two moves required");
            }

            DateTime utcNow = DateTime.UtcNow;
            if (utcNow < utcStart)
            {
                //TODO: a bit sketchy, I noticed theres a few millisec difference between the emulator and server..
                utcStart = utcNow;
            }

            _utcStart = utcStart;
            _duration = duration;
            _moves = moves;
            _timerSeconds = (DateTime.UtcNow.Millisecond - utcStart.Millisecond) / 1000f;
            _isLast = false;

            _distances = new List<float>(moves.Count - 1);
            float totalDistance = 0;

            for (int i = 0; i < _moves.Count - 1; i++)
            {
                float distance = Vector2.Distance(Vector2.zero, moves[i] - moves[i + 1]);
                totalDistance += distance;
                _distances.Add(distance);
            }
            _totalDistance = totalDistance;
        }

        public void Update(float elapsedSeconds)
        {
            _timerSeconds += elapsedSeconds;
        }

        public Vector2 CurrentIsoPosition()
        {
            if (_isLast)
            {
                return _moves[^1];
            }

            //TODO: optimize on direct path..
            float percentage = _timerSeconds / (float)_duration.TotalSeconds;

            if (percentage == 0.0)
            {
                return _moves[0];
            }

            if (percentage >= 1.0)
            {
                _isLast = true;
                return _moves[^1];
            }

            // Calculate the distance to the point we want to find
            float targetDistance = percentage * _totalDistance;

            // Find the segment that contains the point we want to find
            int currentSegment = 0;
            float distanceSoFar = 0.0f;
            while (distanceSoFar + _distances[currentSegment] < targetDistance)
            {
                distanceSoFar += _distances[currentSegment];
                currentSegment += 1;
            }

            // Calculate the percentage of the segment that the point is at
            float segmentPercentage =
                (targetDistance - distanceSoFar) / _distances[currentSegment];

            // Calculate the coordinates of the point
            Vector2 p1 = _moves[currentSegment];
            Vector2 p2 = _moves[currentSegment + 1];

            return Vector2.Lerp(p1, p2, segmentPercentage);
        }
    }
}
