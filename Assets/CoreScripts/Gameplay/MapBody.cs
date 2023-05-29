using Asteroid.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Asteroid.Gameplay
{

    public interface IMapBody
    {
        public event Action<IMapBody, Vector2, MapBodyType> OnCollision;
        public MapBodyType Type { get; }
        public Vector2 Forward { get; set; }
        public Vector2 Position { get; set; }
        public void Move(Vector2 position);
        public void Rotate(float angle);
        public void Initialise(ServiceProvider serviceProvider);
        public bool Enabled { get; set; }
        public void Destroy();

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

        public Vector2 Forward
        {
            get => transform.up;
            set => transform.up = value;
        }

        public bool Enabled { get => gameObject.activeSelf; set => gameObject.SetActive(value); }

        public Vector2 Position
        {
            get => _rigidbody2D.position;
            set => _rigidbody2D.position = value;
        }

        public IMapBody.MapBodyType Type => ownType;

        public event Action<IMapBody, Vector2, IMapBody.MapBodyType> OnCollision;

        private void Awake()
        {
            if (ownType == IMapBody.MapBodyType.None)
            {
                throw new ArgumentException($"Map body type is not set for object {gameObject.name}");
            }

            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        public void Initialise(ServiceProvider serviceProvider)
        {
            _mapHelper = serviceProvider.GetService<IMapConfinementHelper>();
        }

        public void Move(Vector2 deltaPosition)
        {
            Vector2 pos = _rigidbody2D.position + deltaPosition;
            _mapHelper.IsOutOfBounds(pos, 1, out Vector2 updatedPos);

            _rigidbody2D.position = updatedPos;
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
