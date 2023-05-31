using Asteroid.Config;
using Asteroid.Gameplay;
using Asteroid.Services;
using Asteroid.Time;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Asteroid.Tests.Gameplay
{
    public class BulletControllerTest
    {
        [Test]
        public void InitTest()
        {

            TimeMock time = new();
            ConfigMock config = new();
            BulletFactoryMock factory = new();
            BulletController bulletController = new();

            ServiceProvider.Build()
                .RegisterService<ITimeService>(time)
                .RegisterService<IBulletBodyFactory>(factory)
                .RegisterService<IConfigStore>(config)
                .RegisterService<IBulletController>(bulletController)
                .Initialise();
        }

        [Test]
        public void FireTest()
        {

            TimeMock time = new();
            ConfigMock config = new();
            BulletFactoryMock factory = new();
            BulletController bulletController = new();

            config.BulletFireInterval = 0.5f;
            config.BulletLifetime = 1f;
            config.BulletSpeed = 0.3f;
            time.FixedDeltaTime = 0.1f;

            ServiceProvider.Build()
                .RegisterService<ITimeService>(time)
                .RegisterService<IBulletBodyFactory>(factory)
                .RegisterService<IConfigStore>(config)
                .RegisterService<IBulletController>(bulletController)
                .Initialise();

            bulletController.FireBullet(new Vector2(1, 0), new Vector2(0, 1));
            Assert.AreEqual(1, factory.CreatedBodies.Count);
            bulletController.FixedTick();
            Assert.AreEqual(new Vector2(0, config.BulletSpeed * time.FixedDeltaTime), factory.CreatedBodies.First().LastMoveValue);

            //fire interval block test
            time.Time = 0.3f;
            bulletController.FireBullet(new Vector2(1, 0), new Vector2(0, 1));
            bulletController.FixedTick();
            Assert.AreEqual(1, factory.CreatedBodies.Count);

            //fire again after interval
            time.Time = 0.7f;
            bulletController.FireBullet(new Vector2(1, 0), new Vector2(0, 1));
            bulletController.FixedTick();
            Assert.AreEqual(2, factory.CreatedBodies.Count);

            //bullet end of life
            Assert.IsTrue(factory.CreatedBodies[0].Enabled);
            time.Time = 1.1f;
            bulletController.FixedTick();
            bulletController.FixedTick();
            Assert.IsFalse(factory.CreatedBodies[0].Enabled);
            Assert.IsTrue(factory.CreatedBodies[1].Enabled);

            //fire again, no new instance created, but first is reused
            time.Time = 2f;
            bulletController.FireBullet(new Vector2(1, 0), new Vector2(0, 1));
            bulletController.FixedTick();
            Assert.AreEqual(2, factory.CreatedBodies.Count);
            Assert.IsTrue(factory.CreatedBodies[0].Enabled);
        }

        [Test]
        public void CollisionTest()
        {
            TimeMock time = new();
            ConfigMock config = new();
            BulletFactoryMock factory = new();
            BulletController bulletController = new();

            ServiceProvider.Build()
                .RegisterService<ITimeService>(time)
                .RegisterService<IBulletBodyFactory>(factory)
                .RegisterService<IConfigStore>(config)
                .RegisterService<IBulletController>(bulletController)
                .Initialise();

            bulletController.FireBullet(new Vector2(1, 0), new Vector2(0, 1));
            Assert.AreEqual(1, factory.CreatedBodies.Count);
            bulletController.FixedTick();
            Assert.IsTrue(factory.CreatedBodies[0].Enabled);

            factory.CreatedBodies[0].InvokeOnCollision(Vector2.zero, IMapBody.MapBodyType.Asteroid);
            bulletController.FixedTick();
            Assert.IsFalse(factory.CreatedBodies[0].Enabled);
        }

        private class BulletFactoryMock : IBulletBodyFactory
        {
            public List<MapBodyMock> CreatedBodies { get; } = new List<MapBodyMock>();
            public IMapBody CreateBody(Vector2 position)
            {
                MapBodyMock mapBodyMock = new MapBodyMock();
                CreatedBodies.Add(mapBodyMock);
                return mapBodyMock;
            }
        }
    }
}
