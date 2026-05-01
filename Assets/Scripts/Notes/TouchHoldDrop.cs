using System;
using MajSimai;
using UnityEngine;
#nullable enable
public class TouchHoldDrop : NoteLongBase
{
    public char areaPosition;
    public bool isFirework;
    
    [SerializeField]
    GameObject touchEffect;
    [SerializeField]
    GameObject gr_TouchEffect;
    [SerializeField]
    GameObject gd_TouchEffect;
    [SerializeField]
    GameObject judgeEffect;
    
    [SerializeField]
    GameObject[] fans = new GameObject[6]; //01,02,03,04,point,border
    [SerializeField]
    SpriteMask mask;
    
    private SpriteRenderer[] fansRenderers = new SpriteRenderer[5];
    private SpriteRenderer border;
    private GameObject firework;
    private Animator fireworkEffect;
    
    private float wholeDuration;
    private float moveDuration;
    private float displayDuration;
    
    private bool _isTouched = false; //for mine judge
    private Sprite _borderSprite;
    private bool isSfxPlaying;

    // Start is called before the first frame update
    private void Start()
    {
        wholeDuration = 3.209385682f * Mathf.Pow(speed, -0.9549621752f);
        moveDuration = 0.8f * wholeDuration;
        displayDuration = 0.2f * wholeDuration;

        var notes = GameObject.Find("Notes").transform;
        objectCounter = Majdata<ObjectCounter>.Instance!;
        noteManager = Majdata<NoteManager>.Instance!;
        timeProvider = Majdata<TimeProvider>.Instance!;
        inputManager = Majdata<InputManager>.Instance!;
        skinManager = Majdata<SkinManager>.Instance!;
        audioManager = Majdata<AudioManager>.Instance!;
        
        holdEffect = Instantiate(holdEffect, notes);
        holdEffect.SetActive(false);
        
        firework = GameObject.Find("FireworkEffect");
        fireworkEffect = firework.GetComponent<Animator>();
        
        for (var i = 0; i < 5; i++)
        {
            fansRenderers[i] = fans[i].GetComponent<SpriteRenderer>();
            fansRenderers[i].sortingOrder += noteSortOrder;
        }
        border = fans[5].GetComponent<SpriteRenderer>();
        border.sortingOrder += noteSortOrder;
        
        LoadSkin();
        
        transform.position = GetAreaPos(startPosition, areaPosition);
        
        SetFanColor(new Color(1f, 1f, 1f, 0f));

        mask.backSortingOrder = border.sortingOrder - 1;
        mask.frontSortingOrder = border.sortingOrder;
        mask.enabled = false;

        sensor = GameObject.Find("Sensors")
                                   .transform.GetChild((int)GetSensor())
                                   .GetComponent<Sensor>();
        inputManager.BindSensor(Check, sensor);
    }

    private void LoadSkin()
    {
        for (var i = 0; i < 4; i++) 
            fansRenderers[i].sprite = skinManager.TouchHold[i];
        fansRenderers[4].sprite = skinManager.TouchPoint; //point
        border.sprite = _borderSprite = skinManager.TouchHold_Border;
        if (isEach)
        {
            fansRenderers[4].sprite = skinManager.TouchPoint_Each;
        }
        if (isBreak)
        {
            for (var i = 0; i < 4; i++) 
                fansRenderers[i].sprite = skinManager.TouchHold_Break[i];
            fansRenderers[4].sprite = skinManager.TouchPoint_Break;
            border.sprite = _borderSprite = skinManager.TouchHold_Border_Break;
        }
        if (isMine)
        {
            for (var i = 0; i < 4; i++) 
                fansRenderers[i].sprite = skinManager.TouchHold_Mine[i];
            fansRenderers[4].sprite = skinManager.TouchPoint_Mine;
            border.sprite = _borderSprite = skinManager.TouchHold_Border_Mine;
        }
    }

