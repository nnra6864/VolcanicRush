using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.U2D;
using Random = UnityEngine.Random;

namespace Level
{
    public class TerrainGeneration : MonoBehaviour
    {
        [System.Serializable]
        private struct Chunk
        {
            public SpriteShapeController SSC;
            public SpriteShapeRenderer SSR;

            public Chunk(SpriteShapeController ssc, SpriteShapeRenderer ssr)
            {
                SSC = ssc;
                SSR = ssr;
            }
        }
        
        [SerializeField] private SpriteShapeController _chunkPrefab;
        [SerializeField] private float _genViewThreshold;
        [SerializeField] private Vector2 _pointDistanceRangeX, _pointDistanceRangeY, _yLimit;
        [SerializeField] private Vector2 _leftTangentSmoothing, _rightTangentSmoothing;
        private List<Chunk> _chunks = new();
        private int _dir = 1;
        
        private void Start()
        {
            GenerateStartingShape();
            GameManager.OnRespawned += OnRespawned;
        }

        private void Update()
        {
            UpdateRendererBounds();
            if (GameManager.IsDead) return;
            GenerateTerrain();
        }

        private void OnRespawned()
        {
            foreach(var c in _chunks)
                Destroy(c.SSC.gameObject);
            _chunks.Clear();
            GenerateStartingShape();
        }
        
        private void GenerateTerrain()
        {
            var cam = GameManager.CameraManager.Cam;
            var min = cam.ScreenToWorldPoint(Vector3.zero);
            var max = cam.ScreenToWorldPoint(new(cam.pixelWidth, cam.pixelHeight));
            GenerateChunks(max);
            RemoveChunks(min);
        }
        
        private void GenerateStartingShape()
        {
            var min = GameManager.CameraManager.Cam.ScreenToWorldPoint(Vector3.zero);
            var nc = Instantiate(_chunkPrefab, transform);
            nc.tag = "Terrain";
            var s = nc.spline;
            s.Clear();
        
            s.InsertPointAt(0, new(10, -_genViewThreshold));
            s.InsertPointAt(1, new(min.x - _genViewThreshold, -_genViewThreshold));
            s.InsertPointAt(2, new(min.x - _genViewThreshold, 0));
        
            s.InsertPointAt(3, new(0, 0));
            s.SetTangentMode(3, ShapeTangentMode.Continuous);
            s.SetRightTangent(3, new(5, 0));
            
            s.InsertPointAt(4, new(10, 5));
            s.SetTangentMode(4, ShapeTangentMode.Continuous);
            s.SetLeftTangent(4, new (-1, -1f));
            
            _chunks.Add(new(nc, nc.GetComponent<SpriteShapeRenderer>()));
            UpdateRendererBounds();
        }
        
        private void GenerateChunks(Vector3 max)
        {
            while (_chunks[^1].SSC.transform.position.x < max.x + _genViewThreshold)
            {
                var prevSSC = _chunks[^1].SSC;
                var prevPointPos = prevSSC.spline.GetPosition(prevSSC.spline.GetPointCount() - 1);
                var prevPos = new Vector3(prevPointPos.x + prevSSC.transform.position.x, prevPointPos.y);
                var pos = new Vector3(prevPos.x, 0);
                var size = Random.Range(_pointDistanceRangeX.x, _pointDistanceRangeX.y);
                var nc = Instantiate(_chunkPrefab, pos, Quaternion.identity, transform);
                nc.tag = "Terrain";
                var newY = Mathf.Clamp(
                    prevPos.y + Random.Range(_pointDistanceRangeY.x, _pointDistanceRangeY.y) * (_dir *= -1),
                    _yLimit.x, _yLimit.y);

                var ns = nc.spline;
                ns.Clear();
                var pos0 = new Vector3(0, prevPos.y);
                ns.InsertPointAt(0, pos0);
                var pos1 = new Vector3(0, -_genViewThreshold);
                ns.InsertPointAt(1, pos1);
                var pos2 = new Vector3(size, -_genViewThreshold);
                ns.InsertPointAt(2, pos2);
                var pos3 = new Vector3(size, newY);
                ns.InsertPointAt(3, pos3);
                
                Vector2 smoothingMultiplier = new(pos3.x - pos0.x, Mathf.Abs(pos3.y - pos0.y));
                ns.SetTangentMode(0, ShapeTangentMode.Continuous);
                ns.SetLeftTangent(0, _rightTangentSmoothing * smoothingMultiplier);
                ns.SetTangentMode(3, ShapeTangentMode.Continuous);
                ns.SetRightTangent(3, _leftTangentSmoothing * smoothingMultiplier);
                _chunks.Add(new(nc, nc.GetComponent<SpriteShapeRenderer>()));
            }
        }
        
        private void RemoveChunks(Vector3 min)
        {
            if (_chunks.Count < 2) return;
            while (_chunks[1].SSC.transform.position.x <= min.x - _genViewThreshold)
            {
                Destroy(_chunks[0].SSC.gameObject);
                _chunks.RemoveAt(0);
            }
        }
        
        private void UpdateRendererBounds()
        {
            foreach (var c in _chunks)
            {
                var b = c.SSR.localBounds;
                var min = c.SSC.spline.GetPosition(1);
                var max = c.SSC.spline.GetPosition(c.SSC.spline.GetPointCount() - 1);
                max.y = 5000;
                b.SetMinMax(min, max);
                c.SSR.localBounds = b;
            }
        }
    }
}