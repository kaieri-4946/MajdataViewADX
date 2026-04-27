using System;
using MajSimai;
using UnityEngine;
#nullable enable
public class HoldDrop : NoteLongBase
{
    private EffectManager effectManager;
    
    public GameObject tapLine;
    
    private Animator animator;
    private bool holdAnimStart;
    private SpriteRenderer lineSpriteRender;
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer holdEndRender;
    private SpriteRenderer exSpriteRender;

    private bool isTouched = false; //for mine judge
    private bool isPlayedJudgeSFX = false; //for Enable / Random mode


    private void Start()
    {
        var notes = GameObject.Find("Notes").transform;
        timeProvider = Majdata<TimeProvider>.Instance!;
        objectCounter = Majdata<ObjectCounter>.Instance!;
        noteManager = Majdata<NoteManager>.Instance!;
        skinManager = Majdata<SkinManager>.Instance!;
        inputManager = Majdata<InputManager>.Instance!;
        effectManager = Majdata<EffectManager>.Instance!;
        audioManager = Majdata<AudioManager>.Instance!;
        
        holdEffect = Instantiate(holdEffect, notes);
        holdEffect.SetActive(false);

        tapLine = Instantiate(tapLine, notes);
        tapLine.SetActive(false);
        
        animator = GetComponent<Animator>();
        animator.enabled = false;
        
        lineSpriteRender = tapLine.GetComponent<SpriteRenderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        holdEndRender = transform.GetChild(1).GetComponent<SpriteRenderer>();
        exSpriteRender = transform.GetChild(0).GetComponent<SpriteRenderer>();

        spriteRenderer.sortingOrder += noteSortOrder;
        holdEndRender.sortingOrder += noteSortOrder;
        exSpriteRender.sortingOrder += noteSortOrder;

        LoadSkin();
        spriteRenderer.forceRenderingOff = true;
        exSpriteRender.forceRenderingOff = true;
        holdEndRender.enabled = false;

        sensor = GameObject.Find("Sensors")
                                   .transform.GetChild(startPosition - 1)
                                   .GetComponent<Sensor>();
        inputManager.BindArea(Check, sensor);
    }

    private void LoadSkin()
    {
        lineSpriteRender.sprite = skinManager.Line;
        spriteRenderer.sprite = skinManager.Hold;
        exSpriteRender.sprite = skinManager.Hold_Ex;
        holdEndRender.sprite = skinManager.HoldEnd;
        if (isEx)
        {
            exSpriteRender.color = skinManager.Ex;
        }
        if (isEach)
        {
            spriteRenderer.sprite = skinManager.Hold_Each;
            lineSpriteRender.sprite = skinManager.Line_Each;
            holdEndRender.sprite = skinManager.HoldEnd_Each;
            if (isEx) exSpriteRender.color = skinManager.Ex_Each;
        }
        if (isBreak)
        {
            spriteRenderer.sprite = skinManager.Hold_Break;
            lineSpriteRender.sprite = skinManager.Line_Break;
            holdEndRender.sprite = skinManager.HoldEnd_Break;
            if (isEx) exSpriteRender.color = skinManager.Ex_Break;
            spriteRenderer.material = skinManager.BreakMaterial;
        }
        if (isMine)
        {
            lineSpriteRender.sprite = skinManager.Line_Mine;
            spriteRenderer.sprite = skinManager.Hold_Mine;
        }
    }

    private void FixedUpdate()
    {
        var timing = GetJudgeTiming();
        var remainingTime = GetRemainingTime();

        if (remainingTime == 0 && isJudged) // Hold完成后Destroy
        {
            Destroy(tapLine);
            Destroy(holdEffect);
            Destroy(gameObject);
        }
        else if(timing >= -0.01f)
        {
            // AutoPlay相关
            switch (InputManager.Mode)
            {
                case AutoPlayMode.Enable:
                    if(!isJudged)
                        noteManager.NextNote(startPosition);

                    if (isMine)
                        judgeResult = JudgeType.Miss;
                    else 
                        judgeResult = JudgeType.Perfect;

                    isJudged = true;
                    isTouched = true; //算是点到了
                    PlayHoldEffect();
                    PlayJudgeSFX();
                    break;
                case AutoPlayMode.DjAuto:
                    if (!isJudged && !isMine) //mine buda
                        inputManager.SetSensorOn(sensor, guid);
                    break;
                case AutoPlayMode.Random:
                    if (!isJudged)
                    {
                        noteManager.NextNote(startPosition);
                        judgeResult = (JudgeType)UnityEngine.Random.Range(1, 14);
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

                            if (judgeResult != JudgeType.Miss) isTouched = true; //必有摸
                        } 
                        isJudged = true;
                    }
                    PlayHoldEffect();
                    PlayJudgeSFX();
                    return;
                case AutoPlayMode.Disable:
                    inputManager.SetSensorOff(sensor, guid);
                    break;
            }
        }

