using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Asteroid.Random
{
    /// <summary>
    /// Service for getting random values
    /// </summary>
    public interface IRandomService
    {
        /// <summary>
        /// Returns a float value between min (inclusive) and max (exclusive)
        /// </summary>
        float NextFloat(float min, float max);

        /// <summary>
        /// Returns a float value between 0 (inclusive) and 1 (exclusive)
        /// </summary>
        float NextFloat();

        /// <summary>
        /// Returns a new vector whoes x and y are between min's (inclusive) and max's (exclusive) x and y respectively
        /// </summary>
        Vector2 NextVec2(Vector2 min, Vector2 max);

        /// <summary>
        /// Returns a random unit vector
        /// </summary>
        Vector2 NextUnitVec2();
    }

    internal class RandomService : IRandomService
    {
        private System.Random _random;

        public RandomService(int seed)
        {
            _random = new System.Random(seed);
        }

        public float NextFloat()
        {
            return (float)_random.NextDouble();
        }
        public float NextFloat(float minInc, float maxInc)
        {
            return (float)_random.NextDouble() * (maxInc - minInc) + minInc;
        }

        public Vector2 NextUnitVec2()
        {
            return new Vector2(NextFloat(), NextFloat()).normalized;
        }

        public Vector2 NextVec2(Vector2 minInc, Vector2 maxInc)
        {
            return new Vector2(NextFloat(minInc.x, maxInc.x), NextFloat(minInc.y, maxInc.y));
        }
    }
}
