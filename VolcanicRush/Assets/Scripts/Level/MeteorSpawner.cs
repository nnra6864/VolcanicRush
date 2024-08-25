using System.Collections;
using Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Level
{
    public class MeteorSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _meteorPrefab;
        [SerializeField] private Vector2 _spawnTimeRange;
        [SerializeField] private Vector2 _spawnOffsetRange;
        [SerializeField] private Vector2 _spawnRangeY;
        [SerializeField] private Vector2Int _spawnAmountRange;
        private Transform _player;
        private IEnumerator Start()
        {
            while (!GameManager.IsPlaying) yield return null;
            _player = GameObject.FindWithTag("Player").transform;
            while (true)
            {
                yield return new WaitForSecondsRealtime(Random.Range(_spawnTimeRange.x, _spawnTimeRange.y));
                var spawnAmount = (int)Random.Range(_spawnAmountRange.x, _spawnOffsetRange.y);
                for (int i = 0; i < spawnAmount; i++)
                {
                    while (!GameManager.IsPlaying) yield return null;
                    Spawn();
                    yield return new WaitForSeconds(0.25f);
                }
            }
        }

        private void Spawn()
        {
            Vector3 pos = new(
                _player.position.x + Random.Range(_spawnOffsetRange.x, _spawnOffsetRange.y),
                Random.Range(_spawnRangeY.x, _spawnRangeY.y));
            if (_player.position.y > 400) pos = _player.position;
            Vector2 dir = new(Random.Range(-0.75f, 0.75f), -1);
            dir.Normalize();
            Instantiate(_meteorPrefab, pos,
                Quaternion.AngleAxis(Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90, Vector3.forward));
        }
    }
}
