using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MajSimai;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Networking.UnityWebRequest;
#nullable enable
public class WifiDrop : NoteLongBase, ICanShine
{
    public GameObject star_slidePrefab;
    
    public bool isJustR;
    public float timeStart;
    public int endPosition;
    public int sortIndex;
    public float fadeInTime;
    public float fullFadeInTime;


    public List<int> areaStep = new List<int>();
    public bool smoothSlideAnime = false;

    private float arriveTime = -1;
    private List<GameObject> sensors = new();
    private List<Sensor> boundSensors = new();
    private List<List<SlideArea>> judgeQueues = new(3);
    private Dictionary<GameObject, List<Sensor>> triggerSensors = new();
    
    private bool isFinished => judgeQueues.All(x => x.Count == 0);
    
    private Animator fadeInAnimator = null;
    private readonly GameObject[] star_slides = new GameObject[3];
    private readonly SpriteRenderer[] star_Renderer = new SpriteRenderer[3];
    private readonly List<SpriteRenderer> sbRender = new();
    private readonly List<GameObject> slideBars = new();
    private readonly Vector3[] SlidePositionEnd = new Vector3[3];
    private GameObject slideOK;

    private Vector3 SlidePositionStart;

    bool isDestroying = false;
    bool canShine = false;
    bool isChecking = false;
    bool canCheck = false;
    bool isSoundPlayed = false;
    Dictionary<GameObject, Guid> guids = new();


