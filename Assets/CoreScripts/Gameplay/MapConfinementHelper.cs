using Asteroid.Config;
using Asteroid.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Asteroid.Services.ServiceProvider;

namespace Asteroid.Gameplay
{

    public interface IMapConfinementHelper : IInitialisableService
    {
        public bool IsOutOfBounds(Vector2 position, float radius, out Vector2 updatedPosition);
    }

    public class MapConfinementHelper : IMapConfinementHelper
    {

        private Vector2 mapSize = new Vector2(8.642f, 5.5f);

        public void Initialise(ServiceProvider serviceProvider)
        {
            var configStore = serviceProvider.GetService<IConfigStore>();
            mapSize = configStore.MapExtents;
        }

        public bool IsOutOfBounds(Vector2 position, float radius, out Vector2 updatedPosition)
        {
            bool isUpdated = false;
            updatedPosition = position;
            if (position.x < -mapSize.x)
            {
                updatedPosition.x += 2 * mapSize.x;
                isUpdated = true;
            }
            if (position.x > mapSize.x)
            {
                updatedPosition.x -= 2 * mapSize.x;
                isUpdated = true;
            }
            if (position.y < -mapSize.y)
            {
                updatedPosition.y += 2 * mapSize.y;
                isUpdated = true;
            }
            if (position.y > mapSize.y)
            {
                updatedPosition.y -= 2 * mapSize.y;
                isUpdated = true;
            }

            return isUpdated;
        }
    }
}
