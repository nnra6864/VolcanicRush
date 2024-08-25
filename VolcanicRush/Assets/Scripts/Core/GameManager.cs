using System;
using System.Collections;
using NnUtils.Scripts;
using NnUtils.Scripts.Audio;
using UI;
using UnityEngine;

namespace Core
{
    public class GameManager : NnBehaviour
    {
        private static GameManager _instance;
        public static GameManager Instance => _instance;

        [SerializeField] private GameObject _playerPrefab;
        private static GameObject _player;
        public static GameObject Player => _player;
        [SerializeField] private Vector3 _startPos = new(-3, 5);
        public static Vector3 StartPos => Instance._startPos;
        [SerializeField] private Sound _ambienceSound;
        
        [SerializeField] private float _defaultTimeScale;
        public static float DefaultTimeScale
        {
            get => Instance._defaultTimeScale;
            set => Instance._defaultTimeScale = value;
        }

        public static float PrePauseTimeScale;

        private static int _score;

        public static int Score
        {
            get => _score;
            set
            {
                _score = value;
                OnScoreChanged?.Invoke();
            }
        }
        public static Action OnScoreChanged;

        public static bool HasStarted { get; private set; }
        public static bool IsPlaying { get; private set; }
        public static bool IsDead { get; set; }

        public static Action OnStartedPlaying, OnResumedPlaying, OnRestartedPlaying, OnPausedPlaying, OnDied, OnRespawned;

        #region  Functions
        private void Awake()
        {
            if (Instance != null) Destroy(Instance.gameObject);
            _instance = this;
            Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
            PrePauseTimeScale = 1;
        }

        private void Start()
        {
            StartCoroutine(PlayAmbienceRoutine());
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && IsPlaying) PauseGame();
        }

        public static void StartGame()
        {
            IsPlaying = true;
            _player = Instantiate(Instance._playerPrefab, StartPos, Quaternion.identity);
            HasStarted = true;
            OnStartedPlaying?.Invoke();
        }

        public static void PauseGame()
        {
            ToggleGame(false);
            UIManager.LoadMenu(UIManager.MenuTypes.Pause);
        }

        public static void ToggleGame(bool state)
        {
            IsPlaying = state;
            if (!state) PrePauseTimeScale = Time.timeScale;
            TimeManager.ToggleTimeScale(state);
            if (state) OnResumedPlaying?.Invoke();
            else OnPausedPlaying?.Invoke();
        }

        public static void Die()
        {
            Score = 0;
            IsPlaying = false;
            IsDead = true;
            UIManager.LoadMenu(UIManager.MenuTypes.Restart);
            PrePauseTimeScale = 1;
            TimeManager.ToggleTimeScale(false);
            OnDied?.Invoke();
        }
        
        public static void RestartGame()
        {
            Score = 0;
            IsPlaying = true;
            IsDead = false;
            TimeManager.ToggleTimeScale(true);
            OnRestartedPlaying?.Invoke();
        }

        private IEnumerator PlayAmbienceRoutine()
        {
            while (true)
            {
                NnManager.AudioManager.PlayAt(_ambienceSound, Vector3.zero);
                yield return new WaitForSecondsRealtime(_ambienceSound.Clip.length * 0.8f - 5);
            }
        }
        #endregion

        #region Components 
        private static TimeManager _timeManager;
        public static TimeManager TimeManager
        {
            get
            {
                if (_timeManager == null) _timeManager = Instance.GetComponent<TimeManager>();
                return _timeManager;
            }
        }
    
        private static CameraManager _cameraManager;
        public static CameraManager CameraManager
        {
            get
            {
                if (_cameraManager == null) _cameraManager = Instance.GetComponent<CameraManager>();
                return _cameraManager;
            }
        }

        private static UIManager _uiManager;
        public static UIManager UIManager
        {
            get
            {
                if (_uiManager == null) _uiManager = Instance.GetComponent<UIManager>();
                return _uiManager;
            }
        }

        #endregion
    }
}
