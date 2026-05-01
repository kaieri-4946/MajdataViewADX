using System;
using System.Collections.Generic;
using System.Linq;
using MajSimai;
using UnityEngine;

public partial class ObjectCounter : MonoBehaviour
{
    public void ReportResult(SimaiNoteType type, JudgeType result, bool isBreak = false)
    {
        switch(type)
        {
            case SimaiNoteType.Tap:
                if (isBreak)
                {
                    judgedBreakCount[result]++;
                    breakCount++;
                }
                else
                {
                    judgedTapCount[result]++;
                    tapCount++;
                }
                break;
            case SimaiNoteType.Slide:
                if (isBreak)
                {
                    judgedBreakCount[result]++;
                    breakCount++;
                }
                else
                {
                    judgedSlideCount[result]++;
                    slideCount++;
                }
                break;
            case SimaiNoteType.Hold:
                if (isBreak)
                {
                    judgedBreakCount[result]++;
                    breakCount++;
                }
                else
                {
                    judgedHoldCount[result]++;
                    holdCount++;
                }
                break;
            case SimaiNoteType.Touch:
                if (isBreak)
                {
                    judgedBreakCount[result]++;
                    breakCount++;
                }
                else
                {
                    judgedTouchCount[result]++;
                    touchCount++;
                }
                break;
            case SimaiNoteType.TouchHold:
                if (isBreak)
                {
                    judgedBreakCount[result]++;
                    breakCount++;
                }
                else
                {
                    judgedTouchHoldCount[result]++;
                    holdCount++;
                }
                break;

        }
        totalJudgedCount[result]++;
        if(result != 0)
            combo++;
        switch (result)
        {
            case JudgeType.Miss:
                missCount++;
                combo = 0;
                break;
            case JudgeType.Perfect:
                cPerfectCount++; 
                break;
            case JudgeType.LatePerfect2:
            case JudgeType.LatePerfect1:
            case JudgeType.FastPerfect1:
            case JudgeType.FastPerfect2:
                perfectCount++;
                break;
            case JudgeType.LateGreat2:
            case JudgeType.LateGreat1:
            case JudgeType.LateGreat:
            case JudgeType.FastGreat:
            case JudgeType.FastGreat1:
            case JudgeType.FastGreat2:
                greatCount++;
                break;
            case JudgeType.LateGood:
            case JudgeType.FastGood:
                goodCount++;
                break;
        }
    }
    public void CountNoteSum(IEnumerable<SimaiNote> notes)
    {
        foreach (var note in notes)
            if (!note.IsBreak)
            {
                if (note.Type == SimaiNoteType.Tap) tapSum++;
                if (note.Type == SimaiNoteType.Hold) holdSum++;
                if (note.Type == SimaiNoteType.TouchHold) holdSum++;
                if (note.Type == SimaiNoteType.Touch) touchSum++;
                if (note.Type == SimaiNoteType.Slide)
                {
                    if (!note.IsSlideNoHead) tapSum++;
                    if (note.IsSlideBreak)
                        breakSum++;
                    else
                        slideSum++;
                }
            }
            else
            {
                if (note.Type == SimaiNoteType.Slide)
                {
                    if (!note.IsSlideNoHead) breakSum++;
                    if (note.IsSlideBreak)
                        breakSum++;
                    else
                        slideSum++;
                }
                else
                {
                    breakSum++;
                }
            }
    }

    public void CountNoteCount(IEnumerable<SimaiNote> notes)
    {
        foreach (var note in notes)
            if (!note.IsBreak)
            {
                if (note.Type == SimaiNoteType.Tap) tapCount++;
                if (note.Type == SimaiNoteType.Hold) holdCount++;
                if (note.Type == SimaiNoteType.TouchHold) holdCount++;
                if (note.Type == SimaiNoteType.Touch) touchCount++;
                if (note.Type == SimaiNoteType.Slide)
                {
                    if (!note.IsSlideNoHead) tapCount++;
                    if (note.IsSlideBreak)
                        breakCount++;
                    else
                        slideCount++;
                }
            }
            else
            {
                if (note.Type == SimaiNoteType.Slide)
                {
                    if (!note.IsSlideNoHead) breakCount++;
                    if (note.IsSlideBreak)
                        breakCount++;
                    else
                        slideCount++;
                }
                else
                {
                    breakCount++;
                }
            }
    }
    public void ReportMeterBpm(SimaiTimingPoint timing)
    {
        var (lastNum, lastDeno) = meterList.LastOrDefault().Value;
        if (timing.SignatureNumerator != lastNum || timing.SignatureDenominator != lastDeno)
            meterList.TryAdd(timing.Timing, (timing.SignatureNumerator, timing.SignatureDenominator));
        if (timing.Bpm != bpmList.LastOrDefault().Value)
            bpmList.TryAdd(timing.Timing, timing.Bpm);
    }
    
