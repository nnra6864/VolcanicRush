using UnityEngine;
using UnityEngine.U2D;
using Random = UnityEngine.Random;
using NnUtils.Scripts;

[RequireComponent(typeof(SpriteShapeController))]
public class TerrainGeneration : MonoBehaviour
{
    [SerializeField] private SpriteShapeController _groundShape;
    [SerializeField] private float _genViewThreshold;
    [SerializeField] private Vector2 _pointDistanceRangeX, _pointDistanceRangeY, _yPointRange;
    [SerializeField] private float _minYDiff = 1f;
    [SerializeField] private float _tangentSmoothing = 2f;
    private Camera _cam;

    private void Reset() => _groundShape = GetComponent<SpriteShapeController>();

    private void Start()
    {
        _cam = Camera.main;
        GenerateStartingShape();
    }

    private void Update()
    {
        GenerateTerrain();
    }

    private void GenerateStartingShape()
    {
        var min = _cam.ScreenToWorldPoint(Vector3.zero);
        var s = _groundShape.spline;
        s.Clear();
        
        // Starting with the bottom-right corner so that I can access it with index 0 and left with index 1
        s.InsertPointAt(0, new(10, min.y - _genViewThreshold - 30));
        s.InsertPointAt(1, new(min.x - _genViewThreshold, min.y - _genViewThreshold - 30));
        s.InsertPointAt(2, new(min.x - _genViewThreshold, 0));
        
        s.InsertPointAt(3, new(0, 0));
        s.SetTangentMode(3, ShapeTangentMode.Continuous);
        s.SetLeftTangent(3, Vector3.left * _tangentSmoothing);
        s.SetRightTangent(3, Vector3.right * +_tangentSmoothing);
        
        s.InsertPointAt(4, new(5, 5));
        s.SetTangentMode(4, ShapeTangentMode.Continuous);
        s.SetLeftTangent(4, Vector3.left * _tangentSmoothing);
        s.SetRightTangent(4, Vector3.right * +_tangentSmoothing);
    }
    
    private void GenerateTerrain()
    {
        var min = _cam.ScreenToWorldPoint(Vector3.zero);
        var max = _cam.ScreenToWorldPoint(new(_cam.pixelWidth, _cam.pixelHeight));
        RemovePassedPoints(min);
        GeneratePoints(max);
    }

    private void RemovePassedPoints(Vector3 min)
    {
        if (_groundShape.spline.GetPointCount() < 4) return;
        if (_groundShape.spline.GetPosition(3).x > min.x - _genViewThreshold) return;
        _groundShape.spline.RemovePointAt(2);
        var x = _groundShape.spline.GetPosition(2).x;
        _groundShape.spline.SetPosition(1, new(x, _groundShape.spline.GetPosition(1).y));
        RemovePassedPoints(min);
    }

    private void GeneratePoints(Vector3 max)
    {
        var s = _groundShape.spline;
        while (s.GetPosition(s.GetPointCount() - 1).x < max.x + _genViewThreshold)
        {
            var pc = s.GetPointCount();
            var pos = s.GetPosition(pc - 1);
            
            RecalculatePos:
            Vector2 newPos = new(
                pos.x + Random.Range(_pointDistanceRangeX.x, _pointDistanceRangeX.y),
                Mathf.Clamp(
                    pos.y + Random.Range(_pointDistanceRangeY.x, _pointDistanceRangeY.y) * Misc.RandomInvert,
                    _yPointRange.x, _yPointRange.y));
            if (Mathf.Abs(pos.y - newPos.y) < _minYDiff) goto RecalculatePos;
            
            s.InsertPointAt(pc, newPos);
            s.SetTangentMode(pc - 1, ShapeTangentMode.Continuous);
            s.SetLeftTangent(pc - 1, Vector3.left * _tangentSmoothing);
            s.SetRightTangent(pc - 1, Vector3.right * +_tangentSmoothing);
        }
        s.SetPosition(0, new(s.GetPosition(s.GetPointCount() - 1).x, s.GetPosition(0).y));
    }
}
