#region

using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using MajSimai;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#endregion

public class ObjectCounter : MonoBehaviour
{
    [SerializeField]
    Color AchievementDudColor; // = new Color32(63, 127, 176, 255);
    [SerializeField]
    Color AchievementBronzeColor; // = new Color32(127, 48, 32, 255);
    [SerializeField]
    Color AchievementSilverColor; // = new Color32(160, 160, 160, 255);
    [SerializeField]
    Color AchievementGoldColor; // = new Color32(224, 191, 127, 255);

    public BgInfoDisplay TextMode { get; private set; }
    public UIType? CurrentUIType { get; private set; } = null;

    public bool AllFinished =>
        TapFinishedCount == TapSum &&
        HoldFinishedCount == HoldSum &&
        SlideFinishedCount == SlideSum &&
        TouchFinishedCount == TouchSum &&
        BreakFinishedCount == BreakSum;
    public int TapFinishedCount { get; private set; }
    public int HoldFinishedCount { get; private set; }
    public int SlideFinishedCount { get; private set; }
    public int TouchFinishedCount { get; private set; }
    public int BreakFinishedCount { get; private set; }
    public int NoteFinishedCount =>
        TapFinishedCount +
        HoldFinishedCount +
        SlideFinishedCount +
        TouchFinishedCount +
        BreakFinishedCount;

    public int TapSum { get; private set; }
    public int HoldSum { get; private set; }
    public int SlideSum { get; private set; }
    public int TouchSum { get; private set; }
    public int BreakSum { get; private set; }
    public int NoteSum { get; private set; }

    public long TotalNoteScore => TotalNoteBaseScore + TotalNoteExtraScore;
    public long TotalNoteBaseScore { get; private set; }
    public long TotalNoteExtraScore { get; private set; }

    public long CurrentNoteScore => CurrentNoteBaseScore + CurrentNoteExtraScore;
    public long CurrentNoteScoreClassic => CurrentNoteBaseScore + CurrentNoteExtraScoreClassic;
    public long CurrentNoteBaseScore { get; private set; }
    public long CurrentNoteExtraScore { get; private set; }
    public long CurrentNoteExtraScoreClassic { get; private set; }

    public long TotalLostNoteScore => LostNoteBaseScore + LostNoteExtraScore;
    public long TotalLostNoteScoreClassic => LostNoteBaseScore + LostNoteExtraScoreClassic;
    public long LostNoteBaseScore { get; private set; }
    public long LostNoteExtraScore { get; private set; }
    public long LostNoteExtraScoreClassic { get; private set; }

    double[] accRate = new double[5]
    {
        0.00,    // classic acc (+)
        100.00,  // classic acc (-)
        101.0000,// acc 101(-)
        100.0000,// acc 100(-)
        0.0000,  // acc (+)
    };

    private double ClassicRateFromCount
    {
        get
        {
            // 提取公约数 500，简化系数：
            // Tap(1), Hold(2), Slide(3), Touch(1), Break(5.2 / 5.0)
            // Break 的权重在分子是 2600 (5.2倍)，分母是 2500 (5.0倍)
            var currentScore = TapFinishedCount +
                                  HoldFinishedCount * 2.0 +
                                  SlideFinishedCount * 3.0 +
                                  TouchFinishedCount * 1.0 +
                                  BreakFinishedCount * 5.2;

            var totalScore = TapSum +
                                HoldSum * 2.0 +
                                SlideSum * 3.0 +
                                TouchSum * 1.0 +
                                BreakSum * 5.0;

            if (totalScore <= 0) return 0.0;
            var rate = (currentScore / totalScore) * 100.0;
            return Math.Round(rate, 4);
        }
    }

    private double DeluxeRateFromCount
    {
        get
        {
            var currentDeluxe = TapFinishedCount +
                                   HoldFinishedCount * 2.0 +
                                   SlideFinishedCount * 3.0 +
                                   TouchFinishedCount * 1.0 +
                                   BreakFinishedCount * 5.0;

            var totalDeluxe = TapSum +
                                 HoldSum * 2.0 +
                                 SlideSum * 3.0 +
                                 TouchSum * 1.0 +
                                 BreakSum * 5.0;

            var baseRate = totalDeluxe > 0 ? (currentDeluxe / totalDeluxe) * 100.0 : 0.0;
            var breakBonus = BreakSum > 0 ? (double)BreakFinishedCount / BreakSum : 0.0;
            var finalRate = baseRate + breakBonus;

            return Math.Round(finalRate, 4);
        }
    }
    long cPerfectCount = 0;
    long perfectCount = 0;
    long greatCount = 0;
    long goodCount = 0;
    long missCount = 0;

