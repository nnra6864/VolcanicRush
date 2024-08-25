using System.Collections;
using System.Collections.Generic;
using Core;
using NnUtils.Scripts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Level
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Meteor : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D _rb;
        [SerializeField] private TrailRenderer _trailRenderer;
        [SerializeField] private Vector2 _speedRange;
        [SerializeField] private AnimationCurve _spawnCurve;
        [SerializeField] private ParticleSystem _particles;
        [SerializeField] private List<ParticleSystem> _deathParticles;
        private float _speed;
        private Transform _player;

        private void Start()
        {
            transform.localScale = Vector3.zero;
            _speed = Random.Range(_speedRange.x, _speedRange.y);
            _player = GameManager.Player.transform;
            StartCoroutine(Spawn());
        }

        private void Reset() => _rb = GetComponent<Rigidbody2D>();

        private void Update()
        {
            _rb.linearVelocity += (Vector2)transform.up * (_speed * Time.deltaTime);
        }

        private IEnumerator Spawn()
        {
            float lerpPos = 0;
            var spawnTime = _player.position.y > 400 ? 0 : Random.Range(0.25f, 1f);
            var size = Vector3.one * (_player.position.y > 400 ? 30f : Random.Range(5f, 10f));
            var keys = _trailRenderer.widthCurve.keys;
            var endTrailSize =  keys[0].value * size.x;
            while (lerpPos < 1)
            {
                var t = _spawnCurve.Evaluate(Misc.UpdateLerpPos(ref lerpPos, spawnTime, true));
                transform.localScale = size * t;
                keys[0].value = endTrailSize * t;
                _trailRenderer.widthCurve = new (keys);
                yield return null;
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Projectile"))
                GameManager.Score += (int)Misc.Remap(transform.localScale.x - 4.5f, 1, 10, 10, 1);
            Explode();
        }
        
        private void Explode()
        {
            _particles.Stop();
            _particles.transform.SetParent(null);
            Destroy(_particles.gameObject, _particles.main.startLifetime.constantMax + 1);
            NnManager.AudioManager.PlayAt("MeteorExplosion", transform.position);
            foreach (var particles in _deathParticles)
            {
                particles.transform.SetParent(null);
                particles.Play();
                Destroy(particles.gameObject, particles.main.startLifetime.constantMax + 1);
            }
            Destroy(gameObject);
        }
    }
}
