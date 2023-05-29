using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Asteroid.Gameplay
{
    public interface IAsteroidBodyFactory
    {
        public IMapBody CreateBody(Vector2 position, int generation);
    }

    public class ResourceAsteroidBodyFactory : IAsteroidBodyFactory
    {
        public IMapBody CreateBody(Vector2 position, int generation)
        {

            //prefab loading
            MapBody prefab = Resources.Load<MapBody>("Asteroid");
            if (prefab == null)
            {
                throw new Exception("Asteroid not found");
            }

            var instance = UnityEngine.Object.Instantiate(prefab);

            instance.transform.position = position;
            instance.transform.localScale = instance.transform.localScale * Mathf.Pow(0.7f, generation);

            return instance;
        }
    }
}
