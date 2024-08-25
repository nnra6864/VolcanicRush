using System;
using System.Collections;
using Core;
using NnUtils.Scripts;
using UnityEngine;

namespace Player
{
    public class Weapon : NnBehaviour
    {
        [SerializeField] private Projectile _projectilePrefab;
        [SerializeField] private LineRenderer _trajectory;
        [SerializeField] private Gradient _gradient, _hitGradient;
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private Rigidbody2D _playerRb;

        private void Start()
        {
            GameManager.OnPausedPlaying += StopShooting;
            GameManager.OnDied += StopShooting;
        }

        private void Update()
        {
            if (!GameManager.IsPlaying || Misc.IsPointerOverUI) return;
            if (Input.GetKeyDown(KeyCode.Mouse0)) 
                RestartRoutine(ref _shootRoutine, ShootRoutine());
            if (Input.touchCount > 0 &&  _shootRoutine == null)
                StartCoroutine(ShootRoutine());
        }

        private Coroutine _shootRoutine;
        private IEnumerator ShootRoutine()
        {
            while (true)
            {
                var mousePos = Input.touchCount > 0
                    ? GameManager.CameraManager.Cam.ScreenToWorldPoint(Input.GetTouch(0).position)
                    : GameManager.CameraManager.Cam.ScreenToWorldPoint(Input.mousePosition);
                var hadTouch = Input.touchCount > 0;
                var pPos = transform.position;
                var dir = (mousePos - pPos).normalized;
                dir.z = 0;
                var dist = Vector3.Distance(pPos, mousePos);
                var hit = GetHit(pPos, dir, dist);
            
                DrawRay(hit, pPos, mousePos);
                if (hadTouch && Input.GetTouch(0).phase != TouchPhase.Ended) { yield return null; continue; }
                if (!Input.GetKeyUp(KeyCode.Mouse0)) { yield return null; continue; }
                Shoot(dir, pPos);
                break;
            }

            _trajectory.positionCount = 0;
            GameManager.TimeManager.SetTimescale(1.25f, 0);
            GameManager.TimeManager.SetTimescale(GameManager.DefaultTimeScale, 0.5f);
            _shootRoutine = null;
        }

        private RaycastHit2D? GetHit(Vector3 playerPos, Vector3 dir, float dist)
        {
            var hit = Physics2D.CircleCast(playerPos, 0.25f, dir, dist, _layerMask);
            if (hit.collider != null) return hit;
            return null;
        }

        private void Shoot(Vector3 dir, Vector3 pPos)
        {
            Instantiate(_projectilePrefab, pPos,
                Quaternion.AngleAxis(Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90, Vector3.forward));
            _playerRb.AddForce(-dir * 250, ForceMode2D.Impulse);
            NnManager.AudioManager.Play("Shoot");
        }

        private void DrawRay(RaycastHit2D? hit, Vector3 pPos, Vector3 mousePos)
        {
            _trajectory.colorGradient = hit == null ? _gradient : _hitGradient;
            var targetPos = hit?.point ?? mousePos;
            _trajectory.positionCount = 2;
            _trajectory.SetPositions(new[] { pPos, (Vector3)targetPos });
        }

        private void StopShooting()
        {
            if (_shootRoutine == null) return;
            StopCoroutine(_shootRoutine);
            _shootRoutine = null;
            _trajectory.positionCount = 0;
        }
    }
}