    private NoteScore GetNoteScoreSum()
    {
        Dictionary<JudgeType, int> collection = null;
        long score = 0;
        long lostScore = 0;
        long extraScore = 0;
        long extraScoreClassic = 0;
        long lostExtraScore = 0;
        long lostExtraScoreClassic = 0;
        int baseScore = 500;

        foreach(var type in new[] { SimaiNoteType.Tap, SimaiNoteType.Slide, SimaiNoteType.Hold, SimaiNoteType.Touch })
        {
            switch (type)
            {
                case SimaiNoteType.Tap:
                    collection = judgedTapCount;
                    baseScore = 500;
                    break;
                case SimaiNoteType.Slide:
                    collection = judgedSlideCount;
                    baseScore = 1500;
                    break;
                case SimaiNoteType.TouchHold:
                case SimaiNoteType.Hold:
                    collection = judgedHoldCount;
                    baseScore = 1000;
                    break;
                case SimaiNoteType.Touch:
                    collection = judgedTouchCount;
                    baseScore = 500;
                    break;
            }

            foreach (var judgeResult in collection)
            {
                var count = judgeResult.Value;
                switch (judgeResult.Key)
                {
                    case JudgeType.LatePerfect2:
                    case JudgeType.LatePerfect1:
                    case JudgeType.Perfect:
                    case JudgeType.FastPerfect1:
                    case JudgeType.FastPerfect2:
                        score += baseScore * 1 * count;
                        break;
                    case JudgeType.LateGreat2:
                    case JudgeType.LateGreat1:
                    case JudgeType.LateGreat:
                    case JudgeType.FastGreat:
                    case JudgeType.FastGreat1:
                    case JudgeType.FastGreat2:
                        score += (long)(baseScore * 0.8) * count;
                        lostScore += (long)(baseScore * 0.2) * count;
                        break;
                    case JudgeType.LateGood:
                    case JudgeType.FastGood:
                        score += (long)(baseScore * 0.5) * count;
                        lostScore += (long)(baseScore * 0.5) * count;
                        break;
                    case JudgeType.Miss:
                        lostScore += baseScore * count;
                        break;
                }
            }
        }
        foreach (var judgeResult in judgedBreakCount)
        {
            var count = judgeResult.Value;
            switch (judgeResult.Key)
            {
                case JudgeType.Perfect:
                    score += 2500 * count;
                    extraScore += 100 * count;
                    extraScoreClassic += 100 * count;
                    break;
                case JudgeType.LatePerfect1:  
                case JudgeType.FastPerfect1:
                    score += 2500 * count;
                    extraScore += 75 * count;
                    extraScoreClassic += 50 * count;
                    lostExtraScore += 25 * count;
                    lostExtraScoreClassic += 50 * count;
                    break;
                case JudgeType.LatePerfect2:
                case JudgeType.FastPerfect2:
                    score += 2500 * count;
                    extraScore += 50 * count;
                    extraScoreClassic += 0 * count;
                    lostExtraScore += 50 * count;
                    lostExtraScoreClassic += 100 * count;
                    break;
                case JudgeType.LateGreat:
                case JudgeType.FastGreat:
                    score += 2000 * count;
                    extraScore += 40 * count;
                    extraScoreClassic += 0 * count;
                    lostScore += 500 * count;
                    lostExtraScore += 60 * count;
                    lostExtraScoreClassic += 100 * count;
                    break;
                case JudgeType.LateGreat1:
                case JudgeType.FastGreat1:
                    score += 1500 * count;
                    extraScore += 40 * count;
                    extraScoreClassic += 0 * count;
                    lostScore += 1000 * count;
                    lostExtraScore += 60 * count;
                    lostExtraScoreClassic += 100 * count;
                    break;
                case JudgeType.LateGreat2:
                case JudgeType.FastGreat2:
                    score += 1250 * count;
                    extraScore += 40 * count;
                    extraScoreClassic += 0 * count;
                    lostScore += 1250 * count;
                    lostExtraScore += 60 * count;
                    lostExtraScoreClassic += 100 * count;
                    break;
                case JudgeType.LateGood:
                case JudgeType.FastGood:
                    score += 1000 * count;
                    extraScore += 30 * count;
                    extraScoreClassic += 0 * count;
                    lostScore += 1500 * count;
                    lostExtraScore += 70 * count;
                    lostExtraScoreClassic += 100 * count;
                    break;
                case JudgeType.Miss:
                    score += 0 * count;
                    extraScore += 0 * count;
                    extraScoreClassic += 0 * count;
                    lostScore += 2500 * count;
                    lostExtraScore += 100 * count;
                    lostExtraScoreClassic += 100 * count;
                    break;
            }
        }
        return new NoteScore()
        {
            TotalScore = score,
            TotalExtraScore = extraScore,
            TotalExtraScoreClassic = extraScoreClassic,
            LostScore = lostScore,
            LostExtraScore = lostExtraScore,
            LostExtraScoreClassic = lostExtraScoreClassic
        };
    }
    private void CalAccRate()
    {
        var currentNoteScore = GetNoteScoreSum();

        long totalScore = (tapSum + touchSum) * 500 + holdSum * 1000 + slideSum * 1500 + breakSum * 2500;
        long totalExtraScore = breakSum * 100;

        accRate[0] = ((currentNoteScore.TotalScore + currentNoteScore.TotalExtraScoreClassic) / (double)totalScore) * 100;
        accRate[1] = ((totalScore + currentNoteScore.TotalExtraScoreClassic - currentNoteScore.LostScore) / (double)totalScore) * 100;
        accRate[2] = ((totalScore - currentNoteScore.LostScore) / (double)totalScore) * 100 + ((totalExtraScore - currentNoteScore.LostExtraScore) / (double)totalExtraScore);
        accRate[3] = ((totalScore - currentNoteScore.LostScore) / (double)totalScore) * 100 + (currentNoteScore.TotalExtraScore / (double)totalExtraScore);
        accRate[4] = (currentNoteScore.TotalScore / (double)totalScore) * 100 + (currentNoteScore.TotalExtraScore / (double)totalExtraScore);
    }
    
