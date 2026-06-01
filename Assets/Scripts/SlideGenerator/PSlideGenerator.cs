using System.Drawing;
using UnityEngine;
public class PSlideGenerator
{
    // Initialized points
    private readonly Vector3 _start = new Vector3();
    private readonly Vector3 _end = new Vector3();

    // Calculated
    private Vector3 _intersectA;
    private Vector3 _intersectB;

    private float _totalLength = 0f;
    private float _sectionALength = 0f;
    private float _sectionBLength = 0f;
    private float _spanAngle = 0f;
    private float _angleStart = 0f;

    // Consts
    private static readonly Vector3 _center = Vector3.zero;
    private static readonly float _radius = 1.8f;

    public PSlideGenerator(Vector3 start, Vector3 end)
    {
        _start = start;
        _end = end;

        _intersectA = ShapeFunctions.GetDirectionalTangentPoint(_center, _radius, _start, cw: true);
        _intersectB = ShapeFunctions.GetDirectionalTangentPoint(_center, _radius, _end, cw: false);

        _sectionALength = (_intersectA - _start).magnitude;
        _totalLength += _sectionALength;

        _spanAngle = ShapeFunctions.GetSpan(_intersectA - _center, _intersectB - _center, cw: false);
        _angleStart = ShapeFunctions.GetAngle(_intersectA - _center);
        var arcLength = 2 * _radius * Mathf.Asin((_intersectB - _intersectA).magnitude / 2 / _radius);
        if (_spanAngle > Mathf.PI) arcLength = 2 * Mathf.PI * _radius - arcLength;
        if (_spanAngle < Mathf.PI / 8)
        {
            _spanAngle += Mathf.PI * 2;
            arcLength += 2 * Mathf.PI * _radius;
        }
        _totalLength += arcLength;

        _sectionBLength = (_end - _intersectB).magnitude;
        _totalLength += _sectionBLength;
    }

    public Vector3 CalculatePosition(Vector3 start, Vector3 end, float t)
    {
        var length = t * _totalLength;
        if (length <= _sectionALength)
        {
            return ShapeFunctions.StraightLine(_start, _intersectA, length / _sectionALength);
        }

        if (length >= _totalLength - _sectionBLength)
        {
            return ShapeFunctions.StraightLine(_intersectB, _end, (length - (_totalLength - _sectionBLength)) / _sectionBLength);
        }

        var percentOnCircle = (length - _sectionALength) / (_totalLength - _sectionALength - _sectionBLength);
        var angle = _angleStart + _spanAngle * percentOnCircle;
        return new Vector3(_center.x + _radius * Mathf.Cos(angle), _center.y + _radius * Mathf.Sin(angle));
    }
}