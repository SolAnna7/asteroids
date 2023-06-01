using Asteroid.Services;
using System;
using UnityEngine;

namespace Asteroid.Gameplay
{
    /// <summary>
    /// General api for an object that is on the game map
    /// </summary>
    public interface IMapBody
    {
        /// <summary>
        /// Invoked when the body collides with an other.
        /// The parameters are the body that invokes it, the position of the other body, and the type of the other body 
        /// </summary>
        public event Action<IMapBody, Vector2, MapBodyType> OnCollision;

        /// <summary>
        /// The MapBodyType of this body
        /// </summary>
        public MapBodyType Type { get; }

        /// <summary>
        /// The vector this body faces
        /// </summary>
        public Vector2 Forward { get; set; }

        /// <summary>
        /// The position of this body on the map
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Moves the body with the specified deltaPosition on the map
        /// </summary>
        /// <param name="deltaPosition">The position to move the body with</param>
        public void Move(Vector2 deltaPosition);

        /// <summary>
        /// Rotates the body with the specified angle. Clockwise is negative.
        /// </summary>
        /// <param name="angle">The angle to rotate the body with</param>
        public void Rotate(float angle);

        /// <summary>
        /// Sets up the body
        /// </summary>
        public void Initialise(ServiceProvider serviceProvider);

        /// <summary>
        /// Enables/Disables the body, but does not destroy it
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Destroys the body
        /// </summary>
        public void Destroy();

        /// <summary>
        /// Returns the radius of the body
        /// </summary>
        /// <returns></returns>
        public float Radius { get; }

        public enum MapBodyType
        {
            None = 0,
            Ship,
            Asteroid,
            UFO,
            Bullet,
        }
    }


    public class MapBody : MonoBehaviour, IMapBody
    {
        [SerializeField]
        private IMapBody.MapBodyType ownType = IMapBody.MapBodyType.None;

        private IMapConfinementHelper _mapHelper;
        private Rigidbody2D _rigidbody2D;
        private CircleCollider2D _collider;

        public Vector2 Forward
        {
            get => transform.up;
            set => transform.up = value;
        }

        public bool Enabled { get => gameObject.activeSelf; set => gameObject.SetActive(value); }

        public Vector2 Position
        {
            get => _rigidbody2D.position;
            set
            {
                _mapHelper.IsOutOfBounds(value, 1, out Vector2 updatedPos);
                _rigidbody2D.position = updatedPos;
            }
        }

        public IMapBody.MapBodyType Type => ownType;

        public float Radius => _collider.radius * transform.lossyScale.x;

        public event Action<IMapBody, Vector2, IMapBody.MapBodyType> OnCollision;

        private void Awake()
        {
            if (ownType == IMapBody.MapBodyType.None)
            {
                throw new ArgumentException($"Map body type is not set for object {gameObject.name}");
            }

            _rigidbody2D = GetComponent<Rigidbody2D>();
            _collider = GetComponent<CircleCollider2D>();
        }

        public void Initialise(ServiceProvider serviceProvider)
        {
            _mapHelper = serviceProvider.GetService<IMapConfinementHelper>();
        }

        public void Move(Vector2 deltaPosition)
        {
            Position = _rigidbody2D.position + deltaPosition;
        }

        public void Rotate(float angle)
        {
            transform.Rotate(Vector3.forward, angle);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var otherBody = (IMapBody)other.GetComponent(typeof(IMapBody));
            OnCollision?.Invoke(this, other.transform.position, otherBody.Type);
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}