    long fastCount = 0;
    long lateCount = 0;

    long totalDXScore = 0;
    long lostDXScore = 0;

    long combo = 0; // Combo

    readonly Dictionary<JudgeType, int> judgedTapCount = new();
    readonly Dictionary<JudgeType, int> judgedHoldCount = new();
    readonly Dictionary<JudgeType, int> judgedTouchCount = new();
    readonly Dictionary<JudgeType, int> judgedTouchHoldCount = new();
    readonly Dictionary<JudgeType, int> judgedSlideCount = new();
    readonly Dictionary<JudgeType, int> judgedBreakCount = new();
    readonly Dictionary<JudgeType, int> totalJudgedCount = new();

    readonly Dictionary<double, (int, int)> meterList = new();
    readonly Dictionary<double, float> bpmList = new();

    //Legacy UI
    [SerializeField]
    private GameObject legacyUIRoot;
    [SerializeField]
    private Text timeDisplay;
    [SerializeField]
    private Text objectCount;
    [SerializeField]
    private Text objectRate;
    [SerializeField]
    private Text judgeResultCount;

    //Trg UI
    [SerializeField]
    private GameObject trgUIRoot;
    [SerializeField]
    private TextMeshProUGUI objTime;
    [SerializeField]
    private TextMeshProUGUI objRate;
    [SerializeField]
    private TextMeshProUGUI objCombo;
    [SerializeField]
    private TextMeshProUGUI objNoteCount;
    [SerializeField]
    private TextMeshProUGUI objMeter;
    [SerializeField]
    private TextMeshProUGUI objBpm;
    [SerializeField]
    private TextMeshProUGUI objBpmRange;
    [SerializeField]
    private TextMeshProUGUI objJudgeResult;
    [SerializeField]
    private TextMeshProUGUI objAutoMode;


    //Main Output
    [SerializeField]
    private Text statusAchievement;
    [SerializeField]
    private Text statusCombo;
    [SerializeField]
    private Text statusDXScore;

    private void Awake()
    {
        Majdata<ObjectCounter>.Instance = this;
    }

    private void Start()
    {
        ResetState();
    }

    private void Update()
    {
        UpdateOutput();
    }

    public async UniTask CountNoteSumAsync(SimaiChart chart)
    {
        await UniTask.RunOnThreadPool(() =>
        {
            foreach (var timing in chart.NoteTimings)
            {
                foreach (var note in timing.Notes)
                {
                    if (!note.IsBreak)
                    {
                        switch (note.Type)
                        {
                            case SimaiNoteType.Tap:
                                TapSum++;
                                break;
                            case SimaiNoteType.Hold:
                            case SimaiNoteType.TouchHold:
                                HoldSum++;
                                break;
                            case SimaiNoteType.Slide:
                                if (!note.IsSlideNoHead)
                                    TapSum++;
                                if (note.IsSlideBreak)
                                    BreakSum++;
                                else
                                    SlideSum++;
                                break;
                            case SimaiNoteType.Touch:
                                TouchSum++;
                                break;
                        }
                    }
                    else
                    {
                        if (note.Type == SimaiNoteType.Slide)
                        {
                            if (!note.IsSlideNoHead)
                                BreakSum++;
                            if (note.IsSlideBreak)
                                BreakSum++;
                            else
                                SlideSum++;
                        }
                        else
                        {
                            BreakSum++;
                        }
                    }
                }
            }
            NoteSum = TapSum + HoldSum + TouchSum + BreakSum + SlideSum;
            TotalNoteBaseScore = (TapSum + TouchSum) * 500 + HoldSum * 1000 + SlideSum * 1500 + BreakSum * 2500;
            TotalNoteExtraScore = BreakSum * 100;
            totalDXScore = NoteSum * 3;
        });
    }

