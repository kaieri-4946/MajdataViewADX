using System;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;

public class SkinManager : MonoBehaviour
{
    public Sprite Tap;
    public Sprite Tap_Each;
    public Sprite Tap_Break;
    public Sprite Tap_Ex;
    public Sprite Tap_Mine;

    public Sprite Slide;
    public Sprite Slide_Each;
    public Sprite Slide_Break;
    public Sprite Slide_Mine;
    public Sprite[] Wifi = new Sprite[11];
    public Sprite[] Wifi_Each = new Sprite[11];
    public Sprite[] Wifi_Break = new Sprite[11];
    public Sprite[] Wifi_Mine = new Sprite[11];

    public Sprite Star;
    public Sprite Star_Double;
    public Sprite Star_Each;
    public Sprite Star_Each_Double;
    public Sprite Star_Break;
    public Sprite Star_Break_Double;
    public Sprite Star_Mine;
    public Sprite Star_Mine_Double;
    public Sprite Star_Ex;
    public Sprite Star_Ex_Double;

    public Sprite Hold;
    public Sprite Hold_On;
    public Sprite Hold_Off;
    public Sprite Hold_Each;
    public Sprite Hold_Each_On;
    public Sprite Hold_Break;
    public Sprite Hold_Break_On;
    public Sprite Hold_Mine;
    //public Sprite Hold_Mine_On; //no need
    public Sprite Hold_Ex;

    public Sprite[] Just = new Sprite[36];
    public Sprite[] JudgeText = new Sprite[5];
    public Sprite JudgeText_Break;
    public Sprite FastText;
    public Sprite LateText;

    public Sprite Touch;
    public Sprite Touch_Each;
    public Sprite Touch_Break;
    public Sprite Touch_Mine;
    public Sprite TouchPoint;
    public Sprite TouchPoint_Each;
    public Sprite TouchPoint_Break;
    public Sprite TouchPoint_Mine;
    public Sprite TouchJust;
    public Sprite[] TouchBorder = new Sprite[2];
    public Sprite[] TouchBorder_Each = new Sprite[2];
    public Sprite[] TouchBorder_Break = new Sprite[2];
    public Sprite[] TouchBorder_Mine = new Sprite[2];

    public Sprite[] TouchHold = new Sprite[4];
    public Sprite[] TouchHold_Break = new Sprite[4];
    public Sprite[] TouchHold_Mine = new Sprite[4];
    public Sprite TouchHold_Border;
    public Sprite TouchHold_Border_Break;
    public Sprite TouchHold_Border_Mine;
    public Sprite TouchHold_Border_Miss;

    public Color Ex;
    public Color Ex_Star;
    public Color Ex_Each;
    public Color Ex_Break;
    
    [SerializeField]
    public Sprite Line;
    [SerializeField]
    public Sprite Line_Each;
    [SerializeField]
    public Sprite Line_Mine;
    [SerializeField]
    public Sprite Line_Break;
    [SerializeField]
    public Sprite Line_Star;
    
    [SerializeField]
    public Sprite HoldEnd;
    [SerializeField]
    public Sprite HoldEnd_Each;
    [SerializeField]
    public Sprite HoldEnd_Break;
    
    [SerializeField]
    public RuntimeAnimatorController Shine;
    [SerializeField]
    public RuntimeAnimatorController Shine_Break;
    [SerializeField]
    public RuntimeAnimatorController Shine_JudgeBreak;
    [SerializeField]
    public Material BreakMaterial;

    
    private SpriteRenderer Outline;

