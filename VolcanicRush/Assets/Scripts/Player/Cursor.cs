using NnUtils.Scripts;
using UnityEngine;

namespace Player
{
    public class Cursor : NnBehaviour
    {
        private Camera _cam;
        
        private void Awake()
        {
            _cam = Camera.main;
            UnityEngine.Cursor.visible = false;
        }

        private void Update() => transform.position = (Vector2)_cam.ScreenToWorldPoint(Misc.GetPointerPos());
    }
}