using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Asteroid.Visuals
{
    public class BulletEmissionController : MonoBehaviour
    {

        [SerializeField]
        private ParticleSystem _emission;

        private Transform _bullet;

        private void Awake()
        {
            _bullet = transform.parent;
            transform.SetParent(null);
        }

        private void Update()
        {
            transform.position = _bullet.position;
            transform.rotation = _bullet.rotation;

            if (_bullet.gameObject.gameObject.activeSelf && !_emission.isPlaying)
            {
                _emission.Clear();
                _emission.Play();
            }
            else if(!_bullet.gameObject.gameObject.activeSelf && _emission.isPlaying)
            {
                _emission.Stop();
            }
        }
    }
}
