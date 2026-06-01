#nullable enable

#region

using System;
using MajSimai;
using UnityEngine;
using Random = UnityEngine.Random;

#endregion

public class TouchDrop : NoteBase
{
    protected MultTouchHandler multTouchHandler;

    public char areaPosition;
    public bool isFirework;
    public TouchGroup? GroupInfo;

    [SerializeField]
    protected GameObject justEffect;
    [SerializeField]
    protected GameObject multTouchEffect2;
    [SerializeField]
    protected GameObject multTouchEffect3;

    [SerializeField]
    protected GameObject touchEffect;
    [SerializeField]
    protected GameObject gr_TouchEffect;
    [SerializeField]
    protected GameObject gd_TouchEffect;
    [SerializeField]
    protected GameObject judgeEffect;

    public GameObject[] fans = new GameObject[7]; //01,02,03,04,point,border_02,border_03

    protected readonly SpriteRenderer[] fansRenderers = new SpriteRenderer[7];
    protected GameObject firework;
    protected Animator fireworkEffect;

    protected float wholeDuration;
    protected float moveDuration;
    protected float displayDuration;
    protected bool isStarted;
    protected int layer;
    protected bool isTriggered = false;

    void Start()
    {
        wholeDuration = 3.209385682f * Mathf.Pow(speed, -0.9549621752f);
        moveDuration = 0.8f * wholeDuration;
        displayDuration = 0.2f * wholeDuration;

        noteManager = Majdata<NoteManager>.Instance!;
        timeProvider = Majdata<TimeProvider>.Instance!;
        multTouchHandler = Majdata<MultTouchHandler>.Instance!;
        objectCounter = Majdata<ObjectCounter>.Instance!;
        inputManager = Majdata<InputManager>.Instance!;
        skinManager = Majdata<SkinManager>.Instance!;
        audioManager = Majdata<AudioManager>.Instance!;

        firework = GameObject.Find("FireworkEffect");
        fireworkEffect = firework.GetComponent<Animator>();

        for (var i = 0; i < 7; i++)
        {
            fansRenderers[i] = fans[i].GetComponent<SpriteRenderer>();
            fansRenderers[i].sortingOrder += noteSortOrder;
        }

        LoadSkin();

        transform.position = GetAreaPos(startPosition, areaPosition);
        justEffect.SetActive(false);
        SetFanColor(new Color(1f, 1f, 1f, 0f));

        sensor = GetSensor();
        inputManager.BindSensor(Check, sensor);
    }

    private void LoadSkin()
    {
        SetFanSprite(skinManager.Touch);
        fansRenderers[4].sprite = skinManager.TouchPoint;
        fansRenderers[5].sprite = skinManager.TouchBorder[0];
        fansRenderers[6].sprite = skinManager.TouchBorder[1];
        if (isEach)
        {
            SetFanSprite(skinManager.Touch_Each);
            fansRenderers[4].sprite = skinManager.TouchPoint_Each;
            fansRenderers[5].sprite = skinManager.TouchBorder_Each[0];
            fansRenderers[6].sprite = skinManager.TouchBorder_Each[1];
        }
        if (isBreak)
        {
            SetFanSprite(skinManager.Touch_Break);
            fansRenderers[4].sprite = skinManager.TouchPoint_Break;
            fansRenderers[5].sprite = skinManager.TouchBorder_Break[0];
            fansRenderers[6].sprite = skinManager.TouchBorder_Break[1];
        }
        if (isMine)
        {
            SetFanSprite(skinManager.Touch_Mine);
            fansRenderers[4].sprite = skinManager.TouchPoint_Mine;
            fansRenderers[5].sprite = skinManager.TouchBorder_Mine[0];
            fansRenderers[6].sprite = skinManager.TouchBorder_Mine[1];
        }
        justEffect.GetComponent<SpriteRenderer>().sprite = skinManager.TouchJust;
    }

