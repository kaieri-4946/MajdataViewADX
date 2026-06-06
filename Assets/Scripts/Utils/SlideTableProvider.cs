using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class SlideTableProvider
{
    private readonly static List<GameObject> _sensors = new List<GameObject>();
    private const float HAND_RADIUS = 0.038f; // Coming from InputManager.cs, adding this to radius to make sure behavior is consistent with how InputManager handling them

    private static List<GameObject> GetSensorList()
    {
        if (_sensors.Count != 0) return _sensors;

        var sensorsObj = GameObject.Find("Sensors");
        for (var i = 0; i < sensorsObj.transform.childCount; i++)
        {
            _sensors.Add(sensorsObj.transform.GetChild(i).gameObject);
        }
        return _sensors;
    }

    public static SlideTable GetSlideTable(string slideType, List<GameObject> slideBars)
    {
        var buildingSensorList = new List<SlideArea>();
        GameObject lastSensor = null;
        List<(GameObject sensor, int start)> list = new();
        for (var i = 0; i < slideBars.Count; i++)
        {
            var pos = slideBars[i].transform.position;

            foreach (var sensor in GetSensorList())
            {
                var s = sensor.GetComponent<RectTransform>();

                var rCenter = s.position;
                var rWidth = s.rect.width * s.lossyScale.x;
                var rHeight = s.rect.height * s.lossyScale.y;

                var radius = Math.Max(rWidth, rHeight) / 2;

                var combinedRadius = radius + HAND_RADIUS;
                if ((pos - rCenter).sqrMagnitude <= (combinedRadius * combinedRadius))
                {
                    if (lastSensor is null || sensor != lastSensor)
                    {
                        lastSensor = sensor;
                        list.Add((sensor, i));
                        break;
                    }
                }
            }
        }

        int last = 0;
        for (int i = 1; i < list.Count - 1; i++)
        {
            buildingSensorList.Add(
                new SlideArea()
                {
                    Areas = new[] { list[i].sensor.GetComponent<Sensor>().Type },
                    ArrowProgressWhenOn = last,
                    ArrowProgressWhenFinished = list[i].start - 1,
                    IsSkippable = true
                });
            last = list[i].start;
        }
        buildingSensorList.Add(
            new SlideArea()
            {
                Areas = new[] { list.Last().sensor.GetComponent<Sensor>().Type },
                ArrowProgressWhenOn = last,
                ArrowProgressWhenFinished = slideBars.Count - 1,
                IsSkippable = true,
                IsLast = true,
            });
        return new SlideTable()
        {
            Name = slideType,
            Const = 0.1f,
            JudgeQueue = buildingSensorList.ToArray(),
        };
    }
}
