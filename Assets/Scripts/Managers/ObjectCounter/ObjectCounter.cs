using MajSimai;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class ObjectCounter : MonoBehaviour
{
    [SerializeField]
    Color AchievementDudColor; // = new Color32(63, 127, 176, 255);
    [SerializeField]
    Color AchievementBronzeColor; // = new Color32(127, 48, 32, 255);
    [SerializeField]
    Color AchievementSilverColor; // = new Color32(160, 160, 160, 255);
    [SerializeField]
    Color AchievementGoldColor; // = new Color32(224, 191, 127, 255);

    public EditorComboIndicator TextMode { get; private set; }
    public UIType? CurrentUIType { get; private set; } = null;
    
    public bool AllFinished => 
        tapCount == tapSum && 
        holdCount == holdSum && 
        slideCount == slideSum && 
        touchCount == touchSum && 
        breakCount == breakSum;

    private int tapCount;
    private int holdCount;
    private int slideCount;
    private int touchCount;
    private int breakCount;

    private int tapSum;
    private int holdSum;
    private int slideSum;
    private int touchSum;
    private int breakSum;
    
    private int cPerfectCount = 0;
    private int perfectCount = 0;
    private int greatCount = 0;
    private int goodCount = 0;
    private int missCount = 0;
    private int combo = 0;

    private Dictionary<JudgeType, int> judgedTapCount;
    private Dictionary<JudgeType, int> judgedHoldCount;
    private Dictionary<JudgeType, int> judgedTouchCount;
    private Dictionary<JudgeType, int> judgedTouchHoldCount;
    private Dictionary<JudgeType, int> judgedSlideCount;
    private Dictionary<JudgeType, int> judgedBreakCount;
    private Dictionary<JudgeType, int> totalJudgedCount;

    private Dictionary<double, (int, int)> meterList = new();
    private Dictionary<double, float> bpmList = new();
    
    private double[] accRate = new double[5]
    {
        0.00,    // classic acc (+)
        100.00,  // classic acc (-)
        101.0000,// acc 101(-)
        100.0000,// acc 100(-)
        0.0000,  // acc (+)
    };
    
    
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
    private Text statusScore;
    [SerializeField]
    private Text statusDXScore;

    private void Awake()
    {
        Majdata<ObjectCounter>.Instance = this;
    }
    
    private void Start()
    {
        statusCombo.gameObject.SetActive(false);
        statusScore.gameObject.SetActive(false);
        statusAchievement.gameObject.SetActive(false);
        statusDXScore.gameObject.SetActive(false);
        
        SetJudgeLists();
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateMainOutput();
        UpdateTimeOutput();
        if (FiSumScore() == 0) return;
        UpdateSideOutput();
    }

    public void ComboSetActive(EditorComboIndicator newComboMode)
    {
        TextMode = newComboMode;
        var isActive = TextMode > 0;
        var isAccClassic = TextMode == EditorComboIndicator.AchievementClassic ||
                           TextMode == EditorComboIndicator.AchievementDownClassic;
        var isPtsClassic = TextMode == EditorComboIndicator.ScoreClassic;
        var isAccDeluxe = TextMode == EditorComboIndicator.AchievementDeluxe ||
                          TextMode == EditorComboIndicator.AchievementDownDeluxe;
        var isPtsDeluxe = TextMode == EditorComboIndicator.ScoreDeluxe;
        var isPtsNormDeluxe = TextMode == EditorComboIndicator.CScoreDedeluxe ||
                              TextMode == EditorComboIndicator.CScoreDownDedeluxe;
        var isDefault = !(
            isAccClassic || isPtsClassic ||
            isAccDeluxe || isPtsDeluxe ||

            // De-DXfied 
            isPtsNormDeluxe
        );

        statusCombo.gameObject.SetActive(isActive && isDefault);
        statusScore.gameObject.SetActive(isActive && (isPtsClassic || isPtsNormDeluxe));
        statusAchievement.gameObject.SetActive(isActive && (isAccClassic || isAccDeluxe));
        statusDXScore.gameObject.SetActive(isActive && isPtsDeluxe);
    }
    
    public void SetUIType(UIType type)
    {
        StartSideOutput();
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
    
    private void SetJudgeLists()
    {
        judgedTapCount = new Dictionary<JudgeType, int>()
        {
            {JudgeType.FastGood, 0 },
            {JudgeType.FastGreat2, 0 },
            {JudgeType.FastGreat1, 0 },
            {JudgeType.FastGreat, 0 },
            {JudgeType.FastPerfect2, 0 },
            {JudgeType.FastPerfect1, 0 },
            {JudgeType.Perfect, 0 },
            {JudgeType.LatePerfect1, 0 },
            {JudgeType.LatePerfect2, 0 },
            {JudgeType.LateGreat, 0 },
            {JudgeType.LateGreat1, 0 },
            {JudgeType.LateGreat2, 0 },
            {JudgeType.LateGood, 0 },
            {JudgeType.Miss, 0 },
        };;
        judgedHoldCount = new Dictionary<JudgeType, int>(judgedTapCount);
        judgedSlideCount = new Dictionary<JudgeType, int>(judgedTapCount);
        judgedBreakCount = new Dictionary<JudgeType, int>(judgedTapCount);
        judgedTouchCount = new Dictionary<JudgeType, int>(judgedTapCount);
        judgedTouchHoldCount = new Dictionary<JudgeType, int>(judgedTapCount);
        totalJudgedCount = new Dictionary<JudgeType, int>(judgedTapCount);
    }

    private void ResetJudgeLists()
    {
        foreach (var dict in new[]
                 {
                     judgedTapCount,
                     judgedHoldCount,
                     judgedSlideCount,
                     judgedBreakCount,
                     judgedTouchCount,
                     judgedTouchHoldCount,
                     totalJudgedCount
                 })
        {
            var keys = new List<JudgeType>(dict.Keys);
            foreach (var key in keys)
                dict[key] = 0;
        }
    }
    
    public void ResetState()
    {
        tapCount = 0;
        holdCount = 0;
        slideCount = 0;
        touchCount = 0;
        breakCount = 0;

        tapSum = 0;
        holdSum = 0;
        slideSum = 0;
        touchSum = 0;
        breakSum = 0;

        cPerfectCount = 0;
        perfectCount = 0;
        greatCount = 0;
        goodCount = 0;
        missCount = 0;
        
        combo = 0;

        ResetJudgeLists();
        
        meterList.Clear();
        bpmList.Clear();

        statusCombo.gameObject.SetActive(false);
        statusScore.gameObject.SetActive(false);
        statusAchievement.gameObject.SetActive(false);
        statusDXScore.gameObject.SetActive(false);
    }
}