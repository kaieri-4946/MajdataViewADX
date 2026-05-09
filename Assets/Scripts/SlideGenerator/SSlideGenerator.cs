using UnityEngine;
public class SSlideGenerator
{
    // Initialized points
    private readonly Vector3 _start = new Vector3();
    private readonly Vector3 _end = new Vector3();

    // Calculated
    private Vector3 _intersectA;
    private Vector3 _intersectB;

    private float _totalLength => _sectionALength + _sectionBLength + _sectionCLength;
    private float _sectionALength = 0f;
    private float _sectionBLength = 0f;
    private float _sectionCLength = 0f;

    public SSlideGenerator(Vector3 start, Vector3 end)
    {
        _start = start;
        _end = end;

        _intersectA = GetIntersect(_start);
        _intersectB = GetIntersect(_end);

        _sectionALength = (_intersectA - _start).magnitude;
        _sectionBLength = (_intersectB - _intersectA).magnitude;
        _sectionCLength = (_end - _intersectB).magnitude;
    }

    public Vector3 CalculatePosition(Vector3 start, Vector3 end, float t)
    {
        var length = t * _totalLength;
        if (length <= _sectionALength)
        {
            return ShapeFunctions.StraightLine(_start, _intersectA, length / _sectionALength);
        }

        if (length <= _totalLength - _sectionCLength)
        {
            return ShapeFunctions.StraightLine(_intersectA, _intersectB, (length - _sectionALength) / _sectionBLength);
        }

        return ShapeFunctions.StraightLine(_intersectB, _end, (length - _sectionALength - _sectionBLength) / _sectionCLength);
    }

    private Vector3 GetIntersect(Vector3 start)
    {
        // 1 -> B3, 2 -> B4, D1 -> middle of B3 and B4
        var angle = ShapeFunctions.GetAngle(start) + Mathf.PI / 2;
        return new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * 2f;
    }
}