using System.Collections;
using UnityEngine;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        public enum MenuTypes { Main, Settings, Overlay, Pause, Restart, Previous }
        [SerializeField] private RectTransform _mainPanel, _settingsPanel, _overlayPanel, _pausePanel, _restartPanel;
        private RectTransform _activePanel;
        private RectTransform _prevPanel;

        private IEnumerator Start()
        {
            yield return new WaitForSecondsRealtime(2);
            LoadMenu(MenuTypes.Main);
        }

        public void LoadMenu(MenuTypes menu)
        {
            var prevPanel = _activePanel;
            _activePanel = menu switch
            {
                MenuTypes.Main => _mainPanel,
                MenuTypes.Settings => _settingsPanel,
                MenuTypes.Overlay => _overlayPanel,
                MenuTypes.Pause => _pausePanel,
                MenuTypes.Restart => _restartPanel,
                MenuTypes.Previous => _prevPanel,
                _ => null
            };
            _prevPanel = prevPanel;
            if (prevPanel != null) prevPanel.GetComponent<IMenuAnimator>().Toggle(false);
            if (_activePanel != null) _activePanel.GetComponent<IMenuAnimator>()?.Toggle(true);
        }
    }
}