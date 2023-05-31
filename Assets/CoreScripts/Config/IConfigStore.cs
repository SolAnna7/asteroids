using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Asteroid.Config
{
    /// <summary>
    /// General interface for accessing global configuration values
    /// Later when there are too many fields it should be separated into multiple interfaces
    /// </summary>
    public interface IConfigStore
    {
        Vector2 MapExtents { get; }
        float ShipMaxSpeed { get; }
        float ShipRotationSpeed { get; }
        float ShipAccelearation { get; }
        float ShipDecelearation { get; }
        int ShipMaxHealth { get; }
        float ShipInvulnerablilitySec { get; }
        float AsteroidSpeed { get; }
        int AsteroidStartNumber { get; }
        float AsteroidFreeZoneAtStartupRadius { get; }
        float AsteroidInvulnerablilitySec { get; }
        float BulletSpeed { get; }
        float BulletFireInterval { get; }
        float BulletLifetime { get; }
        int[] AsteroidScoresByGeneration { get; }
        int AsteroidMaxGeneration { get; }
    }
}