    private int FiSumScore()
    {
        return tapSum * 500 + holdSum * 1000 + slideSum * 1500 + touchSum * 500 + breakSum * 2500;
    }

    private int FiNowScore()
    {
        return tapCount * 500 + holdCount * 1000 + slideCount * 1500 + touchCount * 500 + breakCount * 2600;
    }

    private int FiNowBreakScore()
    {
        return tapSum * 500 + holdSum * 1000 + slideSum * 1500 + touchSum * 500 + breakSum * 2500 + breakCount * 100;
    }

    private int DxSumScore()
    {
        return tapSum * 1 + holdSum * 2 + slideSum * 3 + touchSum * 1 + breakSum * 5;
    }

    private int DxNowScore()
    {
        return tapCount * 1 + holdCount * 2 + slideCount * 3 + touchCount * 1 + breakCount * 5;
    }

    private int DxExSumScore()
    {
        return (tapSum + holdSum + slideSum + touchSum + breakSum) * 3;
    }

    private int DxExNowScore()
    {
        return (tapCount + holdCount + slideCount + touchCount + breakCount) * 3;
    }

    private int DeDxNowScore()
    {
        return (int)Math.Round(FiSumScore() * ((float)DxNowScore() / DxSumScore() + BreakRate() / 100f) / 5) * 5;
    }

    private int DeDxNowBreakScore()
    {
        return (int)Math.Round(FiSumScore() * (1f + BreakRate() / 100f) / 5) * 5;
    }
    private float BreakRate()
    {
        return breakSum > 0 ? (float)breakCount / breakSum : 0f;
    }
}