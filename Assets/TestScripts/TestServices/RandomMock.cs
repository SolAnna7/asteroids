using Asteroid.Random;
using System.Collections.Generic;
using UnityEngine;

namespace Asteroid.Tests
{
    internal class RandomMock : IRandomService
    {
        public float NextFloat(float minInc, float maxInc)
        {
            var next = NextFloat();
            if (minInc <= next && maxInc >= minInc)
            {
                return next;
            }

            next %= (maxInc - minInc);
            next += minInc;

            return next;
        }

        public float NextFloat()
        {
            var next = UpcomingValueArray[CurrentValueIndex];
            CurrentValueIndex++;
            CurrentValueIndex %= UpcomingValueArray.Count;
            return next;
        }

        public Vector2 NextUnitVec2()
        {
            return new Vector2(NextFloat(), NextFloat()).normalized;
        }

        public Vector2 NextVec2(Vector2 minInc, Vector2 maxInc)
        {
            return new Vector2(NextFloat(minInc.x, maxInc.x), NextFloat(minInc.y, maxInc.y));
        }


        public List<float> UpcomingValueArray { get; set; } = new List<float>() { 0 };
        public int CurrentValueIndex { get; set; } = 0;
    }
}
