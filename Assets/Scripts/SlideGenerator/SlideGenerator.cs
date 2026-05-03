using Assets.Scripts.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UIElements;

// This is attached to SlideGenerator object in SampleScene, disable that object before running
// This will generate the prefabs and the slide section needed to add to the dictionary in JsonDataLoader
public class SlideGenerator : MonoBehaviour
{
    private List<GameObject> _sensors = new();
    private List<RectTransform> _sensorTransforms = new();
    private List<string> _touchSensorString = new();

    public GameObject SensorObj;

    public AnimatorController animatorController;
    public GameObject SlideArrowPrefab;
    public GameObject JudgePrefab;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize sensor list
        var count = SensorObj.transform.childCount;
        for (int i = 0; i < count; i++)
            _sensors.Add(SensorObj.transform.GetChild(i).gameObject);
        _sensorTransforms = _sensors.Select(x => x.GetComponent<RectTransform>()).ToList();

        // Initialize slide endpoint list
        for (int i = 1; i <= 8; i++)
        {
            _touchSensorString.Add(i.ToString());
        }
        for (int i = 0; i < 33; i++)
        {
            _touchSensorString.Add(((SensorType)i).ToString());
        }

        CreateAllStraightSlide(42);
    }

    private int parseSlideAnchor(string anchor)
    {
        if (anchor.StartsWith('C')) return 0;
        if (anchor.Length == 1) return anchor[0] - '0';
        return anchor[1] - '0';
    }
    private char toDictName(char c)
    {
        if (c >= '1' && c <= '8') return '1';
        return c;
    }

    private void CreateAllStraightSlide(int startCounter)
    {
        var sb = new StringBuilder();
        var sb2 = new StringBuilder();
        string[] startSensors = new string[] { "1", "A1", "B1", "C", "D1", "E1" };
        string[] endSensors = _touchSensorString.ToArray();

        foreach (var start in startSensors)
        {
            foreach (var end in endSensors)
            {
                // Skip unnecessary slide
                if (start == "1" && end[0] >= '1' && end[0] <= '8') continue;
                if (start == end) continue;
                if (start == "C" && parseSlideAnchor(end) != 1) continue;

                // Draw slide on screen
                var slideBars = InstantiateSlideShape(start, end, ShapeFunctions.StraightLine).ToList();
                var parentObject = slideBars[0].transform.parent.gameObject;
                // Add judgeObj to parentObject
                InstantiateJudgeObject(parentObject, slideBars.Last());

                // Get all sensor the slide cover
                var sections = GetSlideSectionCount(slideBars, _sensorTransforms);

                // Name convention
                var assetName = string.Empty;
                // If either start or end is in C position, single prefab for all 8 direction
                if (start[0] == 'C' || end[0] == 'C')
                {
                    assetName = $"{toDictName(start[0])}{toDictName(end[0])}_Line";
                }
                else
                // If either start or end is non C touch
                if (isTouch(start[0]) || isTouch(end[0]))
                {
                    assetName = $"{toDictName(start[0])}{toDictName(end[0])}_Line_{parseSlideAnchor(end)}";
                }

                // We do not generate existing normal slide here, so this is sufficient

                // Assign an Animator component to the parent gameObject
                AssignAnimator(parentObject);

                // Generate prefab
                Directory.CreateDirectory("Assets/GeneratedPrefab");
                PrefabUtility.SaveAsPrefabAsset(parentObject, $"Assets/GeneratedPrefab/{assetName}.prefab");
                sb.AppendLine(@$"{{""{assetName}"",new List<int>() {{{string.Join(", ", sections)}}}}},");
                sb2.AppendLine(@$"{{""{assetName}"", {startCounter++}}},");

                // Destroy gameObject
                Destroy(parentObject);
            }
        }

        // Write slide section to the file
        File.WriteAllText("Assets/GeneratedPrefab/StraightSlideSection.txt", sb.ToString() + '\n' + sb2.ToString());
    }

    private void AssignAnimator(GameObject slide)
    {
        var animator = slide.AddComponent<Animator>();
        animator.runtimeAnimatorController = animatorController;
        animator.applyRootMotion = false;
        animator.updateMode = AnimatorUpdateMode.Normal;
        animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        animator.enabled = false;
    }

    /// <summary>
    /// Draw slide on screen
    /// </summary>
    /// <param name="startSensor">A1, A2, ... if touch sensor; 1, 2, ... if tap button</param>
    /// <param name="endSensor">Same as above</param>
    /// <param name=""></param>
    private IEnumerable<GameObject> InstantiateSlideShape(
        string startSensor,
        string endSensor,
        Func<Vector3, Vector3, float, Vector3> interpolateFunction
    )
    {
        var posList = ShapeFunctions.CalculatePosition(
            GetSensorPosition(startSensor),
            GetSensorPosition(endSensor),
            interpolateFunction,
            4,
            256
        ).ToList();

        Debug.Log($"{startSensor} - {endSensor} - {posList.Count}");

        var slideParentObject = new GameObject();
        float angleInDeg = 0;
        for (int i = 1; i < posList.Count; i++)
        {
            if (i < posList.Count - 1)
                angleInDeg = GetAngleFromOxBetweenTwoPoint(posList[i], posList[i + 1]) * Mathf.Rad2Deg;

            var slideBar = Instantiate(
                SlideArrowPrefab,
                posList[i],
                Quaternion.AngleAxis(angleInDeg, Vector3.forward),
                slideParentObject.transform
            );

            slideBar.name = i == 0 ? "Slide_01" : $"Slide_01 ({i})";

            yield return slideBar;
        }
    }

    private void InstantiateJudgeObject(GameObject parent, GameObject lastSlideBar)
    {
        var angleInDeg = lastSlideBar.transform.rotation.eulerAngles.z;
        var rotation = Quaternion.AngleAxis(angleInDeg - 180, Vector3.forward);

        Instantiate(
            JudgePrefab,
            lastSlideBar.transform.position,
            rotation,
            parent.transform
        );
    }

    private Vector3 GetSensorPosition(string sensor)
    {
        if (!isTouch(sensor[0]))
        {
            return getPositionFromDistance(4.8f, parsePosition(sensor[0]));
        }

        return GetAreaPos(sensor[0] == 'C' ? 1 : parsePosition(sensor[1]), sensor[0]);
    }

    private float GetAngleFromOxBetweenTwoPoint(Vector3 start, Vector3 end)
    {
        var rise = end.y - start.y;
        var run = end.x - start.x;
        var gradient = rise / run;
        var angle = Mathf.Atan(gradient);
        return run > 0 ? angle - Mathf.PI : angle;
    }

    private Vector3 GetAreaPos(int index, char area)
    {
        /// <summary>
        /// AreaDistance: 
        /// C:   0
        /// E:   3.1
        /// B:   2.21
        /// A,D: 4.8
        /// </summary>
        if (area == 'C') return Vector3.zero;
        if (area == 'B')
        {
            var angle = -index * (Mathf.PI / 4) + Mathf.PI * 5 / 8;
            return new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * 2.3f;
        }

        if (area == 'A')
        {
            var angle = -index * (Mathf.PI / 4) + Mathf.PI * 5 / 8;
            return new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * 4.1f;
        }

        if (area == 'E')
        {
            var angle = -index * (Mathf.PI / 4) + Mathf.PI * 6 / 8;
            return new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * 3.0f;
        }

        if (area == 'D')
        {
            var angle = -index * (Mathf.PI / 4) + Mathf.PI * 6 / 8;
            return new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * 4.1f;
        }

        return Vector3.zero;
    }

    private int parsePosition(char c) => c - '0';
    private bool isTouch(char c) => c >= 'A' && c <= 'E';
    private Vector3 getPositionFromDistance(float distance, int position)
    {
        return new Vector3(
            distance * Mathf.Cos((position * -2f + 5f) * 0.125f * Mathf.PI),
            distance * Mathf.Sin((position * -2f + 5f) * 0.125f * Mathf.PI));
    }
    private List<int> GetSlideSectionCount(List<GameObject> slideBars, List<RectTransform> sensors)
    {
        List<int> result = new List<int>();
        List<Sensor> judgeSensors = new List<Sensor>();
        Sensor lastSensor = null;
        for (var i = 0; i < slideBars.Count; i++)
        {
            var bar = slideBars[i];
            var pos = bar.transform.position;

            foreach (var s in sensors)
            {
                var sensor = s.GetComponent<Sensor>();

                // Count E and D touch toward sensor needed if the slide can be completed with just the slide start sensor otherwise
                if (i != slideBars.Count - 1 || judgeSensors.Count > 1)
                    if (sensor.Group == SensorGroup.E || sensor.Group == SensorGroup.D)
                        continue;

                var rCenter = s.position;
                var rWidth = s.rect.width * s.lossyScale.x;
                var rHeight = s.rect.height * s.lossyScale.y;

                var radius = Math.Max(rWidth, rHeight) / 2;

                if ((pos - rCenter).sqrMagnitude <= radius * radius)
                {
                    if (lastSensor is null || sensor != lastSensor)
                    {
                        judgeSensors.Add(sensor);
                        lastSensor = sensor;
                        if (result.Count == 0) result.Add(0);
                        else result.Add(i);
                        break;
                    }
                }
            }
        }
        return result;
    }
}
