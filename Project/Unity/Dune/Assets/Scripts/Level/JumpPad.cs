using Core;
using Player;
using UnityEngine;

namespace Level
{
    public class JumpPad : MonoBehaviour
    {
        [SerializeField] private Vector2 _force;
    
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            var rb = other.GetComponent<Rigidbody2D>();
            var newVel = rb.velocity;
            if (rb.velocity.x < 0) newVel.x = 0;
            if (rb.velocity.y < 0) newVel.y = 0;
            rb.velocity = newVel;
            rb.AddForce(_force, ForceMode2D.Impulse);
            GameManager.TimeManager.SetTimescale(1, 0);
            GameManager.TimeManager.SetTimescale(GameManager.DefaultTimeScale, 1);
            other.GetComponent<Movement>().StopGrounding();
        }
    }
}