    void Check(object sender, InputEventArgs arg)
    {
        if (arg.Type != sensor)
            return;
        if (isJudged || !noteManager.CanJudge(gameObject, sensor))
            return;
        if (Majdata<InputManager>.Instance!.Mode is AutoPlayMode.Enable or AutoPlayMode.Random)
            return;

        if (arg.IsClick)
        {
            if (!inputManager.IsIdle(arg))
                return;

            inputManager.SetBusy(arg);
            Judge();
        }
    }
    private void FixedUpdate()
    {
        var timing = timeProvider.NoteTime - time;
        if (isMine && !isJudged && timing >= 0.016667f)
        {
            judgeResult = JudgeType.Perfect;
            isJudged = true;
        }
        else if (!isJudged && timing <= 0.316667f)
        {
            if (GroupInfo is not null)
            {
                if (GroupInfo.Percent > 0.5f && GroupInfo.JudgeResult != null)
                {
                    isJudged = true;
                    judgeResult = (JudgeType)GroupInfo.JudgeResult;
                    DestroySelf();
                }
            }
        }
        else if (!isJudged)
        {
            judgeResult = JudgeType.Miss;
            isJudged = true;
            DestroySelf();
        }
        else if (isJudged)
        {
            DestroySelf();
        }


        if (timeProvider.NoteTime - time >= 0)
        {
            switch (Majdata<InputManager>.Instance!.Mode)
            {
                case AutoPlayMode.Enable:
                    if (isMine)
                        judgeResult = JudgeType.Miss;
                    else
                        judgeResult = JudgeType.Perfect;
                    isJudged = true;
                    break;
                case AutoPlayMode.Random:
                    judgeResult = (JudgeType)Random.Range(1, 14);
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
                    }
                    isJudged = true;
                    break;
                case AutoPlayMode.DJAuto:
                    if (isTriggered)
                        return;
                    if (!isMine) //mine buda
                        inputManager.ClickSensor(sensor);
                    isTriggered = true;
                    break;
                case AutoPlayMode.Disable:
                default:
                    break;
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

        if (isMine)
        {
            judgeResult = JudgeType.Miss;
            isJudged = true;
            return;
        }

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

        judgeResult = result;
        isJudged = true;
    }
    // Update is called once per frame
    private void Update()
    {
        var timing = timeProvider.NoteTime - time;
        var pow = -Mathf.Exp(8 * (timing * 0.43f / moveDuration) - 0.85f) + 0.42f;
        var distance = Mathf.Clamp(pow, 0f, 0.4f);

        var fakeTiming = timeProvider.FakeNoteTime - timeProvider.GetPositionAtTime(time);
        var fakePow = -Mathf.Exp(8 * (fakeTiming * 0.43f / moveDuration) - 0.85f) + 0.42f;
        var fakeDistance = Mathf.Clamp(fakePow, 0f, 0.4f);

        if (!usingSV)
        {
            fakeTiming = timing;
            fakePow = pow;
            fakeDistance = distance;
        }

        if (fakeTiming >= 0)
        {
            var _pow = -Mathf.Exp(-0.85f) + 0.42f;
            var _distance = Mathf.Clamp(_pow, 0f, 0.4f);
            for (var i = 0; i < 4; i++)
            {
                var pos = (0.226f + _distance) * GetAngle(i);
                fans[i].transform.localPosition = pos;
            }
            return;
        }

        if (fakeTiming > -0.02f) justEffect.SetActive(true);

        if (-fakeTiming <= wholeDuration && -fakeTiming > moveDuration)
        {
            if (!isStarted)
            {
                isStarted = true;
                multTouchHandler.RegisterTouch(this);
            }

            SetFanColor(new Color(1f, 1f, 1f, Mathf.Clamp((wholeDuration + fakeTiming) / displayDuration, 0f, 1f)));
        }
        else if (-fakeTiming < moveDuration)
        {
            SetFanColor(Color.white);
        }

        if (float.IsNaN(fakeDistance)) fakeDistance = 0f;
        for (var i = 0; i < 4; i++)
        {
            var pos = (0.226f + fakeDistance) * GetAngle(i);
            fans[i].transform.localPosition = pos;
        }
    }

    private void DestroySelf()
    {
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
        Destroy(gameObject);
    }
    private void OnDestroy()
    {
        if (PlayManager.IsReloading) return;
        multTouchHandler.CancelTouch(this);
        noteManager.NextTouch(sensor);
        PlayJudgeEffect();
        if (GroupInfo is not null && judgeResult != JudgeType.Miss)
            GroupInfo.JudgeResult = judgeResult;
        objectCounter.ReportResult(SimaiNoteType.Touch, judgeResult, isBreak);

        if (isFirework && judgeResult != JudgeType.Miss)
        {
            fireworkEffect.SetTrigger("Fire");
            firework.transform.position = transform.position;
        }
        inputManager.UnbindSensor(Check, sensor);
    }
    void PlayJudgeEffect()
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
                case JudgeType.Miss:
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
            if (sensor != SensorType.C)
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
            }
            anim.SetTrigger("touch");
        }

        //show fast late
        if (EffectManager.showFL)
        {
            if (judgeResult is JudgeType.Miss or JudgeType.Perfect)
            {
                return;
            }
            //get obj
            var customSkin = GameObject.Find("Outline").GetComponent<SkinManager>();
            var obj = Instantiate(judgeEffect, Vector3.zero, transform.rotation);
            var flObj = obj.transform.GetChild(0);
            if (sensor != SensorType.C)
                flObj.transform.position = GetPosition(-0.92f);
            else
                flObj.transform.position = new Vector3(0, -1.08f, 0);
            flObj.GetChild(0).transform.rotation = GetRotation();
            var flAnim = obj.GetComponent<Animator>();
            //show
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
    Vector3 GetPosition(float distance)
    {
        var d = transform.position.magnitude;
        var ratio = MathF.Max(0, d + distance) / d;
        return transform.position * ratio;
    }
    private Quaternion GetRotation()
    {
        if (sensor == SensorType.C)
            return Quaternion.Euler(Vector3.zero);
        var d = Vector3.zero - transform.position;
        var deg = 180 + Mathf.Atan2(d.x, d.y) * Mathf.Rad2Deg;

        return Quaternion.Euler(new Vector3(0, 0, -deg));
    }
    private Vector3 GetAngle(int index)
    {
        var angle = index * (Mathf.PI / 2);
        return new Vector3(Mathf.Sin(angle), Mathf.Cos(angle));
    }

    public SensorType GetSensor() => InputManager.GetSensor(areaPosition, startPosition);

    private Vector3 GetAreaPos(int index, char area)
    {
        // AreaDistance: 
        // C:   0
        // E:   3.1
        // B:   2.21
        // A,D: 4.8
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

    private void SetFanColor(Color color)
    {
        foreach (var fan in fansRenderers) fan.color = color;
    }

    private void SetFanSprite(Sprite sprite)
    {
        for (var i = 0; i < 4; i++) fansRenderers[i].sprite = sprite;
    }
    public void setLayer(int newLayer)
    {
        layer = newLayer;
        if (layer == 1)
        {
            multTouchEffect2.SetActive(true);
            multTouchEffect3.SetActive(false);
        }
        else if (layer == 2)
        {
            multTouchEffect2.SetActive(false);
            multTouchEffect3.SetActive(true);
        }
        else
        {
            multTouchEffect2.SetActive(false);
            multTouchEffect3.SetActive(false);
        }
    }
    public void LayerDown()
    {
        setLayer(layer - 1);
    }
}