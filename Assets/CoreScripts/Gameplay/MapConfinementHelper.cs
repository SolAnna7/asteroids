using Asteroid.Config;
using Asteroid.Services;
using UnityEngine;
using static Asteroid.Services.ServiceProvider;

namespace Asteroid.Gameplay
{
    /// <summary>
    /// Service api to help keeping objects in the map bounds
    /// </summary>
    public interface IMapConfinementHelper : IInitialisableService
    {
        /// <summary>
        /// Tells wheter the specified position is in the map bounds, and returns the confined position if isn't
        /// </summary>
        /// <param name="position">Input position</param>
        /// <param name="radius">The radius of the input object</param>
        /// <param name="updatedPosition">updated output position</param>
        /// <returns>Wheter the position in in the map bounds</returns>
        public bool IsOutOfBounds(Vector2 position, float radius, out Vector2 updatedPosition);
    }


    /// <summary>
    /// Service to help keeping objects in the map bounds
    /// </summary>
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
