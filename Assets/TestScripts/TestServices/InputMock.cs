using Asteroid.Input;
using System;

namespace Asteroid.Tests
{
    internal class InputMock : IInputHandler
    {
        public bool Forward { get; set; }

        public bool Backward { get; set; }

        public bool RotateLeft { get; set; }

        public bool RotateRight { get; set; }

        public event Action PauseToggle;
        public event Action Fire;


        public void InvokePauseToggle() => PauseToggle?.Invoke();
        public void InvokeFire() => Fire?.Invoke();
    }
}
