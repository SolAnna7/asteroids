using Asteroid.Config;
using UnityEngine;

namespace Asteroid.Tests
{
    internal class ConfigMock : IConfigStore
    {
        public Vector2 MapExtents { get; set; }

        public float ShipMaxSpeed { get; set; }

        public float ShipRotationSpeed { get; set; }

        public float ShipAccelearation { get; set; }

        public float ShipDecelearation { get; set; }

        public int ShipMaxHealth { get; set; }

        public float ShipInvulnerablilitySec { get; set; }

        public float AsteroidSpeed { get; set; }

        public float AsteroidFreeZoneAtStartupRadius { get; set; }

        public float AsteroidInvulnerablilitySec { get; set; }

        public float BulletSpeed { get; set; }

        public float BulletFireInterval { get; set; }

        public float BulletLifetime { get; set; }

        public int[] AsteroidScoresByGeneration { get; set; }

        public int AsteroidMaxGeneration { get; set; }

        public int AsteroidStartNumber { get; set; }
    }
}
