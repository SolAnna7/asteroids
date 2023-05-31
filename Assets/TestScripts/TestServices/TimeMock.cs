using Asteroid.Time;

namespace Asteroid.Tests
{
    internal class TimeMock : ITimeService
    {
        public float Time { get; set; }

        public float RealTime { get; set; }

        public float DeltaTime { get; set; }

        public float FixedDeltaTime { get; set; }

        public bool TimeRunning { get; set; }
    }
}