    public async UniTask CountIgnoreNoteCountAsync(IEnumerable<SimaiNote> notes)
    {
        await UniTask.RunOnThreadPool(() =>
        {
            foreach (var note in notes)
            {
                if (!note.IsBreak)
                {
                    switch (note.Type)
                    {
                        case SimaiNoteType.Tap:
                            TapFinishedCount++;
                            break;
                        case SimaiNoteType.Hold:
                        case SimaiNoteType.TouchHold:
                            HoldFinishedCount++;
                            break;
                        case SimaiNoteType.Slide:
                            if (!note.IsSlideNoHead)
                                TapFinishedCount++;
                            if (note.IsSlideBreak)
                                BreakFinishedCount++;
                            else
                                SlideFinishedCount++;
                            break;
                        case SimaiNoteType.Touch:
                            TouchFinishedCount++;
                            break;
                    }
                }
                else
                {
                    if (note.Type == SimaiNoteType.Slide)
                    {
                        if (!note.IsSlideNoHead)
                            BreakFinishedCount++;
                        if (note.IsSlideBreak)
                            BreakFinishedCount++;
                        else
                            SlideFinishedCount++;
                    }
                    else
                    {
                        BreakFinishedCount++;
                    }
                }
            }
        });
    }

    public void StartOutput(BgInfoDisplay mode, UIType type)
    {
        TextMode = mode;
        switch (mode)
        {
            case BgInfoDisplay.None:
                statusCombo.gameObject.SetActive(false);
                statusAchievement.gameObject.SetActive(false);
                statusDXScore.gameObject.SetActive(false);
                break;
            case BgInfoDisplay.Combo:
                statusCombo.gameObject.SetActive(true);
                statusAchievement.gameObject.SetActive(false);
                statusDXScore.gameObject.SetActive(false);
                break;
            case BgInfoDisplay.Achievement_101:
            case BgInfoDisplay.Achievement_100:
            case BgInfoDisplay.Achievement:
            case BgInfoDisplay.AchievementClassical:
            case BgInfoDisplay.AchievementClassical_100:
            case BgInfoDisplay.S_Border:
            case BgInfoDisplay.SS_Border:
            case BgInfoDisplay.SSS_Border:
                statusCombo.gameObject.SetActive(false);
                statusAchievement.gameObject.SetActive(true);
                statusDXScore.gameObject.SetActive(false);
                break;
            case BgInfoDisplay.DXScore:
                statusCombo.gameObject.SetActive(false);
                statusAchievement.gameObject.SetActive(false);
                statusDXScore.gameObject.SetActive(true);
                break;
        }
        if (type is UIType.TrgUI)
        {
            switch (Majdata<InputManager>.Instance!.Mode)
            {
                case AutoPlayMode.Enable:
                    objAutoMode.text = "ENABLED\nNONE";
                    break;
                case AutoPlayMode.DJAuto:
                    objAutoMode.text = "ENABLED\nDJAuto";
                    break;
                case AutoPlayMode.Random:
                    objAutoMode.text = "ENABLED\nRANDOM";
                    break;
                case AutoPlayMode.Disable:
                    objAutoMode.text = "DISABLED\nNONE";
                    break;
            }

            float min, max;
            min = max = bpmList.FirstOrDefault().Value;
            foreach (var bpm in bpmList.Values)
            {
                if (bpm < min) min = bpm;
                if (bpm > max) max = bpm;
            }

            objBpmRange.text = $"{min} ～ {max}";
        }
        if (CurrentUIType == type) return;
        switch (type)
        {
            case UIType.Legacy:
                {
                    CurrentUIType = type;
                    legacyUIRoot.SetActive(true);
                    trgUIRoot.SetActive(false);
                    break;
                }
            case UIType.TrgUI:
                {
                    CurrentUIType = type;
                    legacyUIRoot.SetActive(false);
                    trgUIRoot.SetActive(true);
                    break;
                }
        }
    }

    public async UniTask ReportMeterBpmAsync(SimaiChart chart)
    {
        await UniTask.RunOnThreadPool(() =>
        {
            foreach (var timing in chart.CommaTimings)
            {
                var (lastNum, lastDeno) = meterList.LastOrDefault().Value;
                if (timing.SignatureNumerator != lastNum || timing.SignatureDenominator != lastDeno)
                    meterList.TryAdd(timing.Timing, (timing.SignatureNumerator, timing.SignatureDenominator));
                if (timing.Bpm != bpmList.LastOrDefault().Value)
                    bpmList.TryAdd(timing.Timing, timing.Bpm);
            }
        });
    }

