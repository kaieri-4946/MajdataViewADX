using System;
using UnityEngine;
using UnityEngine.UIElements;
#nullable enable
public class EffectManager : MonoBehaviour
{
    public static bool showFL;
    public static bool showLevel;
    
    private readonly Animator[] judgeAnimators = new Animator[8];
    private readonly GameObject[] judgeEffects = new GameObject[8];
    private readonly Animator[] tapAnimators = new Animator[8];

    private readonly GameObject[] tapEffects = new GameObject[8];
    private readonly GameObject[] greatEffects = new GameObject[8];
    private readonly GameObject[] goodEffects = new GameObject[8];

    private readonly Animator[] fastLateAnims = new Animator[8];
    private readonly GameObject[] fastLateEffects = new GameObject[8];
    Sprite[] judgeText;

    private SkinManager skinManager;

    private void Awake()
    {
        Majdata<EffectManager>.Instance = this;
    }


    private void Start()
    {
        var tapEffectParent = transform.GetChild(0).gameObject;
        var greatEffectParent = transform.GetChild(1).gameObject;
        var goodEffectParent = transform.GetChild(2).gameObject;
        var judgeEffectParent = transform.GetChild(3).gameObject;
        var flParent = transform.GetChild(4).gameObject;

        for (var i = 0; i < 8; i++)
        {
            judgeEffects[i] = judgeEffectParent.transform.GetChild(i).gameObject;
            judgeAnimators[i] = judgeEffects[i].GetComponent<Animator>();

            fastLateEffects[i] = flParent.transform.GetChild(i).gameObject;
            fastLateAnims[i] = fastLateEffects[i].GetComponent<Animator>();

            goodEffects[i] = goodEffectParent.transform.GetChild(i).gameObject;
            greatEffects[i] = greatEffectParent.transform.GetChild(i).gameObject;
            tapEffects[i] = tapEffectParent.transform.GetChild(i).gameObject;
            
            tapAnimators[i] = tapEffects[i].GetComponent<Animator>();
            

            goodEffects[i].SetActive(false);
            greatEffects[i].SetActive(false);
            tapEffects[i].SetActive(false);
        }

        //Load Skin
        skinManager = Majdata<SkinManager>.Instance!;
        judgeText = skinManager.JudgeText;

        foreach (var judgeEffect in judgeEffects)
        {
            judgeEffect.transform.GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite =
                skinManager.JudgeText[0];
            judgeEffect.transform.GetChild(0).GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite =
                skinManager.JudgeText_Break;
        }
    }

    public void SetDisplayMode(JudgeDisplayMode mode)
    {
        switch (mode)
        {
            case JudgeDisplayMode.None:
                showFL = showLevel = false;
                break;
            case JudgeDisplayMode.FastLate:
                showFL = true;
                showLevel = false;
                break;
            case JudgeDisplayMode.Level:
                showFL = false;
                showLevel = true;
                break;
            case JudgeDisplayMode.Both:
            default:
                showFL = showLevel = true;
                break;
        }
    }
    public void PlayEffect(int position, bool isBreak, JudgeType judge = JudgeType.Perfect)
    {
        var pos = position - 1;

        ResetEffect(pos);

        switch (judge)
        {
            case JudgeType.LateGood:
            case JudgeType.FastGood:
                SetJudgeEffect(pos, judgeText[1]);

                if (isBreak)
                {
                    tapEffects[pos].SetActive(true);
                    tapAnimators[pos].speed = 0.9f;
                    tapAnimators[pos].SetTrigger("bGood");
                }
                else
                {
                    goodEffects[pos].SetActive(true);
                }
                break;
            case JudgeType.LateGreat:
            case JudgeType.LateGreat1:
            case JudgeType.LateGreat2:
            case JudgeType.FastGreat2:
            case JudgeType.FastGreat1:
            case JudgeType.FastGreat:
                SetJudgeEffect(pos, judgeText[2]);

                if (isBreak)
                {
                    tapEffects[pos].SetActive(true);
                    tapAnimators[pos].speed = 0.9f;
                    tapAnimators[pos].SetTrigger("bGreat");
                }
                else
                {
                    greatEffects[pos].SetActive(true);
                    greatEffects[pos].gameObject.GetComponent<Animator>().SetTrigger("great");
                }
                break;
            case JudgeType.LatePerfect2:
            case JudgeType.FastPerfect2:
            case JudgeType.LatePerfect1:
            case JudgeType.FastPerfect1:
                SetJudgeEffect(pos, judgeText[3]);

                tapEffects[pos].SetActive(true);
                if (isBreak)
                {
                    tapAnimators[pos].speed = 0.9f;
                    tapAnimators[pos].SetTrigger("break");
                }
                break;
            case JudgeType.Perfect:
                SetJudgeEffect(pos, judgeText[4]);

                tapEffects[pos].SetActive(true);
                if (isBreak)
                {
                    tapAnimators[pos].speed = 0.9f;
                    tapAnimators[pos].SetTrigger("break");
                }
                break;
            default:
                SetJudgeEffect(pos, judgeText[0]);
                break;
        }

        //play judge text anim
        if (showLevel)
        {
            if (isBreak && judge == JudgeType.Perfect)
                judgeAnimators[pos].SetTrigger("break");
            else
                judgeAnimators[pos].SetTrigger("perfect");
        }
    }

    private void SetJudgeEffect(int pos, Sprite sprite)
    {
        if (!showLevel) return;

        judgeEffects[pos].transform.GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
    }

    public void PlayFastLate(int position, JudgeType judge)
    {
        if (!showFL) return;

        var pos = position - 1;

        if (judge is JudgeType.Miss or JudgeType.Perfect)
        {
            fastLateEffects[pos].SetActive(false);
            return;
        }


        fastLateEffects[pos].SetActive(true);
        var isFast = (int)judge > 7;
        if(isFast)
             fastLateEffects[pos].transform.GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = skinManager.FastText;
        else
            fastLateEffects[pos].transform.GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = skinManager.LateText;
        fastLateAnims[pos].SetTrigger("perfect");
    }

    public void ResetEffect(int pos)
    {
        tapEffects[pos].SetActive(false);
        greatEffects[pos].SetActive(false);
        goodEffects[pos].SetActive(false);
    }

    public void ResetState()
    {
    }
}