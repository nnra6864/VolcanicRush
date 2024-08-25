using System;
using Core;
using UI;
using UnityEngine;

namespace Player
{
    public class Player : MonoBehaviour
    {
        public bool IsGrounded { get; private set; }

        [SerializeField] private Movement _movement;
        public Movement Movement => _movement;

        public void Start()
        {
            GameManager.CameraManager.VirtCam.m_Follow = transform;
            GameManager.OnRestartedPlaying += Respawn;
        }
        
        public void Die()
        {
            GameManager.Die();
        }
        
        public void Respawn()
        {
            transform.position = GameManager.StartPos;
            GameManager.CameraManager.Cam.transform.position = GameManager.StartPos;
            GameManager.OnRespawned?.Invoke();
        }
        
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Meteor")) MeteorCollision();
            if (other.gameObject.CompareTag("Wall")) Die();
            if (other.gameObject.CompareTag("Terrain")) IsGrounded = true;
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Terrain")) IsGrounded = true;
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Terrain")) IsGrounded = false;
        }

        private void MeteorCollision()
        {
            if (Movement.IsGrounding || Movement.IsDashing) return;
            Die();
        }
    }
}