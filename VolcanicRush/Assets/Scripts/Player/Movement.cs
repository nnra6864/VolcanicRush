using System.Collections;
using Core;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Movement : MonoBehaviour
    {
        [SerializeField] private Player _player;
        [SerializeField] private Rigidbody2D _rb;
        [SerializeField] private float _groundForce, _dashForce;
        [HideInInspector] public bool IsGrounding, IsDashing;

        private void Start()
        {
            GameManager.OnDied += ResetMovement;
            GameManager.OnRespawned += ResetMovement;
            GameManager.OnPausedPlaying += StopGrounding;
        }

        private void ResetMovement()
        {
            StopGrounding();
            _rb.linearVelocity = Vector2.zero;
        }

        private void Reset()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            if (!GameManager.IsPlaying || GameManager.IsDead) return;
            if (Input.GetKeyDown(KeyCode.D)) Dash();
            if (Input.GetKeyDown(KeyCode.S)) StartGrounding();
            if (Input.GetKeyUp(KeyCode.S)) StopGrounding();
            if (IsGrounding) Ground();
        }

        private void Ground()
        {
            _rb.linearVelocity -= new Vector2(0, _groundForce * Time.deltaTime);
            GameManager.TimeManager.SetTimescale(1, 0);
            GameManager.TimeManager.SetTimescale(GameManager.DefaultTimeScale, 1);
        }
        public void StartGrounding() => IsGrounding = true;
        public void StopGrounding() => IsGrounding = false;
    
        public void Dash()
        {
            if (_dashRoutine != null) return;
            _dashRoutine = StartCoroutine(DashRoutine());
        }
        private Coroutine _dashRoutine;
        private IEnumerator DashRoutine()
        {
            IsDashing = true;
            var startGravity = _rb.gravityScale;
            _rb.gravityScale = 0;
            _rb.linearVelocity = new(_rb.linearVelocity.x <= 0 ? 0 : _rb.linearVelocity.x, 0);
            _rb.AddForce(Vector2.right * _dashForce, ForceMode2D.Impulse);
        
            GameManager.TimeManager.SetTimescale(1.25f, 0);
            GameManager.TimeManager.SetTimescale(GameManager.DefaultTimeScale, 0.5f);
            
            float t = 0;
            while (t < 0.5f)
            {
                if (GameManager.IsPlaying) t += Time.unscaledDeltaTime;
                if (GameManager.IsDead) t = 0.5f;
                else yield return null;
            }
            
            IsDashing = false;
            _rb.gravityScale = startGravity;

            t = 0;
            while (t < 3)
            {
                if (GameManager.IsPlaying) t += Time.unscaledDeltaTime;
                if (GameManager.IsDead) t = 3;
                else yield return null;
            }
            _dashRoutine = null;
        }
    }
}
