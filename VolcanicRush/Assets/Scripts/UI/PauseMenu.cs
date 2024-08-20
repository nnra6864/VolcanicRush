using Core;
using UnityEngine;

namespace UI
{
    public class PauseMenu : MonoBehaviour, IMenuAnimator
    {
        [SerializeField] private UIManager _uiManagerScript;
        private float _lerpPos;
        
        public void Resume()
        {
            GameManager.ToggleGame(true);
            GameManager.UIManager.LoadMenu(UIManager.MenuTypes.Overlay);
        }

        public void Restart()
        {
            GameManager.RestartGame();
            GameManager.UIManager.LoadMenu(UIManager.MenuTypes.Overlay);
        }
        
        public void Settings()
        {
            GameManager.UIManager.LoadMenu(UIManager.MenuTypes.Settings);
        }

        public void Quit() => Application.Quit();
        
        public void Toggle(bool state)
        {
            gameObject.SetActive(state);
        }
    }
}