    public void ReportResult(SimaiNoteType type, JudgeType judgeType, bool isBreak = false)
    {
        UpdateNoteScoreCount(type, judgeType, isBreak);
        UpdateAccRate();
        UpdateNoteCount(type, judgeType, isBreak);
        UpdateFastLateCount(judgeType);
    }

    private void UpdateNoteScoreCount(SimaiNoteType type, JudgeType judgeType, bool isBreak)
    {
        var baseScore = 500;

        switch (type)
        {
            case SimaiNoteType.Tap:
            case SimaiNoteType.Touch:
                baseScore = 500;
                break;
            case SimaiNoteType.Hold:
            case SimaiNoteType.TouchHold:
                baseScore = 1000;
                break;
            case SimaiNoteType.Slide:
                baseScore = 1500;
                break;
        }

        if (!isBreak)
        {
            switch (judgeType)
            {
                case JudgeType.Miss:
                    //CurrentNoteBaseScore += baseScore * 0;
                    LostNoteBaseScore += baseScore;
                    break;
                case JudgeType.LateGood:
                case JudgeType.FastGood:
                    CurrentNoteBaseScore += (long)(baseScore * 0.5);
                    LostNoteBaseScore += (long)(baseScore * 0.5);
                    break;
                case JudgeType.LateGreat2:
                case JudgeType.LateGreat1:
                case JudgeType.LateGreat:
                case JudgeType.FastGreat:
                case JudgeType.FastGreat1:
                case JudgeType.FastGreat2:
                    CurrentNoteBaseScore += (long)(baseScore * 0.8);
                    LostNoteBaseScore += (long)(baseScore * 0.2);
                    break;
                default:
                    CurrentNoteBaseScore += baseScore;
                    //LostNoteBaseScore += 0;
                    break;
            }
        }
        else
        {

            switch (judgeType)
            {
                case JudgeType.Miss:
                    LostNoteBaseScore += 2500;
                    LostNoteExtraScore += 100;
                    LostNoteExtraScoreClassic += 100;
                    break;
                case JudgeType.LateGood:
                case JudgeType.FastGood:
                    CurrentNoteBaseScore += 1000;
                    CurrentNoteExtraScore += 30;
                    LostNoteBaseScore += 1500;
                    LostNoteExtraScore += 70;
                    LostNoteExtraScoreClassic += 100;
                    break;
                case JudgeType.LateGreat2:
                case JudgeType.FastGreat2:
                    CurrentNoteBaseScore += 1250;
                    CurrentNoteExtraScore += 40;
                    LostNoteBaseScore += 1250;
                    LostNoteExtraScore += 60;
                    LostNoteExtraScoreClassic += 100;
                    break;
                case JudgeType.FastGreat1:
                case JudgeType.LateGreat1:
                    CurrentNoteBaseScore += 1500;
                    CurrentNoteExtraScore += 40;
                    LostNoteBaseScore += 1000;
                    LostNoteExtraScore += 60;
                    LostNoteExtraScoreClassic += 100;
                    break;
                case JudgeType.LateGreat:
                case JudgeType.FastGreat:
                    CurrentNoteBaseScore += 2000;
                    CurrentNoteExtraScore += 40;
                    LostNoteBaseScore += 500;
                    LostNoteExtraScore += 60;
                    LostNoteExtraScoreClassic += 100;
                    break;
                case JudgeType.LatePerfect2:
                case JudgeType.FastPerfect2:
                    CurrentNoteBaseScore += 2500;
                    CurrentNoteExtraScore += 50;
                    LostNoteExtraScore += 50;
                    LostNoteExtraScoreClassic += 100;
                    break;
                case JudgeType.LatePerfect1:
                case JudgeType.FastPerfect1:
                    CurrentNoteBaseScore += 2500;
                    CurrentNoteExtraScore += 75;
                    CurrentNoteExtraScoreClassic += 50;
                    LostNoteExtraScore += 25;
                    LostNoteExtraScoreClassic += 50;
                    break;
                case JudgeType.Perfect:
                    CurrentNoteBaseScore += 2500;
                    CurrentNoteExtraScore += 100;
                    CurrentNoteExtraScoreClassic += 100;
                    LostNoteExtraScore += 0;
                    LostNoteExtraScoreClassic += 0;
                    break;
            }
        }
    }
    private void UpdateAccRate()
    {
        // classic acc (+)
        // classic acc (-)
        // acc 101(-)
        // acc 100(-)
        // acc (+)
        Span<decimal> newAccRate = stackalloc decimal[5];

        newAccRate[0] = CurrentNoteScoreClassic / (decimal)TotalNoteBaseScore;
        newAccRate[1] = (CurrentNoteBaseScore - LostNoteBaseScore + CurrentNoteExtraScoreClassic) / (decimal)TotalNoteBaseScore;
        newAccRate[2] = ((TotalNoteBaseScore - LostNoteBaseScore) / (decimal)TotalNoteBaseScore) + ((TotalNoteExtraScore - LostNoteExtraScore) / ((decimal)(TotalNoteExtraScore is 0 ? 1 : TotalNoteExtraScore) * 100));
        newAccRate[3] = ((TotalNoteBaseScore - LostNoteBaseScore) / (decimal)TotalNoteBaseScore) + ((CurrentNoteExtraScore) / ((decimal)(TotalNoteExtraScore is 0 ? 1 : TotalNoteExtraScore) * 100));
        newAccRate[4] = ((CurrentNoteBaseScore) / (decimal)TotalNoteBaseScore) + ((CurrentNoteExtraScore) / ((decimal)(TotalNoteExtraScore is 0 ? 1 : TotalNoteExtraScore) * 100));

        accRate[0] = decimal.ToDouble(newAccRate[0] * 100);
        accRate[1] = decimal.ToDouble(newAccRate[1] * 100);
        accRate[2] = decimal.ToDouble(newAccRate[2] * 100);
        accRate[3] = decimal.ToDouble(newAccRate[3] * 100);
        accRate[4] = decimal.ToDouble(newAccRate[4] * 100);
    }
    private void UpdateNoteCount(SimaiNoteType type, JudgeType judgeType, bool isBreak)
    {
        if (isBreak)
        {
            judgedBreakCount[judgeType]++;
            BreakFinishedCount++;
        }
        else
        {
            switch (type)
            {
                case SimaiNoteType.Tap:
                    {
                        judgedTapCount[judgeType]++;
                        TapFinishedCount++;
                    }
                    break;
                case SimaiNoteType.Slide:
                    {
                        judgedSlideCount[judgeType]++;
                        SlideFinishedCount++;
                    }
                    break;
                case SimaiNoteType.Hold:
                    {
                        judgedHoldCount[judgeType]++;
                        HoldFinishedCount++;
                    }
                    break;
                case SimaiNoteType.Touch:
                    {
                        judgedTouchCount[judgeType]++;
                        TouchFinishedCount++;
                    }
                    break;
                case SimaiNoteType.TouchHold:
                    {
                        judgedTouchHoldCount[judgeType]++;
                        HoldFinishedCount++;
                    }
                    break;
            }
        }
        totalJudgedCount[judgeType]++;

        if (judgeType != 0) combo++;
        switch (judgeType)
        {
            case JudgeType.Miss:
                missCount++;
                combo = 0;
                lostDXScore -= 3;
                break;
            case JudgeType.Perfect:
                cPerfectCount++;
                break;
            case JudgeType.LatePerfect2:
            case JudgeType.LatePerfect1:
            case JudgeType.FastPerfect1:
            case JudgeType.FastPerfect2:
                perfectCount++;
                lostDXScore -= 1;
                break;
            case JudgeType.LateGreat2:
            case JudgeType.LateGreat1:
            case JudgeType.LateGreat:
            case JudgeType.FastGreat:
            case JudgeType.FastGreat1:
            case JudgeType.FastGreat2:
                lostDXScore -= 2;
                greatCount++;
                break;
            case JudgeType.LateGood:
            case JudgeType.FastGood:
                lostDXScore -= 3;
                goodCount++;
                break;
        }
    }
    private void UpdateFastLateCount(JudgeType judgeType)
    {
        if ((int)judgeType < 7)
        {
            fastCount++;
        }
        else if ((int)judgeType > 7)
        {
            lateCount++;
        }
    }