    void Check(object sender, InputEventArgs arg)
    {
        if (isJudged || !noteManager.CanJudge(gameObject, sensor.Type))
            return;
        if (Majdata<InputManager>.Instance!.Mode is AutoPlayMode.Enable or AutoPlayMode.Random)
            return;
        if (arg.IsClick)
        {
            if (!inputManager.IsIdle(arg))
                return;
            
            inputManager.SetBusy(arg);
            Judge();
            if (isJudged)
            {
                inputManager.UnbindSensor(Check, sensor);
                noteManager.NextTouch(GetSensor());
            }
        }
    }
    void Judge()
    {
        const float JUDGE_GOOD_AREA = 316.667f;
        const int JUDGE_GREAT_AREA = 250;
        const int JUDGE_PERFECT_AREA = 200;

        const float JUDGE_SEG_PERFECT = 150f;

        if (isJudged)
            return;

        var timing = timeProvider.NoteTime - time;
        var isFast = timing < 0;
        var diff = MathF.Abs(timing * 1000);
        JudgeType result;
        if (diff > JUDGE_SEG_PERFECT && isFast)
            return;
        else if (diff < JUDGE_SEG_PERFECT)
            result = JudgeType.Perfect;
        else if (diff < JUDGE_PERFECT_AREA)
            result = JudgeType.LatePerfect2;
        else if (diff < JUDGE_GREAT_AREA)
            result = JudgeType.LateGreat;
        else if (diff < JUDGE_GOOD_AREA)
            result = JudgeType.LateGood;
        else
            result = JudgeType.Miss;
        if (isFast)
            judgeDiff = 0;
        else
            judgeDiff = diff;

        judgeResult = result;
        isJudged = true;
        PlayHoldEffect();
        audioManager.PlayTouchSound();
    }
    private void FixedUpdate()
    {
        var remainingTime = GetRemainingTime();
        var timing = GetJudgeTiming();

        if (remainingTime == 0 && isJudged)
        {
            Destroy(holdEffect);
            Destroy(gameObject);
        }
        else if (timing >= -0.01f)
        {
            // AutoPlay相关
            switch (Majdata<InputManager>.Instance!.Mode)
            {
                case AutoPlayMode.Enable:
                    if (!isJudged)
                        noteManager.NextTouch(GetSensor());

                    if (isMine)
                        judgeResult = JudgeType.Miss;
                    else
                        judgeResult = JudgeType.Perfect;

                    isJudged = true;
                    _isTouched = true;
                    PlayHoldEffect();
                    audioManager.PlayTouchHoldSound();
                    return;
                case AutoPlayMode.DJAuto:
                    if (!isJudged && !isMine)
                        inputManager.SetSensorOn(sensor, guid);
                    break;
                case AutoPlayMode.Random:
                    if (!isJudged)
                    {
                        noteManager.NextTouch(GetSensor());
                        if (isMine)
                        {
                            if (judgeResult > JudgeType.Perfect) //Fast
                            {
                                judgeResult = JudgeType.Miss;
                            }
                            else
                            {
                                judgeResult = JudgeType.Perfect;
                            }

                            if (judgeResult != JudgeType.Miss) _isTouched = true; //必有摸
                        }
                        isJudged = true;
                    }
                    PlayHoldEffect();
                    audioManager.PlayTouchHoldSound();
                    return;
                case AutoPlayMode.Disable:
                default:
                    break;
            }
        }

        if (isJudged)
        {
            if (!timeProvider.IsStart) // 忽略暂停
                return;
            
            if (inputManager.CheckSensorStatus(sensor, SensorStatus.On))
            {
                _isTouched = true;
                audioManager.PlayTouchHoldSound();
            }
            else
            {
                audioManager.StopTouchHoldSound();
            }
            
            if (timing <= 0.25f) // 忽略头部15帧
                return;
            if (remainingTime <= 0.2f) // 忽略尾部12帧
                return;

            var on = inputManager.CheckSensorStatus(sensor, SensorStatus.On);
            if (on)
            {
                PlayHoldEffect();
            }
            else
            {
                playerIdleTime += Time.fixedDeltaTime;
                StopHoldEffect();
            }
        }
        else if (timing > 0.316667f)
        {
            judgeDiff = 316.667f;
            judgeResult = JudgeType.Miss;
            inputManager.UnbindSensor(Check, sensor);
            isJudged = true;
            noteManager.NextTouch(GetSensor());
        }
    }
    // Update is called once per frame
    private void Update()
    {
        var timing = GetJudgeTiming();
        var pow = -Mathf.Exp(8 * (timing * 0.4f / moveDuration) - 0.85f) + 0.42f;
        var distance = Mathf.Clamp(pow, 0f, 0.4f);

        if (-timing <= wholeDuration && -timing > moveDuration)
        {
            SetFanColor(new Color(1f, 1f, 1f, Mathf.Clamp((wholeDuration + timing) / displayDuration, 0f, 1f)));
            fans[5].SetActive(false);
            mask.enabled = false;
        }
        else if (-timing < moveDuration)
        {
            fans[5].SetActive(true);
            mask.enabled = true;
            SetFanColor(Color.white);
            mask.alphaCutoff = Mathf.Clamp(0.91f * (1 - (LastFor - timing) / LastFor), 0f, 1f);
        }

        if (float.IsNaN(distance)) distance = 0f;
        if (distance == 0f)
        {
            //holdEffect.SetActive(true);
            holdEffect.transform.position = transform.position;
        }
        for (var i = 0; i < 4; i++)
        {
            var pos = (0.226f + distance) * GetAngle(i);
            fans[i].transform.localPosition = pos;
        }
    }
    private void OnDestroy()
    {
        audioManager.StopTouchHoldSound();
        if (PlayManager.IsReloading) return;
        var realityHT = LastFor - 0.45f - (judgeDiff / 1000f);
        var percent = Math.Clamp((realityHT - playerIdleTime) / realityHT, 0, 1);
        JudgeType result = judgeResult;
        if (realityHT > 0) 
        {
            if (percent >= 1f)
            {
                if (judgeResult == JudgeType.Miss)
                    result = JudgeType.LateGood;
                else if (MathF.Abs((int)judgeResult - 7) == 6)
                    result = (int)judgeResult < 7 ? JudgeType.LateGreat : JudgeType.FastGreat;
                else
                    result = judgeResult;
            }
            else if (percent >= 0.67f)
            {
                if (judgeResult == JudgeType.Miss)
                    result = JudgeType.LateGood;
                else if (MathF.Abs((int)judgeResult - 7) == 6)
                    result = (int)judgeResult < 7 ? JudgeType.LateGreat : JudgeType.FastGreat;
                else if (judgeResult == JudgeType.Perfect)
                    result = (int)judgeResult < 7 ? JudgeType.LatePerfect1 : JudgeType.FastPerfect1;
            }
            else if (percent >= 0.33f)
            {
                if (MathF.Abs((int)judgeResult - 7) >= 6)
                    result = (int)judgeResult < 7 ? JudgeType.LateGood : JudgeType.FastGood;
                else
                    result = (int)judgeResult < 7 ? JudgeType.LateGreat : JudgeType.FastGreat;
            }
            else if (percent >= 0.05f)
                result = (int)judgeResult < 7 ? JudgeType.LateGood : JudgeType.FastGood;
            else if (percent >= 0)
            {
                if (judgeResult == JudgeType.Miss)
                    result = JudgeType.Miss;
                else
                    result = (int)judgeResult < 7 ? JudgeType.LateGood : JudgeType.FastGood;
            }
        }

        switch (Majdata<InputManager>.Instance!.Mode)
        {
            case AutoPlayMode.Enable:
                result = JudgeType.Perfect;
                break;
            case AutoPlayMode.Random:
                result = (JudgeType)UnityEngine.Random.Range(1, 14);
                break;
            case AutoPlayMode.DJAuto:
            case AutoPlayMode.Disable:
                break;
        }

        if (isMine) //覆盖掉前面的判定
        {
            if (_isTouched)
                result = JudgeType.Miss;
            else
                result = JudgeType.Perfect;
        }

        print($"TouchHold: {MathF.Round(percent * 100, 2)}%\nTotal Len : {MathF.Round(realityHT * 1000, 2)}ms");
        objectCounter.ReportResult(SimaiNoteType.TouchHold, result, isBreak);
        if (!isJudged)
            noteManager.NextTouch(GetSensor());
        if (isFirework && result != JudgeType.Miss)
        {
            fireworkEffect.SetTrigger("Fire");
            firework.transform.position = transform.position;
        }
        if (judgeResult != JudgeType.Miss)
        {
            if (isBreak)
            {
                audioManager.PlayTapSound(judgeResult, false, isBreak);
            }
            else if (isFirework)
            {
                audioManager.PlayHanabiSound();
            }
            else
            {
                audioManager.PlayTouchSound();
            }
        }
        inputManager.UnbindSensor(Check, sensor);
        inputManager.SetSensorOff(sensor, guid);
        PlayJudgeEffect(result);
    }

