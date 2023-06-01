using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Asteroid.Visuals
{
    public class BackgroundController : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _backgroundSprite;

        [SerializeField]
        private Sprite[] _backgrounds;


        // Start is called before the first frame update
        void Start()
        {
            System.Random rnd = new();

            _backgroundSprite.sprite = _backgrounds[rnd.Next(0, _backgrounds.Length)];
            _backgroundSprite.flipX = rnd.Next() % 2 == 0;
        }
    }
}
