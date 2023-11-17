using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);
        _instance = this;   
    }

    private CameraManager _cameraManager;
    public CameraManager CameraManager
    {
        get
        {
            if (_cameraManager == null) _cameraManager = Instance.GetComponent<CameraManager>();
            return _cameraManager;
        }
    }
}
