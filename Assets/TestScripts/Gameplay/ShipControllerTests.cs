using Asteroid.Config;
using Asteroid.Gameplay;
using Asteroid.Input;
using Asteroid.Services;
using Asteroid.Time;
using NUnit.Framework;
using UnityEngine;

namespace Asteroid.Tests.Gameplay
{
    public class ShipControllerTests
    {
        [Test]
        public void TestCollisionAndDeath()
        {
            MapBodyMock bodyMock = new();
            TimeMock time = new();
            ConfigMock config = new();
            InputMock input = new();
            BulletControllerMock bullets = new();
            ShipController ship = new(bodyMock);

            bool hasShipDied = false;
            ship.OnDeath += () => hasShipDied = true;
            config.ShipMaxHealth = 2;

            var sp = ServiceProvider.Build()
                .RegisterService<IInputHandler>(input)
                .RegisterService<ITimeService>(time)
                .RegisterService<IConfigStore>(config)
                .RegisterService<IBulletController>(bullets)
                .RegisterService<IShipController>(ship)
                .Initialise();

            Assert.AreEqual(ship, sp.GetService<IShipController>());

            Assert.AreEqual(2, ship.CurrentHealth);

            bodyMock.InvokeOnCollision(default, IMapBody.MapBodyType.Bullet);

            Assert.AreEqual(2, ship.CurrentHealth);

            bodyMock.InvokeOnCollision(default, IMapBody.MapBodyType.Asteroid);

            Assert.AreEqual(1, ship.CurrentHealth);
            Assert.AreEqual(false, hasShipDied);

            bodyMock.InvokeOnCollision(default, IMapBody.MapBodyType.Asteroid);
            Assert.AreEqual(true, hasShipDied);
        }

        [Test]
        public void TestInvilnerability()
        {
            MapBodyMock bodyMock = new();
            TimeMock time = new();
            ConfigMock config = new();
            InputMock input = new();
            BulletControllerMock bullets = new();
            ShipController ship = new(bodyMock);

            config.ShipMaxHealth = 2;
            config.ShipInvulnerablilitySec = 1;

            ServiceProvider.Build()
                .RegisterService<IInputHandler>(input)
                .RegisterService<ITimeService>(time)
                .RegisterService<IConfigStore>(config)
                .RegisterService<IBulletController>(bullets)
                .RegisterService<IShipController>(ship)
                .Initialise();

            Assert.AreEqual(2, ship.CurrentHealth);

            bodyMock.InvokeOnCollision(default, IMapBody.MapBodyType.Asteroid);

            time.Time = 0.5f;

            bodyMock.InvokeOnCollision(default, IMapBody.MapBodyType.Asteroid);
            Assert.AreEqual(2, ship.CurrentHealth);

            time.Time = 1.1f;

            bodyMock.InvokeOnCollision(default, IMapBody.MapBodyType.Asteroid);
            Assert.AreEqual(1, ship.CurrentHealth);
        }

        [Test]
        public void TestFire()
        {
            MapBodyMock bodyMock = new();
            TimeMock time = new();
            ConfigMock config = new();
            InputMock input = new();
            BulletControllerMock bullets = new();
            ShipController ship = new(bodyMock);

            bodyMock.Position = new Vector2(2, 3);
            bodyMock.Forward = new Vector2(7, 11).normalized;

            ServiceProvider.Build()
                .RegisterService<IInputHandler>(input)
                .RegisterService<ITimeService>(time)
                .RegisterService<IConfigStore>(config)
                .RegisterService<IBulletController>(bullets)
                .RegisterService<IShipController>(ship)
                .Initialise();

            Assert.AreEqual(0, bullets.FiredBulletCntr);
            Assert.AreEqual(Vector2.zero, bullets.LastBulletPosition);
            Assert.AreEqual(Vector2.zero, bullets.LastBulletForward);

            input.InvokeFire();

            Assert.AreEqual(1, bullets.FiredBulletCntr);
            Assert.AreEqual(bodyMock.Position, bullets.LastBulletPosition);
            Assert.AreEqual(bodyMock.Forward, bullets.LastBulletForward);

            time.Time = 1f;
            input.InvokeFire();
            Assert.AreEqual(2, bullets.FiredBulletCntr);
        }

        [Test]
        public void TestControl()
        {
            MapBodyMock bodyMock = new();
            TimeMock time = new();
            ConfigMock config = new();
            InputMock input = new();
            BulletControllerMock bullets = new();
            ShipController ship = new(bodyMock);

            config.ShipAccelearation = 0.3f;
            config.ShipDecelearation = 0.3f;
            config.ShipMaxSpeed = 1;
            config.ShipRotationSpeed = 90;
            time.FixedDeltaTime = 0.2f;
            bodyMock.Forward = Vector2.up;

            ServiceProvider.Build()
                .RegisterService<IInputHandler>(input)
                .RegisterService<ITimeService>(time)
                .RegisterService<IConfigStore>(config)
                .RegisterService<IBulletController>(bullets)
                .RegisterService<IShipController>(ship)
                .Initialise();

            ship.FixedTick();
            Assert.AreEqual(Vector2.zero, bodyMock.LastMoveValue);
            Assert.AreEqual(0, bodyMock.LastRotationValue);

            input.RotateLeft = true;
            ship.FixedTick();
            Assert.AreEqual(Vector2.zero, bodyMock.LastMoveValue);
            Assert.AreEqual(config.ShipRotationSpeed * time.FixedDeltaTime, bodyMock.LastRotationValue);

            input.RotateLeft = false;

            input.Backward = true;
            ship.FixedTick();
            Assert.AreEqual(config.ShipDecelearation * time.FixedDeltaTime * -bodyMock.Forward, bodyMock.LastMoveValue);
            input.Backward = false;

            input.Forward = true;
            ship.FixedTick();
            ship.FixedTick();
            Assert.AreEqual(config.ShipAccelearation * time.FixedDeltaTime * bodyMock.Forward, bodyMock.LastMoveValue);

            for (int i = 0; i < 50; i++)
            {
                ship.FixedTick();
            }
            Assert.AreEqual(config.ShipMaxSpeed * bodyMock.Forward, bodyMock.LastMoveValue);
        }

        private class BulletControllerMock : IBulletController
        {
            public void FireBullet(Vector2 position, Vector2 forward)
            {
                LastBulletPosition = position;
                LastBulletForward = forward;

                FiredBulletCntr++;
            }

            public Vector2 LastBulletPosition { get; private set; }
            public Vector2 LastBulletForward { get; private set; }
            public int FiredBulletCntr { get; private set; } = 0;
        }
    }
}
