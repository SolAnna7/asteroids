using Asteroid.Gameplay;
using Asteroid.Services;
using System;
using UnityEngine;

namespace Asteroid.Tests
{
    internal class MapBodyMock : IMapBody
    {
        public IMapBody.MapBodyType Type { get; set; }

        public Vector2 Forward { get; set; }
        public Vector2 Position { get; set; }
        public bool Enabled { get; set; } = true;

        public event Action<IMapBody, Vector2, IMapBody.MapBodyType> OnCollision;

        public void Destroy() => IsDestroyCalled = true;

        public void Initialise(ServiceProvider serviceProvider)
        {
        }

        public void MoveToPosition(Vector2 position) => LastMoveValue = position;

        public void Rotate(float angle) => LastRotationValue = angle;

        public bool IsDestroyCalled { get; private set; } = false;
        public Vector2 LastMoveValue { get; private set; }
        public float LastRotationValue { get; private set; }

        public void InvokeOnCollision(Vector2 pos, IMapBody.MapBodyType bodyType) => OnCollision?.Invoke(this, pos, bodyType);
    }
}
