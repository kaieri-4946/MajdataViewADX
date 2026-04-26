using System;
using MajSimai;
using UnityEngine;
#nullable enable

public class TapBase : NoteBase
{
    public GameObject tapLine;
    
    protected SpriteRenderer spriteRenderer;
    protected SpriteRenderer exSpriteRender;
    protected SpriteRenderer lineSpriteRenderer;
    
    protected bool isTriggered = false;

    protected void PreLoad()
    {
        var notes = GameObject.Find("Notes").transform;
        noteManager = Majdata<NoteManager>.Instance!;
        timeProvider = Majdata<TimeProvider>.Instance!;
        objectCounter = Majdata<ObjectCounter>.Instance!;
        inputManager = Majdata<InputManager>.Instance!;
        skinManager = Majdata<SkinManager>.Instance!;
        audioManager = Majdata<AudioManager>.Instance!;
        
        tapLine = Instantiate(tapLine, notes);
        tapLine.SetActive(false);
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        lineSpriteRenderer = tapLine.GetComponent<SpriteRenderer>();
        exSpriteRender = transform.GetChild(0).GetComponent<SpriteRenderer>();
        
        spriteRenderer.sortingOrder += noteSortOrder;
        exSpriteRender.sortingOrder += noteSortOrder;
    }

    protected void FixedUpdate()
    {
        var timing = GetJudgeTiming();
        if (isMine && !isJudged && timing >= 0.016667f)
        {
            judgeResult = JudgeType.Perfect;
            isJudged = true;
        }
        else if (!isJudged && timing > 0.15f)
        {
            judgeResult = JudgeType.Miss;
            isJudged = true;
            Destroy(tapLine);
            Destroy(gameObject);
        }
        else if (isJudged)
        {
            Destroy(tapLine);
            Destroy(gameObject);
        }
        else if (timing >= -0.01f)
        {
            switch (InputManager.Mode)
            {
                case AutoPlayMode.Enable:
                    if (isMine)
                        judgeResult = JudgeType.Miss;
                    else
                        judgeResult = JudgeType.Perfect;
                    isJudged = true;
                    break;
                case AutoPlayMode.Random:
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
                    }

                    isJudged = true;
                    break;
                case AutoPlayMode.DjAuto:
                    if (isTriggered)
                        break;
                    //mine就不打了
                    if (!isMine)
                        inputManager.ClickSensor(sensor);
                    isTriggered = true;
                    break;
            }
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        var timing = GetJudgeTiming();
        var distance = timing * speed + 4.8f;
        var destScale = distance * 0.4f + 0.51f;

        switch (State)
        {
            case NoteStatus.Initialized:
                if (destScale >= 0f)
                {
                    tapLine.transform.rotation = Quaternion.Euler(0, 0, -22.5f + -45f * (startPosition - 1));
                    State = NoteStatus.Pending;
                    goto case NoteStatus.Pending;
                }
                
                transform.localScale = new Vector3(0, 0);
                return;
            case NoteStatus.Pending:
            {
                if (destScale > 0.3f)
                    tapLine.SetActive(true);
                if (distance < 1.225f)
                {
                    transform.localScale = new Vector3(destScale, destScale);
                    transform.position = getPositionFromDistance(1.225f);
                    var lineScale = Mathf.Abs(1.225f / 4.8f);
                    tapLine.transform.localScale = new Vector3(lineScale, lineScale, 1f);
                }
                else
                {
                    State = NoteStatus.Running;
                    goto case NoteStatus.Running;
                }
            }
                break;
            case NoteStatus.Running:
            {
                transform.position = getPositionFromDistance(distance);
                transform.localScale = new Vector3(1f, 1f);
                var lineScale = Mathf.Abs(distance / 4.8f);
                tapLine.transform.localScale = new Vector3(lineScale, lineScale, 1f);
            }
                break;
        }

        spriteRenderer.forceRenderingOff = false;
        if (isEx) exSpriteRender.forceRenderingOff = false;
        if (isBreak)
        {
            var extra = Math.Max(Mathf.Sin(timeProvider.GetFrame() * 0.17f) * 0.5f, 0);
            spriteRenderer.material.SetFloat("_Brightness", 0.95f + extra);
        }
    }

    protected void Check(object sender, InputEventArgs arg)
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
                Destroy(tapLine);
                Destroy(gameObject);
            }
        }
    }

    protected void Judge()
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

        if (isMine)
        {
            judgeResult = JudgeType.Miss;
            isJudged = true;
            return;
        }

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

        judgeResult = result;
        isJudged = true;
    }

    protected virtual void OnDestroy()
    {
        if (PlayManager.IsReloading) return;
        var effectManager = Majdata<EffectManager>.Instance!;
        effectManager.PlayEffect(startPosition, isBreak, judgeResult);
        effectManager.PlayFastLate(startPosition, judgeResult);
        audioManager.PlayTapSound(judgeResult, isEx, isBreak);
        noteManager.NextNote(startPosition);
        objectCounter.ReportResult(SimaiNoteType.Tap, judgeResult, isBreak);
        inputManager.UnbindArea(Check, sensor);
    }
}