    protected override void PlayHoldEffect()
    {
        base.PlayHoldEffect();
        border.sprite = _borderSprite;
    }
    protected override void StopHoldEffect()
    {
        base.StopHoldEffect();
        border.sprite = skinManager.TouchHold_Border_Miss;
    }

    private void PlayJudgeEffect(JudgeType judgeResult)
    {
        //show effect
        if (judgeResult != JudgeType.Miss)
        {
            switch (judgeResult)
            {
                case JudgeType.LateGood:
                case JudgeType.FastGood:
                    Instantiate(gd_TouchEffect, transform.position, transform.rotation);
                    break;
                case JudgeType.LateGreat:
                case JudgeType.LateGreat1:
                case JudgeType.LateGreat2:
                case JudgeType.FastGreat2:
                case JudgeType.FastGreat1:
                case JudgeType.FastGreat:
                    Instantiate(gr_TouchEffect, transform.position, transform.rotation);
                    break;
                case JudgeType.LatePerfect2:
                case JudgeType.FastPerfect2:
                case JudgeType.LatePerfect1:
                case JudgeType.FastPerfect1:
                case JudgeType.Perfect:
                    Instantiate(touchEffect, transform.position, transform.rotation);
                    break;
                default:
                    break;
            }
        }

        //show level
        if (EffectManager.showLevel)
        {
            //get obj
            var obj = Instantiate(judgeEffect, Vector3.zero, transform.rotation);
            var judgeObj = obj.transform.GetChild(0);
            if (sensor.Group != SensorGroup.C)
                judgeObj.transform.position = GetPosition(-0.46f);
            else
                judgeObj.transform.position = new Vector3(0, -0.6f, 0);
            judgeObj.GetChild(0).transform.rotation = GetRotation();
            var anim = obj.GetComponent<Animator>();

            //show
            switch (judgeResult)
            {
                case JudgeType.LateGood:
                case JudgeType.FastGood:
                    judgeObj.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = skinManager.JudgeText[1];
                    break;
                case JudgeType.LateGreat:
                case JudgeType.LateGreat1:
                case JudgeType.LateGreat2:
                case JudgeType.FastGreat2:
                case JudgeType.FastGreat1:
                case JudgeType.FastGreat:
                    judgeObj.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = skinManager.JudgeText[2];
                    break;
                case JudgeType.LatePerfect2:
                case JudgeType.FastPerfect2:
                case JudgeType.LatePerfect1:
                case JudgeType.FastPerfect1:
                    judgeObj.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = skinManager.JudgeText[3];
                    break;
                case JudgeType.Perfect:
                    judgeObj.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = skinManager.JudgeText[4];
                    break;
                case JudgeType.Miss:
                    judgeObj.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = skinManager.JudgeText[0];
                    break;
                default:
                    break;
            }
            anim.SetTrigger("touch");
        }

        //show fastlate
        if (EffectManager.showFL)
        {
            //get obj
            var obj = Instantiate(judgeEffect, Vector3.zero, transform.rotation);
            var flObj = obj.transform.GetChild(0);
            if (sensor.Group != SensorGroup.C)
                flObj.transform.position = GetPosition(-0.92f);
            else
                flObj.transform.position = new Vector3(0, -1.08f, 0);
            flObj.GetChild(0).transform.rotation = GetRotation();
            var flAnim = obj.GetComponent<Animator>();

            //show
            var customSkin = GameObject.Find("Outline").GetComponent<SkinManager>();
            if (judgeResult == JudgeType.Miss || judgeResult == JudgeType.Perfect)
            {
                obj.SetActive(false);
                Destroy(obj);
                return;
            }
            obj.SetActive(true);
            if (judgeResult > JudgeType.Perfect) //Fast
                obj.transform.GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = customSkin.FastText;
            else
                obj.transform.GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = customSkin.LateText;
            flAnim.SetTrigger("touch");
        }
    }