    private void UpdateOutput()
    {
        OutputMain();
        OutputSide();
        OutputTime();
    }

    private void OutputMain()
    {
        switch (TextMode)
        {
            case BgInfoDisplay.Combo:
                {
                    statusCombo.text = combo > 0 ?
                        combo.ToString() : string.Empty;
                }
                break;
            case BgInfoDisplay.Achievement_101:
                {
                    statusAchievement.text = $"{accRate[2]:0.0000}%";
                    UpdateAchievementColor(accRate[2], statusAchievement);
                }
                break;
            case BgInfoDisplay.Achievement_100:
                {
                    statusAchievement.text = $"{accRate[3]:0.0000}%";
                    UpdateAchievementColor(accRate[3], statusAchievement);
                }
                break;
            case BgInfoDisplay.Achievement:
                {
                    statusAchievement.text = $"{accRate[4]:0.0000}%";
                    UpdateAchievementColor(accRate[4], statusAchievement);
                }
                break;
            case BgInfoDisplay.AchievementClassical:
                {
                    statusAchievement.text = $"{accRate[0]:0.0000}%";
                    UpdateAchievementColor(accRate[0], statusAchievement);
                }
                break;
            case BgInfoDisplay.AchievementClassical_100:
                {
                    statusAchievement.text = $"{accRate[1]:0.0000}%";
                    UpdateAchievementColor(accRate[1], statusAchievement);
                }
                break;
            case BgInfoDisplay.DXScore:
                {
                    statusDXScore.text = (totalDXScore + lostDXScore).ToString();
                }
                break;
            case BgInfoDisplay.S_Border:
                {
                    var rate = accRate[2] - 97;
                    UpdateBorder(rate, statusAchievement);
                }
                break;
            case BgInfoDisplay.SS_Border:
                {
                    var rate = accRate[2] - 99;
                    UpdateBorder(rate, statusAchievement);
                }
                break;
            case BgInfoDisplay.SSS_Border:
                {
                    var rate = accRate[2] - 100;
                    UpdateBorder(rate, statusAchievement);
                }
                break;
        }
        void UpdateAchievementColor(double rate, Text textElement)
        {
            var newColor = rate switch
            {
                >= 100 => AchievementGoldColor,
                >= 97f => AchievementSilverColor,
                >= 80f => AchievementBronzeColor,
                _ => AchievementDudColor
            };

            if (textElement.color != newColor)
            {
                textElement.color = newColor;
            }

            var headerElement = textElement.transform.GetChild(0).GetComponent<Text>();
            if (headerElement.color != newColor)
            {
                headerElement.color = newColor;
            }
        }

        void UpdateBorder(double rate, Text textElement)
        {
            if (rate <= 0)
            {
                textElement.gameObject.SetActive(false);
                return;
            }
            textElement.text = $"{rate:0.0000}%";
        }
    }

