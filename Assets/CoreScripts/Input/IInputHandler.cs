using System;

namespace Asteroid.Input
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
}