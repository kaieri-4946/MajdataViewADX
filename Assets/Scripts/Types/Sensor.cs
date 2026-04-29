using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    public bool IsJudging { get; set; }
    public SensorStatus Status = SensorStatus.Off;
    public SensorArea Type;
    public SensorGroup Group 
    { 
        get
        {
            var i = (int)Type;
            if (i <= 7)
                return SensorGroup.A;
            else if (i <= 15)
                return SensorGroup.B;
            else if (i <= 16)
                return SensorGroup.C;
            else if (i <= 24)
                return SensorGroup.D;
            else
                return SensorGroup.E;
        }
    }

    public event EventHandler<InputEventArgs> OnStatusChanged; //oStatus nStatus

    List<Guid> tasks = new();
    public void SetOn(Guid id)
    {
        if (tasks.Contains(id))
            return;
        var oStatus = Status;
        var nStatus = SensorStatus.On;

        Status = nStatus;
        
        if(!tasks.Contains(id))
            tasks.Add(id);
        if (oStatus != nStatus)
        {
            if (OnStatusChanged != null)
            {
                OnStatusChanged(this, new InputEventArgs()
                {
                    IsButton = false,
                    Sensor = this,
                    OldStatus = oStatus,
                    Status = nStatus
                });
                IsJudging = false;
            }
            print($"Sensor:{Type} On");
        }
    }
    public void SetOff(Guid id) 
    {
        if (!tasks.Contains(id))
            return;
        var nStatus = SensorStatus.Off;

        tasks.Remove(id);
        if(tasks.Count == 0)
        {
            var oStatus = Status;
            if (OnStatusChanged != null)
            {
                OnStatusChanged(this, new InputEventArgs()
                {
                    IsButton = false,
                    Sensor = this,
                    OldStatus = oStatus,
                    Status = nStatus
                });
            }
            Status = nStatus;
            print($"Sensor:{Type} Off");
        }
    }
    public IEnumerator Click()
    {
        var guid = Guid.NewGuid();
        SetOn(guid);
        yield return new WaitForEndOfFrame();
        SetOff(guid);
    }

    public void ForceReset()
    {
        tasks.Clear();
        Status = SensorStatus.Off;
        IsJudging = false;
    }
}
