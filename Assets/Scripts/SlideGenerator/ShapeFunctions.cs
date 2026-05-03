using System;
using System.Collections.Generic;
using UnityEngine;

public static class ShapeFunctions
{
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
}