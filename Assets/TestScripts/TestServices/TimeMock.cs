using Asteroid.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
