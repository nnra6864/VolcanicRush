using System;
using System.Collections;
using Cinemachine;
using NnUtils.Scripts;
using UnityEngine;

namespace Core
{
    public class CameraManager : NnBehaviour
    {
        public Camera Cam;
        public CinemachineVirtualCamera VirtCam;
        [SerializeField] private float _minSize, _maxSize;
        [SerializeField] private float _sizeDiffThreshold;

        private void Awake()
        {
            Cam = FindFirstObjectByType<Camera>();
            VirtCam = FindFirstObjectByType<CinemachineVirtualCamera>();
        }

        private void Update()
        {
            if (!GameManager.HasStarted) return;
            ChangeCamSize(GameManager.Player.transform.position.y);
        }

        public void ChangeCamSize(float size, bool ignoreThreshold = false)
        {
            if (!ignoreThreshold && Mathf.Abs(Cam.orthographicSize - size) < _sizeDiffThreshold) return;
            RestartRoutine(ref _changeSizeRoutine, ChangeSizeRoutine(size));
        }

        private Coroutine _changeSizeRoutine;
        private IEnumerator ChangeSizeRoutine(float size)
        {
            float lerpPos = 0;
            var startSize = VirtCam.m_Lens.OrthographicSize;
            while (lerpPos < 1)
            {
                var t = Misc.UpdateLerpPos(ref lerpPos, 0.1f, true);
                VirtCam.m_Lens.OrthographicSize = Mathf.Clamp(Mathf.Lerp(startSize, size, t), _minSize, _maxSize);
                yield return null;
            }
        }
    }
}
