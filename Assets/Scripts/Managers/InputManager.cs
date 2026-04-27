using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#nullable enable
public class InputManager : MonoBehaviour
{
    public static AutoPlayMode Mode { get; set; } = AutoPlayMode.Enable;
    
    private Guid guid = Guid.NewGuid();
    
    public List<GameObject> sensorObjs = new();
    public List<Sensor> sensors = new();
    public Dictionary<int,List<Sensor>> triggerSensors = new();
    public List<Button> buttons = new();
    

    private void Awake()
    {
        Majdata<InputManager>.Instance = this;
    }
    
    private void Start()
    {
        //init sensors and buttons
        var sManagerObj = GameObject.Find("Sensors");
        for (var i = 0; i < sManagerObj.transform.childCount; i++)
        {
            var obj = sManagerObj.transform.GetChild(i).gameObject;
            sensorObjs.Add(obj);
            sensors.Add(obj.GetComponent<Sensor>());
        }
        
        buttons = new(new Button[] 
        {
            new(KeyCode.W, sensors[0]), //A1~8
            new(KeyCode.E, sensors[1]),
            new(KeyCode.D, sensors[2]),
            new(KeyCode.C, sensors[3]),
            new(KeyCode.X, sensors[4]),
            new(KeyCode.Z, sensors[5]),
            new(KeyCode.A, sensors[6]),
            new(KeyCode.Q, sensors[7]),
        });
    }
    
    private void Update()
    {
        //check keyboard and mouse input
        CheckButton();
        if (Input.GetMouseButton(0))
            PositionHandle(-1, Input.mousePosition);
        else
            Untrigger(-1);
        
        if (Input.touchCount > 0)
        {
            foreach(var touch in Input.touches)
            {
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        PositionHandle(touch.fingerId, touch.position);
                        break;
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        Untrigger(touch.fingerId);
                        break;
                }
            }
        }
    }

    public Button GetButton(Sensor sensor) => buttons[(int)sensor.Type];
    public Sensor GetSensor(SensorArea sensorArea) => sensors.Find(s => s.Type == sensorArea);
    
    public void BindSensor(EventHandler<InputEventArgs> checker, Sensor sensor)
    {
        sensor.OnStatusChanged += checker;
    }
    public void UnbindSensor(EventHandler<InputEventArgs> checker, Sensor sensor)
    {
        sensor.OnStatusChanged -= checker;
    }
    public void BindArea(EventHandler<InputEventArgs> checker, Sensor sensor)
    {
        var button = GetButton(sensor);

        sensor.OnStatusChanged += checker;
        button.OnStatusChanged += checker;
    }
    public void UnbindArea(EventHandler<InputEventArgs> checker, Sensor sensor)
    {
        var button = GetButton(sensor);

        sensor.OnStatusChanged -= checker;
        button.OnStatusChanged -= checker;
    }
    
    
    public bool CheckAreaStatus(Sensor sensor, SensorStatus targetStatus)
    {
        var button = GetButton(sensor);
        return sensor.Status == targetStatus || button.Status == targetStatus; 
    }
    public bool CheckSensorStatus(Sensor sensor, SensorStatus targetStatus)
    {
        return sensor.Status == targetStatus;
    }

    
    public void ClickSensor(Sensor sensor)
    {
        StartCoroutine(sensor.Click());
    }
    public void SetSensorOn(Sensor sensor, Guid id)
    {
        sensor.SetOn(id);
    }
    public void SetSensorOff(Sensor sensor, Guid id)
    {
        sensor.SetOff(id);
    }

    public void SetBusy(InputEventArgs args)
    {
        if(args.IsButton)
        {
            GetButton(args.Sensor).IsJudging = true;
        }
        else
        {
            args.Sensor.IsJudging = true;
        }
    }
    public bool IsIdle(InputEventArgs args)
    {
        bool isIdle;
        if (args.IsButton)
        {
            isIdle = GetButton(args.Sensor).IsJudging;
        }
        else
        {
            isIdle = !args.Sensor.IsJudging;
        }
        return isIdle;
    }
    
    

    void Untrigger(int id)
    {
        if (!triggerSensors.TryGetValue(id, out var triggerSensor)) 
            return;

        foreach (var s in triggerSensor)
            SetSensorOff(s, guid);
        triggerSensor.Clear();
    }

    void PositionHandle(int id, Vector3 pos)
    {
        var mainCamera = Camera.main!;
        var sPosition = pos;
        sPosition.z = mainCamera.nearClipPlane;
        var wPosition = mainCamera.ScreenToWorldPoint(sPosition);
        wPosition.z = 0;
        if (!triggerSensors.ContainsKey(id))
            triggerSensors.Add(id, new());
        
        var starRadius = 0.763736616f;
        var starPos = pos;
        var oldList = new List<Sensor>(triggerSensors[id]);
        triggerSensors[id].Clear();
        foreach (var s in sensorObjs.Select(x => x.GetComponent<RectTransform>()))
        {
            var sensor = s.GetComponent<Sensor>();

            var rCenter = s.position;
            var rWidth = s.rect.width * s.lossyScale.x;
            var rHeight = s.rect.height * s.lossyScale.y;

            var radius = Math.Max(rWidth, rHeight) / 2;

            if ((starPos - rCenter).sqrMagnitude <= (radius * radius + starRadius * starRadius))
                triggerSensors[id].Add(sensor);
        }
        var untriggerSensors = oldList.Where(x => !triggerSensors[id].Contains(x));

        foreach (var s in untriggerSensors)
            SetSensorOff(s, guid);
        foreach (var s in triggerSensors[id])
            SetSensorOn(s, guid);
    }
    void CheckButton()
    {
        foreach (var button in buttons)
        {
            var nStatus = Input.GetKey(button.BindingKey) ? SensorStatus.On : SensorStatus.Off;
            var oStatus = button.Status;
            if (oStatus == nStatus) return;

            print($"Key \"{button.BindingKey}\": {nStatus}");
            button.PushEvent(new InputEventArgs()
            {
                Sensor = button.Sensor,
                OldStatus = oStatus,
                Status = nStatus,
                IsButton = true
            });
            button.Status = nStatus;
            button.IsJudging = false;
        }
    }

    public void ResetState()
    {
        triggerSensors.Clear();

        foreach (var sensor in sensors)
        {
            sensor.ForceReset();
        }

        foreach (var button in buttons)
        {
            button.Status = SensorStatus.Off;
            button.IsJudging = false;
        }
    }
}
