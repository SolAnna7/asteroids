using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Asteroid.Gameplay
{
    public interface IBulletBodyFactory
    {
        public IMapBody CreateBody(Vector2 position);
    }

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
