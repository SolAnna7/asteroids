using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroid.Time
{
    /// <summary>
    /// Service to provide time info
    /// </summary>
    public interface ITimeService
    {
        float Time { get; }
        float RealTime { get; }
        float DeltaTime { get; }
        float FixedDeltaTime { get; }
        bool TimeRunning { get; set; }
    }

    public class DefaultTimeService : ITimeService
    {
        public float Time => UnityEngine.Time.time;

        public float RealTime => UnityEngine.Time.realtimeSinceStartup;

        public float DeltaTime => UnityEngine.Time.deltaTime;

        public float FixedDeltaTime => UnityEngine.Time.fixedDeltaTime;

        public bool TimeRunning { get => UnityEngine.Time.timeScale != 0; set => UnityEngine.Time.timeScale = value ? 1 : 0; }
    }
}
