using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroid.Time
{
    public interface ITimeService
    {
        float Time { get; }
        float RealTime { get; }
        float DeltaTime { get; }
        float FixedDeltaTime { get; }
    }

    public class DefaultTimeService : ITimeService
    {
        public float Time => UnityEngine.Time.time;

        public float RealTime => UnityEngine.Time.realtimeSinceStartup;

        public float DeltaTime => UnityEngine.Time.deltaTime;

        public float FixedDeltaTime => UnityEngine.Time.fixedDeltaTime;
    }
}
