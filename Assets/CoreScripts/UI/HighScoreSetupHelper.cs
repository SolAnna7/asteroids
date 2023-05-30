using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroid.UI
{
    public static class HighScoreSetupHelper
    {
        public static HighScoreOpenMode OpenMode { get; set; }
        public static int NewScore { get; set; }

        public enum HighScoreOpenMode
        {
            FromMenu,
            FromGame
        }
    }
}
