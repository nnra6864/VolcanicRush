using System.Collections;
using Assets.NnUtils.Scripts;
using NnUtils.Scripts;
using UnityEngine;

namespace Core
{
    public class TimeManager : MonoBehaviour
    {
        public void SetTimescale(float s, float t)
        {
            if (Mathf.Approximately(s, Time.timeScale)) return;
            Misc.RestartCoroutine(this, ref _setTimescaleRoutine, SetTimeScaleRoutine(s, t));
        }
        private Coroutine _setTimescaleRoutine;
        private IEnumerator SetTimeScaleRoutine(float s, float time, bool constantTime = false, bool pause = true)
        {
            var startTS = Time.timeScale;
            var startFDT = Time.fixedDeltaTime;
            var targetFT = 0.02f * s;
            var lerpPos = 0f;
            float lerpTime;
            if (time == 0) lerpTime = 0;
            else lerpTime = constantTime ? time : Mathf.Abs(startTS - s) * time;
        
            while (lerpPos < 1)
            {
                while (pause && !GameManager.IsPlaying) yield return null;
                var t = Misc.UpdateLerpPos(ref lerpPos, lerpTime, true, Easings.Types.SineOut);
                Time.timeScale = Mathf.Lerp(startTS, s, t);
                Time.fixedDeltaTime = Mathf.Lerp(startFDT, targetFT, t);
                yield return null;
            }
            _setTimescaleRoutine = null;
        }

        private Coroutine _toggleTimeScaleRoutine;
        public void ToggleTimeScale(bool state) =>
            Misc.RestartCoroutine(this, ref _toggleTimeScaleRoutine,
                SetTimeScaleRoutine(state ? GameManager.PrePauseTimeScale : 0, state ? 0 : 0.5f, true, false));
    }
}
