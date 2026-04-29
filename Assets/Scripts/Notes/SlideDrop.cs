using System;
using System.Collections.Generic;
using System.Linq;
using MajSimai;
using UnityEngine;
using static UnityEngine.Networking.UnityWebRequest;
#nullable enable
public class SlideDrop : NoteLongBase, ICanShine
{
    public int endPosition;

    public bool isMirror;
    public bool isJustR;
    public bool isSpecialFlip; // fixes known star problem
    
    public float timeStart;
    public int sortIndex;
    public float fadeInTime;
    public float fullFadeInTime;
    
    public ConnSlideInfo ConnectInfo = new();
    public List<int> areaStep = new();
    public bool smoothSlideAnime = false;
    
    public string slideType;

    private float arriveTime = -1;
    private List<Sensor> boundSensors = new();
    private List<Sensor> triggerSensors = new(); // AutoPlay; 标记已触发的Sensor 
    private List<SlideArea> judgeQueue = new(); // 判定队列(目前剩余的)
    private List<SlideArea> _judgeQueue = new(); // 判定队列

    public bool isFinished => judgeQueue.Count == 0; 
    public bool isPendingFinish => judgeQueue.Count == 1;

    public GameObject star_slide;
    private SpriteRenderer starRenderer;
    
    private readonly List<GameObject> slideBars = new();
    private readonly List<Vector3> slidePositions = new();
    private readonly List<Quaternion> slideRotations = new();
    private Animator fadeInAnimator;
    private GameObject slideOK;

    bool canShine = false;
    bool canCheck = false;
    bool isChecking = false;
    float judgeTiming; // 正解帧
    bool isInitialized = false; //防止重复初始化
    bool isDestroying = false; // 防止重复销毁
    bool isSoundPlayed = false;

