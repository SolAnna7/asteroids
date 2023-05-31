using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Asteroid.Random
{
    public interface IRandomService
    {
        float NextFloat(float minInc, float maxInc);
        float NextFloat();
        Vector2 NextVec2(Vector2 minInc, Vector2 maxInc);
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
