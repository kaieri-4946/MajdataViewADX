using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AssignSlidePrefabs : Editor
{
    [MenuItem("Tools/Assign Slide Prefabs")]
    public static void AssignSlidePrefab()
    {
        var targetScript = Selection.activeGameObject.GetComponent<DataLoader>();
        if (targetScript == null)
        {
            Debug.LogWarning($"Selected object doesn't contain a {nameof(DataLoader)} script");
            return;
        }

        // Preserve standard slides
        var maxStandardSlideIndex = 41;

        var slideMap = DataLoader.GetSlidePrefabMap()
            .Where(x => x.Value > maxStandardSlideIndex)
            .OrderBy(x => x.Value)
            .ToList();

        // Build list and validate data
        var clone = targetScript.slidePrefab.Take(maxStandardSlideIndex + 1).ToList();

        var currentIndex = maxStandardSlideIndex + 1;
        foreach (var slide in slideMap)
        {
            if (slide.Value != currentIndex)
            {
                Debug.LogError("DataLoader SlidePrefabMap is not contiguous");
                return;
            }

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/SlidePrefab/{slide.Key}.prefab");
            if (prefab == null)
            {
                Debug.LogError($"Assets/SlidePrefab/{slide.Key}.prefab not found");
                return;
            }

            clone.Add(prefab);
            currentIndex++;
        }

        // Actual assignment
        targetScript.slidePrefab = clone.ToArray();
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(Selection.activeGameObject.scene);
    }
}
