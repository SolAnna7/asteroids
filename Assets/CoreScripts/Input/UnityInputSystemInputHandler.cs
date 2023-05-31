using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroid.Input
{
    /// <summary>
    /// Input handler using Unity's new Input System
    /// </summary>
    public class UnityInputSystemInputHandler : IInputHandler
    {
        private readonly AsteroidsInputGenerated _inputSource;

        public UnityInputSystemInputHandler()
        {
            _inputSource = new AsteroidsInputGenerated();
            _inputSource.Enable();

            _inputSource.GameplayMap.Pause.started += (_) => PauseToggle?.Invoke();
            _inputSource.GameplayMap.Fire.started += (_) => Fire?.Invoke();
        }

        public bool Forward => _inputSource.GameplayMap.Forward.IsPressed();

        public bool Backward => _inputSource.GameplayMap.Backward.IsPressed();

        public bool RotateLeft => _inputSource.GameplayMap.RotateLeft.IsPressed();

        public bool RotateRight => _inputSource.GameplayMap.RotateRight.IsPressed();

        public event Action PauseToggle;
        public event Action Fire;
    }
}
