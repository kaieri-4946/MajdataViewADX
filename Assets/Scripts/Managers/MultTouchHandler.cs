using System;
using System.Collections.Generic;
using UnityEngine;
#nullable enable
public class MultTouchHandler : MonoBehaviour
{
    private readonly List<TouchDrop>[] touchSlots = new List<TouchDrop>[33]; // A1-8,B1-8,C,D1-8,E1-8

    private void Awake()
    {
        Majdata<MultTouchHandler>.Instance = this;
    }

    private void Start()
    {
        for (var i = 0; i < 33; i++) touchSlots[i] = new List<TouchDrop>();
    }

    public void RegisterTouch(TouchDrop obj)
    {
        var areaIndex = (int)obj.sensor.Type;
        obj.setLayer(touchSlots[areaIndex].Count);
        touchSlots[areaIndex].Add(obj);
    }

    public void CancelTouch(TouchDrop obj)
    {
        var areaIndex = (int)obj.sensor.Type;
        var touchSlot = touchSlots[areaIndex];

        if (touchSlot.Count != 0)
            touchSlot.RemoveAt(0);

        foreach (var each in touchSlot) each.LayerDown();
    }

    public void ResetState()
    {
        for (var i = 0; i < 33; i++) touchSlots[i] = new List<TouchDrop>();
    }
}