using System.Collections;
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
        private float _speed;

        private void Start()
        {
            transform.localScale = Vector3.zero;
            _speed = Random.Range(_speedRange.x, _speedRange.y);
            StartCoroutine(Spawn());
        }

        private void Reset() => _rb = GetComponent<Rigidbody2D>();

        private void Update()
        {
            _rb.velocity += (Vector2)transform.up * (_speed * Time.deltaTime);
        }

        private IEnumerator Spawn()
        {
            float lerpPos = 0;
            var spawnTime = Random.Range(0.25f, 1f);
            var size = Vector3.one * Random.Range(5f, 10f);
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
            Destroy(gameObject);
        }
    }
}
