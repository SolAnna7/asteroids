using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Asteroid.Gameplay
{
    /// <summary>
    /// The service for creating new instances of bullet bodies
    /// </summary>
    public interface IBulletBodyFactory
    {
        /// <summary>
        /// Generates a new bullet body
        /// </summary>
        /// <param name="position">Where the bullet should be created on the map</param>
        /// <returns></returns>
        public IMapBody CreateBody(Vector2 position);
    }

    /// <summary>
    /// Loads and instantiates the bullet prefab using unity Resources
    /// </summary>
    public class ResourceBulletBodyFactory : IBulletBodyFactory
    {
        public IMapBody CreateBody(Vector2 position)
        {

            //prefab loading
            MapBody prefab = Resources.Load<MapBody>("Bullet");
            if (prefab == null)
            {
                throw new Exception("Bullet not found");
            }

            var instance = UnityEngine.Object.Instantiate(prefab);

            instance.transform.position = position;

            return instance;
        }
    }
}
