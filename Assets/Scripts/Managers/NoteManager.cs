using System;
using System.Collections.Generic;
using UnityEngine;
#nullable enable
public class NoteManager : MonoBehaviour
{
    public List<GameObject> notes = new();
    public Dictionary<GameObject, int> noteOrder = new();
    public Dictionary<int, int> noteIndex = new();

    public Dictionary<GameObject, int> touchOrder = new();
    public Dictionary<SensorArea, int> touchIndex = new();

    private void Awake()
    {
        Majdata<NoteManager>.Instance = this;
    }
    
    public void AddNote(GameObject obj,int index) => noteOrder.Add(obj, index);
    public void AddTouch(GameObject obj,int index) => touchOrder.Add(obj, index);
    public void NextNote(int pos) => noteIndex[pos]++;
    public void NextTouch(SensorArea pos) => touchIndex[pos]++;
    
    public void Refresh()
    {
        ResetIndex();
        var count = transform.childCount;
        for (int i = 0; i < count; i++)
        {
            var child = transform.GetChild(i);
            var tap = child.GetComponent<TapDrop>();
            var hold = child.GetComponent<HoldDrop>();
            var star = child.GetComponent<StarDrop>();
            var touch = child.GetComponent<TouchDrop>();
            var touchHold = child.GetComponent<TouchHoldDrop>();

            if (tap != null)
                noteOrder.Add(tap.gameObject, noteIndex[tap.startPosition]++);
            else if (hold != null)
                noteOrder.Add(hold.gameObject, noteIndex[hold.startPosition]++);
            else if (star != null && !star.isNoHead)
                noteOrder.Add(star.gameObject, noteIndex[star.startPosition]++);
            else if (touch != null)
                touchOrder.Add(touch.gameObject, touchIndex[touch.GetSensor()]++);
            else if(touchHold != null)
                touchOrder.Add(touchHold.gameObject, touchIndex[touchHold.GetSensor()]++);

            notes.Add(child.gameObject);
        }
    }
    void ResetIndex()
    {
        noteIndex.Clear();
        touchIndex.Clear();
        for (int i = 1; i < 9; i++)
            noteIndex.Add(i, 0);
        var sensorParent = GameObject.Find("Sensors");
        var count = sensorParent.transform.childCount;
        for (int i = 0; i < count; i++)
            touchIndex.Add(sensorParent.transform
                                       .GetChild(i)
                                       .GetComponent<Sensor>().Type, 0);
    }
    public bool CanJudge(GameObject obj, int pos)
    {
        if (!noteOrder.ContainsKey(obj))
            return false;
        var index = noteOrder[obj];
        var nowIndex = noteIndex[pos];

        return index <= nowIndex;
    }

    public bool CanJudge(GameObject obj,SensorArea t)
    {
        if (!touchOrder.ContainsKey(obj))
            return false;
        var index = touchOrder[obj];
        var nowIndex = touchIndex[t];

        return index <= nowIndex;
    }
}