        if (isJudged) // 头部判定完成后开始累计按压时长
        {
            if (inputManager.CheckAreaStatus(sensor, SensorStatus.On)) isTouched = true;
            
            if (timing <= 0.1f) // 忽略头部6帧
                return;
            if (remainingTime <= 0.2f) // 忽略尾部12帧
                return;
            if (!timeProvider.IsStart || InputManager.Mode is AutoPlayMode.Enable or AutoPlayMode.Random) // 忽略暂停
                return;
            
            if (inputManager.CheckAreaStatus(sensor,SensorStatus.On))
            {
                PlayHoldEffect();
            }
            else
            {
                playerIdleTime += Time.fixedDeltaTime;
                StopHoldEffect();
            }
        }
        else if (timing > 0.15f && !isJudged) // 头部Miss
        {
            judgeDiff = 150;
            judgeResult = JudgeType.Miss;
            isJudged = true;
            noteManager.NextNote(startPosition);
        }
    }
    void Check(object sender, InputEventArgs arg)
    {
        if (arg.Sensor != sensor)
            return;
        if (isJudged || !noteManager.CanJudge(gameObject, startPosition))
            return;
        if (InputManager.Mode is AutoPlayMode.Enable or AutoPlayMode.Random)
            return;
        
        if (arg.IsClick)
        {
            if (!inputManager.IsIdle(arg))
                return;
            
            inputManager.SetBusy(arg);
            Judge();
            if (isJudged)
            {
                inputManager.UnbindArea(Check, sensor);
                noteManager.NextNote(startPosition);
            }
        }
    }
    private void Judge() //hold类头判正常检查，在destroy统一处理
    {
        const int JUDGE_GOOD_AREA = 150;
        const int JUDGE_GREAT_AREA = 100;
        const int JUDGE_PERFECT_AREA = 50;

        const float JUDGE_SEG_PERFECT1 = 16.66667f;
        const float JUDGE_SEG_PERFECT2 = 33.33334f;
        const float JUDGE_SEG_GREAT1 = 66.66667f;
        const float JUDGE_SEG_GREAT2 = 83.33334f;

        if (isJudged)
            return;

        var timing = timeProvider.NoteTime - time;
        var isFast = timing < 0;
        var diff = MathF.Abs(timing * 1000);
        JudgeType result;
        if (diff > JUDGE_GOOD_AREA && isFast)
            return;
        else if (diff < JUDGE_SEG_PERFECT1)
            result = JudgeType.Perfect;
        else if (diff < JUDGE_SEG_PERFECT2)
            result = JudgeType.LatePerfect1;
        else if (diff < JUDGE_PERFECT_AREA)
            result = JudgeType.LatePerfect2;
        else if (diff < JUDGE_SEG_GREAT1)
            result = JudgeType.LateGreat;
        else if (diff < JUDGE_SEG_GREAT2)
            result = JudgeType.LateGreat1;
        else if (diff < JUDGE_GREAT_AREA)
            result = JudgeType.LateGreat;
        else if (diff < JUDGE_GOOD_AREA)
            result = JudgeType.LateGood;
        else
            result = JudgeType.Miss;

        if (result != JudgeType.Miss && isFast)
            result = 14 - result;
        if (result != JudgeType.Miss && isEx)
            result = JudgeType.Perfect;
        if (isFast)
            judgeDiff = 0;
        else
            judgeDiff = diff;

        judgeResult = result;
        isJudged = true;
        PlayHoldEffect();
        PlayJudgeSFX();
    }
    
    private void Update()
    {
        var timing = GetJudgeTiming();
        var distance = timing * speed + 4.8f;
        var destScale = distance * 0.4f + 0.51f;
        if (destScale < 0f)
        {
            destScale = 0f;
            return;
        }

        spriteRenderer.forceRenderingOff = false;
        if (isEx) exSpriteRender.forceRenderingOff = false;

        spriteRenderer.size = new Vector2(1.22f, 1.4f);

        var holdTime = timing - LastFor;
        var holdDistance = holdTime * speed + 4.8f;
        if (holdTime >= 0 || 
            holdTime >= 0 && LastFor <= 0.15f)
        {
            tapLine.transform.localScale = new Vector3(1f, 1f, 1f);
            transform.position = getPositionFromDistance(4.8f);
            return;
        }


        transform.rotation = Quaternion.Euler(0, 0, -22.5f + -45f * (startPosition - 1));
        tapLine.transform.rotation = transform.rotation;
        holdEffect.transform.position = getPositionFromDistance(4.8f);

        if (isBreak &&
            !holdAnimStart && 
            !isJudged)
        {
            var extra = Math.Max(Mathf.Sin(timeProvider.GetFrame() * 0.17f) * 0.5f, 0);
            spriteRenderer.material.SetFloat("_Brightness", 0.95f + extra);
        }


        if (destScale > 0.3f) tapLine.SetActive(true);

        if (distance < 1.225f)
        {
            transform.localScale = new Vector3(destScale, destScale);
            spriteRenderer.size = new Vector2(1.22f, 1.42f);
            distance = 1.225f;
            var pos = getPositionFromDistance(distance);
            transform.position = pos;            
        }
        else
        {
            if (holdDistance < 1.225f && distance >= 4.8f) // 头到达 尾未出现
            {
                holdDistance = 1.225f;
                distance = 4.8f;
            }
            else if (holdDistance < 1.225f && distance < 4.8f) // 头未到达 尾未出现
            {
                holdDistance = 1.225f;
            }
            else if (holdDistance >= 1.225f && distance >= 4.8f) // 头到达 尾出现
            {
                distance = 4.8f;

                holdEndRender.enabled = true;
            }
            else if (holdDistance >= 1.225f && distance < 4.8f) // 头未到达 尾出现
            {
                holdEndRender.enabled = true;
            }

            var dis = (distance - holdDistance) / 2 + holdDistance;
            transform.position = getPositionFromDistance(dis); //0.325
            var size = distance - holdDistance + 1.4f;
            spriteRenderer.size = new Vector2(1.22f, size);
            holdEndRender.transform.localPosition = new Vector3(0f, 0.6825f - size / 2);
            transform.localScale = new Vector3(1f, 1f);
        }

        var lineScale = Mathf.Abs(distance / 4.8f);
        lineScale = lineScale >= 1f ? 1f : lineScale;
        tapLine.transform.localScale = new Vector3(lineScale, lineScale, 1f);
        exSpriteRender.size = spriteRenderer.size;
    }
    private void OnDestroy()
    {
        if (PlayManager.IsReloading) return;
        var realityHT = LastFor - 0.3f - (judgeDiff / 1000f);
        var percent = Math.Clamp((realityHT - playerIdleTime) / realityHT, 0, 1);
        var result = judgeResult; //头判
        if(realityHT > 0)
        {
            if (percent >= 1f)
            {
                if(judgeResult == JudgeType.Miss)
                    result = JudgeType.LateGood;
                else if (Math.Abs((int)judgeResult - 7) == 6)
                    result = (int)judgeResult < 7 ? JudgeType.LateGreat : JudgeType.FastGreat;
                else
                    result = judgeResult;
            }
            else if (percent >= 0.67f)
            {
                if (judgeResult == JudgeType.Miss)
                    result = JudgeType.LateGood;
                else if (Math.Abs((int)judgeResult - 7) == 6)
                    result = (int)judgeResult < 7 ? JudgeType.LateGreat : JudgeType.FastGreat;
                else if (judgeResult == JudgeType.Perfect)
                    result = (int)judgeResult < 7 ? JudgeType.LatePerfect1 : JudgeType.FastPerfect1;
            }
            else if (percent >= 0.33f)
            {
                if (Math.Abs((int)judgeResult - 7) >= 6)
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

        switch (InputManager.Mode)
        {
            case AutoPlayMode.Enable:
                result = JudgeType.Perfect;
                break;
            case AutoPlayMode.Random:
                result = (JudgeType)UnityEngine.Random.Range(1, 14);
                break;
            case AutoPlayMode.DjAuto:
            case AutoPlayMode.Disable:
                break;
        }

        if (isMine) //覆盖掉前面的判定
        {
            if (isTouched)
                result = JudgeType.Miss;
            else
                result = JudgeType.Perfect;
        }

        effectManager.PlayEffect(startPosition, isBreak, result);
        effectManager.PlayFastLate(startPosition, result);
        PlaySFX();
        print($"Hold: {MathF.Round(percent * 100,2)}%\nTotal Len : {MathF.Round(realityHT * 1000,2)}ms");

        objectCounter.ReportResult(SimaiNoteType.Hold, result, isBreak);
        if (!isJudged)
            noteManager.NextNote(startPosition);

        inputManager.SetSensorOff(sensor, guid);
        inputManager.UnbindArea(Check, sensor);
    }
    protected override void PlayHoldEffect()
    {
        base.PlayHoldEffect();
        Majdata<EffectManager>.Instance!.ResetEffect(startPosition - 1);
        if (LastFor <= 0.3)
            return;
        if (!holdAnimStart && GetJudgeTiming() >= 0.1f && !isMine)//忽略开头6帧与结尾12帧和mine
        {
            holdAnimStart = true;

            if (isBreak)
            {
                spriteRenderer.sprite = skinManager.Hold_Break_On;
                animator.runtimeAnimatorController = skinManager.Shine_Break;
            }
            else if (isEach)
            {
                spriteRenderer.sprite = skinManager.Hold_Each_On;
                animator.runtimeAnimatorController = skinManager.Shine;
            }
            else
            {
                spriteRenderer.sprite = skinManager.Hold_On;
                animator.runtimeAnimatorController = skinManager.Shine;
            }
            animator.enabled = true;
        }
    }
    protected override void StopHoldEffect()
    {
        base.StopHoldEffect();
        holdAnimStart = false;
        animator.enabled = false;
        spriteRenderer.sprite = skinManager.Hold_Off;
    }

    
    private void PlayJudgeSFX()
    {
        if (isPlayedJudgeSFX) return;

        audioManager.PlayTapSound(judgeResult, false, false);
        isPlayedJudgeSFX = true;
    }

    private void PlaySFX()
    {
        audioManager.PlayTapSound(judgeResult, isEx, isBreak);
    }
}