    /// <summary>
    /// Slide初始化
    /// </summary>
    public void Initialize()
    {
        if (isInitialized)
            return;
        isInitialized = true;
        
        objectCounter = Majdata<ObjectCounter>.Instance!;
        skinManager = Majdata<SkinManager>.Instance!;
        timeProvider = Majdata<TimeProvider>.Instance!;
        inputManager = Majdata<InputManager>.Instance!;
        audioManager = Majdata<AudioManager>.Instance!;
        
        //star
        starRenderer = star_slide.GetComponent<SpriteRenderer>();
        starRenderer.sprite = skinManager.Star;
        if (isEach) starRenderer.sprite = skinManager.Star_Each;
        if (isMine) starRenderer.sprite = skinManager.Star_Mine;
        if (isBreak)
        {
            starRenderer.sprite = skinManager.Star_Break;
            starRenderer.material = skinManager.BreakMaterial;
            starRenderer.material.SetFloat("_Brightness", 0.95f);
            var controller = star_slide.AddComponent<BreakShineController>();
            controller.parent = this;
            controller.enabled = true;
        }

        //bars
        for (var i = 0; i < transform.childCount - 1; i++) 
            slideBars.Add(transform.GetChild(i).gameObject);
        
        //slideok
        slideOK = transform.GetChild(transform.childCount - 1).gameObject; //slideok is the last one        
        if (isMirror)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
            transform.rotation = Quaternion.Euler(0f, 0f, -45f * startPosition);
            slideOK.transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, -45f * (startPosition - 1));
        }
        if (isJustR)
        {
            if (slideOK.GetComponent<LoadJustSprite>().setR() == 1 && isMirror)
            {
                slideOK.transform.Rotate(new Vector3(0f, 0f, 180f));
                var angel = slideOK.transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
                slideOK.transform.position += new Vector3(Mathf.Sin(angel) * 0.27f, Mathf.Cos(angel) * -0.27f);
            }
        }
        else
        {
            if (slideOK.GetComponent<LoadJustSprite>().setL() == 1 && !isMirror)
            {
                slideOK.transform.Rotate(new Vector3(0f, 0f, 180f));
                var angel = slideOK.transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
                slideOK.transform.position += new Vector3(Mathf.Sin(angel) * 0.27f, Mathf.Cos(angel) * -0.27f);
            }
        }
        slideOK.SetActive(false);
        slideOK.transform.SetParent(transform.parent);
        
        //bars
        slidePositions.Add(getPositionFromDistance(4.8f));
        foreach (var bars in slideBars)
        {
            slidePositions.Add(bars.transform.position);
            slideRotations.Add(Quaternion.Euler(bars.transform.rotation.eulerAngles + new Vector3(0f, 0f, 18f)));
        }
        
        //bars pos
        var endPos = getPositionFromDistance(4.8f, endPosition);
        var x = slidePositions.LastOrDefault() - Vector3.zero;
        var y = endPos - Vector3.zero;
        var angle = Mathf.Acos(Vector3.Dot(x, y) / (x.magnitude * y.magnitude)) * Mathf.Rad2Deg;
        var offset = slideRotations.TakeLast(1).First().eulerAngles - slideRotations.TakeLast(2).First().eulerAngles;
        if (offset.z < 0)
            angle = -angle;
            
        var q = slideRotations.LastOrDefault() * Quaternion.Euler(0, 0, angle);
        slidePositions.Add(endPos);
        slideRotations.Add(q);
        
        //bars skin
        foreach (var gm in slideBars)
        {
            var sr = gm.GetComponent<SpriteRenderer>();
            sr.color = new Color(1f, 1f, 1f, 0f);
            sr.sortingOrder = sortIndex--;
            sr.sortingLayerName = "Slide";

            sr.sprite = skinManager.Slide; //注意赋值顺序
            if (isEach)
            {
                sr.sprite = skinManager.Slide_Each;
            }
            if (isBreak)
            {
                sr.sprite = skinManager.Slide_Break;
                sr.material = skinManager.BreakMaterial;
                //sr.material.SetFloat("_Brightness", 0.95f);
                var controller = gm.AddComponent<BreakShineController>();
                controller.parent = this;
                controller.enabled = true;
            }
            if (isMine)
            {
                sr.sprite = skinManager.Slide_Mine;
            }
        }

        //bars fadein
        // 计算Slide淡入时机
        // 在8.0速时应当提前300ms显示Slide
        fadeInTime = -3.926913f / speed;
        // Slide完全淡入时机
        // 正常情况下应为负值；速度过高将忽略淡入
        fullFadeInTime = Math.Min(fadeInTime + 0.2f, 0);
        var interval = fullFadeInTime - fadeInTime;
        fadeInAnimator = this.GetComponent<Animator>();
        //淡入时机与正解帧间隔小于200ms时，加快淡入动画的播放速度; interval永不为0
        fadeInAnimator.speed = 0.2f / interval;
        fadeInAnimator.SetTrigger("slide");
        
        //judgeQueue
        var table = SlideTables.FindTableByName(slideType);
        if (isMirror)
        {
            table!.Mirror(SensorArea.A1);
        }
        var diff = Math.Abs(1 - startPosition);
        if (diff != 0)
        {
            table!.Diff(diff);
        }
        judgeQueue = table!.JudgeQueue.ToList();

        if (ConnectInfo.IsConnSlide)
        {
            if (ConnectInfo.IsGroupPartEnd)
            {
                judgeQueue.LastOrDefault().SetIsLast();
            }
            else
            {
                judgeQueue.LastOrDefault().SetNonLast();
            }
            UpdateJudgeQueue();
        }

        _judgeQueue = new(judgeQueue);

        foreach (var area in judgeQueue.SelectMany(x => x.Areas))
            boundSensors.Add(inputManager.GetSensor(area));
        
        foreach (var boundSensor in boundSensors)
            inputManager.BindSensor(Check, boundSensor);
        
        //judge timing
        if( (ConnectInfo.IsConnSlide && ConnectInfo.IsGroupPartEnd) || 
            !ConnectInfo.IsConnSlide)
        {
            judgeTiming = time + LastFor * CalJudgeTiming();
        }
        else
        {
            LastFor = (ConnectInfo.TotalLength / ConnectInfo.TotalSlideLen) * GetSlideLength();
            if(!ConnectInfo.IsGroupPartHead)
            {
                var parent = ConnectInfo.Parent!.GetComponent<SlideDrop>();
                time = parent.time + parent.LastFor;
                judgeTiming = time + LastFor * CalJudgeTiming();
            }
        }

        void UpdateJudgeQueue()
        {
            if (ConnectInfo.TotalJudgeQueueLen < 4)
            {
                if (ConnectInfo.IsGroupPartHead)
                {
                    judgeQueue[0].IsSkippable = true;
                    judgeQueue[1].IsSkippable = false;
                }
                else if (ConnectInfo.IsGroupPartEnd)
                {
                    judgeQueue[0].IsSkippable = false;
                    judgeQueue[1].IsSkippable = true;
                }
            }
            else
            {
                foreach (var judgeArea in judgeQueue)
                {
                    judgeArea.IsSkippable = true;
                }
            }
        }
    }

    /// <summary>
    /// Connection Slide
    /// <para>强制完成该Slide</para>
    /// </summary>
    public void ForceFinish()
    {
        if (!ConnectInfo.IsConnSlide || ConnectInfo.IsGroupPartEnd)
            return;
        judgeQueue.Clear();
    }
    
    private void FixedUpdate()
    {
        if (InputManager.Mode is AutoPlayMode.Enable or AutoPlayMode.Random)
            return;

        // time      是Slide启动的时间点
        // timeStart 是Slide完全显示但未启动
        // LastFor   是Slide的时值
        var timing = timeProvider.NoteTime - time;
        var startTiming = timeProvider.NoteTime - timeStart;
        var forceJudgeTiming = time + LastFor + (isMine ? 0 : 0.6); //mine一到就判

        if (ConnectInfo.IsConnSlide)
        {
            if (ConnectInfo.IsGroupPartHead && startTiming >= -0.05f)
                canCheck = true;
            else if (!ConnectInfo.IsGroupPartHead)
                canCheck = ConnectInfo.ParentFinished || ConnectInfo.ParentPendingFinish;
        }
        else if (startTiming >= -0.05f)
            canCheck = true;

        if (timing > 0)
            Running();

        //此处对mine音符的处理：一进judge就判定为miss并销毁，能进too late就判为perfect
        if (ConnectInfo.IsConnSlide)
        {
            if(ConnectInfo.IsGroupPartEnd && isFinished)
            {
                HideBar(areaStep.LastOrDefault());
                Judge();
            }
            else if (ConnectInfo.IsGroupPartEnd && timeProvider.NoteTime - forceJudgeTiming >= 0)
                TooLateJudge();
            else if(isFinished)
                HideBar(areaStep.LastOrDefault());
        }
        else if (isFinished)
        {
            HideBar(areaStep.LastOrDefault());
            Judge();
        }
        else if (timeProvider.NoteTime - forceJudgeTiming >= 0)
        {
            TooLateJudge();
        }
    }
    // Update is called once per frame
    private void Update()
    {
        if (star_slide == null)
        {
            if (isFinished)
                DestroySelf();
            return;
        }
        // Slide淡入期间，不透明度从0到0.55耗时200ms
        var startiming = timeProvider.NoteTime - timeStart;
        if (startiming <= 0f)
        {
            if (startiming >= -0.05f)
            {
                fadeInAnimator.enabled = false;
                setSlideBarAlpha(1f);
            }
            else if (!fadeInAnimator.enabled && startiming >= fadeInTime)
                fadeInAnimator.enabled = true;
            return;
            
        }
        fadeInAnimator.enabled = false;
        setSlideBarAlpha(1f);

        star_slide.SetActive(true);
        var timing = timeProvider.NoteTime - time;
        if (timing <= 0f)
        {
            canShine = true;
            float alpha;
            if (ConnectInfo.IsConnSlide && !ConnectInfo.IsGroupPartHead)
                alpha = 0;
            else
            {
                // 只有当它是一个起点Slide（而非Slide Group中的子部分）的时候，才会有开始的星星渐入动画
                alpha = 1f - -timing / (time - timeStart);
                alpha = alpha > 1f ? 1f : alpha;
                alpha = alpha < 0f ? 0f : alpha;                
            }

            starRenderer.color = new Color(1, 1, 1, alpha);
            star_slide.transform.localScale = new Vector3(alpha + 0.5f, alpha + 0.5f, alpha + 0.5f);
            star_slide.transform.position = slidePositions[0];
            applyStarRotation(slideRotations[0]);
        }
        else
        {
            UpdateStar();
            Running();
        }
        Check();
    }

    public float GetSlideLength()
    {
        if (areaStep.Count > 0)
            return areaStep.Last();

        return Math.Max(slideBars.Count, 1);
    }

    
    public void Check(object sender, InputEventArgs arg) => Check();
    /// <summary>
    /// 判定队列检查
    /// </summary>
    public void Check()
    {
        if (!canCheck || isChecking || isFinished)
            return;
        if (InputManager.Mode is AutoPlayMode.Enable or AutoPlayMode.Random)
            return;
        
        isChecking = true;
        
        //parent conn slide
        if (ConnectInfo.Parent != null && judgeQueue.Count < _judgeQueue.Count)
        {
            if (!ConnectInfo.ParentFinished)
                ConnectInfo.Parent.GetComponent<SlideDrop>().ForceFinish();
        }

        //slide
        var first = judgeQueue.First();
        SlideArea? second = null;

        if (judgeQueue.Count >= 2)
            second = judgeQueue[1];
        foreach (var t in first.Areas)
        {
            var sensor = inputManager.GetSensor(t);
            first.Judge(sensor.Status);
        }
        
        if (first.On)
        {
            PlaySFX();
        }

        if (second is not null && (first.IsSkippable || first.On))
        {
            var sType = second.Areas;
            foreach (var t in sType)
            {
                var sensor = inputManager.GetSensor(t);
                second.Judge(sensor.Status);
            }

            if (second.IsFinished)
            {
                HideBar(second.ArrowProgressWhenFinished);
                judgeQueue = judgeQueue.Skip(2).ToList();
                isChecking = false;
                return;
            }
            else if (second.On)
            {
                HideBar(second.ArrowProgressWhenOn);
                judgeQueue = judgeQueue.Skip(1).ToList();
                isChecking = false;
                return;
            }
        }
        if (first.IsFinished)
        {
            HideBar(first.ArrowProgressWhenFinished);
            judgeQueue = judgeQueue.Skip(1).ToList();
            isChecking = false;
            return;
        }
        
        isChecking = false;
    }
    void HideBar(int endIndex)
    {
        endIndex = Math.Min(endIndex - 1, slideBars.Count - 1);
        for (int i = 0; i <= endIndex; i++)
            slideBars[i].SetActive(false);
    }
    
    /// <summary>
    /// AutoPlay
    /// <para>
    /// 用于触发Sensor
    /// </para>
    /// </summary>
    void Running()
    {
        var startTiming = timeProvider.NoteTime - timeStart;
        if (startTiming < 0f || isMine) 
            return; 
        if (InputManager.Mode is AutoPlayMode.Enable or AutoPlayMode.Random or AutoPlayMode.Disable)
            return;

        var starRadius = 0.763736616f;
        var starPos = star_slide.transform.position;
        var oldList = new List<Sensor>(triggerSensors);
        triggerSensors.Clear();
        foreach (var sensor in inputManager.sensors)
        {
            if (sensor.Group is SensorGroup.E or SensorGroup.D)
                continue;

            var s = (RectTransform)sensor.gameObject.transform;
            var rCenter = s.position;
            var rWidth = s.rect.width * s.lossyScale.x;
            var rHeight = s.rect.height * s.lossyScale.y;

            var radius = Math.Max(rWidth, rHeight) / 2;

            if ((starPos - rCenter).sqrMagnitude <= (radius * radius + starRadius * starRadius))
                triggerSensors.Add(sensor);
        }
        var untriggerSensors = oldList.Where(x => !triggerSensors.Contains(x));

        foreach (var s in untriggerSensors)
            inputManager.SetSensorOff(s, guid);
        foreach (var s in triggerSensors)
            inputManager.SetSensorOn(s, guid);
    }
    /// <summary>
    /// Slide判定
    /// </summary>
    void Judge()
    {
        if (isMine)
        {
            judgeResult = JudgeType.Miss;
            SetJust();
            isJudged = true;
            DestroySelf();
            return;
        }
        if (!ConnectInfo.IsGroupPartEnd && ConnectInfo.IsConnSlide)
            return;
        var starTiming = timeStart + (time - timeStart) * 0.75;
        var stayTime = (time + LastFor) - judgeTiming; // 停留时间
        if (!isJudged)
        {
            arriveTime = timeProvider.NoteTime;
            var triggerTime = timeProvider.NoteTime;           

            const float totalInterval = 1.2f; // 秒
            const float nPInterval = 0.4666667f; // Perfect基础区间

            float extInterval = MathF.Min(stayTime / 4, 0.733333f);           // Perfect额外区间
            float pInterval = MathF.Min(nPInterval + extInterval, totalInterval);// Perfect总区间
            var ext = MathF.Max(extInterval - 0.4f,0);
            float grInterval = MathF.Max(0.4f - extInterval, 0);        // Great总区间
            float gdInterval = MathF.Max(0.3333334f - ext, 0); // Good总区间

            var diff = judgeTiming - triggerTime; // 大于0为Fast，小于为Late
            bool isFast = false;
            JudgeType? judge = null;

            if (diff > 0)
                isFast = true;

            var p = pInterval / 2;
            var gr = grInterval / 2;
            var gd = gdInterval / 2;
            diff = MathF.Abs(diff);

            if( gr == 0 )
            {
                if(diff >= p)
                    judge = isFast ? JudgeType.FastGood : JudgeType.LateGood;
                else
                    judge = JudgeType.Perfect;
            }
            else
            {
                if (diff >= gr + p || diff >= totalInterval / 2)
                    judge = isFast ? JudgeType.FastGood : JudgeType.LateGood;
                else if (diff >= p)
                    judge = isFast ? JudgeType.FastGreat : JudgeType.LateGreat;
                else
                    judge = JudgeType.Perfect;
            }            
            print($"Slide diff : {MathF.Round(diff * 1000,2)} ms");
            judgeResult = judge ?? JudgeType.Miss;
            SetJust();
            isJudged = true;
        }
        else if (arriveTime < starTiming && timeProvider.NoteTime >= starTiming + stayTime * 0.8)
            DestroySelf();
        else if (arriveTime >= starTiming && timeProvider.NoteTime >= arriveTime + stayTime * 0.8)
            DestroySelf();
    }
    void SetJust()
    {
        switch (judgeResult)
        {
            case JudgeType.FastGreat2:
            case JudgeType.FastGreat1:
            case JudgeType.FastGreat:
                slideOK.GetComponent<LoadJustSprite>().setFastGr();
                break;
            case JudgeType.FastGood:
                slideOK.GetComponent<LoadJustSprite>().setFastGd();
                break;
            case JudgeType.LateGood:
                slideOK.GetComponent<LoadJustSprite>().setLateGd();
                break;
            case JudgeType.LateGreat1:
            case JudgeType.LateGreat2:
            case JudgeType.LateGreat:
                slideOK.GetComponent<LoadJustSprite>().setLateGr();
                break;
            case JudgeType.Miss:
                slideOK.GetComponent<LoadJustSprite>().setMiss();
                break;

        }
    }
    /// <summary>
    /// 计算引导Star进入最后一个判定区的时机
    /// </summary>
    /// <returns>正解帧 (单位: s)</returns>
    float CalJudgeTiming()
    {
        var s = inputManager.GetSensor(judgeQueue.Last().Areas[0]).gameObject.transform as RectTransform;
        var starRadius = 0.763736616f;
        var rCenter = s.position;
        var rWidth = s.rect.width * s.lossyScale.x;
        var rHeight = s.rect.height * s.lossyScale.y;

        var radius = Math.Max(rWidth, rHeight) / 2;
        for (var process = 0.85f; process < 1; process += 0.01f)
        {
            var indexProcess = (slidePositions.Count - 1) * process;
            var index = (int)indexProcess;
            var pos = indexProcess - index;

            var a = slidePositions[index + 1];
            var b = slidePositions[index];
            var ba = a - b;
            var newPos = ba * pos + b;

            if ((newPos - rCenter).sqrMagnitude <= (radius * radius + starRadius * starRadius))
                return process;
        }
        return 0.9f;
    }
    /// <summary>
    /// 强制将Slide判定为TooLate并销毁
    /// </summary>
    void TooLateJudge()
    {
        if (isMine)
        {
            judgeResult = JudgeType.Perfect;
            isJudged = true;
            DestroySelf();
            return;
        }
        if (judgeQueue.Count == 1)
            slideOK.GetComponent<LoadJustSprite>().setLateGd();
        else
            slideOK.GetComponent<LoadJustSprite>().setMiss();
        isJudged = true;
        DestroySelf();
    }
    /// <summary>
    /// 销毁当前Slide
    /// <para>当 <paramref name="onlyStar"/> 为true时，仅销毁引导Star</para>
    /// </summary>
    /// <param name="onlyStar"></param>
    void DestroySelf(bool onlyStar = false)
    {
        if (onlyStar)
        { 
            Destroy(star_slide);
            star_slide = null!;
            ClearTriggeredSensor();
        }
        else
        {
            if(ConnectInfo.Parent != null)
                Destroy(ConnectInfo.Parent);

            foreach (var obj in slideBars)
                obj.SetActive(false);

            if (star_slide != null)
                Destroy(star_slide);
            Destroy(gameObject);
        }
    }
    /// <summary>
    /// 清空所有已触发的Sensor
    /// </summary>
    void ClearTriggeredSensor()
    {
        foreach (var t in triggerSensors)
            inputManager.SetSensorOff(t, guid);
    }
    void OnDestroy()
    {
        if (PlayManager.IsReloading) return;
        if (isDestroying)
            return;
        if (ConnectInfo.Parent != null)
            Destroy(ConnectInfo.Parent);
        if(star_slide != null)
            Destroy(star_slide);
        if (ConnectInfo.IsGroupPartEnd || !ConnectInfo.IsConnSlide)
        {
            switch(InputManager.Mode)
            {
                case AutoPlayMode.Enable:
                    if (isMine)
                        judgeResult = JudgeType.Miss;
                    else
                        judgeResult = JudgeType.Perfect;
                    SetJust();
                    break;
                case AutoPlayMode.Random:
                    judgeResult = (JudgeType)UnityEngine.Random.Range(1, 14);
                    if (isMine)
                    {
                        if (judgeResult != JudgeType.Miss)
                        { //Too Late Only, 不考虑留一个判定区的那种LateGd，都随机了，能支持就是随机的荣幸
                            judgeResult = JudgeType.Miss;
                        }
                        else
                        {
                            judgeResult = JudgeType.Perfect;
                        }
                    }
                    SetJust();
                    break;
            }
            if ((ConnectInfo.IsGroupPartHead || !ConnectInfo.IsConnSlide) && 
                isBreak && 
                judgeResult == JudgeType.Perfect)
            {
                audioManager.PlayBreakSlideEndSound();
            }
            // 只有组内最后一个Slide完成 才会显示判定条并增加总数
            objectCounter.ReportResult(SimaiNoteType.Slide, judgeResult, isBreak);
            if (isBreak && judgeResult == JudgeType.Perfect)
                slideOK.GetComponent<Animator>().runtimeAnimatorController = skinManager.Shine_JudgeBreak;
            if (!EffectManager.showLevel) slideOK.GetComponent<SpriteRenderer>().sprite =
                    Sprite.Create(new Texture2D(0, 0), new Rect(0, 0, 0, 0), new Vector2(0.5f, 0.5f));

            slideOK.SetActive(true);
        }
        else
        {
            // 如果不是组内最后一个 那么也要将判定条删掉
            Destroy(slideOK);
        }
        foreach (var sensor in boundSensors)
            inputManager.UnbindSensor(Check, sensor);
        ClearTriggeredSensor();
        isDestroying = true;
    }
    /// <summary>
    /// 更新引导Star状态
    /// <para>包括位置，角度</para>
    /// </summary>
    void UpdateStar()
    {
        starRenderer.color = Color.white;
        star_slide.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

        var process = MathF.Min((LastFor - GetRemainingTime()) / LastFor, 1);
        if (LastFor == 0) process = 1;
        var indexProcess = (slidePositions.Count - 1) * process;
        var index = (int)indexProcess;
        var pos = indexProcess - index;

        if(process >= 1f)
        {
            switch (InputManager.Mode)
            {
                case AutoPlayMode.Enable:
                    if (smoothSlideAnime) HideBar(index + 1);
                    else HideBar(areaStep[(int)(process * (areaStep.Count - 1))]);
                    DestroySelf();
                    judgeQueue.Clear();
                    return;
                case AutoPlayMode.Random:
                    var barIndex = areaStep[(int)(process * (areaStep.Count - 1))];
                    HideBar(barIndex);
                    DestroySelf();
                    judgeQueue.Clear();
                    return;
            }
            star_slide.transform.position = slidePositions.LastOrDefault();
            applyStarRotation(slideRotations.LastOrDefault());
            if (ConnectInfo.IsConnSlide && !ConnectInfo.IsGroupPartEnd)
                DestroySelf(true);
            else if (isFinished && isJudged)
                DestroySelf();
        }
        else
        {
            var a = slidePositions[index + 1];
            var b = slidePositions[index];
            var ba = a - b;
            var newPos = ba * pos + b;

            star_slide.transform.position = newPos;
            if (index < slideRotations.Count - 1)
            {
                var _a = slideRotations[index + 1].eulerAngles.z;
                var _b = slideRotations[index].eulerAngles.z;
                var dAngle = Mathf.DeltaAngle(_b, _a) * pos;
                dAngle = Mathf.Abs(dAngle);
                var newRotation = Quaternion.Euler(0f, 0f,
                                Mathf.MoveTowardsAngle(_b, _a, dAngle));
                applyStarRotation(newRotation);
            }
        } 
        switch(InputManager.Mode)
        {
            case AutoPlayMode.Enable:
                judgeQueue = judgeQueue.Skip((int)(process * (judgeQueue.Count - 1))).ToList();
                if (smoothSlideAnime) HideBar(index + 1);
                else HideBar(areaStep[(int)(process * (areaStep.Count - 1))]);
                PlaySFX();
                break;
            case AutoPlayMode.Random:
                judgeQueue = judgeQueue.Skip((int)(process * (judgeQueue.Count - 1))).ToList();
                var barIndex = areaStep[(int)(process * (areaStep.Count - 1))];
                HideBar(barIndex);
                PlaySFX();
                break;
        }
    }
   
    private void setSlideBarAlpha(float alpha)
    {
        foreach (var gm in slideBars) gm.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, alpha);
    }
    private void applyStarRotation(Quaternion newRotation)
    {
        var halfFlip = newRotation.eulerAngles;
        halfFlip.z += 180f;
        if (isSpecialFlip)
            star_slide.transform.rotation = Quaternion.Euler(halfFlip);
        else
            star_slide.transform.rotation = newRotation;
    }
    
    private void PlaySFX()
    {
        if (isSoundPlayed) return;
        
        if (ConnectInfo.IsGroupPartHead || !ConnectInfo.IsConnSlide)
        {
            isSoundPlayed = true;
            audioManager.PlaySlideSound(isBreak);
        }
    }
    public bool CanShine() => canShine;
}