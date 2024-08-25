using System;
using Core;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace UI
{
    public class SettingsMenu : MonoBehaviour, IMenuAnimator
    {
        [SerializeField] private AudioMixer _mixer;
        [SerializeField] private Slider _ambienceSlider;
        [SerializeField] private Slider _sfxSlider;

        public void OnEnable()
        {
            var ambience = PlayerPrefs.GetFloat("Ambience", 0);
            _ambienceSlider.SetValueWithoutNotify(ambience);
            
            var sfx = PlayerPrefs.GetFloat("SFX", 0);
            _sfxSlider.SetValueWithoutNotify(sfx);
        }

        public void SetAmbience(float vol)
        {
            _mixer.SetFloat("Ambience", vol);
            PlayerPrefs.SetFloat("Ambience", vol);
        }

        public void SetSFX(float vol)
        {
            _mixer.SetFloat("SFX", vol);
            PlayerPrefs.SetFloat("SFX", vol);
        }

        public void Back() => GameManager.UIManager.LoadMenu(UIManager.MenuTypes.Previous);
        public void Toggle(bool state) => gameObject.SetActive(state);
    }
}