    private void OutputSide()
    {
        if (CurrentUIType is UIType.Legacy)
        {
            objectCount.text = string.Format(
                "TAP: {0} / {5}\n" +
                "HOD: {1} / {6}\n" +
                "SLD: {2} / {7}\n" +
                "TOH: {3} / {8}\n" +
                "BRK: {4} / {9}\n" +
                "ALL: {10} / {11}\n" +
                "MOD: {12}",
                TapFinishedCount, HoldFinishedCount, SlideFinishedCount, TouchFinishedCount, BreakFinishedCount,
                TapSum, HoldSum, SlideSum, TouchSum, BreakSum,
                NoteFinishedCount, NoteSum,
                Majdata<InputManager>.Instance!.Mode
            );

            objectRate.text = string.Format(
                "FiNALE  Rate:\n" +
                $"{ClassicRateFromCount:000.00}   %\n" +
                "DELUXE Rate:\n" +
                $"{DeluxeRateFromCount:000.0000} % "
            );

            judgeResultCount.text = $"{cPerfectCount}\n" +
                                    $"{perfectCount}\n" +
                                    $"{greatCount}\n" +
                                    $"{goodCount}\n" +
                                    $"{missCount}\n\n" +
                                    $"{fastCount}\n" +
                                    $"{lateCount}";
        }
        else
        {
            objNoteCount.text =
                $"{TapFinishedCount} / {TapSum}\n" +
                $"{HoldFinishedCount} / {HoldSum}\n" +
                $"{SlideFinishedCount} / {SlideSum}\n" +
                $"{TouchFinishedCount} / {TouchSum}\n" +
                $"{BreakFinishedCount} / {BreakSum}\n" +
                $"{NoteFinishedCount} / {NoteSum}";

            var rate = DeluxeRateFromCount;
            var intPart = (int)rate;
            var fracPart = (rate - intPart) * 10000;
            objRate.text =
                $"<size=7.5>{intPart:0}</size><size=5.7>.{fracPart:0000}</size> <size=3.7>%</size>";

            objJudgeResult.text =
                $"{cPerfectCount}\n{perfectCount}\n{greatCount}\n{goodCount}\n{missCount}";

            objCombo.text = combo.ToString();

            var time = Majdata<TimeProvider>.Instance!.NoteTime;
            for (var i = meterList.Count - 1; i >= 0; i--)
            {
                var meter = meterList.ElementAt(i);
                if (meter.Key > time) continue;

                var (num, deno) = meter.Value;
                objMeter.text = $"{num}\n{deno}";
                break;
            }
            for (var i = bpmList.Count - 1; i >= 0; i--)
            {
                var bpm = bpmList.ElementAt(i);
                if (bpm.Key > time) continue;

                objBpm.text = bpm.Value.ToString();
                break;
            }
        }
    }
    private void OutputTime()
    {
        // Lock AudioTime variable for real
        var ctime = Majdata<TimeProvider>.Instance!.AudioTime;
        var timeNowInt = (int)ctime;
        var minute = timeNowInt / 60;
        var second = timeNowInt - 60 * minute;
        double milli = (ctime - timeNowInt) * 10000;

        // Make timing display "cleaner" on negative timing.
        string target;
        if (ctime < 0)
        {
            minute = Math.Abs(minute);
            second = Math.Abs(second);
            milli = Math.Abs(milli);
            target = string.Format("-{0}:{1:00}.{2:000}", minute, second, milli / 10);
        }
        else
        {
            target = string.Format("{0}:{1:00}.{2:0000}", minute, second, milli);
        }

        if (CurrentUIType == UIType.Legacy)
            timeDisplay.text = target;
        else
            objTime.text = target;
    }