    private void Awake()
    {
        Majdata<SkinManager>.Instance = this;
        
        var path = Path.Combine(new DirectoryInfo(Application.dataPath).Parent!.FullName, "Skin");
        Outline = gameObject.GetComponent<SpriteRenderer>();

        Outline.sprite = SpriteLoader.Load(path + "/outline.png");

        Tap = SpriteLoader.Load(path + "/tap.png");
        Tap_Each = SpriteLoader.Load(path + "/tap_each.png");
        Tap_Break = SpriteLoader.Load(path + "/tap_break.png");
        Tap_Ex = SpriteLoader.Load(path + "/tap_ex.png");
        Tap_Mine = SpriteLoader.Load(path + "/tap_mine.png");

        Slide = SpriteLoader.Load(path + "/slide.png");
        Slide_Each = SpriteLoader.Load(path + "/slide_each.png");
        Slide_Break = SpriteLoader.Load(path + "/slide_break.png");
        Slide_Mine = SpriteLoader.Load(path + "/slide_mine.png");
        for (var i = 0; i < 11; i++)
        {
            Wifi[i] = SpriteLoader.Load(path + "/wifi_" + i + ".png");
            Wifi_Each[i] = SpriteLoader.Load(path + "/wifi_each_" + i + ".png");
            Wifi_Break[i] = SpriteLoader.Load(path + "/wifi_break_" + i + ".png");
            Wifi_Mine[i] = SpriteLoader.Load(path + "/wifi_mine_" + i + ".png");
        }

        Star = SpriteLoader.Load(path + "/star.png");
        Star_Double = SpriteLoader.Load(path + "/star_double.png");
        Star_Each = SpriteLoader.Load(path + "/star_each.png");
        Star_Each_Double = SpriteLoader.Load(path + "/star_each_double.png");
        Star_Break = SpriteLoader.Load(path + "/star_break.png");
        Star_Break_Double = SpriteLoader.Load(path + "/star_break_double.png");
        Star_Ex = SpriteLoader.Load(path + "/star_ex.png");
        Star_Ex_Double = SpriteLoader.Load(path + "/star_ex_double.png");
        Star_Mine = SpriteLoader.Load(path + "/star_mine.png");
        Star_Mine_Double = SpriteLoader.Load(path + "/star_mine_double.png");

        var border = new Vector4(0, 58, 0, 58);
        Hold = SpriteLoader.Load(path + "/hold.png", border);        
        Hold_Each = SpriteLoader.Load(path + "/hold_each.png", border);
        Hold_Each_On = SpriteLoader.Load(path + "/hold_each_on.png", border);
        Hold_Break = SpriteLoader.Load(path + "/hold_break.png", border);
        Hold_Break_On = SpriteLoader.Load(path + "/hold_break_on.png", border);
        Hold_Mine = SpriteLoader.Load(path + "/hold_mine.png", border);
        Hold_Ex = SpriteLoader.Load(path + "/hold_ex.png", border);

        if (File.Exists(Path.Combine(path, "hold_on.png")))
            Hold_On = SpriteLoader.Load(path + "/hold_on.png", border);
        else
            Hold_On = Hold;
        Hold_Off = SpriteLoader.Load(path + "/hold_off.png", border);
        if (File.Exists(Path.Combine(path, "hold_each_on.png")))
            Hold_Each_On = SpriteLoader.Load(path + "/hold_each_on.png", border);
        else
            Hold_Each_On = Hold_Each;

        if (File.Exists(Path.Combine(path, "hold_break_on.png")))
            Hold_Break_On = SpriteLoader.Load(path + "/hold_break_on.png", border);
        else
            Hold_Break_On = Hold_Break;

        Just[0] = SpriteLoader.Load(path + "/just_curv_r.png");
        Just[1] = SpriteLoader.Load(path + "/just_str_r.png");
        Just[2] = SpriteLoader.Load(path + "/just_wifi_u.png");
        Just[3] = SpriteLoader.Load(path + "/just_curv_l.png");
        Just[4] = SpriteLoader.Load(path + "/just_str_l.png");
        Just[5] = SpriteLoader.Load(path + "/just_wifi_d.png");

        Just[6] = SpriteLoader.Load(path + "/just_curv_r_fast_gr.png");
        Just[7] = SpriteLoader.Load(path + "/just_str_r_fast_gr.png");
        Just[8] = SpriteLoader.Load(path + "/just_wifi_u_fast_gr.png");
        Just[9] = SpriteLoader.Load(path + "/just_curv_l_fast_gr.png");
        Just[10] = SpriteLoader.Load(path + "/just_str_l_fast_gr.png");
        Just[11] = SpriteLoader.Load(path + "/just_wifi_d_fast_gr.png");

        Just[12] = SpriteLoader.Load(path + "/just_curv_r_fast_gd.png");
        Just[13] = SpriteLoader.Load(path + "/just_str_r_fast_gd.png");
        Just[14] = SpriteLoader.Load(path + "/just_wifi_u_fast_gd.png");
        Just[15] = SpriteLoader.Load(path + "/just_curv_l_fast_gd.png");
        Just[16] = SpriteLoader.Load(path + "/just_str_l_fast_gd.png");
        Just[17] = SpriteLoader.Load(path + "/just_wifi_d_fast_gd.png");

        Just[18] = SpriteLoader.Load(path + "/just_curv_r_late_gr.png");
        Just[19] = SpriteLoader.Load(path + "/just_str_r_late_gr.png");
        Just[20] = SpriteLoader.Load(path + "/just_wifi_u_late_gr.png");
        Just[21] = SpriteLoader.Load(path + "/just_curv_l_late_gr.png");
        Just[22] = SpriteLoader.Load(path + "/just_str_l_late_gr.png");
        Just[23] = SpriteLoader.Load(path + "/just_wifi_d_late_gr.png");

        Just[24] = SpriteLoader.Load(path + "/just_curv_r_late_gd.png");
        Just[25] = SpriteLoader.Load(path + "/just_str_r_late_gd.png");
        Just[26] = SpriteLoader.Load(path + "/just_wifi_u_late_gd.png");
        Just[27] = SpriteLoader.Load(path + "/just_curv_l_late_gd.png");
        Just[28] = SpriteLoader.Load(path + "/just_str_l_late_gd.png");
        Just[29] = SpriteLoader.Load(path + "/just_wifi_d_late_gd.png");

        Just[30] = SpriteLoader.Load(path + "/miss_curv_r.png");
        Just[31] = SpriteLoader.Load(path + "/miss_str_r.png");
        Just[32] = SpriteLoader.Load(path + "/miss_wifi_u.png");
        Just[33] = SpriteLoader.Load(path + "/miss_curv_l.png");
        Just[34] = SpriteLoader.Load(path + "/miss_str_l.png");
        Just[35] = SpriteLoader.Load(path + "/miss_wifi_d.png");

        JudgeText[0] = SpriteLoader.Load(path + "/judge_text_miss.png");
        JudgeText[1] = SpriteLoader.Load(path + "/judge_text_good.png");
        JudgeText[2] = SpriteLoader.Load(path + "/judge_text_great.png");
        JudgeText[3] = SpriteLoader.Load(path + "/judge_text_perfect.png");
        JudgeText[4] = SpriteLoader.Load(path + "/judge_text_cPerfect.png");
        JudgeText_Break = SpriteLoader.Load(path + "/judge_text_break.png");

        FastText = SpriteLoader.Load(path + "/fast.png");
        LateText = SpriteLoader.Load(path + "/late.png");

        Touch = SpriteLoader.Load(path + "/touch.png");
        Touch_Each = SpriteLoader.Load(path + "/touch_each.png");
        Touch_Break = SpriteLoader.Load(path + "/touch_break.png");
        Touch_Mine = SpriteLoader.Load(path + "/touch_mine.png");
        TouchPoint = SpriteLoader.Load(path + "/touch_point.png");
        TouchPoint_Each = SpriteLoader.Load(path + "/touch_point_each.png");
        TouchPoint_Break = SpriteLoader.Load(path + "/touch_point_break.png");
        TouchPoint_Mine = SpriteLoader.Load(path + "/touch_point_mine.png");

        TouchJust = SpriteLoader.Load(path + "/touch_just.png");

        TouchBorder[0] = SpriteLoader.Load(path + "/touch_border_2.png");
        TouchBorder[1] = SpriteLoader.Load(path + "/touch_border_3.png");
        TouchBorder_Each[0] = SpriteLoader.Load(path + "/touch_border_2_each.png");
        TouchBorder_Each[1] = SpriteLoader.Load(path + "/touch_border_3_each.png");
        TouchBorder_Break[0] = SpriteLoader.Load(path + "/touch_border_2_break.png");
        TouchBorder_Break[1] = SpriteLoader.Load(path + "/touch_border_3_break.png");
        TouchBorder_Mine[0] = SpriteLoader.Load(path + "/touch_border_2_mine.png");
        TouchBorder_Mine[1] = SpriteLoader.Load(path + "/touch_border_3_mine.png");

        for (var i = 0; i < 4; i++)
        {
            TouchHold[i] = SpriteLoader.Load(path + "/touchhold_" + i + ".png");
            TouchHold_Break[i] = SpriteLoader.Load(path + "/touchhold_" + i + "_break.png");
            TouchHold_Mine[i] = SpriteLoader.Load(path + "/touchhold_" + i + "_mine.png");
        }
        TouchHold_Border = SpriteLoader.Load(path + "/touchhold_border.png");
        TouchHold_Border_Break = SpriteLoader.Load(path + "/touchhold_border_break.png");
        TouchHold_Border_Mine = SpriteLoader.Load(path + "/touchhold_border_mine.png");
        TouchHold_Border_Miss = SpriteLoader.Load(path + "/touchhold_border_miss.png");

        Ex = new Color32(255, 172, 255, 255);
        Ex_Star = new Color32(172, 251, 255, 255);
        Ex_Each = new Color32(255, 254, 119, 255);
        Ex_Break = Ex_Each;
    }
}