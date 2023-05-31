using Asteroid.Config;
using Asteroid.Services;
using NUnit.Framework;

namespace Asteroid.Tests
{
    public class ConfigStoreTests
    {
        [Test]
        public void TestStreamingAssetsConfogStore()
        {
            var sp = ServiceProvider.Build()
                .RegisterService<IConfigStore>(new StreamingAssetsConfigStore())
                .Initialise();

            var store = sp.GetService<IConfigStore>();

            Assert.IsNotNull(store.ShipMaxSpeed);
        }

    }
}
