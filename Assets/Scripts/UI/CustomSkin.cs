using System.IO;
using UnityEngine;

public class CustomSkin : MonoBehaviour
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

    public Sprite TouchStar;
    public Sprite TouchStar_Double;
    public Sprite TouchStar_Each;
    public Sprite TouchStar_Each_Double;
    public Sprite TouchStar_Break;
    public Sprite TouchStar_Break_Double;
    public Sprite TouchStar_Mine;
    public Sprite TouchStar_Mine_Double;

    public Sprite Hold;
    public Sprite Hold_On;
    public Sprite Hold_Off;
    public Sprite Hold_Each;
    public Sprite Hold_Each_On;
    public Sprite Hold_Break;
    public Sprite Hold_Break_On;
    public Sprite Hold_Star;
    public Sprite Hold_Star_On;
    public Sprite Hold_Mine;
    //public Sprite Hold_Mine_On; //no need
    public Sprite Hold_Ex;

    public Sprite[] Just = new Sprite[36];
    public Sprite[] JustPivot = new Sprite[36];
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

    public Sprite[] TouchHold = new Sprite[5];
    public Sprite[] TouchHold_Break = new Sprite[5];
    public Sprite[] TouchHold_Mine = new Sprite[5];

    public Texture2D test;
    private SpriteRenderer Outline;

    // Start is called before the first frame update
    private void Start()
    {
        var path = new DirectoryInfo(Application.dataPath).Parent.FullName + "/Skin";
        Outline = gameObject.GetComponent<SpriteRenderer>();
        print(path);

        Outline.sprite = SpriteLoader.LoadSpriteFromFile(path + "/outline.png");

        Tap = SpriteLoader.LoadSpriteFromFile(path + "/tap.png");
        Tap_Each = SpriteLoader.LoadSpriteFromFile(path + "/tap_each.png");
        Tap_Break = SpriteLoader.LoadSpriteFromFile(path + "/tap_break.png");
        Tap_Ex = SpriteLoader.LoadSpriteFromFile(path + "/tap_ex.png");
        Tap_Mine = SpriteLoader.LoadSpriteFromFile(path + "/tap_mine.png");

        Slide = SpriteLoader.LoadSpriteFromFile(path + "/slide.png");
        Slide_Each = SpriteLoader.LoadSpriteFromFile(path + "/slide_each.png");
        Slide_Break = SpriteLoader.LoadSpriteFromFile(path + "/slide_break.png");
        Slide_Mine = SpriteLoader.LoadSpriteFromFile(path + "/slide_mine.png");
        for (var i = 0; i < 11; i++)
        {
            Wifi[i] = SpriteLoader.LoadSpriteFromFile(path + "/wifi_" + i + ".png");
            Wifi_Each[i] = SpriteLoader.LoadSpriteFromFile(path + "/wifi_each_" + i + ".png");
            Wifi_Break[i] = SpriteLoader.LoadSpriteFromFile(path + "/wifi_break_" + i + ".png");
            Wifi_Mine[i] = SpriteLoader.LoadSpriteFromFile(path + "/wifi_mine_" + i + ".png");
        }

        Star = SpriteLoader.LoadSpriteFromFile(path + "/star.png");
        Star_Double = SpriteLoader.LoadSpriteFromFile(path + "/star_double.png");
        Star_Each = SpriteLoader.LoadSpriteFromFile(path + "/star_each.png");
        Star_Each_Double = SpriteLoader.LoadSpriteFromFile(path + "/star_each_double.png");
        Star_Break = SpriteLoader.LoadSpriteFromFile(path + "/star_break.png");
        Star_Break_Double = SpriteLoader.LoadSpriteFromFile(path + "/star_break_double.png");
        Star_Ex = SpriteLoader.LoadSpriteFromFile(path + "/star_ex.png");
        Star_Ex_Double = SpriteLoader.LoadSpriteFromFile(path + "/star_ex_double.png");
        Star_Mine = SpriteLoader.LoadSpriteFromFile(path + "/star_mine.png");
        Star_Mine_Double = SpriteLoader.LoadSpriteFromFile(path + "/star_mine_double.png");

        TouchStar = SpriteLoader.LoadSpriteFromFile(path + "/touch_star.png");
        TouchStar_Double = SpriteLoader.LoadSpriteFromFile(path + "/touch_star_double.png");
        TouchStar_Each = SpriteLoader.LoadSpriteFromFile(path + "/touch_star_each.png");
        TouchStar_Each_Double = SpriteLoader.LoadSpriteFromFile(path + "/touch_star_each_double.png");
        TouchStar_Break = SpriteLoader.LoadSpriteFromFile(path + "/touch_star_break.png");
        TouchStar_Break_Double = SpriteLoader.LoadSpriteFromFile(path + "/touch_star_break_double.png");
        TouchStar_Mine = SpriteLoader.LoadSpriteFromFile(path + "/touch_star_mine.png");
        TouchStar_Mine_Double = SpriteLoader.LoadSpriteFromFile(path + "/touch_star_mine_double.png");

        var border = new Vector4(0, 58, 0, 58);
        Hold = SpriteLoader.LoadSpriteFromFile(path + "/hold.png", border);
        Hold_Each = SpriteLoader.LoadSpriteFromFile(path + "/hold_each.png", border);
        Hold_Each_On = SpriteLoader.LoadSpriteFromFile(path + "/hold_each_on.png", border);
        Hold_Break = SpriteLoader.LoadSpriteFromFile(path + "/hold_break.png", border);
        Hold_Break_On = SpriteLoader.LoadSpriteFromFile(path + "/hold_break_on.png", border);
        Hold_Star = SpriteLoader.LoadSpriteFromFile(path + "/hold_star.png", border);
        Hold_Star_On = SpriteLoader.LoadSpriteFromFile(path + "/hold_star_on.png", border);
        Hold_Mine = SpriteLoader.LoadSpriteFromFile(path + "/hold_mine.png", border);
        Hold_Ex = SpriteLoader.LoadSpriteFromFile(path + "/hold_ex.png", border);

        if (File.Exists(Path.Combine(path, "hold_on.png")))
            Hold_On = SpriteLoader.LoadSpriteFromFile(path + "/hold_on.png", border);
        else
            Hold_On = Hold;
        Hold_Off = SpriteLoader.LoadSpriteFromFile(path + "/hold_off.png", border);
        if (File.Exists(Path.Combine(path, "hold_each_on.png")))
            Hold_Each_On = SpriteLoader.LoadSpriteFromFile(path + "/hold_each_on.png", border);
        else
            Hold_Each_On = Hold_Each;

        if (File.Exists(Path.Combine(path, "hold_break_on.png")))
            Hold_Break_On = SpriteLoader.LoadSpriteFromFile(path + "/hold_break_on.png", border);
        else
            Hold_Break_On = Hold_Break;

        Just[0] = SpriteLoader.LoadSpriteFromFile(path + "/just_curv_r.png");
        Just[1] = SpriteLoader.LoadSpriteFromFile(path + "/just_str_r.png");
        Just[2] = SpriteLoader.LoadSpriteFromFile(path + "/just_wifi_u.png");
        Just[3] = SpriteLoader.LoadSpriteFromFile(path + "/just_curv_l.png");
        Just[4] = SpriteLoader.LoadSpriteFromFile(path + "/just_str_l.png");
        Just[5] = SpriteLoader.LoadSpriteFromFile(path + "/just_wifi_d.png");

        Just[6] = SpriteLoader.LoadSpriteFromFile(path + "/just_curv_r_fast_gr.png");
        Just[7] = SpriteLoader.LoadSpriteFromFile(path + "/just_str_r_fast_gr.png");
        Just[8] = SpriteLoader.LoadSpriteFromFile(path + "/just_wifi_u_fast_gr.png");
        Just[9] = SpriteLoader.LoadSpriteFromFile(path + "/just_curv_l_fast_gr.png");
        Just[10] = SpriteLoader.LoadSpriteFromFile(path + "/just_str_l_fast_gr.png");
        Just[11] = SpriteLoader.LoadSpriteFromFile(path + "/just_wifi_d_fast_gr.png");

        Just[12] = SpriteLoader.LoadSpriteFromFile(path + "/just_curv_r_fast_gd.png");
        Just[13] = SpriteLoader.LoadSpriteFromFile(path + "/just_str_r_fast_gd.png");
        Just[14] = SpriteLoader.LoadSpriteFromFile(path + "/just_wifi_u_fast_gd.png");
        Just[15] = SpriteLoader.LoadSpriteFromFile(path + "/just_curv_l_fast_gd.png");
        Just[16] = SpriteLoader.LoadSpriteFromFile(path + "/just_str_l_fast_gd.png");
        Just[17] = SpriteLoader.LoadSpriteFromFile(path + "/just_wifi_d_fast_gd.png");

        Just[18] = SpriteLoader.LoadSpriteFromFile(path + "/just_curv_r_late_gr.png");
        Just[19] = SpriteLoader.LoadSpriteFromFile(path + "/just_str_r_late_gr.png");
        Just[20] = SpriteLoader.LoadSpriteFromFile(path + "/just_wifi_u_late_gr.png");
        Just[21] = SpriteLoader.LoadSpriteFromFile(path + "/just_curv_l_late_gr.png");
        Just[22] = SpriteLoader.LoadSpriteFromFile(path + "/just_str_l_late_gr.png");
        Just[23] = SpriteLoader.LoadSpriteFromFile(path + "/just_wifi_d_late_gr.png");

        Just[24] = SpriteLoader.LoadSpriteFromFile(path + "/just_curv_r_late_gd.png");
        Just[25] = SpriteLoader.LoadSpriteFromFile(path + "/just_str_r_late_gd.png");
        Just[26] = SpriteLoader.LoadSpriteFromFile(path + "/just_wifi_u_late_gd.png");
        Just[27] = SpriteLoader.LoadSpriteFromFile(path + "/just_curv_l_late_gd.png");
        Just[28] = SpriteLoader.LoadSpriteFromFile(path + "/just_str_l_late_gd.png");
        Just[29] = SpriteLoader.LoadSpriteFromFile(path + "/just_wifi_d_late_gd.png");

        Just[30] = SpriteLoader.LoadSpriteFromFile(path + "/miss_curv_r.png");
        Just[31] = SpriteLoader.LoadSpriteFromFile(path + "/miss_str_r.png");
        Just[32] = SpriteLoader.LoadSpriteFromFile(path + "/miss_wifi_u.png");
        Just[33] = SpriteLoader.LoadSpriteFromFile(path + "/miss_curv_l.png");
        Just[34] = SpriteLoader.LoadSpriteFromFile(path + "/miss_str_l.png");
        Just[35] = SpriteLoader.LoadSpriteFromFile(path + "/miss_wifi_d.png");

        #region JustPivot
        JustPivot[0] = SpriteLoader.LoadSpriteFromFile(path + "/just_curv_r.png");
        JustPivot[1] = PivotStraightRight(path + "/just_str_r.png");
        JustPivot[2] = SpriteLoader.LoadSpriteFromFile(path + "/just_wifi_u.png");
        JustPivot[3] = SpriteLoader.LoadSpriteFromFile(path + "/just_curv_l.png");
        JustPivot[4] = PivotStraightLeft(path + "/just_str_l.png");
        JustPivot[5] = SpriteLoader.LoadSpriteFromFile(path + "/just_wifi_d.png");
            
        JustPivot[6] = SpriteLoader.LoadSpriteFromFile(path + "/just_curv_r_fast_gr.png");
        JustPivot[7] = PivotStraightRight(path + "/just_str_r_fast_gr.png");
        JustPivot[8] = SpriteLoader.LoadSpriteFromFile(path + "/just_wifi_u_fast_gr.png");
        JustPivot[9] = SpriteLoader.LoadSpriteFromFile(path + "/just_curv_l_fast_gr.png");
        JustPivot[10] = PivotStraightLeft(path + "/just_str_l_fast_gr.png");
        JustPivot[11] = SpriteLoader.LoadSpriteFromFile(path + "/just_wifi_d_fast_gr.png");
            
        JustPivot[12] = SpriteLoader.LoadSpriteFromFile(path + "/just_curv_r_fast_gd.png");
        JustPivot[13] = PivotStraightRight(path + "/just_str_r_fast_gd.png");
        JustPivot[14] = SpriteLoader.LoadSpriteFromFile(path + "/just_wifi_u_fast_gd.png");
        JustPivot[15] = SpriteLoader.LoadSpriteFromFile(path + "/just_curv_l_fast_gd.png");
        JustPivot[16] = PivotStraightLeft(path + "/just_str_l_fast_gd.png");
        JustPivot[17] = SpriteLoader.LoadSpriteFromFile(path + "/just_wifi_d_fast_gd.png");
            
        JustPivot[18] = SpriteLoader.LoadSpriteFromFile(path + "/just_curv_r_late_gr.png");
        JustPivot[19] = PivotStraightRight(path + "/just_str_r_late_gr.png");
        JustPivot[20] = SpriteLoader.LoadSpriteFromFile(path + "/just_wifi_u_late_gr.png");
        JustPivot[21] = SpriteLoader.LoadSpriteFromFile(path + "/just_curv_l_late_gr.png");
        JustPivot[22] = PivotStraightLeft(path + "/just_str_l_late_gr.png");
        JustPivot[23] = SpriteLoader.LoadSpriteFromFile(path + "/just_wifi_d_late_gr.png");
            
        JustPivot[24] = SpriteLoader.LoadSpriteFromFile(path + "/just_curv_r_late_gd.png");
        JustPivot[25] = PivotStraightRight(path + "/just_str_r_late_gd.png");
        JustPivot[26] = SpriteLoader.LoadSpriteFromFile(path + "/just_wifi_u_late_gd.png");
        JustPivot[27] = SpriteLoader.LoadSpriteFromFile(path + "/just_curv_l_late_gd.png");
        JustPivot[28] = PivotStraightLeft(path + "/just_str_l_late_gd.png");
        JustPivot[29] = SpriteLoader.LoadSpriteFromFile(path + "/just_wifi_d_late_gd.png");
            
        JustPivot[30] = SpriteLoader.LoadSpriteFromFile(path + "/miss_curv_r.png");
        JustPivot[31] = PivotStraightRight(path + "/miss_str_r.png");
        JustPivot[32] = SpriteLoader.LoadSpriteFromFile(path + "/miss_wifi_u.png");
        JustPivot[33] = SpriteLoader.LoadSpriteFromFile(path + "/miss_curv_l.png");
        JustPivot[34] = PivotStraightLeft(path + "/miss_str_l.png");
        JustPivot[35] = SpriteLoader.LoadSpriteFromFile(path + "/miss_wifi_d.png");
        #endregion

        JudgeText[0] = SpriteLoader.LoadSpriteFromFile(path + "/judge_text_miss.png");
        JudgeText[1] = SpriteLoader.LoadSpriteFromFile(path + "/judge_text_good.png");
        JudgeText[2] = SpriteLoader.LoadSpriteFromFile(path + "/judge_text_great.png");
        JudgeText[3] = SpriteLoader.LoadSpriteFromFile(path + "/judge_text_perfect.png");
        JudgeText[4] = SpriteLoader.LoadSpriteFromFile(path + "/judge_text_cPerfect.png");
        JudgeText_Break = SpriteLoader.LoadSpriteFromFile(path + "/judge_text_break.png");

        FastText = SpriteLoader.LoadSpriteFromFile(path + "/fast.png");
        LateText = SpriteLoader.LoadSpriteFromFile(path + "/late.png");

        Touch = SpriteLoader.LoadSpriteFromFile(path + "/touch.png");
        Touch_Each = SpriteLoader.LoadSpriteFromFile(path + "/touch_each.png");
        Touch_Break = SpriteLoader.LoadSpriteFromFile(path + "/touch_break.png");
        Touch_Mine = SpriteLoader.LoadSpriteFromFile(path + "/touch_mine.png");
        TouchPoint = SpriteLoader.LoadSpriteFromFile(path + "/touch_point.png");
        TouchPoint_Each = SpriteLoader.LoadSpriteFromFile(path + "/touch_point_each.png");
        TouchPoint_Break = SpriteLoader.LoadSpriteFromFile(path + "/touch_point_break.png");
        TouchPoint_Mine = SpriteLoader.LoadSpriteFromFile(path + "/touch_point_mine.png");

        TouchJust = SpriteLoader.LoadSpriteFromFile(path + "/touch_just.png");

        TouchBorder[0] = SpriteLoader.LoadSpriteFromFile(path + "/touch_border_2.png");
        TouchBorder[1] = SpriteLoader.LoadSpriteFromFile(path + "/touch_border_3.png");
        TouchBorder_Each[0] = SpriteLoader.LoadSpriteFromFile(path + "/touch_border_2_each.png");
        TouchBorder_Each[1] = SpriteLoader.LoadSpriteFromFile(path + "/touch_border_3_each.png");
        TouchBorder_Break[0] = SpriteLoader.LoadSpriteFromFile(path + "/touch_border_2_break.png");
        TouchBorder_Break[1] = SpriteLoader.LoadSpriteFromFile(path + "/touch_border_3_break.png");
        TouchBorder_Mine[0] = SpriteLoader.LoadSpriteFromFile(path + "/touch_border_2_mine.png");
        TouchBorder_Mine[1] = SpriteLoader.LoadSpriteFromFile(path + "/touch_border_3_mine.png");

        for (var i = 0; i < 4; i++)
        {
            TouchHold[i] = SpriteLoader.LoadSpriteFromFile(path + "/touchhold_" + i + ".png");
            TouchHold_Break[i] = SpriteLoader.LoadSpriteFromFile(path + "/touchhold_" + i + "_break.png");
            TouchHold_Mine[i] = SpriteLoader.LoadSpriteFromFile(path + "/touchhold_" + i + "_mine.png");
        }
        TouchHold[4] = SpriteLoader.LoadSpriteFromFile(path + "/touchhold_border.png");
        TouchHold_Break[4] = SpriteLoader.LoadSpriteFromFile(path + "/touchhold_border_break.png");
        TouchHold_Mine[4] = SpriteLoader.LoadSpriteFromFile(path + "/touchhold_border_mine.png");

        Debug.Log(test);
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private Sprite PivotStraightLeft(string path)
    {
        var original = SpriteLoader.LoadSpriteFromFile(path);
        var rect = original.rect;
        Vector2 customPivot = new Vector2(0.125f, 0.38f);
        return Sprite.Create(original.texture, rect, customPivot);
    }
    private Sprite PivotStraightRight(string path)
    {
        var original = SpriteLoader.LoadSpriteFromFile(path);
        var rect = original.rect;
        Vector2 customPivot = new Vector2(0.875f, 0.62f);
        return Sprite.Create(original.texture, rect, customPivot);
    }
}