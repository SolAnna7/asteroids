using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Asteroid.Gameplay
{
    /// <summary>
    /// The service for creating new instances of asteroid bodies
    /// </summary>
    public interface IAsteroidBodyFactory
    {
        /// <summary>
        /// Generates a new asteroid body
        /// </summary>
        /// <param name="position">Where the asteroid should be created on the map</param>
        /// <param name="generation">The generation of the new asteroid</param>
        /// <returns></returns>
        public IMapBody CreateBody(Vector2 position, int generation);
    }

    /// <summary>
    /// Loads and instantiates the asteroid prefab using unity Resources
    /// </summary>
    public class ResourceAsteroidBodyFactory : IAsteroidBodyFactory
    {
        public IMapBody CreateBody(Vector2 position, int generation)
        {
            MapBody prefab = Resources.Load<MapBody>("Asteroid");
            if (prefab == null)
            {
                throw new Exception("Asteroid prefab not found");
            }

            var instance = UnityEngine.Object.Instantiate(prefab);

            instance.transform.position = position;
            instance.transform.localScale = instance.transform.localScale * Mathf.Pow(0.7f, generation);

            return instance;
        }
    }
}
