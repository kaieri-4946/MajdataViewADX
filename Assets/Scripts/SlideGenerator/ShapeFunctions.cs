using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public static class ShapeFunctions
{
    const float TAU = Mathf.PI * 2;

    /// <param name="minSection">Min number of section, if reaching this number, force generation even if too dense</param>
    /// <param name="maxSection">Max number of section, this function will try to not make it too dense</param>
    /// <param name="densestMagnitude">If the first two slide bar is not closer than this, use it as number of section</param>
    public static IEnumerable<Vector3> CalculatePosition(
        Vector3 start,
        Vector3 end,
        Func<Vector3, Vector3, float, Vector3> interpolateFunction,
        int minSection,
        int maxSection,
        float densestMagnitude = 0.425f
    )
    {
        var downScaleThreshold = 64;

        if (minSection > maxSection)
        {
            yield break;
        }
        int section = maxSection;
        float currentMagnitude = 0f;

        while (currentMagnitude < densestMagnitude)
        {
            var first = interpolateFunction(start, end, 0);
            var second = interpolateFunction(start, end, 1f / section);

            currentMagnitude = (second - first).magnitude;

            section = DownSection(section, downScaleThreshold);

            if (section <= minSection)
            {
                section = minSection;
                break;
            }
        }


        for (int i = 0; i <= section; i++)
        {
            yield return interpolateFunction(start, end, (float)i / section);
        }
    }

    // Based on implementation at yoinked-h/simaiboot
    public static List<Vector3> CalculatePositionFixedStepLength(
        Vector3 start,
        Vector3 end,
        Func<Vector3, Vector3, float, Vector3> interpolateFunction,
        Func<Vector3, Vector3, float> slideLengthFunction,
        float stepLength = 0.13879132f, // Magic number, equal to 1>C length / 5
        float densestMagnitude = 0.425f
    )
    {
        var result = new List<Vector3>();

        var slideLength = slideLengthFunction(start, end);

        if (stepLength > slideLength / 2)
        {
            stepLength = slideLength / 2;
        }

        for (float i = 0; i <= 1; i += stepLength / slideLength)
        {
            result.Add(interpolateFunction(start, end, (float)i));
        }

        for (int i = 0; i < result.Count - 1; i++)
        {
            if ((result[i + 1] - result[i]).magnitude < densestMagnitude)
            {
                result.RemoveAt(i + 1);
                i--;
            }
        }

        return result;
    }

    private static int DownSection(int input, int threshold)
    {
        if (input > threshold)
        {
            return input / 2;
        }
        if (input > threshold && input < threshold * 2)
        {
            return threshold;
        }
        return input - 1;
    }

    public static Vector3 StraightLine(Vector3 start, Vector3 end, float t) => (end - start) * t + start;

    public static Vector3 CWCircle(Vector3 start, Vector3 end, float t)
    {
        var magnitude = start.magnitude + (end.magnitude - start.magnitude) * t;
        var span = GetSpan(start, end, cw: true);
        var angle = GetAngle(start) - span * t;
        return new Vector3(Mathf.Cos(angle) * magnitude, Mathf.Sin(angle) * magnitude, 0);
    }

    public static float CWCircleSlideLength(Vector3 start, Vector3 end)
    {
        return (GetAngle(start) + GetAngle(end)) * GetSpan(start, end, cw: true) / 2;
    }

    private static float GetSpan(Vector3 start, Vector3 end, bool cw)
    {
        var span = cw
            ? (GetAngle(start) - GetAngle(end) + 2 * TAU) % TAU
            : (GetAngle(end) - GetAngle(start) + 2 * TAU) % TAU;
        if (span < TAU / 32) span += TAU;
        return span;
    }

    private static float GetAngle(Vector3 point)
    {
        var angle = Mathf.Atan2(point.y, point.x);
        if (angle < 0) angle += TAU;
        return angle;
    }
}