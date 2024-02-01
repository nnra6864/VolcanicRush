using Core;
using Player;
using TMPro;
using UnityEngine;

namespace UI
{
    public class OverlayMenu : MonoBehaviour, IMenuAnimator
    {
        [SerializeField] private TMP_Text _score;
        private float _lerpPos;
        private Movement _movement;

        private void Start()
        {
            GameManager.OnScoreChanged += () => _score.text = GameManager.Score.ToString();
            _movement = GameManager.Player.GetComponent<Movement>();
        }

        public void StartGrounding() => _movement.StartGrounding();
        public void StopGrounding() => _movement.StopGrounding();
        public void Dash() => _movement.Dash();
        
        public void Pause()
        {
            GameManager.PauseGame();
        }
        
        public void Toggle(bool state)
        {
            gameObject.SetActive(state);
        }
    }
}