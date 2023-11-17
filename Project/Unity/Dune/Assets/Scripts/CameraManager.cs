using System.Collections;
using Assets.NnUtils.Scripts;
using Cinemachine;
using NnUtils.Scripts;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera Cam;
    public CinemachineVirtualCamera VirtCam;
    [SerializeField] private float _minSize, _maxSize;

    private void Awake()
    {
        Cam = FindObjectOfType<Camera>();
        VirtCam = FindObjectOfType<CinemachineVirtualCamera>();
    }

    public void ChangeCamSize(float size) => Misc.RestartCoroutine(this, ref _changeSizeRoutine, ChangeSizeRoutine(size));

    private Coroutine _changeSizeRoutine;
    private IEnumerator ChangeSizeRoutine(float size)
    {
        float lerpPos = 0;
        var startSize = VirtCam.m_Lens.OrthographicSize;
        while (lerpPos < 1)
        {
            var t = Misc.UpdateLerpPos(ref lerpPos, 0.1f, Easings.Types.SineInOut);
            VirtCam.m_Lens.OrthographicSize = Mathf.Clamp(Mathf.Lerp(startSize, size, t), _minSize, _maxSize);
            yield return null;
        }
    }
}
