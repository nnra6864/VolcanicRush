using Core;
using TMPro;
using UnityEngine;

namespace UI
{
    public class SettingsMenu : MonoBehaviour, IMenuAnimator
    {
        [SerializeField] private TMP_Text _valueText;
        public void SetBulletTime(float input) => _valueText.text = (GameManager.DefaultTimeScale = input).ToString();

        public void Back() => GameManager.UIManager.LoadMenu(UIManager.MenuTypes.Previous);
        
        public void Toggle(bool state)
        {
            gameObject.SetActive(state);
        }
    }
}