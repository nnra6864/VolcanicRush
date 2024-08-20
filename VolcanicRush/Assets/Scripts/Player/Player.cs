using Core;
using UI;
using UnityEngine;

namespace Player
{
    public class Player : MonoBehaviour
    {
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
        }

        private void MeteorCollision()
        {
            if (Movement.IsGrounding || Movement.IsDashing) return;
            Die();
        }
    }
}