    public void ResetState()
    {
        for (var i = 0; i < 14; i++)
        {
            judgedTapCount[(JudgeType)i] = 0;
            judgedHoldCount[(JudgeType)i] = 0;
            judgedTouchCount[(JudgeType)i] = 0;
            judgedTouchHoldCount[(JudgeType)i] = 0;
            judgedSlideCount[(JudgeType)i] = 0;
            judgedBreakCount[(JudgeType)i] = 0;
            totalJudgedCount[(JudgeType)i] = 0;
        }

        accRate[0] = 0.00;
        accRate[1] = 100.00;
        accRate[2] = 101.0000;
        accRate[3] = 100.0000;
        accRate[4] = 0.0000;

        TapFinishedCount = 0;
        HoldFinishedCount = 0;
        SlideFinishedCount = 0;
        TouchFinishedCount = 0;
        BreakFinishedCount = 0;

        TapSum = 0;
        HoldSum = 0;
        SlideSum = 0;
        TouchSum = 0;
        BreakSum = 0;

        TotalNoteBaseScore = 0;
        TotalNoteExtraScore = 0;
        CurrentNoteBaseScore = 0;
        CurrentNoteExtraScore = 0;
        CurrentNoteExtraScoreClassic = 0;
        LostNoteBaseScore = 0;
        LostNoteExtraScore = 0;
        LostNoteExtraScoreClassic = 0;

        cPerfectCount = 0;
        perfectCount = 0;
        greatCount = 0;
        goodCount = 0;
        missCount = 0;

        fastCount = 0;
        lateCount = 0;

        totalDXScore = 0;
        lostDXScore = 0;

        combo = 0;

        meterList.Clear();
        bpmList.Clear();

        statusAchievement.gameObject.SetActive(false);
        statusCombo.gameObject.SetActive(false);
        statusDXScore.gameObject.SetActive(false);
    }
}