    private void Start()
    {
        objectCounter = Majdata<ObjectCounter>.Instance!;
        timeProvider = Majdata<TimeProvider>.Instance!;
        skinManager = Majdata<SkinManager>.Instance!;
        inputManager = Majdata<InputManager>.Instance!;
        audioManager = Majdata<AudioManager>.Instance!;
        var notes = GameObject.Find("Notes").transform;
        
        // 计算Slide淡入时机
        // 在8.0速时应当提前300ms显示Slide
        fadeInTime = -3.926913f / speed;
        // Slide完全淡入时机
        // 正常情况下应为负值；速度过高将忽略淡入
        fullFadeInTime = Math.Min(fadeInTime + 0.2f, 0);
        var interval = fullFadeInTime - fadeInTime;
        fadeInAnimator = this.GetComponent<Animator>();
        fadeInAnimator.speed = 0.2f / interval; //淡入时机与正解帧间隔小于200ms时，加快淡入动画的播放速度; interval永不为0
        fadeInAnimator.SetTrigger("wifi");

        //stars skin
        for (var i = 0; i < star_slides.Length; i++)
        {
            star_slides[i] = Instantiate(star_slidePrefab, notes);
            star_Renderer[i] = star_slides[i].GetComponent<SpriteRenderer>();

            star_Renderer[i].sprite = skinManager.Star;
            if (isBreak) star_Renderer[i].sprite = skinManager.Star_Break;
            if (isEach) star_Renderer[i].sprite = skinManager.Star_Each;
            if (isMine) star_Renderer[i].sprite = skinManager.Star_Mine;
            
            star_slides[i].transform.rotation = Quaternion.Euler(0, 0, -22.5f * (8 + i + 2 * (startPosition - 1)));
            star_slides[i].SetActive(false);
        }

        var ne = GameObject.Find("NoteEffects");
        SlidePositionEnd[0] = ne.transform.GetChild(0).GetChild(endPosition - 2 < 0 ? 7 : endPosition - 2).position;// R
        SlidePositionEnd[1] = ne.transform.GetChild(0).GetChild(endPosition - 1).position;// Center
        SlidePositionEnd[2] = ne.transform.GetChild(0).GetChild(endPosition >= 8 ? 0 : endPosition).position; // L


        //bars
        transform.rotation = Quaternion.Euler(0f, 0f, -45f * (startPosition - 1));
        slideBars.Clear();
        for (var i = 0; i < transform.childCount - 1; i++) slideBars.Add(transform.GetChild(i).gameObject);
        slideOK = transform.GetChild(transform.childCount - 1).gameObject; //slideok is the last one
        if (isJustR)
        {
            slideOK.GetComponent<LoadJustSprite>().setR();
        }
        else
        {
            slideOK.GetComponent<LoadJustSprite>().setL();
            slideOK.transform.Rotate(new Vector3(0f, 0f, 180f));
        }

        if (isBreak)
        {
            foreach(var star in star_slides)
            {
                var renderer = star.GetComponent<SpriteRenderer>();
                renderer.material = skinManager.BreakMaterial;
                renderer.material.SetFloat("_Brightness", 0.95f);
                var controller = star.AddComponent<BreakShineController>();
                controller.enabled = true;
                controller.parent = this;
            }
        }

        slideOK.SetActive(false);
        slideOK.transform.SetParent(transform.parent);
        SlidePositionStart = getPositionFromDistance(4.8f);

        
        //bars skin
        for (var i = 0; i < slideBars.Count; i++)
        {
            var sr = slideBars[i].GetComponent<SpriteRenderer>();

            sr.sprite = skinManager.Wifi[i]; //注意赋值顺序
            if (isEach)
            {
                sr.sprite = skinManager.Wifi_Each[i];
            }
            if (isBreak)
            {
                sr.sprite = skinManager.Wifi_Break[i];
                sr.material = skinManager.BreakMaterial;
                sr.material.SetFloat("_Brightness", 0.95f);
                var controller = slideBars[i].AddComponent<BreakShineController>();
                controller.parent = this;
                controller.enabled = true;
            }
            if (isMine)
            {
                sr.sprite = skinManager.Wifi_Mine[i];
            }
                
            sbRender.Add(sr);
            sr.color = new Color(1f, 1f, 1f, 0f);
            sr.sortingOrder = sortIndex--;
            sr.sortingLayerName = "Slide";
        }
        
        foreach (var star in star_slides)
        {
            triggerSensors.Add(star, new());
            guids.Add(star, Guid.NewGuid());
        }

        //judge queue
        var table = SlideTables.GetWifiTable(startPosition);
        judgeQueues.Add(table.Left.ToList());
        judgeQueues.Add(table.Center.ToList());
        judgeQueues.Add(table.Right.ToList());
        
        foreach (var area in judgeQueues.SelectMany(y => y.SelectMany(x => x.Areas)))
            boundSensors.Add(inputManager.GetSensor(area));
        
        foreach (var boundSensor in boundSensors)
            inputManager.BindSensor(Check, boundSensor);
    }
    private void FixedUpdate()
    {
        // time      是Slide启动的时间点
        // timeStart 是Slide完全显示但未启动
        // LastFor   是Slide的时值
        var timing = timeProvider.NoteTime - time;
        var startTiming = timeProvider.NoteTime - timeStart;
        var forceJudgeTiming = time + LastFor + (isMine ? 0 : 0.6); //mine头一到就判

        if (startTiming >= -0.05f)
            canCheck = true;
        else if (timing > 0)
            Running();        
        
        if (isFinished)
        {
            HideBar(areaStep.LastOrDefault());
            Judge();
        }
        else if (timeProvider.NoteTime - forceJudgeTiming >= 0)
            TooLateJudge();
    }
    int GetLastIndex()
    {
        if (judgeQueues.All(x => x.Count == 0))
            return areaStep.LastOrDefault();
        
        return areaStep[4 - judgeQueues.Select(q => q.Count).Min()];
    }
    void TooLateJudge()
    {
        if (isMine)
        {
            judgeResult = JudgeType.Perfect;
            isJudged = true;
            DestroySelf();
            return;
        }
        if (judgeQueues.Count == 1)
            slideOK.GetComponent<LoadJustSprite>().setLateGd();
        else
            slideOK.GetComponent<LoadJustSprite>().setMiss();
        isJudged = true;
        DestroySelf();
    }
    public void Check(object sender, InputEventArgs arg) => CheckAll();
    void CheckAll()
    {
        if (isFinished || isChecking || !canCheck)
            return;
        if (InputManager.Mode is AutoPlayMode.Enable or AutoPlayMode.Random)
            return;
        isChecking = true;
        for (int i = 0; i < 3; i++)
        {
            var queue = judgeQueues[i];
            Check(ref queue);
            judgeQueues[i] = queue;
        }
        isChecking = false;
    }
    void Check(ref List<SlideArea> judgeQueue)
    {
        if (judgeQueue.Count == 0)
            return;

        var first = judgeQueue.First(); 
        SlideArea? second = null;

        if (judgeQueue.Count >= 2)
            second = judgeQueue[1];
        var fType = first.Areas;
        foreach (var t in fType)
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
                //HideBar(first.SlideIndex);
                judgeQueue = judgeQueue.Skip(2).ToList();
                return;
            }
            else if (second.On)
            {
                //HideBar(first.SlideIndex);
                judgeQueue = judgeQueue.Skip(1).ToList();
                return;
            }
        }

        if (first.IsFinished)
        {
            //HideBar(first.SlideIndex);
            judgeQueue = judgeQueue.Skip(1).ToList();
            return;
        }
        if (!isFinished)
            HideBar(GetLastIndex());

    }
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
        var timing = timeProvider.NoteTime - time;
        var starTiming = timeStart + (time - timeStart) * 0.667;
        var pTime = LastFor / areaStep.Last();
        var judgeTime = time + pTime * (areaStep.LastOrDefault() - 2.1f);// 正解帧
        var stayTime = (time + LastFor) - judgeTime; // 停留时间
        if (!isJudged)
        {
            arriveTime = timeProvider.NoteTime;
            var triggerTime = timeProvider.NoteTime;

            const float totalInterval = 1.2f; // 秒
            const float nPInterval = 0.4666667f; // Perfect基础区间

            float extInterval = MathF.Min(stayTime / 4, 0.733333f);           // Perfect额外区间
            float pInterval = MathF.Min(nPInterval + extInterval, totalInterval);// Perfect总区间
            var ext = MathF.Max(extInterval - 0.4f, 0);
            float grInterval = MathF.Max(0.4f - extInterval, 0);        // Great总区间
            float gdInterval = MathF.Max(0.3333334f - ext, 0); // Good总区间

            var diff = judgeTime - triggerTime; // 大于0为Fast，小于为Late
            bool isFast = false;
            JudgeType? judge = null;

            if (diff > 0)
                isFast = true;

            var p = pInterval / 2;
            var gr = grInterval / 2;
            var gd = gdInterval / 2;
            diff = MathF.Abs(diff);

            if (gr == 0)
            {
                if (diff >= p)
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

            print($"diff : {diff} ms");
            judgeResult = (JudgeType)judge;
            SetJust();
            isJudged = true;
        }
        else if (arriveTime < starTiming && timeProvider.NoteTime >= starTiming + stayTime * 0.667)
            DestroySelf();
        else if (arriveTime >= starTiming && timeProvider.NoteTime >= arriveTime + stayTime * 0.667)
            DestroySelf();
    }
    void HideBar(int endIndex)
    {
        endIndex = Math.Min(endIndex, slideBars.Count - 1);
        for (int i = 0; i <= endIndex; i++)
            slideBars[i].SetActive(false);
    }
    void Running()
    {
        if (InputManager.Mode is AutoPlayMode.Enable or AutoPlayMode.Random or AutoPlayMode.Disable || isMine)
            return;
        foreach (var star in star_slides)
        {
            var starRadius = 0.763736616f;
            var starPos = star.transform.position;
            var oldList = new List<Sensor>(triggerSensors[star]);
            triggerSensors[star].Clear();
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
                    triggerSensors[star].Add(sensor);
            }
            var untriggerSensors = oldList.Where(x => !triggerSensors[star].Contains(x));

            foreach (var s in untriggerSensors)
                inputManager.SetSensorOff(s, guids[star]);
            foreach (var s in triggerSensors[star])
                inputManager.SetSensorOn(s, guids[star]);
        }
    }
    // Update is called once per frame
    private void Update()
    {
        // Wifi Slide淡入期间，不透明度从0到1耗时200ms
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
        foreach (var star in star_slides)
            star.SetActive(true);

        var timing = timeProvider.NoteTime - time;
        if (timing <= 0f)
        {
            canShine = true;
            float alpha;
            alpha = 1f - -timing / (time - timeStart);
            alpha = alpha > 1f ? 1f : alpha;
            alpha = alpha < 0f ? 0f : alpha;

            for (var i = 0; i < star_slides.Length; i++)
            {
                star_Renderer[i].color = new Color(1, 1, 1, alpha);
                star_slides[i].transform.localScale = new Vector3(alpha + 0.5f, alpha + 0.5f, alpha + 0.5f);
                star_slides[i].transform.position = SlidePositionStart;
            }
        }
        else
        {
            UpdateStar();
            Running();
        }
        CheckAll();
    }
    void UpdateStar()
    {
        var timing = timeProvider.NoteTime - time;
        var process = 1f - (LastFor - timing) / LastFor;
        if (LastFor == 0) process = 1;
        var pos = (slideBars.Count - 1) * process;

        if (process >= 1)
        {
            for (var i = 0; i < star_slides.Length; i++)
            {
                star_Renderer[i].color = Color.white;
                star_slides[i].transform.position = SlidePositionEnd[i];
                star_slides[i].transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            }
            switch (InputManager.Mode)
            {
                case AutoPlayMode.Enable:
                    if (smoothSlideAnime) HideBar((int)pos + 1);
                    else HideBar(areaStep[(int)(process * (areaStep.Count - 1))]);
                    DestroySelf();
                    judgeQueues.Clear();
                    return;
                case AutoPlayMode.Random:
                    var barIndex = areaStep[(int)(process * (areaStep.Count - 1))];
                    HideBar(barIndex);
                    DestroySelf();
                    judgeQueues.Clear();
                    return;
            }
            if (isFinished && isJudged)
                DestroySelf();
        }
        else
        {
            for (var i = 0; i < star_slides.Length; i++)
            {
                star_Renderer[i].color = Color.white;
                star_slides[i].transform.position =
                    (SlidePositionEnd[i] - SlidePositionStart) * process + SlidePositionStart; //TODO add some runhua
                star_slides[i].transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            }
        }
        switch (InputManager.Mode)
        {
            case AutoPlayMode.Enable:
                judgeQueues = judgeQueues.Skip((int)(process * (judgeQueues.Count - 1))).ToList();
                if (smoothSlideAnime) HideBar((int)pos + 1);
                else HideBar(areaStep[(int)(process * (areaStep.Count - 1))]);
                PlaySFX();
                break;
            case AutoPlayMode.Random:
                judgeQueues = judgeQueues.Skip((int)(process * (judgeQueues.Count - 1))).ToList();
                var barIndex = areaStep[(int)(process * (areaStep.Count - 1))];
                HideBar(barIndex);
                PlaySFX();
                break;
        }
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
    public bool CanShine() => canShine;
    void DestroySelf()
    {
        foreach (GameObject obj in slideBars)
            obj.SetActive(false);

        for (var i = 0; i < star_slides.Length; i++)
            Destroy(star_slides[i]);
        Destroy(gameObject);
    }
    void OnDestroy()
    {
        if (PlayManager.IsReloading) return;
        if (isDestroying)
            return;

        switch (InputManager.Mode)
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
        if (isBreak && 
            judgeResult == JudgeType.Perfect)
        {
            audioManager.PlayBreakSlideEndSound();
        }
        objectCounter.ReportResult(SimaiNoteType.Slide, judgeResult, isBreak);
        if (isBreak && judgeResult == JudgeType.Perfect)
            slideOK.GetComponent<Animator>().runtimeAnimatorController = skinManager.Shine_JudgeBreak;
        if (!EffectManager.showLevel) slideOK.GetComponent<SpriteRenderer>().sprite = 
                Sprite.Create(new Texture2D(0, 0), new Rect(0, 0, 0, 0), new Vector2(0.5f, 0.5f));

        slideOK.SetActive(true);

        
        foreach (var sensor in boundSensors)
            inputManager.UnbindSensor(Check, sensor);
        ClearTriggeredSensor();
        isDestroying = true;
    }
    /// <summary>
    /// 清空所有已触发的Sensor
    /// </summary>
    void ClearTriggeredSensor()
    {
        foreach (var star in star_slides)
            foreach (var s in triggerSensors[star])
                inputManager.SetSensorOff(s, guids[star]);
    }
    private void setSlideBarAlpha(float alpha)
    {
        foreach (var sr in sbRender)
        {
            var oldColor = sr.color;
            oldColor.a = alpha;
            sr.color = oldColor;
        }
    }
    private void PlaySFX()
    {
        if (isSoundPlayed) return;
        
        isSoundPlayed = true;
        audioManager.PlaySlideSound(isBreak);
    }
}