    /// <summary>
    /// 获取当前坐标指定距离的坐标
    /// <para>方向：原点</para>
    /// </summary>
    /// <param name="magnitude"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    Vector3 GetPosition(float distance)
    {
        var d = transform.position.magnitude;
        var ratio = MathF.Max(0, d + distance) / d;
        return transform.position * ratio;
    }
    private Quaternion GetRotation()
    {
        if (sensor.Type == SensorArea.C)
            return Quaternion.Euler(Vector3.zero);
        var d = Vector3.zero - transform.position;
        var deg = 180 + Mathf.Atan2(d.x, d.y) * Mathf.Rad2Deg;

        return Quaternion.Euler(new Vector3(0, 0, -deg));
    }
    private Vector3 GetAngle(int index)
    {
        var angle = Mathf.PI / 4 + index * (Mathf.PI / 2);
        return new Vector3(Mathf.Sin(angle), Mathf.Cos(angle));
    }

    public SensorArea GetSensor() => GetSensor(areaPosition, startPosition);
    public static SensorArea GetSensor(char areaPos, int startPos)
    {
        switch (areaPos)
        {
            case 'A':
                return (SensorArea)(startPos - 1);
            case 'B':
                return (SensorArea)(startPos + 7);
            case 'C':
                return SensorArea.C;
            case 'D':
                return (SensorArea)(startPos + 16);
            case 'E':
                return (SensorArea)(startPos + 24);
            default:
                return SensorArea.A1;
        }
    }
    Vector3 GetAreaPos(int index, char area)
    {
        // AreaDistance: 
        // C:   0
        // E:   3.1
        // B:   2.21
        // A,D: 4.8
        if (area == 'C') return Vector3.zero;
        if (area == 'B')
        {
            var angle = (-index * (Mathf.PI / 4)) + ((Mathf.PI * 5) / 8);
            return new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * 2.3f;
        }
        if (area == 'A')
        {
            var angle = (-index * (Mathf.PI / 4)) + ((Mathf.PI * 5) / 8);
            return new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * 4.1f;
        }
        if (area == 'E')
        {
            var angle = (-index * (Mathf.PI / 4)) + ((Mathf.PI * 6) / 8);
            return new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * 3.0f;
        }
        if (area == 'D')
        {
            var angle = (-index * (Mathf.PI / 4)) + ((Mathf.PI * 6) / 8);
            return new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * 4.1f;
        }
        return Vector3.zero;
    }
    private void SetFanColor(Color color)
    {
        foreach (var fan in fansRenderers) fan.color = color;
        border.color = color;
    }
}