using System;
using UnityEngine;

namespace Asteroid.Gameplay
{
    /// <summary>
    /// Api for getting the current input state
    /// </summary>
    public interface IInputHandler
    {

        public event Action PauseToggle;
        public event Action Fire;

        public bool Forward { get; }
        public bool Backward { get; }
        public bool RotateLeft { get; }
        public bool RotateRight { get; }
    }

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
            Forward = Input.GetKey(KeyCode.UpArrow);
            Backward = Input.GetKey(KeyCode.DownArrow);
            RotateLeft = Input.GetKey(KeyCode.LeftArrow);
            RotateRight = Input.GetKey(KeyCode.RightArrow);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Fire?.Invoke();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                PauseToggle?.Invoke();
            }
        }
    }
}