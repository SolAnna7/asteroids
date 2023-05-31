using Asteroid.Config;
using Asteroid.Gameplay;
using Asteroid.Random;
using Asteroid.Services;
using Asteroid.Time;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Asteroid.Tests.Gameplay
{
    public class AsteroidControllerTests
    {
        [Test]
        public void AsteroidInitTest()
        {

            TimeMock time = new();
            ConfigMock config = new();
            RandomMock random = new();
            AsteroidFactoryMock factory = new();
            AsteroidController asteroidController = new();

            config.AsteroidStartNumber = 4;

            ServiceProvider.Build()
                .RegisterService<ITimeService>(time)
                .RegisterService<IRandomService>(random)
                .RegisterService<IAsteroidBodyFactory>(factory)
                .RegisterService<IConfigStore>(config)
                .RegisterService<IAsteroidController>(asteroidController)
                .Initialise();

            Assert.AreEqual(config.AsteroidStartNumber, factory.CreatedBodies.Count);
        }

        [Test]
        public void AsteroidBreakingTest()
        {

            TimeMock time = new();
            ConfigMock config = new();
            RandomMock random = new();
            AsteroidFactoryMock factory = new();
            AsteroidController asteroidController = new();

            config.AsteroidStartNumber = 1;
            config.AsteroidMaxGeneration = 2;
            config.AsteroidInvulnerablilitySec = 0.2f;

            ServiceProvider.Build()
                .RegisterService<ITimeService>(time)
                .RegisterService<IRandomService>(random)
                .RegisterService<IAsteroidBodyFactory>(factory)
                .RegisterService<IConfigStore>(config)
                .RegisterService<IAsteroidController>(asteroidController)
                .Initialise();

            Assert.AreEqual(config.AsteroidStartNumber, factory.CreatedBodies.Count);

            time.Time = 0.1f;
            factory.CreatedBodies.Last().InvokeOnCollision(Vector2.zero, IMapBody.MapBodyType.Bullet);
            asteroidController.FixedTick();
            //still invulnerable after creation
            Assert.AreEqual(config.AsteroidStartNumber, factory.CreatedBodies.Count);

            time.Time = 0.5f;
            factory.CreatedBodies.Last().InvokeOnCollision(Vector2.zero, IMapBody.MapBodyType.Bullet);
            asteroidController.FixedTick();
            Assert.AreEqual(config.AsteroidStartNumber + 2, factory.CreatedBodies.Count);

            time.Time = 1f;
            factory.CreatedBodies.Last().InvokeOnCollision(Vector2.zero, IMapBody.MapBodyType.Bullet);
            asteroidController.FixedTick();
            Assert.AreEqual(config.AsteroidStartNumber + 4, factory.CreatedBodies.Count);
            Assert.IsFalse(factory.CreatedBodies.Last().IsDestroyCalled);

            //reached max generation
            time.Time = 1.5f;
            factory.CreatedBodies.Last().InvokeOnCollision(Vector2.zero, IMapBody.MapBodyType.Bullet);
            asteroidController.FixedTick();
            Assert.AreEqual(config.AsteroidStartNumber + 4, factory.CreatedBodies.Count);
            Assert.IsTrue(factory.CreatedBodies.Last().IsDestroyCalled);
        }


        [Test]
        public void AllAsteroidsDestroyedTest()
        {
            TimeMock time = new();
            ConfigMock config = new();
            RandomMock random = new();
            AsteroidFactoryMock factory = new();
            AsteroidController asteroidController = new();

            bool allAsteroidsDestroyed = false;
            config.AsteroidStartNumber = 1;
            config.AsteroidMaxGeneration = 2;
            asteroidController.OnAllAsteroidsDestroyed += () => allAsteroidsDestroyed = true;

            ServiceProvider.Build()
                .RegisterService<ITimeService>(time)
                .RegisterService<IRandomService>(random)
                .RegisterService<IAsteroidBodyFactory>(factory)
                .RegisterService<IConfigStore>(config)
                .RegisterService<IAsteroidController>(asteroidController)
                .Initialise();

            Assert.AreEqual(config.AsteroidStartNumber, factory.CreatedBodies.Count);

            int cntr = 0;

            while (factory.CreatedBodies.Any(b => !b.IsDestroyCalled))
            {
                if (cntr > 1000)
                    throw new Exception("Something went wrong :(");
                factory.CreatedBodies.Where(b => !b.IsDestroyCalled).First().InvokeOnCollision(Vector2.zero, IMapBody.MapBodyType.Bullet);
                asteroidController.FixedTick();
                cntr++;
            }

            Assert.IsTrue(allAsteroidsDestroyed);
        }

        private class AsteroidFactoryMock : IAsteroidBodyFactory
        {
            public List<MapBodyMock> CreatedBodies { get; } = new List<MapBodyMock>();
            public IMapBody CreateBody(Vector2 position, int generation)
            {
                MapBodyMock mapBodyMock = new MapBodyMock();
                CreatedBodies.Add(mapBodyMock);
                return mapBodyMock;
            }
        }
    }
}
