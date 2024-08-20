using System.Collections;
using Core;
using UnityEngine;
using UnityEngine.U2D;

namespace Level
{
    [RequireComponent(typeof(SpriteShapeController), typeof(BoxCollider2D))]
    public class Wall : MonoBehaviour
    {
        [SerializeField] private SpriteShapeController _shape;
        [SerializeField] private SpriteShapeRenderer _rend;
        [SerializeField] private BoxCollider2D _col;
        [SerializeField] private Vector2 xRange, yRange;
        [SerializeField] private float _moveAmount;
        [SerializeField] private Vector2 _moveAmountRange;
        private Transform _player;
        private float _cooldown;

        private float _xScale => xRange.y - xRange.x;
        private float _yScale => yRange.y - yRange.x;

        private void Reset()
        {
            _shape = GetComponent<SpriteShapeController>();
            _rend = GetComponent<SpriteShapeRenderer>();
            _col = GetComponent<BoxCollider2D>();
        }

        private void Start()
        {
            GenerateWall();
            UpdateRendererBounds();
            GameManager.OnStartedPlaying += OnStarted;
            GameManager.OnPausedPlaying += () =>
            {
                StopCoroutine(_moveWallRoutine);
                _moveWallRoutine = null;
            };
            GameManager.OnResumedPlaying += () => _moveWallRoutine = StartCoroutine(MoveWallRoutine());
            GameManager.OnDied += OnDied;
            GameManager.OnRespawned += OnRespawned;
        }

        private void OnStarted()
        {
            _player = GameObject.FindWithTag("Player").transform;
            _cooldown = 3;
            _moveWallRoutine = StartCoroutine(MoveWallRoutine());
        }

        private void OnDied()
        {
            if (_moveWallRoutine == null) return;
            StopCoroutine(_moveWallRoutine);
            _moveWallRoutine = null;
        }
        
        private void OnRespawned()
        {
            transform.localPosition = new(_player.position.x - 150, 0, 0);
            _cooldown = 3;
            _moveWallRoutine = StartCoroutine(MoveWallRoutine());
        }
    
        private void GenerateWall()
        {
            var x = _xScale / 2;
            var y = _yScale / 2;
            var s = _shape.spline;
        
            s.Clear();
            s.InsertPointAt(0, new(-x, y));
            s.InsertPointAt(1, new(-x, -y));
            s.InsertPointAt(2, new(x, -y));
            s.InsertPointAt(3, new(x, y));
        
            _col.size = new(_xScale, _yScale);
        }

        private Coroutine _moveWallRoutine;
        private IEnumerator MoveWallRoutine()
        {
            while (_cooldown > 0)
            {
                _cooldown = Mathf.Clamp(_cooldown - Time.unscaledDeltaTime, 0, Mathf.Infinity);
                yield return null;
            }
            while (true)
            {
                var t = transform;
                var ma = _moveAmount * (_player.position.x - (t.position.x + _xScale / 2));
                ma = Mathf.Clamp(ma, _moveAmountRange.x, _moveAmountRange.y) * Time.unscaledDeltaTime;
                t.position += new Vector3(ma, 0);
                UpdateRendererBounds();
                yield return null;
            }
        }
    
        private void UpdateRendererBounds()
        {
            var b = _rend.localBounds;
            var min = _shape.spline.GetPosition(1);
            var max = _shape.spline.GetPosition(_shape.spline.GetPointCount() - 1);
            max.x += 1000;
            b.SetMinMax(min, max);
            _rend.localBounds = b;
        }

    }
}
