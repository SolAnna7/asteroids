using System;
using UnityEngine;

namespace Asteroid.Input
{

    /// <summary>
    /// Input implementation using the old unity input system
    /// </summary>
    public class SimpleInputHandler : IInputHandler
    {
        public event Action PauseToggle;
        public event Action Fire;

        public bool Forward { get; private set; }

        public bool Backward { get; private set; }

        public bool RotateLeft { get; private set; }

        public bool RotateRight { get; private set; }

        public void Tick()
        {
            Forward = UnityEngine.Input.GetKey(KeyCode.UpArrow);
            Backward = UnityEngine.Input.GetKey(KeyCode.DownArrow);
            RotateLeft = UnityEngine.Input.GetKey(KeyCode.LeftArrow);
            RotateRight = UnityEngine.Input.GetKey(KeyCode.RightArrow);

            if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
            {
                Fire?.Invoke();
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
            {
                PauseToggle?.Invoke();
            }
        }
    }
}