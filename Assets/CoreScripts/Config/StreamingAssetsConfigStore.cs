using Asteroid.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Asteroid.Config
{
    /// <summary>
    /// Loads the config values from the StreamingAssets/Config/config.str file
    /// The file format is a simplified version of the .properties file format (https://en.wikipedia.org/wiki/.properties) with only single line values and comments
    /// </summary>
    public class StreamingAssetsConfigStore : IConfigStore, ServiceProvider.IInitialisableService
    {

        private IDictionary<string, string> _configValues = new Dictionary<string, string>();
        private string _configPath;

        public StreamingAssetsConfigStore()
        {
        }

        public void Initialise(ServiceProvider serviceProvider)
        {
            _configPath = Path.Combine(Application.streamingAssetsPath, "Config", "config.str");
            if (!File.Exists(_configPath))
            {
                throw new FileNotFoundException("Config file not found: " + _configPath);
            }
            foreach (var rowi in File.ReadAllLines(_configPath))
            {
                string row = rowi;
                if (row.Contains("#"))
                {
                    row = row.Split("#")[0];
                }
                if (string.IsNullOrWhiteSpace(row))
                {
                    continue;
                }
                _configValues.Add(row.Split('=')[0], string.Join("=", row.Split('=').Skip(1).ToArray()));
            }
        }

        public float ShipMaxSpeed => GetFloatValueOrThrowException("ship.max_speed");

        public float ShipRotationSpeed => GetFloatValueOrThrowException("ship.rotation_speed");

        public float ShipAccelearation => GetFloatValueOrThrowException("ship.acceleration");

        public float ShipDecelearation => GetFloatValueOrThrowException("ship.deceleration");

        public int ShipMaxHealth => GetIntValueOrThrowException("ship.max_health");

        public Vector2 MapExtents => new Vector2(GetFloatValueOrThrowException("map.ext.x"), GetFloatValueOrThrowException("map.ext.y"));

        public float AsteroidSpeed => GetFloatValueOrThrowException("asteroid.speed");

        public float AsteroidFreeZoneAtStartupRadius => GetFloatValueOrThrowException("asteroid.free_spawn_radius");

        public float ShipInvulnerablilitySec => GetFloatValueOrThrowException("ship.invulnerability_sec");

        public float AsteroidInvulnerablilitySec => GetFloatValueOrThrowException("asteroid.invulnerability_sec");

        public float BulletSpeed => GetFloatValueOrThrowException("bullet.speed");

        public float BulletFireInterval => GetFloatValueOrThrowException("bullet.fire_interval");

        public float BulletLifetime => GetFloatValueOrThrowException("bullet.lifetime");

        public int[] AsteroidScoresByGeneration => new int[]
                {
                GetIntValueOrThrowException("asteroid.score_by_gen.0"),
                GetIntValueOrThrowException("asteroid.score_by_gen.1"),
                GetIntValueOrThrowException("asteroid.score_by_gen.2"),
                GetIntValueOrThrowException("asteroid.score_by_gen.3"),
            };

        public int AsteroidMaxGeneration => GetIntValueOrThrowException("asteroid.max_generation");

        public int AsteroidStartNumber => GetIntValueOrThrowException("asteroid.start_number");

        private string GetStringValueOrThrowException(string key)
        {
            return _configValues.TryGetValue(key, out var value) ? value : throw new Exception($"Key [{key}] not found in configuration file [{_configPath}]");
        }
        private float GetFloatValueOrThrowException(string key) => float.Parse(GetStringValueOrThrowException(key));
        private int GetIntValueOrThrowException(string key) => int.Parse(GetStringValueOrThrowException(key));
    }
}
