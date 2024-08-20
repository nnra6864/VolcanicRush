using Core;
using UnityEngine;

namespace UI
{
    public class RestartMenu : MonoBehaviour, IMenuAnimator
    {
        private float _lerpPos;
        
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