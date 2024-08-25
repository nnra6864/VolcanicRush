using System.Collections;
using Core;
using NnUtils.Scripts;
using UnityEngine;
using UnityEngine.Audio;

namespace Player
{
    public class GroundSound : NnBehaviour
    {
        [SerializeField] private Player _player;
        [SerializeField] private AudioClip _clip;
        [SerializeField] private AudioMixerGroup _group;
        [SerializeField] private Vector2 _loopOffset;
        private AudioSource _source;

        private void Awake()
        {
            _source = gameObject.AddComponent<AudioSource>();
            _source.clip = _clip;
            _source.outputAudioMixerGroup = _group;
        }

        private void Update() => PlaySound();

        private void PlaySound()
        {
            if (!Play) return; 
            if (_soundRoutine != null) return;
            _soundRoutine = StartCoroutine(SoundRoutine());
        }
        
        private Coroutine _soundRoutine;
        private IEnumerator SoundRoutine()
        {
            if (!_source.isPlaying)
            {
                _source.pitch = 1;
                _source.Play();
            }
            if (_source.time < _loopOffset.x) yield return new WaitForSecondsRealtime(_loopOffset.x - _source.time);
            if (_source.time >= _loopOffset.y)
            {
                _source.time = _loopOffset.y;
                _source.pitch = -1;
            }
            
            while (Play)
            {
                if (_source.time >= _loopOffset.y)
                    _source.pitch = -1;
                else if (_source.time <= _loopOffset.x)
                    _source.pitch = 1;
                yield return null;
            }
            _source.pitch = 1;
            _soundRoutine = null;
        }

        private bool Play => _player.IsGrounded && GameManager.IsPlaying && !GameManager.IsDead;
    }
}