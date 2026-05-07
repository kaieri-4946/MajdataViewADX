using Assets.Scripts.Types;
using Assets.Scripts.Notes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Collections;
using System.Diagnostics;
using Assets.Scripts;
using MajSimai;
using System.Runtime.CompilerServices;

public class JsonDataLoader : MonoBehaviour
{
    public float noteSpeed = 7f;
    public float touchSpeed = 7.5f;
    public bool smoothSlideAnime = false;
    public Sprite starEach;
    public GameObject tapPrefab;
    public GameObject holdPrefab;
    public GameObject starPrefab;
    public GameObject touchHoldPrefab;
    public GameObject touchPrefab;
    public GameObject touchStarPrefab;
    public GameObject eachLine;
    public GameObject starLine;
    public GameObject mineLine;
    public GameObject notes;
    public GameObject star_slidePrefab;
    public GameObject[] slidePrefab;
    public Material breakMaterial;
    public RuntimeAnimatorController BreakShine;
    public RuntimeAnimatorController JudgeBreakShine;
    public RuntimeAnimatorController HoldShine;

    public NoteLoaderStatus State { get; private set; } = NoteLoaderStatus.Idle;
    NoteManager noteManager;
    Task<Majson> jsonLoaderTask = null;
    Majson loadedData = null;
    float ignoreOffset = 0;
    Coroutine noteParserTask = null;
    Dictionary<int, int> noteIndex = new();
    Dictionary<SensorType, int> touchIndex = new();

    public Text diffText;
    public Text levelText;
    public Text titleText;
    public Text artistText;
    public Text designText;
    public RawImage cardImage;
    public Color[] diffColors = new Color[7];
    private CustomSkin customSkin;

    private ObjectCounter ObjectCounter;

    private int slideLayer = -1;
    private int noteSortOrder = 0;

    private static readonly Dictionary<SimaiNoteType, int> NOTE_LAYER_COUNT = new Dictionary<SimaiNoteType, int>()
    {
        {SimaiNoteType.Tap, 2 },
        {SimaiNoteType.Hold, 3 },
        {SimaiNoteType.Slide, 2 },
        {SimaiNoteType.Touch, 7 },
        {SimaiNoteType.TouchHold, 6 },
    };
#if UNITY_EDITOR
    public static Dictionary<string, int> GetSlidePrefabMap() => SLIDE_PREFAB_MAP;
#endif
    private static readonly Dictionary<string, int> SLIDE_PREFAB_MAP = new Dictionary<string, int>()
    {
        {"line3", 0 },
        {"line4", 1 },
        {"line5", 2 },
        {"line6", 3 },
        {"line7", 4 },
        {"circle1", 5 },
        {"circle2", 6 },
        {"circle3", 7 },
        {"circle4", 8 },
        {"circle5", 9 },
        {"circle6", 10 },
        {"circle7", 11 },
        {"circle8", 12 },
        {"v1", 41 },
        {"v2", 13 },
        {"v3", 14 },
        {"v4", 15 },
        {"v6", 16 },
        {"v7", 17 },
        {"v8", 18 },
        {"ppqq1", 19 },
        {"ppqq2", 20 },
        {"ppqq3", 21 },
        {"ppqq4", 22 },
        {"ppqq5", 23 },
        {"ppqq6", 24 },
        {"ppqq7", 25 },
        {"ppqq8", 26 },
        {"pq1", 27 },
        {"pq2", 28 },
        {"pq3", 29 },
        {"pq4", 30 },
        {"pq5", 31 },
        {"pq6", 32 },
        {"pq7", 33 },
        {"pq8", 34 },
        {"s", 35 },
        {"wifi", 36 },
        {"L2", 37 },
        {"L3", 38 },
        {"L4", 39 },
        {"L5", 40 },
        {"1A_Line_1", 42},
        {"1A_Line_2", 43},
        {"1A_Line_3", 44},
        {"1A_Line_4", 45},
        {"1A_Line_5", 46},
        {"1A_Line_6", 47},
        {"1A_Line_7", 48},
        {"1A_Line_8", 49},
        {"1B_Line_1", 50},
        {"1B_Line_2", 51},
        {"1B_Line_3", 52},
        {"1B_Line_4", 53},
        {"1B_Line_5", 54},
        {"1B_Line_6", 55},
        {"1B_Line_7", 56},
        {"1B_Line_8", 57},
        {"1C_Line", 58},
        {"1D_Line_1", 59},
        {"1D_Line_2", 60},
        {"1D_Line_3", 61},
        {"1D_Line_4", 62},
        {"1D_Line_5", 63},
        {"1D_Line_6", 64},
        {"1D_Line_7", 65},
        {"1D_Line_8", 66},
        {"1E_Line_1", 67},
        {"1E_Line_2", 68},
        {"1E_Line_3", 69},
        {"1E_Line_4", 70},
        {"1E_Line_5", 71},
        {"1E_Line_6", 72},
        {"1E_Line_7", 73},
        {"1E_Line_8", 74},
        {"A1_Line_1", 75},
        {"A1_Line_2", 76},
        {"A1_Line_3", 77},
        {"A1_Line_4", 78},
        {"A1_Line_5", 79},
        {"A1_Line_6", 80},
        {"A1_Line_7", 81},
        {"A1_Line_8", 82},
        {"AA_Line_2", 83},
        {"AA_Line_3", 84},
        {"AA_Line_4", 85},
        {"AA_Line_5", 86},
        {"AA_Line_6", 87},
        {"AA_Line_7", 88},
        {"AA_Line_8", 89},
        {"AB_Line_1", 90},
        {"AB_Line_2", 91},
        {"AB_Line_3", 92},
        {"AB_Line_4", 93},
        {"AB_Line_5", 94},
        {"AB_Line_6", 95},
        {"AB_Line_7", 96},
        {"AB_Line_8", 97},
        {"AC_Line", 98},
        {"AD_Line_1", 99},
        {"AD_Line_2", 100},
        {"AD_Line_3", 101},
        {"AD_Line_4", 102},
        {"AD_Line_5", 103},
        {"AD_Line_6", 104},
        {"AD_Line_7", 105},
        {"AD_Line_8", 106},
        {"AE_Line_1", 107},
        {"AE_Line_2", 108},
        {"AE_Line_3", 109},
        {"AE_Line_4", 110},
        {"AE_Line_5", 111},
        {"AE_Line_6", 112},
        {"AE_Line_7", 113},
        {"AE_Line_8", 114},
        {"B1_Line_1", 115},
        {"B1_Line_2", 116},
        {"B1_Line_3", 117},
        {"B1_Line_4", 118},
        {"B1_Line_5", 119},
        {"B1_Line_6", 120},
        {"B1_Line_7", 121},
        {"B1_Line_8", 122},
        {"BA_Line_1", 123},
        {"BA_Line_2", 124},
        {"BA_Line_3", 125},
        {"BA_Line_4", 126},
        {"BA_Line_5", 127},
        {"BA_Line_6", 128},
        {"BA_Line_7", 129},
        {"BA_Line_8", 130},
        {"BB_Line_2", 131},
        {"BB_Line_3", 132},
        {"BB_Line_4", 133},
        {"BB_Line_5", 134},
        {"BB_Line_6", 135},
        {"BB_Line_7", 136},
        {"BB_Line_8", 137},
        {"BC_Line", 138},
        {"BD_Line_1", 139},
        {"BD_Line_2", 140},
        {"BD_Line_3", 141},
        {"BD_Line_4", 142},
        {"BD_Line_5", 143},
        {"BD_Line_6", 144},
        {"BD_Line_7", 145},
        {"BD_Line_8", 146},
        {"BE_Line_1", 147},
        {"BE_Line_2", 148},
        {"BE_Line_3", 149},
        {"BE_Line_4", 150},
        {"BE_Line_5", 151},
        {"BE_Line_6", 152},
        {"BE_Line_7", 153},
        {"BE_Line_8", 154},
        {"C1_Line", 155},
        {"CA_Line", 156},
        {"CB_Line", 157},
        {"CD_Line", 158},
        {"CE_Line", 159},
        {"D1_Line_1", 160},
        {"D1_Line_2", 161},
        {"D1_Line_3", 162},
        {"D1_Line_4", 163},
        {"D1_Line_5", 164},
        {"D1_Line_6", 165},
        {"D1_Line_7", 166},
        {"D1_Line_8", 167},
        {"DA_Line_1", 168},
        {"DA_Line_2", 169},
        {"DA_Line_3", 170},
        {"DA_Line_4", 171},
        {"DA_Line_5", 172},
        {"DA_Line_6", 173},
        {"DA_Line_7", 174},
        {"DA_Line_8", 175},
        {"DB_Line_1", 176},
        {"DB_Line_2", 177},
        {"DB_Line_3", 178},
        {"DB_Line_4", 179},
        {"DB_Line_5", 180},
        {"DB_Line_6", 181},
        {"DB_Line_7", 182},
        {"DB_Line_8", 183},
        {"DC_Line", 184},
        {"DD_Line_2", 185},
        {"DD_Line_3", 186},
        {"DD_Line_4", 187},
        {"DD_Line_5", 188},
        {"DD_Line_6", 189},
        {"DD_Line_7", 190},
        {"DD_Line_8", 191},
        {"DE_Line_1", 192},
        {"DE_Line_2", 193},
        {"DE_Line_3", 194},
        {"DE_Line_4", 195},
        {"DE_Line_5", 196},
        {"DE_Line_6", 197},
        {"DE_Line_7", 198},
        {"DE_Line_8", 199},
        {"E1_Line_1", 200},
        {"E1_Line_2", 201},
        {"E1_Line_3", 202},
        {"E1_Line_4", 203},
        {"E1_Line_5", 204},
        {"E1_Line_6", 205},
        {"E1_Line_7", 206},
        {"E1_Line_8", 207},
        {"EA_Line_1", 208},
        {"EA_Line_2", 209},
        {"EA_Line_3", 210},
        {"EA_Line_4", 211},
        {"EA_Line_5", 212},
        {"EA_Line_6", 213},
        {"EA_Line_7", 214},
        {"EA_Line_8", 215},
        {"EB_Line_1", 216},
        {"EB_Line_2", 217},
        {"EB_Line_3", 218},
        {"EB_Line_4", 219},
        {"EB_Line_5", 220},
        {"EB_Line_6", 221},
        {"EB_Line_7", 222},
        {"EB_Line_8", 223},
        {"EC_Line", 224},
        {"ED_Line_1", 225},
        {"ED_Line_2", 226},
        {"ED_Line_3", 227},
        {"ED_Line_4", 228},
        {"ED_Line_5", 229},
        {"ED_Line_6", 230},
        {"ED_Line_7", 231},
        {"ED_Line_8", 232},
        {"EE_Line_2", 233},
        {"EE_Line_3", 234},
        {"EE_Line_4", 235},
        {"EE_Line_5", 236},
        {"EE_Line_6", 237},
        {"EE_Line_7", 238},
        {"EE_Line_8", 239},
        {"1C_Circle_1", 240},
        {"1C_Circle_2", 241},
        {"1C_Circle_3", 242},
        {"1C_Circle_4", 243},
        {"1C_Circle_5", 244},
        {"1C_Circle_6", 245},
        {"1C_Circle_7", 246},
        {"1C_Circle_8", 247},
        {"AC_Circle_1", 248},
        {"AC_Circle_2", 249},
        {"AC_Circle_3", 250},
        {"AC_Circle_4", 251},
        {"AC_Circle_5", 252},
        {"AC_Circle_6", 253},
        {"AC_Circle_7", 254},
        {"AC_Circle_8", 255},
        {"BC_Circle_1", 256},
        {"BC_Circle_2", 257},
        {"BC_Circle_3", 258},
        {"BC_Circle_4", 259},
        {"BC_Circle_5", 260},
        {"BC_Circle_6", 261},
        {"BC_Circle_7", 262},
        {"BC_Circle_8", 263},
        {"DC_Circle_1", 264},
        {"DC_Circle_2", 265},
        {"DC_Circle_3", 266},
        {"DC_Circle_4", 267},
        {"DC_Circle_5", 268},
        {"DC_Circle_6", 269},
        {"DC_Circle_7", 270},
        {"DC_Circle_8", 271},
        {"EC_Circle_1", 272},
        {"EC_Circle_2", 273},
        {"EC_Circle_3", 274},
        {"EC_Circle_4", 275},
        {"EC_Circle_5", 276},
        {"EC_Circle_6", 277},
        {"EC_Circle_7", 278},
        {"EC_Circle_8", 279},
        {"C1_Circle_1", 280},
        {"C1_Circle_2", 281},
        {"C1_Circle_3", 282},
        {"C1_Circle_4", 283},
        {"C1_Circle_5", 284},
        {"C1_Circle_6", 285},
        {"C1_Circle_7", 286},
        {"C1_Circle_8", 287},
        {"CA_Circle_1", 288},
        {"CA_Circle_2", 289},
        {"CA_Circle_3", 290},
        {"CA_Circle_4", 291},
        {"CA_Circle_5", 292},
        {"CA_Circle_6", 293},
        {"CA_Circle_7", 294},
        {"CA_Circle_8", 295},
        {"CB_Circle_1", 296},
        {"CB_Circle_2", 297},
        {"CB_Circle_3", 298},
        {"CB_Circle_4", 299},
        {"CB_Circle_5", 300},
        {"CB_Circle_6", 301},
        {"CB_Circle_7", 302},
        {"CB_Circle_8", 303},
        {"CD_Circle_1", 304},
        {"CD_Circle_2", 305},
        {"CD_Circle_3", 306},
        {"CD_Circle_4", 307},
        {"CD_Circle_5", 308},
        {"CD_Circle_6", 309},
        {"CD_Circle_7", 310},
        {"CD_Circle_8", 311},
        {"CE_Circle_1", 312},
        {"CE_Circle_2", 313},
        {"CE_Circle_3", 314},
        {"CE_Circle_4", 315},
        {"CE_Circle_5", 316},
        {"CE_Circle_6", 317},
        {"CE_Circle_7", 318},
        {"CE_Circle_8", 319},
        {"1A_Circle_1", 320},
        {"1A_Circle_2", 321},
        {"1A_Circle_3", 322},
        {"1A_Circle_4", 323},
        {"1A_Circle_5", 324},
        {"1A_Circle_6", 325},
        {"1A_Circle_7", 326},
        {"1A_Circle_8", 327},
        {"1B_Circle_1", 328},
        {"1B_Circle_2", 329},
        {"1B_Circle_3", 330},
        {"1B_Circle_4", 331},
        {"1B_Circle_5", 332},
        {"1B_Circle_6", 333},
        {"1B_Circle_7", 334},
        {"1B_Circle_8", 335},
        {"1D_Circle_1", 336},
        {"1D_Circle_2", 337},
        {"1D_Circle_3", 338},
        {"1D_Circle_4", 339},
        {"1D_Circle_5", 340},
        {"1D_Circle_6", 341},
        {"1D_Circle_7", 342},
        {"1D_Circle_8", 343},
        {"1E_Circle_1", 344},
        {"1E_Circle_2", 345},
        {"1E_Circle_3", 346},
        {"1E_Circle_4", 347},
        {"1E_Circle_5", 348},
        {"1E_Circle_6", 349},
        {"1E_Circle_7", 350},
        {"1E_Circle_8", 351},
        {"A1_Circle_1", 352},
        {"A1_Circle_2", 353},
        {"A1_Circle_3", 354},
        {"A1_Circle_4", 355},
        {"A1_Circle_5", 356},
        {"A1_Circle_6", 357},
        {"A1_Circle_7", 358},
        {"A1_Circle_8", 359},
        {"AA_Circle_1", 360},
        {"AA_Circle_2", 361},
        {"AA_Circle_3", 362},
        {"AA_Circle_4", 363},
        {"AA_Circle_5", 364},
        {"AA_Circle_6", 365},
        {"AA_Circle_7", 366},
        {"AA_Circle_8", 367},
        {"AB_Circle_1", 368},
        {"AB_Circle_2", 369},
        {"AB_Circle_3", 370},
        {"AB_Circle_4", 371},
        {"AB_Circle_5", 372},
        {"AB_Circle_6", 373},
        {"AB_Circle_7", 374},
        {"AB_Circle_8", 375},
        {"AD_Circle_1", 376},
        {"AD_Circle_2", 377},
        {"AD_Circle_3", 378},
        {"AD_Circle_4", 379},
        {"AD_Circle_5", 380},
        {"AD_Circle_6", 381},
        {"AD_Circle_7", 382},
        {"AD_Circle_8", 383},
        {"AE_Circle_1", 384},
        {"AE_Circle_2", 385},
        {"AE_Circle_3", 386},
        {"AE_Circle_4", 387},
        {"AE_Circle_5", 388},
        {"AE_Circle_6", 389},
        {"AE_Circle_7", 390},
        {"AE_Circle_8", 391},
        {"B1_Circle_1", 392},
        {"B1_Circle_2", 393},
        {"B1_Circle_3", 394},
        {"B1_Circle_4", 395},
        {"B1_Circle_5", 396},
        {"B1_Circle_6", 397},
        {"B1_Circle_7", 398},
        {"B1_Circle_8", 399},
        {"BA_Circle_1", 400},
        {"BA_Circle_2", 401},
        {"BA_Circle_3", 402},
        {"BA_Circle_4", 403},
        {"BA_Circle_5", 404},
        {"BA_Circle_6", 405},
        {"BA_Circle_7", 406},
        {"BA_Circle_8", 407},
        {"BB_Circle_1", 408},
        {"BB_Circle_2", 409},
        {"BB_Circle_3", 410},
        {"BB_Circle_4", 411},
        {"BB_Circle_5", 412},
        {"BB_Circle_6", 413},
        {"BB_Circle_7", 414},
        {"BB_Circle_8", 415},
        {"BD_Circle_1", 416},
        {"BD_Circle_2", 417},
        {"BD_Circle_3", 418},
        {"BD_Circle_4", 419},
        {"BD_Circle_5", 420},
        {"BD_Circle_6", 421},
        {"BD_Circle_7", 422},
        {"BD_Circle_8", 423},
        {"BE_Circle_1", 424},
        {"BE_Circle_2", 425},
        {"BE_Circle_3", 426},
        {"BE_Circle_4", 427},
        {"BE_Circle_5", 428},
        {"BE_Circle_6", 429},
        {"BE_Circle_7", 430},
        {"BE_Circle_8", 431},
        {"D1_Circle_1", 432},
        {"D1_Circle_2", 433},
        {"D1_Circle_3", 434},
        {"D1_Circle_4", 435},
        {"D1_Circle_5", 436},
        {"D1_Circle_6", 437},
        {"D1_Circle_7", 438},
        {"D1_Circle_8", 439},
        {"DA_Circle_1", 440},
        {"DA_Circle_2", 441},
        {"DA_Circle_3", 442},
        {"DA_Circle_4", 443},
        {"DA_Circle_5", 444},
        {"DA_Circle_6", 445},
        {"DA_Circle_7", 446},
        {"DA_Circle_8", 447},
        {"DB_Circle_1", 448},
        {"DB_Circle_2", 449},
        {"DB_Circle_3", 450},
        {"DB_Circle_4", 451},
        {"DB_Circle_5", 452},
        {"DB_Circle_6", 453},
        {"DB_Circle_7", 454},
        {"DB_Circle_8", 455},
        {"DD_Circle_1", 456},
        {"DD_Circle_2", 457},
        {"DD_Circle_3", 458},
        {"DD_Circle_4", 459},
        {"DD_Circle_5", 460},
        {"DD_Circle_6", 461},
        {"DD_Circle_7", 462},
        {"DD_Circle_8", 463},
        {"DE_Circle_1", 464},
        {"DE_Circle_2", 465},
        {"DE_Circle_3", 466},
        {"DE_Circle_4", 467},
        {"DE_Circle_5", 468},
        {"DE_Circle_6", 469},
        {"DE_Circle_7", 470},
        {"DE_Circle_8", 471},
        {"E1_Circle_1", 472},
        {"E1_Circle_2", 473},
        {"E1_Circle_3", 474},
        {"E1_Circle_4", 475},
        {"E1_Circle_5", 476},
        {"E1_Circle_6", 477},
        {"E1_Circle_7", 478},
        {"E1_Circle_8", 479},
        {"EA_Circle_1", 480},
        {"EA_Circle_2", 481},
        {"EA_Circle_3", 482},
        {"EA_Circle_4", 483},
        {"EA_Circle_5", 484},
        {"EA_Circle_6", 485},
        {"EA_Circle_7", 486},
        {"EA_Circle_8", 487},
        {"EB_Circle_1", 488},
        {"EB_Circle_2", 489},
        {"EB_Circle_3", 490},
        {"EB_Circle_4", 491},
        {"EB_Circle_5", 492},
        {"EB_Circle_6", 493},
        {"EB_Circle_7", 494},
        {"EB_Circle_8", 495},
        {"ED_Circle_1", 496},
        {"ED_Circle_2", 497},
        {"ED_Circle_3", 498},
        {"ED_Circle_4", 499},
        {"ED_Circle_5", 500},
        {"ED_Circle_6", 501},
        {"ED_Circle_7", 502},
        {"ED_Circle_8", 503},
        {"EE_Circle_1", 504},
        {"EE_Circle_2", 505},
        {"EE_Circle_3", 506},
        {"EE_Circle_4", 507},
        {"EE_Circle_5", 508},
        {"EE_Circle_6", 509},
        {"EE_Circle_7", 510},
        {"EE_Circle_8", 511},
    };

    static readonly Dictionary<SensorType, SensorType[]> TOUCH_GROUPS = new()
    {
        { SensorType.A1, new SensorType[]{ SensorType.D1, SensorType.D2, SensorType.E1, SensorType.E2 } },
        { SensorType.A2, new SensorType[]{ SensorType.D2, SensorType.D3, SensorType.E2, SensorType.E3 } },
        { SensorType.A3, new SensorType[]{ SensorType.D3, SensorType.D4, SensorType.E3, SensorType.E4 } },
        { SensorType.A4, new SensorType[]{ SensorType.D4, SensorType.D5, SensorType.E4, SensorType.E5 } },
        { SensorType.A5, new SensorType[]{ SensorType.D5, SensorType.D6, SensorType.E5, SensorType.E6 } },
        { SensorType.A6, new SensorType[]{ SensorType.D6, SensorType.D7, SensorType.E6, SensorType.E7 } },
        { SensorType.A7, new SensorType[]{ SensorType.D7, SensorType.D8, SensorType.E7, SensorType.E8 } },
        { SensorType.A8, new SensorType[]{ SensorType.D8, SensorType.D1, SensorType.E8, SensorType.E1 } },

        { SensorType.D1, new SensorType[]{ SensorType.A1, SensorType.A8, SensorType.E1 } },
        { SensorType.D2, new SensorType[]{ SensorType.A2, SensorType.A1, SensorType.E2 } },
        { SensorType.D3, new SensorType[]{ SensorType.A3, SensorType.A2, SensorType.E3 } },
        { SensorType.D4, new SensorType[]{ SensorType.A4, SensorType.A3, SensorType.E4 } },
        { SensorType.D5, new SensorType[]{ SensorType.A5, SensorType.A4, SensorType.E5 } },
        { SensorType.D6, new SensorType[]{ SensorType.A6, SensorType.A5, SensorType.E6 } },
        { SensorType.D7, new SensorType[]{ SensorType.A7, SensorType.A6, SensorType.E7 } },
        { SensorType.D8, new SensorType[]{ SensorType.A8, SensorType.A7, SensorType.E8 } },

        { SensorType.E1, new SensorType[]{ SensorType.D1, SensorType.A1, SensorType.A8, SensorType.B1, SensorType.B8 } },
        { SensorType.E2, new SensorType[]{ SensorType.D2, SensorType.A2, SensorType.A1, SensorType.B2, SensorType.B1 } },
        { SensorType.E3, new SensorType[]{ SensorType.D3, SensorType.A3, SensorType.A2, SensorType.B3, SensorType.B2 } },
        { SensorType.E4, new SensorType[]{ SensorType.D4, SensorType.A4, SensorType.A3, SensorType.B4, SensorType.B3 } },
        { SensorType.E5, new SensorType[]{ SensorType.D5, SensorType.A5, SensorType.A4, SensorType.B5, SensorType.B4 } },
        { SensorType.E6, new SensorType[]{ SensorType.D6, SensorType.A6, SensorType.A5, SensorType.B6, SensorType.B5 } },
        { SensorType.E7, new SensorType[]{ SensorType.D7, SensorType.A7, SensorType.A6, SensorType.B7, SensorType.B6 } },
        { SensorType.E8, new SensorType[]{ SensorType.D8, SensorType.A8, SensorType.A7, SensorType.B8, SensorType.B7 } },

        { SensorType.B1, new SensorType[]{ SensorType.E1, SensorType.E2, SensorType.B8, SensorType.B2, SensorType.A1, SensorType.C } },
        { SensorType.B2, new SensorType[]{ SensorType.E2, SensorType.E3, SensorType.B1, SensorType.B3, SensorType.A2, SensorType.C } },
        { SensorType.B3, new SensorType[]{ SensorType.E3, SensorType.E4, SensorType.B2, SensorType.B4, SensorType.A3, SensorType.C } },
        { SensorType.B4, new SensorType[]{ SensorType.E4, SensorType.E5, SensorType.B3, SensorType.B5, SensorType.A4, SensorType.C } },
        { SensorType.B5, new SensorType[]{ SensorType.E5, SensorType.E6, SensorType.B4, SensorType.B6, SensorType.A5, SensorType.C } },
        { SensorType.B6, new SensorType[]{ SensorType.E6, SensorType.E7, SensorType.B5, SensorType.B7, SensorType.A6, SensorType.C } },
        { SensorType.B7, new SensorType[]{ SensorType.E7, SensorType.E8, SensorType.B6, SensorType.B8, SensorType.A7, SensorType.C } },
        { SensorType.B8, new SensorType[]{ SensorType.E8, SensorType.E1, SensorType.B7, SensorType.B1, SensorType.A8, SensorType.C } },

        { SensorType.C, new SensorType[]{ SensorType.B1, SensorType.B2, SensorType.B3, SensorType.B4, SensorType.B5, SensorType.B6, SensorType.B7, SensorType.B8} },
    };

    static Dictionary<string, float> SLIDE_AREA_CONST = new()
    {
        { "line3", 0.1919f},
        { "line4", 0.1793f},
        { "line5", 0.1629f},
        { "line6", 0.1793f},
        { "line7", 0.1919f},
        { "circle1", 0.7892f},
        { "circle2", 0.2326f},
        { "circle3", 0.1550f},
        { "circle4", 0.1163f},
        { "circle5", 0.0930f},
        { "circle6", 0.0775f},
        { "circle7", 0.0664f},
        { "circle8", 0.0490f},
        { "v1", 0.1629f},
        { "v2", 0.1629f},
        { "v3", 0.1629f},
        { "v4", 0.1629f},
        { "v5", 0.1629f},
        { "v6", 0.1629f},
        { "v7", 0.1629f},
        { "v8", 0.1629f},
        { "ppqq1", 0.1014f},
        { "ppqq2", 0.1204f},
        { "ppqq3", 0.1434f},
        { "ppqq4", 0.0697f},
        { "ppqq5", 0.0867f},
        { "ppqq6", 0.1026f},
        { "ppqq7", 0.1266f},
        { "ppqq8", 0.1413f},
        { "pq1", 0.1021f},
        { "pq2", 0.1144f},
        { "pq3", 0.1247f},
        { "pq4", 0.1436f},
        { "pq5", 0.1627f},
        { "pq6", 0.0752f},
        { "pq7", 0.0984f},
        { "pq8", 0.1126f},
        { "s", 0.1054f},
        { "wifi", 0.1829f},
        { "L2", 0.0948f},
        { "L3", 0.0711f},
        { "L4", 0.0948f},
        { "L5", 0.1186f},
        // TODO: Figure out what this is
        {"1A_Line_1", 0.1186f},
        {"1A_Line_2", 0.1186f},
        {"1A_Line_3", 0.1186f},
        {"1A_Line_4", 0.1186f},
        {"1A_Line_5", 0.1186f},
        {"1A_Line_6", 0.1186f},
        {"1A_Line_7", 0.1186f},
        {"1A_Line_8", 0.1186f},
        {"1B_Line_1", 0.1186f},
        {"1B_Line_2", 0.1186f},
        {"1B_Line_3", 0.1186f},
        {"1B_Line_4", 0.1186f},
        {"1B_Line_5", 0.1186f},
        {"1B_Line_6", 0.1186f},
        {"1B_Line_7", 0.1186f},
        {"1B_Line_8", 0.1186f},
        {"1C_Line", 0.1186f},
        {"1D_Line_1", 0.1186f},
        {"1D_Line_2", 0.1186f},
        {"1D_Line_3", 0.1186f},
        {"1D_Line_4", 0.1186f},
        {"1D_Line_5", 0.1186f},
        {"1D_Line_6", 0.1186f},
        {"1D_Line_7", 0.1186f},
        {"1D_Line_8", 0.1186f},
        {"1E_Line_1", 0.1186f},
        {"1E_Line_2", 0.1186f},
        {"1E_Line_3", 0.1186f},
        {"1E_Line_4", 0.1186f},
        {"1E_Line_5", 0.1186f},
        {"1E_Line_6", 0.1186f},
        {"1E_Line_7", 0.1186f},
        {"1E_Line_8", 0.1186f},
        {"A1_Line_1", 0.1186f},
        {"A1_Line_2", 0.1186f},
        {"A1_Line_3", 0.1186f},
        {"A1_Line_4", 0.1186f},
        {"A1_Line_5", 0.1186f},
        {"A1_Line_6", 0.1186f},
        {"A1_Line_7", 0.1186f},
        {"A1_Line_8", 0.1186f},
        {"AA_Line_2", 0.1186f},
        {"AA_Line_3", 0.1186f},
        {"AA_Line_4", 0.1186f},
        {"AA_Line_5", 0.1186f},
        {"AA_Line_6", 0.1186f},
        {"AA_Line_7", 0.1186f},
        {"AA_Line_8", 0.1186f},
        {"AB_Line_1", 0.1186f},
        {"AB_Line_2", 0.1186f},
        {"AB_Line_3", 0.1186f},
        {"AB_Line_4", 0.1186f},
        {"AB_Line_5", 0.1186f},
        {"AB_Line_6", 0.1186f},
        {"AB_Line_7", 0.1186f},
        {"AB_Line_8", 0.1186f},
        {"AC_Line", 0.1186f},
        {"AD_Line_1", 0.1186f},
        {"AD_Line_2", 0.1186f},
        {"AD_Line_3", 0.1186f},
        {"AD_Line_4", 0.1186f},
        {"AD_Line_5", 0.1186f},
        {"AD_Line_6", 0.1186f},
        {"AD_Line_7", 0.1186f},
        {"AD_Line_8", 0.1186f},
        {"AE_Line_1", 0.1186f},
        {"AE_Line_2", 0.1186f},
        {"AE_Line_3", 0.1186f},
        {"AE_Line_4", 0.1186f},
        {"AE_Line_5", 0.1186f},
        {"AE_Line_6", 0.1186f},
        {"AE_Line_7", 0.1186f},
        {"AE_Line_8", 0.1186f},
        {"B1_Line_1", 0.1186f},
        {"B1_Line_2", 0.1186f},
        {"B1_Line_3", 0.1186f},
        {"B1_Line_4", 0.1186f},
        {"B1_Line_5", 0.1186f},
        {"B1_Line_6", 0.1186f},
        {"B1_Line_7", 0.1186f},
        {"B1_Line_8", 0.1186f},
        {"BA_Line_1", 0.1186f},
        {"BA_Line_2", 0.1186f},
        {"BA_Line_3", 0.1186f},
        {"BA_Line_4", 0.1186f},
        {"BA_Line_5", 0.1186f},
        {"BA_Line_6", 0.1186f},
        {"BA_Line_7", 0.1186f},
        {"BA_Line_8", 0.1186f},
        {"BB_Line_2", 0.1186f},
        {"BB_Line_3", 0.1186f},
        {"BB_Line_4", 0.1186f},
        {"BB_Line_5", 0.1186f},
        {"BB_Line_6", 0.1186f},
        {"BB_Line_7", 0.1186f},
        {"BB_Line_8", 0.1186f},
        {"BC_Line", 0.1186f},
        {"BD_Line_1", 0.1186f},
        {"BD_Line_2", 0.1186f},
        {"BD_Line_3", 0.1186f},
        {"BD_Line_4", 0.1186f},
        {"BD_Line_5", 0.1186f},
        {"BD_Line_6", 0.1186f},
        {"BD_Line_7", 0.1186f},
        {"BD_Line_8", 0.1186f},
        {"BE_Line_1", 0.1186f},
        {"BE_Line_2", 0.1186f},
        {"BE_Line_3", 0.1186f},
        {"BE_Line_4", 0.1186f},
        {"BE_Line_5", 0.1186f},
        {"BE_Line_6", 0.1186f},
        {"BE_Line_7", 0.1186f},
        {"BE_Line_8", 0.1186f},
        {"C1_Line", 0.1186f},
        {"CA_Line", 0.1186f},
        {"CB_Line", 0.1186f},
        {"CD_Line", 0.1186f},
        {"CE_Line", 0.1186f},
        {"D1_Line_1", 0.1186f},
        {"D1_Line_2", 0.1186f},
        {"D1_Line_3", 0.1186f},
        {"D1_Line_4", 0.1186f},
        {"D1_Line_5", 0.1186f},
        {"D1_Line_6", 0.1186f},
        {"D1_Line_7", 0.1186f},
        {"D1_Line_8", 0.1186f},
        {"DA_Line_1", 0.1186f},
        {"DA_Line_2", 0.1186f},
        {"DA_Line_3", 0.1186f},
        {"DA_Line_4", 0.1186f},
        {"DA_Line_5", 0.1186f},
        {"DA_Line_6", 0.1186f},
        {"DA_Line_7", 0.1186f},
        {"DA_Line_8", 0.1186f},
        {"DB_Line_1", 0.1186f},
        {"DB_Line_2", 0.1186f},
        {"DB_Line_3", 0.1186f},
        {"DB_Line_4", 0.1186f},
        {"DB_Line_5", 0.1186f},
        {"DB_Line_6", 0.1186f},
        {"DB_Line_7", 0.1186f},
        {"DB_Line_8", 0.1186f},
        {"DC_Line", 0.1186f},
        {"DD_Line_2", 0.1186f},
        {"DD_Line_3", 0.1186f},
        {"DD_Line_4", 0.1186f},
        {"DD_Line_5", 0.1186f},
        {"DD_Line_6", 0.1186f},
        {"DD_Line_7", 0.1186f},
        {"DD_Line_8", 0.1186f},
        {"DE_Line_1", 0.1186f},
        {"DE_Line_2", 0.1186f},
        {"DE_Line_3", 0.1186f},
        {"DE_Line_4", 0.1186f},
        {"DE_Line_5", 0.1186f},
        {"DE_Line_6", 0.1186f},
        {"DE_Line_7", 0.1186f},
        {"DE_Line_8", 0.1186f},
        {"E1_Line_1", 0.1186f},
        {"E1_Line_2", 0.1186f},
        {"E1_Line_3", 0.1186f},
        {"E1_Line_4", 0.1186f},
        {"E1_Line_5", 0.1186f},
        {"E1_Line_6", 0.1186f},
        {"E1_Line_7", 0.1186f},
        {"E1_Line_8", 0.1186f},
        {"EA_Line_1", 0.1186f},
        {"EA_Line_2", 0.1186f},
        {"EA_Line_3", 0.1186f},
        {"EA_Line_4", 0.1186f},
        {"EA_Line_5", 0.1186f},
        {"EA_Line_6", 0.1186f},
        {"EA_Line_7", 0.1186f},
        {"EA_Line_8", 0.1186f},
        {"EB_Line_1", 0.1186f},
        {"EB_Line_2", 0.1186f},
        {"EB_Line_3", 0.1186f},
        {"EB_Line_4", 0.1186f},
        {"EB_Line_5", 0.1186f},
        {"EB_Line_6", 0.1186f},
        {"EB_Line_7", 0.1186f},
        {"EB_Line_8", 0.1186f},
        {"EC_Line", 0.1186f},
        {"ED_Line_1", 0.1186f},
        {"ED_Line_2", 0.1186f},
        {"ED_Line_3", 0.1186f},
        {"ED_Line_4", 0.1186f},
        {"ED_Line_5", 0.1186f},
        {"ED_Line_6", 0.1186f},
        {"ED_Line_7", 0.1186f},
        {"ED_Line_8", 0.1186f},
        {"EE_Line_2", 0.1186f},
        {"EE_Line_3", 0.1186f},
        {"EE_Line_4", 0.1186f},
        {"EE_Line_5", 0.1186f},
        {"EE_Line_6", 0.1186f},
        {"EE_Line_7", 0.1186f},
        {"EE_Line_8", 0.1186f},
        {"1C_Circle_1", 0.1186f},
        {"1C_Circle_2", 0.1186f},
        {"1C_Circle_3", 0.1186f},
        {"1C_Circle_4", 0.1186f},
        {"1C_Circle_5", 0.1186f},
        {"1C_Circle_6", 0.1186f},
        {"1C_Circle_7", 0.1186f},
        {"1C_Circle_8", 0.1186f},
        {"AC_Circle_1", 0.1186f},
        {"AC_Circle_2", 0.1186f},
        {"AC_Circle_3", 0.1186f},
        {"AC_Circle_4", 0.1186f},
        {"AC_Circle_5", 0.1186f},
        {"AC_Circle_6", 0.1186f},
        {"AC_Circle_7", 0.1186f},
        {"AC_Circle_8", 0.1186f},
        {"BC_Circle_1", 0.1186f},
        {"BC_Circle_2", 0.1186f},
        {"BC_Circle_3", 0.1186f},
        {"BC_Circle_4", 0.1186f},
        {"BC_Circle_5", 0.1186f},
        {"BC_Circle_6", 0.1186f},
        {"BC_Circle_7", 0.1186f},
        {"BC_Circle_8", 0.1186f},
        {"DC_Circle_1", 0.1186f},
        {"DC_Circle_2", 0.1186f},
        {"DC_Circle_3", 0.1186f},
        {"DC_Circle_4", 0.1186f},
        {"DC_Circle_5", 0.1186f},
        {"DC_Circle_6", 0.1186f},
        {"DC_Circle_7", 0.1186f},
        {"DC_Circle_8", 0.1186f},
        {"EC_Circle_1", 0.1186f},
        {"EC_Circle_2", 0.1186f},
        {"EC_Circle_3", 0.1186f},
        {"EC_Circle_4", 0.1186f},
        {"EC_Circle_5", 0.1186f},
        {"EC_Circle_6", 0.1186f},
        {"EC_Circle_7", 0.1186f},
        {"EC_Circle_8", 0.1186f},
        {"C1_Circle_1", 0.1186f},
        {"C1_Circle_2", 0.1186f},
        {"C1_Circle_3", 0.1186f},
        {"C1_Circle_4", 0.1186f},
        {"C1_Circle_5", 0.1186f},
        {"C1_Circle_6", 0.1186f},
        {"C1_Circle_7", 0.1186f},
        {"C1_Circle_8", 0.1186f},
        {"CA_Circle_1", 0.1186f},
        {"CA_Circle_2", 0.1186f},
        {"CA_Circle_3", 0.1186f},
        {"CA_Circle_4", 0.1186f},
        {"CA_Circle_5", 0.1186f},
        {"CA_Circle_6", 0.1186f},
        {"CA_Circle_7", 0.1186f},
        {"CA_Circle_8", 0.1186f},
        {"CB_Circle_1", 0.1186f},
        {"CB_Circle_2", 0.1186f},
        {"CB_Circle_3", 0.1186f},
        {"CB_Circle_4", 0.1186f},
        {"CB_Circle_5", 0.1186f},
        {"CB_Circle_6", 0.1186f},
        {"CB_Circle_7", 0.1186f},
        {"CB_Circle_8", 0.1186f},
        {"CD_Circle_1", 0.1186f},
        {"CD_Circle_2", 0.1186f},
        {"CD_Circle_3", 0.1186f},
        {"CD_Circle_4", 0.1186f},
        {"CD_Circle_5", 0.1186f},
        {"CD_Circle_6", 0.1186f},
        {"CD_Circle_7", 0.1186f},
        {"CD_Circle_8", 0.1186f},
        {"CE_Circle_1", 0.1186f},
        {"CE_Circle_2", 0.1186f},
        {"CE_Circle_3", 0.1186f},
        {"CE_Circle_4", 0.1186f},
        {"CE_Circle_5", 0.1186f},
        {"CE_Circle_6", 0.1186f},
        {"CE_Circle_7", 0.1186f},
        {"CE_Circle_8", 0.1186f},
        {"1A_Circle_1", 0.1186f},
        {"1A_Circle_2", 0.1186f},
        {"1A_Circle_3", 0.1186f},
        {"1A_Circle_4", 0.1186f},
        {"1A_Circle_5", 0.1186f},
        {"1A_Circle_6", 0.1186f},
        {"1A_Circle_7", 0.1186f},
        {"1A_Circle_8", 0.1186f},
        {"1B_Circle_1", 0.1186f},
        {"1B_Circle_2", 0.1186f},
        {"1B_Circle_3", 0.1186f},
        {"1B_Circle_4", 0.1186f},
        {"1B_Circle_5", 0.1186f},
        {"1B_Circle_6", 0.1186f},
        {"1B_Circle_7", 0.1186f},
        {"1B_Circle_8", 0.1186f},
        {"1D_Circle_1", 0.1186f},
        {"1D_Circle_2", 0.1186f},
        {"1D_Circle_3", 0.1186f},
        {"1D_Circle_4", 0.1186f},
        {"1D_Circle_5", 0.1186f},
        {"1D_Circle_6", 0.1186f},
        {"1D_Circle_7", 0.1186f},
        {"1D_Circle_8", 0.1186f},
        {"1E_Circle_1", 0.1186f},
        {"1E_Circle_2", 0.1186f},
        {"1E_Circle_3", 0.1186f},
        {"1E_Circle_4", 0.1186f},
        {"1E_Circle_5", 0.1186f},
        {"1E_Circle_6", 0.1186f},
        {"1E_Circle_7", 0.1186f},
        {"1E_Circle_8", 0.1186f},
        {"A1_Circle_1", 0.1186f},
        {"A1_Circle_2", 0.1186f},
        {"A1_Circle_3", 0.1186f},
        {"A1_Circle_4", 0.1186f},
        {"A1_Circle_5", 0.1186f},
        {"A1_Circle_6", 0.1186f},
        {"A1_Circle_7", 0.1186f},
        {"A1_Circle_8", 0.1186f},
        {"AA_Circle_1", 0.1186f},
        {"AA_Circle_2", 0.1186f},
        {"AA_Circle_3", 0.1186f},
        {"AA_Circle_4", 0.1186f},
        {"AA_Circle_5", 0.1186f},
        {"AA_Circle_6", 0.1186f},
        {"AA_Circle_7", 0.1186f},
        {"AA_Circle_8", 0.1186f},
        {"AB_Circle_1", 0.1186f},
        {"AB_Circle_2", 0.1186f},
        {"AB_Circle_3", 0.1186f},
        {"AB_Circle_4", 0.1186f},
        {"AB_Circle_5", 0.1186f},
        {"AB_Circle_6", 0.1186f},
        {"AB_Circle_7", 0.1186f},
        {"AB_Circle_8", 0.1186f},
        {"AD_Circle_1", 0.1186f},
        {"AD_Circle_2", 0.1186f},
        {"AD_Circle_3", 0.1186f},
        {"AD_Circle_4", 0.1186f},
        {"AD_Circle_5", 0.1186f},
        {"AD_Circle_6", 0.1186f},
        {"AD_Circle_7", 0.1186f},
        {"AD_Circle_8", 0.1186f},
        {"AE_Circle_1", 0.1186f},
        {"AE_Circle_2", 0.1186f},
        {"AE_Circle_3", 0.1186f},
        {"AE_Circle_4", 0.1186f},
        {"AE_Circle_5", 0.1186f},
        {"AE_Circle_6", 0.1186f},
        {"AE_Circle_7", 0.1186f},
        {"AE_Circle_8", 0.1186f},
        {"B1_Circle_1", 0.1186f},
        {"B1_Circle_2", 0.1186f},
        {"B1_Circle_3", 0.1186f},
        {"B1_Circle_4", 0.1186f},
        {"B1_Circle_5", 0.1186f},
        {"B1_Circle_6", 0.1186f},
        {"B1_Circle_7", 0.1186f},
        {"B1_Circle_8", 0.1186f},
        {"BA_Circle_1", 0.1186f},
        {"BA_Circle_2", 0.1186f},
        {"BA_Circle_3", 0.1186f},
        {"BA_Circle_4", 0.1186f},
        {"BA_Circle_5", 0.1186f},
        {"BA_Circle_6", 0.1186f},
        {"BA_Circle_7", 0.1186f},
        {"BA_Circle_8", 0.1186f},
        {"BB_Circle_1", 0.1186f},
        {"BB_Circle_2", 0.1186f},
        {"BB_Circle_3", 0.1186f},
        {"BB_Circle_4", 0.1186f},
        {"BB_Circle_5", 0.1186f},
        {"BB_Circle_6", 0.1186f},
        {"BB_Circle_7", 0.1186f},
        {"BB_Circle_8", 0.1186f},
        {"BD_Circle_1", 0.1186f},
        {"BD_Circle_2", 0.1186f},
        {"BD_Circle_3", 0.1186f},
        {"BD_Circle_4", 0.1186f},
        {"BD_Circle_5", 0.1186f},
        {"BD_Circle_6", 0.1186f},
        {"BD_Circle_7", 0.1186f},
        {"BD_Circle_8", 0.1186f},
        {"BE_Circle_1", 0.1186f},
        {"BE_Circle_2", 0.1186f},
        {"BE_Circle_3", 0.1186f},
        {"BE_Circle_4", 0.1186f},
        {"BE_Circle_5", 0.1186f},
        {"BE_Circle_6", 0.1186f},
        {"BE_Circle_7", 0.1186f},
        {"BE_Circle_8", 0.1186f},
        {"D1_Circle_1", 0.1186f},
        {"D1_Circle_2", 0.1186f},
        {"D1_Circle_3", 0.1186f},
        {"D1_Circle_4", 0.1186f},
        {"D1_Circle_5", 0.1186f},
        {"D1_Circle_6", 0.1186f},
        {"D1_Circle_7", 0.1186f},
        {"D1_Circle_8", 0.1186f},
        {"DA_Circle_1", 0.1186f},
        {"DA_Circle_2", 0.1186f},
        {"DA_Circle_3", 0.1186f},
        {"DA_Circle_4", 0.1186f},
        {"DA_Circle_5", 0.1186f},
        {"DA_Circle_6", 0.1186f},
        {"DA_Circle_7", 0.1186f},
        {"DA_Circle_8", 0.1186f},
        {"DB_Circle_1", 0.1186f},
        {"DB_Circle_2", 0.1186f},
        {"DB_Circle_3", 0.1186f},
        {"DB_Circle_4", 0.1186f},
        {"DB_Circle_5", 0.1186f},
        {"DB_Circle_6", 0.1186f},
        {"DB_Circle_7", 0.1186f},
        {"DB_Circle_8", 0.1186f},
        {"DD_Circle_1", 0.1186f},
        {"DD_Circle_2", 0.1186f},
        {"DD_Circle_3", 0.1186f},
        {"DD_Circle_4", 0.1186f},
        {"DD_Circle_5", 0.1186f},
        {"DD_Circle_6", 0.1186f},
        {"DD_Circle_7", 0.1186f},
        {"DD_Circle_8", 0.1186f},
        {"DE_Circle_1", 0.1186f},
        {"DE_Circle_2", 0.1186f},
        {"DE_Circle_3", 0.1186f},
        {"DE_Circle_4", 0.1186f},
        {"DE_Circle_5", 0.1186f},
        {"DE_Circle_6", 0.1186f},
        {"DE_Circle_7", 0.1186f},
        {"DE_Circle_8", 0.1186f},
        {"E1_Circle_1", 0.1186f},
        {"E1_Circle_2", 0.1186f},
        {"E1_Circle_3", 0.1186f},
        {"E1_Circle_4", 0.1186f},
        {"E1_Circle_5", 0.1186f},
        {"E1_Circle_6", 0.1186f},
        {"E1_Circle_7", 0.1186f},
        {"E1_Circle_8", 0.1186f},
        {"EA_Circle_1", 0.1186f},
        {"EA_Circle_2", 0.1186f},
        {"EA_Circle_3", 0.1186f},
        {"EA_Circle_4", 0.1186f},
        {"EA_Circle_5", 0.1186f},
        {"EA_Circle_6", 0.1186f},
        {"EA_Circle_7", 0.1186f},
        {"EA_Circle_8", 0.1186f},
        {"EB_Circle_1", 0.1186f},
        {"EB_Circle_2", 0.1186f},
        {"EB_Circle_3", 0.1186f},
        {"EB_Circle_4", 0.1186f},
        {"EB_Circle_5", 0.1186f},
        {"EB_Circle_6", 0.1186f},
        {"EB_Circle_7", 0.1186f},
        {"EB_Circle_8", 0.1186f},
        {"ED_Circle_1", 0.1186f},
        {"ED_Circle_2", 0.1186f},
        {"ED_Circle_3", 0.1186f},
        {"ED_Circle_4", 0.1186f},
        {"ED_Circle_5", 0.1186f},
        {"ED_Circle_6", 0.1186f},
        {"ED_Circle_7", 0.1186f},
        {"ED_Circle_8", 0.1186f},
        {"EE_Circle_1", 0.1186f},
        {"EE_Circle_2", 0.1186f},
        {"EE_Circle_3", 0.1186f},
        {"EE_Circle_4", 0.1186f},
        {"EE_Circle_5", 0.1186f},
        {"EE_Circle_6", 0.1186f},
        {"EE_Circle_7", 0.1186f},
        {"EE_Circle_8", 0.1186f},
    };

    private static readonly Dictionary<string, List<int>> SLIDE_AREA_STEP_MAP = new Dictionary<string, List<int>>()
    {
        {"line3", new List<int>(){ 0, 2, 8, 13 } },
        {"line4", new List<int>(){ 0, 3, 8, 12, 18 } },
        {"line5", new List<int>(){ 0, 3, 6, 11, 15, 19 } },
        {"line6", new List<int>(){ 0, 3, 8, 12, 18 } },
        {"line7", new List<int>(){ 0, 2, 8, 13 } },
        {"circle1", new List<int>(){ 0, 3, 11, 19, 27, 35, 43, 50, 58, 63 } },
        {"circle2", new List<int>(){ 0, 3, 7 } },
        {"circle3", new List<int>(){ 0, 3, 11, 15 } },
        {"circle4", new List<int>(){ 0, 3, 11, 19, 23 } },
        {"circle5", new List<int>(){ 0, 3, 11, 19, 27, 31 } },
        {"circle6", new List<int>(){ 0, 3, 11, 19, 27, 35, 39 } },
        {"circle7", new List<int>(){ 0, 3, 11, 19, 27, 35, 43, 47 } },
        {"circle8", new List<int>(){ 0, 3, 11, 19, 27, 35, 43, 50, 55 } },
        {"v1", new List<int>(){ 0, 3, 6, 11, 15, 19 } },
        {"v2", new List<int>(){ 0, 3, 6, 11, 15, 19 } },
        {"v3", new List<int>(){ 0, 3, 6, 11, 15, 19 } },
        {"v4", new List<int>(){ 0, 3, 6, 11, 15, 19 } },
        {"v6", new List<int>(){ 0, 3, 6, 11, 15, 19 } },
        {"v7", new List<int>(){ 0, 3, 6, 11, 15, 19 } },
        {"v8", new List<int>(){ 0, 3, 6, 11, 15, 19 } },
        {"ppqq1", new List<int>(){ 0, 3, 7, 13, 17, 26, 32, 35 } },
        {"ppqq2", new List<int>(){ 0, 3, 7, 12, 16, 25, 28 } },
        {"ppqq3", new List<int>(){ 0, 3, 6, 12, 15, 22 } },
        {"ppqq4", new List<int>(){ 0, 3, 7, 12, 16, 25, 29, 35, 40, 44, 49 } },
        {"ppqq5", new List<int>(){ 0, 3, 7, 12, 16, 25, 29, 35, 40, 44, 49 } },
        {"ppqq6", new List<int>(){ 0, 3, 7, 12, 16, 25, 28, 34, 38, 41, 48 } },
        {"ppqq7", new List<int>(){ 0, 3, 7, 13, 17, 27, 31, 37, 41, 46 } },
        {"ppqq8", new List<int>(){ 0, 3, 7, 12, 16, 25, 29, 35, 41 } },
        {"pq1", new List<int>(){ 0, 3, 8, 11, 14, 17, 21, 24, 27, 33 } },
        {"pq2", new List<int>(){ 0, 3, 8, 11, 14, 18, 21, 24, 30 } },
        {"pq3", new List<int>(){ 0, 3, 9, 12, 16, 19, 23, 27 } },
        {"pq4", new List<int>(){ 0, 3, 9, 13, 16, 20, 24 } },
        {"pq5", new List<int>(){ 0, 3, 9, 13, 17, 21 } },
        {"pq6", new List<int>(){ 0, 3, 8, 11, 15, 18, 21, 25, 28, 31, 35, 38, 42 } },
        {"pq7", new List<int>(){ 0, 3, 8, 12, 15, 18, 22, 25, 28, 32, 35, 39 } },
        {"pq8", new List<int>(){ 0, 3, 8, 11, 14, 17, 21, 24, 27, 30, 36 } },
        {"s", new List<int>(){ 0, 3, 8, 11, 17, 21, 24, 30 } },
        {"wifi", new List<int>(){ 0, 1, 4, 6, 11 } },
        {"L2", new List<int>(){ 0, 2, 7, 15, 21, 26, 32 } },
        {"L3", new List<int>(){ 0, 2, 8, 17, 20, 26, 29, 34 } },
        {"L4", new List<int>(){ 0, 2, 8, 17, 22, 26, 32 } },
        {"L5", new List<int>(){ 0, 2, 8, 16, 22, 28 } },
        // Index of line in prefab on that is on top of the sensor, used when smooth slide aniation is off to turn off part of slides
        {"1A_Line_1",new List<int>() {0}},
        {"1A_Line_2",new List<int>() {0, 1, 5}},
        {"1A_Line_3",new List<int>() {0, 2, 4, 8, 11}},
        {"1A_Line_4",new List<int>() {0, 4, 6, 10, 16}},
        {"1A_Line_5",new List<int>() {0, 4, 7, 13, 17}},
        {"1A_Line_6",new List<int>() {0, 4, 6, 10, 16}},
        {"1A_Line_7",new List<int>() {0, 2, 4, 8, 11}},
        {"1A_Line_8",new List<int>() {0, 1, 5}},
        {"1B_Line_1",new List<int>() {0, 3}},
        {"1B_Line_2",new List<int>() {0, 3, 5}},
        {"1B_Line_3",new List<int>() {0, 3, 6, 9}},
        {"1B_Line_4",new List<int>() {0, 7, 9, 12}},
        {"1B_Line_5",new List<int>() {0, 4, 7, 13}},
        {"1B_Line_6",new List<int>() {0, 7, 9, 12}},
        {"1B_Line_7",new List<int>() {0, 3, 6, 9}},
        {"1B_Line_8",new List<int>() {0, 3, 5}},
        {"1C_Line",new List<int>() {0, 4, 7}},
        {"1D_Line_1",new List<int>() {0, 1}},
        {"1D_Line_2",new List<int>() {0, 1}},
        {"1D_Line_3",new List<int>() {0, 1, 5, 8}},
        {"1D_Line_4",new List<int>() {0, 3, 6, 10, 14}},
        {"1D_Line_5",new List<int>() {0, 4, 8, 12, 15, 17}},
        {"1D_Line_6",new List<int>() {0, 4, 8, 12, 15, 17}},
        {"1D_Line_7",new List<int>() {0, 3, 6, 10, 14}},
        {"1D_Line_8",new List<int>() {0, 1, 5, 8}},
        {"1E_Line_1",new List<int>() {0, 3}},
        {"1E_Line_2",new List<int>() {0, 3}},
        {"1E_Line_3",new List<int>() {0, 2, 3, 8}},
        {"1E_Line_4",new List<int>() {0, 3, 6, 9, 12}},
        {"1E_Line_5",new List<int>() {0, 4, 8, 13, 15}},
        {"1E_Line_6",new List<int>() {0, 4, 8, 13, 15}},
        {"1E_Line_7",new List<int>() {0, 3, 6, 9, 12}},
        {"1E_Line_8",new List<int>() {0, 2, 3, 8}},
        {"A1_Line_1",new List<int>() {0}},
        {"A1_Line_2",new List<int>() {0, 1, 4}},
        {"A1_Line_3",new List<int>() {0, 2, 7, 8, 10}},
        {"A1_Line_4",new List<int>() {0, 5, 8, 12, 14}},
        {"A1_Line_5",new List<int>() {0, 2, 6, 11, 15}},
        {"A1_Line_6",new List<int>() {0, 5, 8, 12, 14}},
        {"A1_Line_7",new List<int>() {0, 2, 7, 8, 10}},
        {"A1_Line_8",new List<int>() {0, 1, 4}},
        {"AA_Line_2",new List<int>() {0, 1, 4}},
        {"AA_Line_3",new List<int>() {0, 2, 7, 10}},
        {"AA_Line_4",new List<int>() {0, 5, 8, 14}},
        {"AA_Line_5",new List<int>() {0, 2, 6, 12, 16}},
        {"AA_Line_6",new List<int>() {0, 5, 8, 14}},
        {"AA_Line_7",new List<int>() {0, 2, 7, 10}},
        {"AA_Line_8",new List<int>() {0, 1, 4}},
        {"AB_Line_1",new List<int>() {0, 2}},
        {"AB_Line_2",new List<int>() {0, 2, 3}},
        {"AB_Line_3",new List<int>() {0, 2, 4, 8}},
        {"AB_Line_4",new List<int>() {0, 3, 7, 11}},
        {"AB_Line_5",new List<int>() {0, 2, 6, 12}},
        {"AB_Line_6",new List<int>() {0, 3, 7, 11}},
        {"AB_Line_7",new List<int>() {0, 2, 4, 8}},
        {"AB_Line_8",new List<int>() {0, 2, 3}},
        {"AC_Line",new List<int>() {0, 2, 5}},
        {"AD_Line_1",new List<int>() {0, 1}},
        {"AD_Line_2",new List<int>() {0, 1}},
        {"AD_Line_3",new List<int>() {0, 2, 4, 7}},
        {"AD_Line_4",new List<int>() {0, 2, 5, 9, 13}},
        {"AD_Line_5",new List<int>() {0, 3, 6, 11, 13, 15}},
        {"AD_Line_6",new List<int>() {0, 3, 6, 11, 13, 15}},
        {"AD_Line_7",new List<int>() {0, 2, 5, 9, 13}},
        {"AD_Line_8",new List<int>() {0, 2, 4, 7}},
        {"AE_Line_1",new List<int>() {0, 2}},
        {"AE_Line_2",new List<int>() {0, 2}},
        {"AE_Line_3",new List<int>() {0, 2, 7}},
        {"AE_Line_4",new List<int>() {0, 2, 4, 7, 11}},
        {"AE_Line_5",new List<int>() {0, 3, 6, 11, 13}},
        {"AE_Line_6",new List<int>() {0, 3, 6, 11, 13}},
        {"AE_Line_7",new List<int>() {0, 2, 4, 7, 11}},
        {"AE_Line_8",new List<int>() {0, 2, 7}},
        {"B1_Line_1",new List<int>() {0}},
        {"B1_Line_2",new List<int>() {0, 1, 4}},
        {"B1_Line_3",new List<int>() {0, 2, 5, 7}},
        {"B1_Line_4",new List<int>() {0, 3, 5, 10}},
        {"B1_Line_5",new List<int>() {0, 2, 7, 11}},
        {"B1_Line_6",new List<int>() {0, 3, 5, 10}},
        {"B1_Line_7",new List<int>() {0, 2, 5, 7}},
        {"B1_Line_8",new List<int>() {0, 1, 4}},
        {"BA_Line_1",new List<int>() {0, 2}},
        {"BA_Line_2",new List<int>() {0, 1, 3}},
        {"BA_Line_3",new List<int>() {0, 2, 5, 8}},
        {"BA_Line_4",new List<int>() {0, 3, 7, 11}},
        {"BA_Line_5",new List<int>() {0, 2, 8, 12}},
        {"BA_Line_6",new List<int>() {0, 3, 7, 11}},
        {"BA_Line_7",new List<int>() {0, 2, 5, 8}},
        {"BA_Line_8",new List<int>() {0, 1, 3}},
        {"BB_Line_2",new List<int>() {0, 2}},
        {"BB_Line_3",new List<int>() {0, 2, 4}},
        {"BB_Line_4",new List<int>() {0, 2, 6}},
        {"BB_Line_5",new List<int>() {0, 2, 7}},
        {"BB_Line_6",new List<int>() {0, 2, 6}},
        {"BB_Line_7",new List<int>() {0, 2, 4}},
        {"BB_Line_8",new List<int>() {0, 2}},
        {"BC_Line",new List<int>() {0, 1}},
        {"BD_Line_1",new List<int>() {0, 2}},
        {"BD_Line_2",new List<int>() {0, 2}},
        {"BD_Line_3",new List<int>() {0, 2, 6}},
        {"BD_Line_4",new List<int>() {0, 2, 4, 8, 9}},
        {"BD_Line_5",new List<int>() {0, 2, 7, 9, 11}},
        {"BD_Line_6",new List<int>() {0, 2, 7, 9, 11}},
        {"BD_Line_7",new List<int>() {0, 2, 4, 8, 9}},
        {"BD_Line_8",new List<int>() {0, 2, 6}},
        {"BE_Line_1",new List<int>() {0, 2}},
        {"BE_Line_2",new List<int>() {0, 2}},
        {"BE_Line_3",new List<int>() {0, 2, 5}},
        {"BE_Line_4",new List<int>() {0, 2, 5, 8}},
        {"BE_Line_5",new List<int>() {0, 2, 7, 9}},
        {"BE_Line_6",new List<int>() {0, 2, 7, 9}},
        {"BE_Line_7",new List<int>() {0, 2, 5, 8}},
        {"BE_Line_8",new List<int>() {0, 2, 5}},
        {"C1_Line",new List<int>() {0, 2, 6}},
        {"CA_Line",new List<int>() {0, 2, 6}},
        {"CB_Line",new List<int>() {0, 2}},
        {"CD_Line",new List<int>() {0, 4, 6}},
        {"CE_Line",new List<int>() {0, 5}},
        {"D1_Line_1",new List<int>() {0, 1}},
        {"D1_Line_2",new List<int>() {0, 1, 4, 7}},
        {"D1_Line_3",new List<int>() {0, 4, 7, 9, 12}},
        {"D1_Line_4",new List<int>() {0, 1, 3, 6, 12, 15}},
        {"D1_Line_5",new List<int>() {0, 1, 3, 6, 12, 15}},
        {"D1_Line_6",new List<int>() {0, 4, 7, 9, 12}},
        {"D1_Line_7",new List<int>() {0, 1, 4, 7}},
        {"D1_Line_8",new List<int>() {0, 1}},
        {"DA_Line_1",new List<int>() {0, 1}},
        {"DA_Line_2",new List<int>() {0, 1, 5, 7}},
        {"DA_Line_3",new List<int>() {0, 4, 7, 10, 13}},
        {"DA_Line_4",new List<int>() {0, 1, 3, 6, 11, 15}},
        {"DA_Line_5",new List<int>() {0, 1, 3, 6, 11, 15}},
        {"DA_Line_6",new List<int>() {0, 4, 7, 10, 13}},
        {"DA_Line_7",new List<int>() {0, 1, 5, 7}},
        {"DA_Line_8",new List<int>() {0, 1}},
        {"DB_Line_1",new List<int>() {0, 1, 3}},
        {"DB_Line_2",new List<int>() {0, 3, 6}},
        {"DB_Line_3",new List<int>() {0, 1, 3, 6, 9}},
        {"DB_Line_4",new List<int>() {0, 1, 3, 6, 11}},
        {"DB_Line_5",new List<int>() {0, 1, 3, 6, 11}},
        {"DB_Line_6",new List<int>() {0, 1, 3, 6, 9}},
        {"DB_Line_7",new List<int>() {0, 3, 6}},
        {"DB_Line_8",new List<int>() {0, 1, 3}},
        {"DC_Line",new List<int>() {0, 1, 5}},
        {"DD_Line_2",new List<int>() {0, 1, 4}},
        {"DD_Line_3",new List<int>() {0, 4, 10}},
        {"DD_Line_4",new List<int>() {0, 1, 3, 7, 9, 13, 14}},
        {"DD_Line_5",new List<int>() {0, 1, 6, 14, 16}},
        {"DD_Line_6",new List<int>() {0, 1, 3, 7, 9, 13, 14}},
        {"DD_Line_7",new List<int>() {0, 4, 10}},
        {"DD_Line_8",new List<int>() {0, 1, 4}},
        {"DE_Line_1",new List<int>() {0, 2}},
        {"DE_Line_2",new List<int>() {0, 1, 4}},
        {"DE_Line_3",new List<int>() {0, 3, 6, 9}},
        {"DE_Line_4",new List<int>() {0, 1, 3, 9, 12}},
        {"DE_Line_5",new List<int>() {0, 1, 6, 14}},
        {"DE_Line_6",new List<int>() {0, 1, 3, 9, 12}},
        {"DE_Line_7",new List<int>() {0, 3, 6, 9}},
        {"DE_Line_8",new List<int>() {0, 1, 4}},
        {"E1_Line_1",new List<int>() {0}},
        {"E1_Line_2",new List<int>() {0, 5, 6}},
        {"E1_Line_3",new List<int>() {0, 1, 4, 8, 10}},
        {"E1_Line_4",new List<int>() {0, 1, 4, 9, 13}},
        {"E1_Line_5",new List<int>() {0, 1, 4, 9, 13}},
        {"E1_Line_6",new List<int>() {0, 1, 4, 8, 10}},
        {"E1_Line_7",new List<int>() {0, 5, 6}},
        {"E1_Line_8",new List<int>() {0}},
        {"EA_Line_1",new List<int>() {0, 1}},
        {"EA_Line_2",new List<int>() {0, 6}},
        {"EA_Line_3",new List<int>() {0, 4, 8, 10}},
        {"EA_Line_4",new List<int>() {0, 1, 4, 9, 13}},
        {"EA_Line_5",new List<int>() {0, 1, 4, 9, 13}},
        {"EA_Line_6",new List<int>() {0, 4, 8, 10}},
        {"EA_Line_7",new List<int>() {0, 6}},
        {"EA_Line_8",new List<int>() {0, 2}},
        {"EB_Line_1",new List<int>() {0, 2}},
        {"EB_Line_2",new List<int>() {0, 4}},
        {"EB_Line_3",new List<int>() {0, 5, 7}},
        {"EB_Line_4",new List<int>() {0, 1, 3, 9}},
        {"EB_Line_5",new List<int>() {0, 1, 3, 9}},
        {"EB_Line_6",new List<int>() {0, 5, 7}},
        {"EB_Line_7",new List<int>() {0, 4}},
        {"EB_Line_8",new List<int>() {0, 2}},
        {"EC_Line",new List<int>() {0}},
        {"ED_Line_1",new List<int>() {0, 1}},
        {"ED_Line_2",new List<int>() {0, 3}},
        {"ED_Line_3",new List<int>() {0, 5, 8}},
        {"ED_Line_4",new List<int>() {0, 1, 7, 11, 12}},
        {"ED_Line_5",new List<int>() {0, 11, 13}},
        {"ED_Line_6",new List<int>() {0, 1, 7, 11, 12}},
        {"ED_Line_7",new List<int>() {0, 5, 8}},
        {"ED_Line_8",new List<int>() {0, 3}},
        {"EE_Line_2",new List<int>() {0}},
        {"EE_Line_3",new List<int>() {0, 4, 7}},
        {"EE_Line_4",new List<int>() {0, 1, 5, 7, 10}},
        {"EE_Line_5",new List<int>() {0, 3, 11}},
        {"EE_Line_6",new List<int>() {0, 1, 5, 7, 10}},
        {"EE_Line_7",new List<int>() {0, 4, 7}},
        {"EE_Line_8",new List<int>() {0}},
        {"1C_Circle_1",new List<int>() {0, 1, 2, 3}},
        {"1C_Circle_2",new List<int>() {0, 1}},
        {"1C_Circle_3",new List<int>() {0, 1, 5, 8, 11, 14, 19, 21, 24, 27}},
        {"1C_Circle_4",new List<int>() {0, 1, 5, 8, 11, 14, 17, 20, 24}},
        {"1C_Circle_5",new List<int>() {0, 1, 4, 7, 8, 13, 14, 19}},
        {"1C_Circle_6",new List<int>() {0, 1, 3, 6, 9, 12, 15}},
        {"1C_Circle_7",new List<int>() {0, 1, 5, 7, 9, 12}},
        {"1C_Circle_8",new List<int>() {0, 1, 5, 9}},
        {"AC_Circle_1",new List<int>() {0, 3}},
        {"AC_Circle_2",new List<int>() {0, 1}},
        {"AC_Circle_3",new List<int>() {0, 1, 4, 7, 12, 14, 17, 23}},
        {"AC_Circle_4",new List<int>() {0, 1, 4, 7, 12, 13, 17, 19}},
        {"AC_Circle_5",new List<int>() {0, 1, 4, 7, 10, 13, 17}},
        {"AC_Circle_6",new List<int>() {0, 1, 6, 8, 11, 13}},
        {"AC_Circle_7",new List<int>() {0, 1, 2, 6, 9}},
        {"AC_Circle_8",new List<int>() {0, 2, 4, 8}},
        {"BC_Circle_1",new List<int>() {0, 2}},
        {"BC_Circle_2",new List<int>() {0}},
        {"BC_Circle_3",new List<int>() {0, 2, 5, 10}},
        {"BC_Circle_4",new List<int>() {0, 2, 5, 9}},
        {"BC_Circle_5",new List<int>() {0, 2, 5, 8}},
        {"BC_Circle_6",new List<int>() {0, 1, 6}},
        {"BC_Circle_7",new List<int>() {0, 2, 5}},
        {"BC_Circle_8",new List<int>() {0, 1, 3}},
        {"DC_Circle_1",new List<int>() {0, 6}},
        {"DC_Circle_2",new List<int>() {0, 1}},
        {"DC_Circle_3",new List<int>() {0, 1, 4, 7, 10, 15, 16, 20, 25}},
        {"DC_Circle_4",new List<int>() {0, 1, 4, 9, 12, 15, 18, 21}},
        {"DC_Circle_5",new List<int>() {0, 1, 5, 6, 9, 11, 15, 18}},
        {"DC_Circle_6",new List<int>() {0, 1, 4, 10, 15}},
        {"DC_Circle_7",new List<int>() {0, 3, 6, 9, 11}},
        {"DC_Circle_8",new List<int>() {0, 1, 5, 6, 9}},
        {"EC_Circle_1",new List<int>() {0, 5}},
        {"EC_Circle_2",new List<int>() {0, 1}},
        {"EC_Circle_3",new List<int>() {0, 5, 9, 12, 17}},
        {"EC_Circle_4",new List<int>() {0, 5, 8, 12, 14}},
        {"EC_Circle_5",new List<int>() {0, 5, 8, 12}},
        {"EC_Circle_6",new List<int>() {0, 1, 4, 7, 10}},
        {"EC_Circle_7",new List<int>() {0, 4, 8}},
        {"EC_Circle_8",new List<int>() {0, 3, 5}},
        {"C1_Circle_1",new List<int>() {0, 3, 6, 8, 10, 12, 13, 15, 16}},
        {"C1_Circle_2",new List<int>() {0, 2, 3, 5, 6, 7}},
        {"C1_Circle_3",new List<int>() {0, 2, 5}},
        {"C1_Circle_4",new List<int>() {0, 3, 6, 7, 8}},
        {"C1_Circle_5",new List<int>() {0, 4, 8, 10}},
        {"C1_Circle_6",new List<int>() {0, 3, 6, 8, 10, 13}},
        {"C1_Circle_7",new List<int>() {0, 3, 5, 8, 10, 12, 15}},
        {"C1_Circle_8",new List<int>() {0, 4, 11, 13, 16, 19}},
        {"CA_Circle_1",new List<int>() {0, 3, 5, 10, 12, 14, 15}},
        {"CA_Circle_2",new List<int>() {0, 3, 4, 6, 7}},
        {"CA_Circle_3",new List<int>() {0, 3, 6}},
        {"CA_Circle_4",new List<int>() {0, 2, 5, 7}},
        {"CA_Circle_5",new List<int>() {0, 4, 7, 10}},
        {"CA_Circle_6",new List<int>() {0, 3, 10, 12}},
        {"CA_Circle_7",new List<int>() {0, 3, 4, 7, 11, 12}},
        {"CA_Circle_8",new List<int>() {0, 4, 6, 8, 11, 13, 15}},
        {"CB_Circle_1",new List<int>() {0, 5, 7, 10}},
        {"CB_Circle_2",new List<int>() {0, 4, 5, 6}},
        {"CB_Circle_3",new List<int>() {0, 2}},
        {"CB_Circle_4",new List<int>() {0, 3}},
        {"CB_Circle_5",new List<int>() {0, 3, 5}},
        {"CB_Circle_6",new List<int>() {0, 4, 6}},
        {"CB_Circle_7",new List<int>() {0, 5, 7}},
        {"CB_Circle_8",new List<int>() {0, 5, 6, 9}},
        {"CD_Circle_1",new List<int>() {0, 3, 5, 8, 12, 14, 16}},
        {"CD_Circle_2",new List<int>() {0, 3, 4, 7, 8, 11, 12}},
        {"CD_Circle_3",new List<int>() {0, 5, 8, 12, 15, 18, 21, 23, 26}},
        {"CD_Circle_4",new List<int>() {0, 2, 6}},
        {"CD_Circle_5",new List<int>() {0, 3, 7, 8}},
        {"CD_Circle_6",new List<int>() {0, 3, 6, 9, 11}},
        {"CD_Circle_7",new List<int>() {0, 3, 5, 8, 11, 13}},
        {"CD_Circle_8",new List<int>() {0, 4, 10, 11, 14}},
        {"CE_Circle_1",new List<int>() {0, 5, 7, 10, 12}},
        {"CE_Circle_2",new List<int>() {0, 3, 5, 7}},
        {"CE_Circle_3",new List<int>() {0, 6, 9, 13, 16, 20}},
        {"CE_Circle_4",new List<int>() {0, 2, 5}},
        {"CE_Circle_5",new List<int>() {0, 4}},
        {"CE_Circle_6",new List<int>() {0, 3, 5, 8}},
        {"CE_Circle_7",new List<int>() {0, 4, 7, 10}},
        {"CE_Circle_8",new List<int>() {0, 4, 6, 10, 11}},
        {"1A_Circle_1",new List<int>() {0, 1, 5, 8, 11, 14, 18, 21, 24, 28, 31, 34, 38, 41, 44, 48, 51}},
        {"1A_Circle_2",new List<int>() {0, 1, 3}},
        {"1A_Circle_3",new List<int>() {0, 1, 4, 8, 11}},
        {"1A_Circle_4",new List<int>() {0, 1, 4, 7, 10, 13, 16}},
        {"1A_Circle_5",new List<int>() {0, 1, 5, 9, 13, 17, 21, 25, 27}},
        {"1A_Circle_6",new List<int>() {0, 1, 5, 8, 11, 14, 18, 21, 24, 28, 31}},
        {"1A_Circle_7",new List<int>() {0, 1, 4, 6, 9, 12, 15, 17, 20, 23, 26, 28, 31}},
        {"1A_Circle_8",new List<int>() {0, 1, 3, 5, 7, 9, 12, 14, 16, 18, 20, 23, 25, 27, 29}},
        {"1B_Circle_1",new List<int>() {0, 1, 4, 8, 11, 14, 18, 21, 24, 27, 30, 33, 35, 38}},
        {"1B_Circle_2",new List<int>() {0, 1, 2, 3}},
        {"1B_Circle_3",new List<int>() {0, 1, 4, 8, 10}},
        {"1B_Circle_4",new List<int>() {0, 1, 4, 7, 13, 15}},
        {"1B_Circle_5",new List<int>() {0, 1, 5, 9, 11, 14, 19, 21}},
        {"1B_Circle_6",new List<int>() {0, 1, 4, 8, 11, 15, 16, 18, 20, 26}},
        {"1B_Circle_7",new List<int>() {0, 1, 4, 6, 9, 12, 15, 18, 23, 26, 29}},
        {"1B_Circle_8",new List<int>() {0, 1, 3, 5, 7, 9, 12, 14, 16, 19, 23, 26, 29}},
        {"1D_Circle_1",new List<int>() {0, 1, 5, 9, 13, 17, 21, 25, 28, 32, 36, 40, 44, 48, 50, 52}},
        {"1D_Circle_2",new List<int>() {0, 1}},
        {"1D_Circle_3",new List<int>() {0, 1, 5, 8}},
        {"1D_Circle_4",new List<int>() {0, 1, 4, 7, 10, 13}},
        {"1D_Circle_5",new List<int>() {0, 2, 6, 10, 13, 16, 19, 21}},
        {"1D_Circle_6",new List<int>() {0, 1, 5, 8, 12, 16, 19, 23, 26, 30}},
        {"1D_Circle_7",new List<int>() {0, 1, 4, 7, 10, 13, 16, 19, 22, 25, 28, 31}},
        {"1D_Circle_8",new List<int>() {0, 1, 3, 6, 8, 11, 13, 16, 18, 21, 23, 26, 28, 31}},
        {"1E_Circle_1",new List<int>() {0, 1, 5, 9, 13, 17, 20, 22, 23, 26, 27, 30, 32, 34, 37}},
        {"1E_Circle_2",new List<int>() {0}},
        {"1E_Circle_3",new List<int>() {0, 1, 5, 8}},
        {"1E_Circle_4",new List<int>() {0, 1, 4, 7, 11, 14}},
        {"1E_Circle_5",new List<int>() {0, 2, 5, 8, 11, 14, 15, 17, 19}},
        {"1E_Circle_6",new List<int>() {0, 1, 5, 8, 12, 16, 18, 21, 26}},
        {"1E_Circle_7",new List<int>() {0, 1, 4, 7, 10, 13, 16, 20, 22, 26, 30}},
        {"1E_Circle_8",new List<int>() {0, 1, 3, 6, 8, 11, 13, 16, 18, 21, 23, 26, 31}},
        {"A1_Circle_1",new List<int>() {0, 1, 4, 8, 11, 14, 18, 21, 24, 28, 31, 34, 38, 41, 44, 48, 51}},
        {"A1_Circle_2",new List<int>() {0, 1, 3}},
        {"A1_Circle_3",new List<int>() {0, 1, 4, 8, 11}},
        {"A1_Circle_4",new List<int>() {0, 1, 4, 7, 10, 13, 16}},
        {"A1_Circle_5",new List<int>() {0, 1, 4, 7, 11, 15, 19, 23, 27}},
        {"A1_Circle_6",new List<int>() {0, 1, 4, 8, 11, 14, 18, 21, 24, 28, 31}},
        {"A1_Circle_7",new List<int>() {0, 1, 3, 6, 9, 12, 15, 17, 20, 23, 26, 29, 32}},
        {"A1_Circle_8",new List<int>() {0, 1, 3, 5, 7, 9, 12, 14, 16, 18, 21, 23, 25, 27, 30}},
        {"AA_Circle_1",new List<int>() {0, 1, 4, 8, 11, 14, 18, 21, 24, 28, 31, 34, 38, 41, 44, 48, 51}},
        {"AA_Circle_2",new List<int>() {0, 1, 3}},
        {"AA_Circle_3",new List<int>() {0, 1, 4, 8, 11}},
        {"AA_Circle_4",new List<int>() {0, 1, 4, 7, 10, 13, 16}},
        {"AA_Circle_5",new List<int>() {0, 1, 3, 6, 8, 11, 14, 16, 19}},
        {"AA_Circle_6",new List<int>() {0, 1, 4, 8, 11, 14, 18, 21, 24, 28, 31}},
        {"AA_Circle_7",new List<int>() {0, 1, 3, 6, 9, 12, 15, 17, 20, 23, 26, 29, 31}},
        {"AA_Circle_8",new List<int>() {0, 1, 3, 5, 7, 9, 12, 14, 16, 18, 20, 23, 25, 27, 29}},
        {"AB_Circle_1",new List<int>() {0, 1, 4, 8, 11, 15, 16, 18, 21, 25, 28, 29, 33}},
        {"AB_Circle_2",new List<int>() {0, 1, 3}},
        {"AB_Circle_3",new List<int>() {0, 1, 7, 9}},
        {"AB_Circle_4",new List<int>() {0, 1, 4, 7, 12, 14}},
        {"AB_Circle_5",new List<int>() {0, 1, 3, 7, 11, 15, 18}},
        {"AB_Circle_6",new List<int>() {0, 1, 4, 8, 11, 13, 17, 20, 24}},
        {"AB_Circle_7",new List<int>() {0, 1, 3, 7, 9, 13, 18, 22, 24, 27}},
        {"AB_Circle_8",new List<int>() {0, 1, 3, 5, 7, 10, 12, 14, 19, 23, 25, 29}},
        {"AD_Circle_1",new List<int>() {0, 1, 2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28}},
        {"AD_Circle_2",new List<int>() {0, 1}},
        {"AD_Circle_3",new List<int>() {0, 1, 4, 8}},
        {"AD_Circle_4",new List<int>() {0, 1, 4, 7, 10, 13}},
        {"AD_Circle_5",new List<int>() {0, 1, 3, 6, 9, 12, 15, 17}},
        {"AD_Circle_6",new List<int>() {0, 1, 5, 8, 12, 16, 19, 23, 26, 30}},
        {"AD_Circle_7",new List<int>() {0, 1, 4, 7, 10, 13, 16, 19, 22, 25, 28, 31}},
        {"AD_Circle_8",new List<int>() {0, 1, 3, 6, 8, 11, 13, 16, 18, 21, 23, 26, 28, 31}},
        {"AE_Circle_1",new List<int>() {0, 1, 2, 4, 6, 8, 10, 12, 14, 17, 18, 20, 24, 28}},
        {"AE_Circle_2",new List<int>() {0, 1}},
        {"AE_Circle_3",new List<int>() {0, 1, 5, 7}},
        {"AE_Circle_4",new List<int>() {0, 1, 4, 8, 9, 13}},
        {"AE_Circle_5",new List<int>() {0, 1, 3, 7, 9, 13, 18}},
        {"AE_Circle_6",new List<int>() {0, 1, 5, 8, 10, 13, 15, 17, 22}},
        {"AE_Circle_7",new List<int>() {0, 1, 4, 7, 10, 14, 17, 20, 23, 25, 29}},
        {"AE_Circle_8",new List<int>() {0, 1, 3, 6, 8, 11, 13, 16, 18, 21, 24, 26, 31}},
        {"B1_Circle_1",new List<int>() {0, 2, 4, 7, 11, 12, 14, 17, 20, 24, 27, 30, 34, 37}},
        {"B1_Circle_2",new List<int>() {0, 2, 3}},
        {"B1_Circle_3",new List<int>() {0, 1, 4, 6, 10}},
        {"B1_Circle_4",new List<int>() {0, 1, 6, 8, 11, 15}},
        {"B1_Circle_5",new List<int>() {0, 1, 5, 8, 10, 13, 16, 20}},
        {"B1_Circle_6",new List<int>() {0, 5, 8, 10, 12, 16, 19, 22, 26}},
        {"B1_Circle_7",new List<int>() {0, 2, 4, 9, 12, 15, 17, 20, 23, 26, 28}},
        {"B1_Circle_8",new List<int>() {0, 5, 9, 12, 14, 16, 18, 20, 22, 24, 27, 29}},
        {"BA_Circle_1",new List<int>() {0, 2, 4, 7, 10, 14, 16, 17, 19, 22, 25, 29, 32}},
        {"BA_Circle_2",new List<int>() {0, 3}},
        {"BA_Circle_3",new List<int>() {0, 1, 6, 9}},
        {"BA_Circle_4",new List<int>() {0, 1, 5, 8, 11, 14}},
        {"BA_Circle_5",new List<int>() {0, 1, 3, 5, 10, 11, 13, 15, 17}},
        {"BA_Circle_6",new List<int>() {0, 2, 5, 9, 12, 14, 16, 19, 22}},
        {"BA_Circle_7",new List<int>() {0, 2, 4, 8, 13, 16, 19, 21, 24, 27}},
        {"BA_Circle_8",new List<int>() {0, 2, 5, 9, 14, 16, 18, 19, 20, 23, 24, 27, 29}},
        {"BB_Circle_1",new List<int>() {0, 2, 5, 8, 12, 15, 18, 22, 25}},
        {"BB_Circle_2",new List<int>() {0}},
        {"BB_Circle_3",new List<int>() {0, 2, 6}},
        {"BB_Circle_4",new List<int>() {0, 2, 5, 9}},
        {"BB_Circle_5",new List<int>() {0, 2, 6, 10, 14}},
        {"BB_Circle_6",new List<int>() {0, 2, 5, 8, 12, 15}},
        {"BB_Circle_7",new List<int>() {0, 2, 6, 9, 13, 17, 20}},
        {"BB_Circle_8",new List<int>() {0, 1, 4, 7, 10, 13, 16, 19}},
        {"BD_Circle_1",new List<int>() {0, 2, 5, 8, 12, 15, 16, 18, 20, 22, 24, 26, 28}},
        {"BD_Circle_2",new List<int>() {0}},
        {"BD_Circle_3",new List<int>() {0, 1, 4, 6}},
        {"BD_Circle_4",new List<int>() {0, 1, 5, 8, 11}},
        {"BD_Circle_5",new List<int>() {0, 1, 5, 8, 11, 13, 16}},
        {"BD_Circle_6",new List<int>() {0, 3, 5, 10, 12, 15, 17, 20}},
        {"BD_Circle_7",new List<int>() {0, 2, 5, 9, 12, 14, 15, 16, 20, 22, 25}},
        {"BD_Circle_8",new List<int>() {0, 2, 4, 8, 14, 16, 19, 21, 23, 26, 28}},
        {"BE_Circle_1",new List<int>() {0, 2, 6, 10, 13, 14, 16, 20, 24, 28}},
        {"BE_Circle_2",new List<int>() {0}},
        {"BE_Circle_3",new List<int>() {0, 3, 5}},
        {"BE_Circle_4",new List<int>() {0, 2, 5, 10}},
        {"BE_Circle_5",new List<int>() {0, 2, 5, 7, 9, 13}},
        {"BE_Circle_6",new List<int>() {0, 2, 6, 8, 11, 13, 18}},
        {"BE_Circle_7",new List<int>() {0, 2, 6, 9, 11, 13, 17, 21}},
        {"BE_Circle_8",new List<int>() {0, 2, 5, 9, 11, 12, 15, 19, 24}},
        {"D1_Circle_1",new List<int>() {0, 1}},
        {"D1_Circle_2",new List<int>() {0, 1, 4, 6}},
        {"D1_Circle_3",new List<int>() {0, 1, 5, 8, 12, 15}},
        {"D1_Circle_4",new List<int>() {0, 1, 4, 7, 10, 14, 17, 20}},
        {"D1_Circle_5",new List<int>() {0, 1, 3, 6, 9, 12, 15, 19, 23, 28}},
        {"D1_Circle_6",new List<int>() {0, 1, 5, 8, 12, 15, 19, 23, 26, 30, 34, 38}},
        {"D1_Circle_7",new List<int>() {0, 1, 4, 7, 10, 13, 16, 19, 22, 25, 28, 32, 35, 38}},
        {"D1_Circle_8",new List<int>() {0, 1, 3, 6, 8, 11, 13, 16, 18, 21, 23, 26, 28, 31, 33, 36}},
        {"DA_Circle_1",new List<int>() {0}},
        {"DA_Circle_2",new List<int>() {0, 1, 4, 6}},
        {"DA_Circle_3",new List<int>() {0, 1, 5, 8, 12, 15}},
        {"DA_Circle_4",new List<int>() {0, 1, 4, 7, 10, 13, 17, 20}},
        {"DA_Circle_5",new List<int>() {0, 1, 4, 6, 9, 12, 15, 17, 20, 23}},
        {"DA_Circle_6",new List<int>() {0, 1, 5, 8, 12, 15, 19, 23, 26, 30, 34, 37}},
        {"DA_Circle_7",new List<int>() {0, 1, 4, 7, 10, 13, 16, 19, 22, 25, 28, 31, 34, 37}},
        {"DA_Circle_8",new List<int>() {0, 1, 3, 6, 8, 10, 13, 16, 18, 20, 23, 26, 28, 31, 33, 35}},
        {"DB_Circle_1",new List<int>() {0, 2}},
        {"DB_Circle_2",new List<int>() {0, 1, 4, 6}},
        {"DB_Circle_3",new List<int>() {0, 1, 5, 10, 12}},
        {"DB_Circle_4",new List<int>() {0, 1, 4, 8, 10, 15, 17}},
        {"DB_Circle_5",new List<int>() {0, 1, 4, 6, 10, 15, 18, 21}},
        {"DB_Circle_6",new List<int>() {0, 1, 5, 7, 10, 13, 15, 19, 22, 25}},
        {"DB_Circle_7",new List<int>() {0, 1, 4, 7, 11, 13, 17, 21, 25, 27, 31}},
        {"DB_Circle_8",new List<int>() {0, 1, 3, 6, 8, 11, 14, 19, 23, 29, 30, 33}},
        {"DD_Circle_1",new List<int>() {0, 3, 5, 7, 9, 12, 14, 16, 18, 20, 23, 25, 27, 29, 31, 34}},
        {"DD_Circle_2",new List<int>() {0, 1, 4}},
        {"DD_Circle_3",new List<int>() {0, 1, 5, 8, 12}},
        {"DD_Circle_4",new List<int>() {0, 1, 4, 8, 11, 14, 18}},
        {"DD_Circle_5",new List<int>() {0, 1, 4, 7, 10, 13, 16, 19, 22}},
        {"DD_Circle_6",new List<int>() {0, 1, 3, 6, 8, 11, 14, 16, 19, 21, 24}},
        {"DD_Circle_7",new List<int>() {0, 1, 4, 8, 11, 14, 18, 21, 24, 27, 31, 34, 38}},
        {"DD_Circle_8",new List<int>() {0, 1, 4, 6, 9, 12, 15, 17, 20, 23, 26, 28, 31, 34, 37}},
        {"DE_Circle_1",new List<int>() {0, 3, 5, 7, 9, 12, 14, 17, 18, 21, 23, 25, 30, 34}},
        {"DE_Circle_2",new List<int>() {0, 1}},
        {"DE_Circle_3",new List<int>() {0, 1, 4, 7, 10}},
        {"DE_Circle_4",new List<int>() {0, 1, 5, 8, 11, 16}},
        {"DE_Circle_5",new List<int>() {0, 1, 4, 7, 10, 13, 16, 21}},
        {"DE_Circle_6",new List<int>() {0, 1, 3, 6, 9, 11, 14, 17, 19, 24}},
        {"DE_Circle_7",new List<int>() {0, 1, 4, 8, 11, 14, 18, 20, 23, 25, 27, 31}},
        {"DE_Circle_8",new List<int>() {0, 1, 4, 6, 9, 12, 15, 18, 21, 23, 26, 32, 37}},
        {"E1_Circle_1",new List<int>() {0}},
        {"E1_Circle_2",new List<int>() {0, 1, 4, 6}},
        {"E1_Circle_3",new List<int>() {0, 2, 4, 6, 10, 14}},
        {"E1_Circle_4",new List<int>() {0, 4, 6, 9, 12, 16, 19}},
        {"E1_Circle_5",new List<int>() {0, 4, 7, 9, 12, 15, 17, 20, 24}},
        {"E1_Circle_6",new List<int>() {0, 3, 6, 8, 10, 13, 16, 20, 23, 27, 31}},
        {"E1_Circle_7",new List<int>() {0, 6, 9, 11, 14, 17, 20, 23, 26, 29, 33, 36}},
        {"E1_Circle_8",new List<int>() {0, 4, 6, 9, 11, 13, 16, 18, 20, 23, 26, 28, 31, 33, 36}},
        {"EA_Circle_1",new List<int>() {0}},
        {"EA_Circle_2",new List<int>() {0, 2, 4, 6}},
        {"EA_Circle_3",new List<int>() {0, 4, 6, 9, 12}},
        {"EA_Circle_4",new List<int>() {0, 3, 6, 9, 12, 15, 18}},
        {"EA_Circle_5",new List<int>() {0, 4, 7, 10, 12, 15, 17, 20, 23}},
        {"EA_Circle_6",new List<int>() {0, 3, 6, 8, 11, 13, 15, 18, 20, 23, 26}},
        {"EA_Circle_7",new List<int>() {0, 5, 7, 10, 14, 16, 19, 22, 25, 28, 31, 34}},
        {"EA_Circle_8",new List<int>() {0, 3, 9, 11, 14, 16, 19, 21, 23, 26, 28, 31, 33, 35}},
        {"EB_Circle_1",new List<int>() {}},
        {"EB_Circle_2",new List<int>() {0, 1, 2, 3}},
        {"EB_Circle_3",new List<int>() {0, 4, 6, 9}},
        {"EB_Circle_4",new List<int>() {0, 3, 6, 8, 10, 14}},
        {"EB_Circle_5",new List<int>() {0, 7, 9, 13, 17}},
        {"EB_Circle_6",new List<int>() {0, 3, 8, 11, 13, 15, 18, 22}},
        {"EB_Circle_7",new List<int>() {0, 7, 9, 11, 13, 15, 17, 21, 25}},
        {"EB_Circle_8",new List<int>() {0, 3, 8, 14, 16, 18, 19, 21, 22, 25, 29}},
        {"ED_Circle_1",new List<int>() {0, 8, 10, 12, 14, 17, 18, 21, 23, 25, 27, 29, 31, 34}},
        {"ED_Circle_2",new List<int>() {0, 4}},
        {"ED_Circle_3",new List<int>() {0, 2, 4, 6, 9}},
        {"ED_Circle_4",new List<int>() {0, 4, 6, 9, 12, 15}},
        {"ED_Circle_5",new List<int>() {0, 6, 10, 12, 15, 18, 21}},
        {"ED_Circle_6",new List<int>() {0, 4, 7, 9, 11, 14, 16, 19, 21, 24}},
        {"ED_Circle_7",new List<int>() {0, 6, 8, 10, 12, 14, 17, 20, 24, 27, 30}},
        {"ED_Circle_8",new List<int>() {0, 4, 10, 12, 16, 18, 21, 23, 26, 28, 31, 34, 37}},
        {"EE_Circle_1",new List<int>() {0, 7, 12, 16, 21, 25, 30, 34}},
        {"EE_Circle_2",new List<int>() {0}},
        {"EE_Circle_3",new List<int>() {0, 7}},
        {"EE_Circle_4",new List<int>() {0, 3, 8, 13}},
        {"EE_Circle_5",new List<int>() {0, 7, 12, 16}},
        {"EE_Circle_6",new List<int>() {0, 4, 9, 14, 19, 24}},
        {"EE_Circle_7",new List<int>() {0, 7, 12, 16, 21, 25}},
        {"EE_Circle_8",new List<int>() {0, 6, 10, 14, 17, 21}},
    };
    private static readonly Dictionary<int, List<List<JudgeArea>>> WIFISLIDE_JUDGE_QUEUE = new Dictionary<int, List<List<JudgeArea>>>()
    {
        { 1,
            new List<List<JudgeArea>>()
            {
                new List<JudgeArea>() // L
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A1, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B8, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B7, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A6, true },{SensorType.D6, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                },
                new List<JudgeArea>() // Center
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A1, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B1, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.C, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A5, true },{SensorType.B5, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                },
                new List<JudgeArea>() // R
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A1, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B2, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B3, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A4, true },{SensorType.D5, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                }
            }
        },
        { 2,
            new List<List<JudgeArea>>()
            {
                new List<JudgeArea>() // L
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A2, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B1, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B8, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A7, true },{SensorType.D7, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                },
                new List<JudgeArea>() // Center
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A2, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B2, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.C, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A6, true },{SensorType.B6, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                },
                new List<JudgeArea>() // R
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A2, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B3, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B4, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A5, true },{SensorType.D6, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                }
            }
        },
        { 3,
            new List<List<JudgeArea>>()
            {
                new List<JudgeArea>() // L
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A3, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B2, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B1, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A8, true },{SensorType.D8, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                },
                new List<JudgeArea>() // Center
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A3, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B3, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.C, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A7, true },{SensorType.B7, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                },
                new List<JudgeArea>() // R
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A3, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B4, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B5, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A6, true },{SensorType.D7, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                }
            }
        },
        { 4,
            new List<List<JudgeArea>>()
            {
                new List<JudgeArea>() // L
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A4, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B3, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B2, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A1, true },{SensorType.D1, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                },
                new List<JudgeArea>() // Center
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A4, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B4, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.C, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A8, true },{SensorType.B8, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                },
                new List<JudgeArea>() // R
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A4, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B5, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B6, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A7, true },{SensorType.D8, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                }
            }
        },
        { 5,
            new List<List<JudgeArea>>()
            {
                new List<JudgeArea>() // L
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A5, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B4, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B3, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A2, true },{SensorType.D2, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                },
                new List<JudgeArea>() // Center
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A5, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B5, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.C, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A1, true },{SensorType.B1, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                },
                new List<JudgeArea>() // R
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A5, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B6, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B7, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A8, true },{SensorType.D1, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                }
            }
        },
        { 6,
            new List<List<JudgeArea>>()
            {
                new List<JudgeArea>() // L
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A6, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B5, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B4, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A3, true },{SensorType.D3, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                },
                new List<JudgeArea>() // Center
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A6, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B6, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.C, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A2, true },{SensorType.B2, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                },
                new List<JudgeArea>() // R
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A6, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B7, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B8, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A1, true },{SensorType.D2, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                }
            }
        },
        { 7,
            new List<List<JudgeArea>>()
            {
                new List<JudgeArea>() // L
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A7, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B6, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B5, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A4, true },{SensorType.D4, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                },
                new List<JudgeArea>() // Center
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A7, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B7, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.C, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A3, true },{SensorType.B3, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                },
                new List<JudgeArea>() // R
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A7, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B8, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B1, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A2, true },{SensorType.D3, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                }
            }
        },
        { 8,
            new List<List<JudgeArea>>()
            {
                new List<JudgeArea>() // L
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A8, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B7, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B6, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A5, true },{SensorType.D5, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                },
                new List<JudgeArea>() // Center
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A8, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B8, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.C, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A4, true },{SensorType.B4, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                },
                new List<JudgeArea>() // R
                {
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A8, false } },SLIDE_AREA_STEP_MAP["wifi"][0]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B1, false } },SLIDE_AREA_STEP_MAP["wifi"][1]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.B2, false } },SLIDE_AREA_STEP_MAP["wifi"][2]),
                    new JudgeArea(new Dictionary<SensorType, bool>(){ {SensorType.A3, true },{SensorType.D4, true }  },SLIDE_AREA_STEP_MAP["wifi"][3] ),
                }
            }
        }
    };
    // Start is called before the first frame update
    private void Start()
    {
        Application.targetFrameRate = 120;
        ObjectCounter = GameObject.Find("ObjectCounter").GetComponent<ObjectCounter>();
        customSkin = GameObject.Find("Outline").GetComponent<CustomSkin>();
        noteManager = GameObject.Find("Notes").GetComponent<NoteManager>();
    }

    // Update is called once per frame
    private void Update()
    {
        switch (State)
        {
            case NoteLoaderStatus.LodingJson:
                if (jsonLoaderTask is null || !jsonLoaderTask.IsCompleted)
                    return;
                loadedData = jsonLoaderTask.Result;
                diffText.text = loadedData.difficulty;
                levelText.text = loadedData.level;
                titleText.text = loadedData.title;
                artistText.text = loadedData.artist;
                designText.text = loadedData.designer;
                cardImage.color = diffColors[loadedData.diffNum];

                CountNoteSum(loadedData);
                var lastNoteTime = loadedData.timingList.Count > 0 ? loadedData.timingList.Last().Timing : 0d;

                noteParserTask = StartCoroutine(LoadNotes(loadedData.timingList, ignoreOffset, lastNoteTime));

                State = NoteLoaderStatus.ParsingNote;
                break;
            case NoteLoaderStatus.ParsingNote:
                if (noteParserTask == null)
                {
                    State = NoteLoaderStatus.Finished;
                    //noteManager.Refresh();
                    return;
                }
                break;
        }

    }
    IEnumerator LoadNotes(IEnumerable<SimaiTimingPoint> timingList, float ignoreOffset, double lastNoteTime)
    {
        noteManager.Refresh();
        noteIndex.Clear();
        touchIndex.Clear();
        for (int i = 1; i < 9; i++)
            noteIndex.Add(i, 0);
        for (int i = 0; i < 33; i++)
            touchIndex.Add((SensorType)i, 0);

        Stopwatch sw = new();
        sw.Start();
        foreach (var timing in timingList)
        {
            if (sw.ElapsedMilliseconds >= 2)
            {
                yield return 0;
                sw.Restart();
            }
            try
            {
                if (timing.Timing < ignoreOffset)
                {
                    CountNoteCount(timing.Notes.ToList());
                    continue;
                }
                List<TouchDrop> members = new();
                for (var i = 0; i < timing.Notes.Length; i++)
                {
                    var note = timing.Notes[i];
                    if (note.Type == SimaiNoteType.Tap)
                    {
                        GameObject GOnote = null;
                        TapBase NDCompo = null;

                        if (note.IsForceStar)
                        {
                            GOnote = Instantiate(starPrefab, notes.transform);
                            var _NDCompo = GOnote.GetComponent<StarDrop>();
                            _NDCompo.tapSpr = customSkin.Star;
                            _NDCompo.eachSpr = customSkin.Star_Each;
                            _NDCompo.breakSpr = customSkin.Star_Break;
                            _NDCompo.exSpr = customSkin.Star_Ex;
                            _NDCompo.mineSpr = customSkin.Star_Mine;
                            _NDCompo.tapLine = starLine;
                            _NDCompo.isFakeStarRotate = note.IsFakeRotate;
                            _NDCompo.isFakeStar = true;
                            NDCompo = _NDCompo;
                        }
                        else
                        {
                            GOnote = Instantiate(tapPrefab, notes.transform);
                            NDCompo = GOnote.GetComponent<TapDrop>();
                            //自定义note样式
                            NDCompo.tapSpr = customSkin.Tap;
                            NDCompo.breakSpr = customSkin.Tap_Break;
                            NDCompo.eachSpr = customSkin.Tap_Each;
                            NDCompo.exSpr = customSkin.Tap_Ex;
                            NDCompo.mineSpr = customSkin.Tap_Mine;
                        }
                        noteManager.AddNote(GOnote, noteIndex[note.StartPosition]++);
                        // note的图层顺序
                        NDCompo.noteSortOrder = noteSortOrder;
                        noteSortOrder -= NOTE_LAYER_COUNT[note.Type];

                        NDCompo.BreakShine = BreakShine;

                        if (timing.Notes.Length > 1) NDCompo.isEach = true;
                        NDCompo.isBreak = note.IsBreak;
                        NDCompo.isEX = note.IsEx;
                        NDCompo.isMine = note.IsMine;
                        NDCompo.time = (float)timing.Timing;
                        NDCompo.startPosition = note.StartPosition;
                        NDCompo.speed = noteSpeed * timing.HSpeed;

                        if (NDCompo.isMine) NDCompo.tapLine = mineLine;
                    }
                    else if (note.Type == SimaiNoteType.Hold)
                    {
                        var GOnote = Instantiate(holdPrefab, notes.transform);
                        noteManager.AddNote(GOnote, noteIndex[note.StartPosition]++);
                        var NDCompo = GOnote.GetComponent<HoldDrop>();

                        // note的图层顺序
                        NDCompo.noteSortOrder = noteSortOrder;
                        noteSortOrder -= NOTE_LAYER_COUNT[note.Type];

                        NDCompo.tapSpr = customSkin.Hold;
                        NDCompo.holdOnSpr = customSkin.Hold_On;
                        NDCompo.holdOffSpr = customSkin.Hold_Off;
                        NDCompo.eachSpr = customSkin.Hold_Each;
                        NDCompo.eachHoldOnSpr = customSkin.Hold_Each_On;
                        NDCompo.exSpr = customSkin.Hold_Ex;
                        NDCompo.breakSpr = customSkin.Hold_Break;
                        NDCompo.breakHoldOnSpr = customSkin.Hold_Break_On;
                        NDCompo.mineSpr = customSkin.Hold_Mine;

                        NDCompo.HoldShine = HoldShine;
                        NDCompo.BreakShine = BreakShine;

                        if (timing.Notes.Length > 1) NDCompo.isEach = true;
                        NDCompo.time = (float)timing.Timing;
                        NDCompo.LastFor = (float)note.HoldTime;
                        NDCompo.startPosition = note.StartPosition;
                        NDCompo.speed = noteSpeed * timing.HSpeed;
                        NDCompo.isEX = note.IsEx;
                        NDCompo.isBreak = note.IsBreak;
                        NDCompo.isMine = note.IsMine;

                        if (NDCompo.isMine) NDCompo.tapLine = mineLine;
                    }
                    else if (note.Type == SimaiNoteType.TouchHold)
                    {
                        var GOnote = Instantiate(touchHoldPrefab, notes.transform);
                        noteManager.AddTouch(GOnote, touchIndex[TouchHoldBase.GetSensor(note.TouchArea, note.StartPosition)]++);
                        var NDCompo = GOnote.GetComponent<TouchHoldDrop>();

                        // note的图层顺序
                        NDCompo.noteSortOrder = noteSortOrder;
                        noteSortOrder -= NOTE_LAYER_COUNT[note.Type];

                        NDCompo.time = (float)timing.Timing;
                        NDCompo.LastFor = (float)note.HoldTime;
                        NDCompo.speed = touchSpeed * timing.HSpeed;
                        NDCompo.isFirework = note.IsHanabi;
                        NDCompo.isBreak = note.IsBreak;
                        NDCompo.isMine = note.IsMine;
                        NDCompo.areaPosition = note.TouchArea;
                        NDCompo.startPosition = note.StartPosition;
                        NDCompo.TouchPointSprite = customSkin.TouchPoint;
                        NDCompo.TouchPointEachSprite = customSkin.TouchPoint_Each;
                        NDCompo.TouchPointBreakSprite = customSkin.TouchPoint_Break;
                        NDCompo.TouchPointMineSprite = customSkin.TouchPoint_Mine;

                        if (timing.Notes.Length > 1) NDCompo.isEach = true;
                        if (note.IsMine)
                        {
                            Array.Copy(customSkin.TouchHold_Mine, NDCompo.TouchHoldSprite, 5);
                        }
                        else if (note.IsBreak)
                        {
                            Array.Copy(customSkin.TouchHold_Break, NDCompo.TouchHoldSprite, 5);
                        }
                        else
                        {
                            Array.Copy(customSkin.TouchHold, NDCompo.TouchHoldSprite, 5);
                        }
                    }
                    else if (note.Type == SimaiNoteType.Touch)
                    {
                        var GOnote = Instantiate(touchPrefab, notes.transform);
                        noteManager.AddTouch(GOnote, touchIndex[TouchBase.GetSensor(note.TouchArea, note.StartPosition)]++);
                        var NDCompo = GOnote.GetComponent<TouchDrop>();

                        // note的图层顺序
                        NDCompo.noteSortOrder = noteSortOrder;
                        noteSortOrder -= NOTE_LAYER_COUNT[note.Type];

                        NDCompo.time = (float)timing.Timing;
                        NDCompo.areaPosition = note.TouchArea;
                        NDCompo.startPosition = note.StartPosition;

                        NDCompo.fanNormalSprite = customSkin.Touch;
                        NDCompo.fanEachSprite = customSkin.Touch_Each;
                        NDCompo.fanBreakSprite = customSkin.Touch_Break;
                        NDCompo.fanMineSprite = customSkin.Touch_Mine;
                        NDCompo.pointNormalSprite = customSkin.TouchPoint;
                        NDCompo.pointEachSprite = customSkin.TouchPoint_Each;
                        NDCompo.pointBreakSprite = customSkin.TouchPoint_Break;
                        NDCompo.pointMineSprite = customSkin.TouchPoint_Mine;
                        NDCompo.justSprite = customSkin.TouchJust;
                        Array.Copy(customSkin.TouchBorder, NDCompo.multTouchNormalSprite, 2);
                        Array.Copy(customSkin.TouchBorder_Each, NDCompo.multTouchEachSprite, 2);
                        Array.Copy(customSkin.TouchBorder_Break, NDCompo.multTouchBreakSprite, 2);
                        Array.Copy(customSkin.TouchBorder_Mine, NDCompo.multTouchMineSprite, 2);

                        if (timing.Notes.Length > 1)
                        {
                            NDCompo.isEach = true;
                            members.Add(NDCompo);
                        }
                        NDCompo.speed = touchSpeed * timing.HSpeed;
                        NDCompo.isFirework = note.IsHanabi;
                        NDCompo.isBreak = note.IsBreak;
                        NDCompo.isMine = note.IsMine;
                        NDCompo.GroupInfo = null;
                    }

                    else if (note.Type == SimaiNoteType.Slide)
                        InstantiateStarGroup(timing, note, i, lastNoteTime); // 星星组
                }


                if (members.Count != 0)
                {
                    var sensorTypes = members.GroupBy(x => x.GetSensor())
                                             .Select(x => x.Key)
                                             .ToList();
                    List<List<SensorType>> sensorGroups = new();

                    while (sensorTypes.Count > 0)
                    {
                        var sensorType = sensorTypes[0];
                        var existsGroup = sensorGroups.FindAll(x => x.Contains(sensorType));
                        var groupMap = TOUCH_GROUPS[sensorType];
                        existsGroup.AddRange(sensorGroups.FindAll(x => x.Any(y => groupMap.Contains(y))));

                        var groupMembers = existsGroup.SelectMany(x => x)
                                                      .ToList();
                        var newMembers = sensorTypes.FindAll(x => groupMap.Contains(x));

                        groupMembers.AddRange(newMembers);
                        groupMembers.Add(sensorType);
                        var newGroup = groupMembers.GroupBy(x => x)
                                                   .Select(x => x.Key)
                                                   .ToList();

                        foreach (var newMember in newGroup)
                            sensorTypes.Remove(newMember);
                        foreach (var oldGroup in existsGroup)
                            sensorGroups.Remove(oldGroup);

                        sensorGroups.Add(newGroup);
                    }
                    List<TouchGroup> touchGroups = new();
                    var groupedMembers = members.GroupBy(x => x.GetSensor());
                    foreach (var group in sensorGroups)
                    {
                        touchGroups.Add(new TouchGroup()
                        {
                            Members = group.SelectMany(x => groupedMembers.Where(g => g.Key == x)
                                                                          .SelectMany(g => g)).ToArray()
                        });
                    }
                    foreach (var member in members)
                        member.GroupInfo = touchGroups.Find(x => x.Members.Any(y => y == member));
                }

                var eachNotes = timing.Notes.ToList().FindAll(o =>
                    o.Type != SimaiNoteType.Touch &&
                    o.Type != SimaiNoteType.TouchHold &&
                    !isTouch(o.TouchArea) // Slide start with touch have simaiNoteType = Slide
                );
                if (eachNotes.Count > 1) //有多个非touchnote
                {
                    var startPos = eachNotes[0].StartPosition;
                    var endPos = eachNotes[1].StartPosition;
                    endPos = endPos - startPos;
                    if (endPos == 0) continue;

                    var line = Instantiate(eachLine, notes.transform);
                    var lineDrop = line.GetComponent<EachLineDrop>();

                    lineDrop.time = (float)timing.Timing;
                    lineDrop.speed = noteSpeed * timing.HSpeed;

                    endPos = endPos < 0 ? endPos + 8 : endPos;
                    endPos = endPos > 8 ? endPos - 8 : endPos;
                    endPos++;

                    if (endPos > 4)
                    {
                        startPos = eachNotes[1].StartPosition;
                        endPos = eachNotes[0].StartPosition;
                        endPos = endPos - startPos;
                        endPos = endPos < 0 ? endPos + 8 : endPos;
                        endPos = endPos > 8 ? endPos - 8 : endPos;
                        endPos++;
                    }

                    lineDrop.startPosition = startPos;
                    lineDrop.curvLength = endPos - 1;
                }
            }
            catch (Exception e)
            {
                GameObject.Find("ErrText").GetComponent<Text>().text =
                    "在第" + (timing.RawTextPositionY + 1) + "行发现问题：\n" + e.Message;
                UnityEngine.Debug.LogError(e);
            }
        }
        noteParserTask = null;
        yield break;
    }

    // 专门为了处理神秘ReadOnlySpan入参。。。虽然在Play+Neo没毛，但向下就有点搞
    public class MajsonConverter : JsonConverter<SimaiTimingPoint>
    {
        public override SimaiTimingPoint ReadJson(JsonReader reader, Type objectType, SimaiTimingPoint existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            double timing = 0; float bpm = 0, hspeed = 1f;
            int textPosX = 0, textPosY = 0, rawPos = 0;
            SimaiNote[] notes = null;
            string rawString = null;

            // 手动流式读取，不生成 JObject
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndObject) break;
                if (reader.TokenType != JsonToken.PropertyName) continue;

                string propName = reader.Value.ToString();
                reader.Read();

                switch (propName)
                {
                    case "Timing": timing = Convert.ToDouble(reader.Value); break;
                    case "Bpm": bpm = Convert.ToSingle(reader.Value); break;
                    case "HSpeed": hspeed = Convert.ToSingle(reader.Value); break;
                    case "RawTextPositionX": textPosX = Convert.ToInt32(reader.Value); break;
                    case "RawTextPositionY": textPosY = Convert.ToInt32(reader.Value); break;
                    case "RawTextPosition": rawPos = Convert.ToInt32(reader.Value); break;
                    case "Notes": notes = serializer.Deserialize<SimaiNote[]>(reader); break;
                    case "RawContent":
                        rawString = (string)reader.Value;
                        break;
                }
            }

            return new SimaiTimingPoint(
                timing, notes, rawString.AsSpan(),
                textPosX, textPosY, bpm, hspeed, rawPos
            );
        }

        public override void WriteJson(JsonWriter writer, SimaiTimingPoint value, JsonSerializer serializer) => throw new NotImplementedException();
    }

    public void LoadJson(string json, float ignoreOffset)
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new MajsonConverter());
        jsonLoaderTask = Task.Run(() => JsonConvert.DeserializeObject<Majson>(json, settings));
        State = NoteLoaderStatus.LodingJson;
        this.ignoreOffset = ignoreOffset;
    }


    private void CountNoteSum(Majson json)
    {
        foreach (var timing in json.timingList)
            foreach (var note in timing.Notes)
                if (!note.IsBreak)
                {
                    if (note.Type == SimaiNoteType.Tap) ObjectCounter.tapSum++;
                    if (note.Type == SimaiNoteType.Hold) ObjectCounter.holdSum++;
                    if (note.Type == SimaiNoteType.TouchHold) ObjectCounter.holdSum++;
                    if (note.Type == SimaiNoteType.Touch) ObjectCounter.touchSum++;
                    if (note.Type == SimaiNoteType.Slide)
                    {
                        if (!note.IsSlideNoHead)
                        {
                            if (isTouch(note.TouchArea)) ObjectCounter.touchSum++;
                            else ObjectCounter.tapSum++;
                        }
                        if (note.IsSlideBreak)
                            ObjectCounter.breakSum++;
                        else
                            ObjectCounter.slideSum++;
                    }
                }
                else
                {
                    if (note.Type == SimaiNoteType.Slide)
                    {
                        if (!note.IsSlideNoHead) ObjectCounter.breakSum++;
                        if (note.IsSlideBreak)
                            ObjectCounter.breakSum++;
                        else
                            ObjectCounter.slideSum++;
                    }
                    else
                    {
                        ObjectCounter.breakSum++;
                    }
                }
    }

    private void CountNoteCount(List<SimaiNote> timing)
    {
        foreach (var note in timing)
            if (!note.IsBreak)
            {
                if (note.Type == SimaiNoteType.Tap) ObjectCounter.tapCount++;
                if (note.Type == SimaiNoteType.Hold) ObjectCounter.holdCount++;
                if (note.Type == SimaiNoteType.TouchHold) ObjectCounter.holdCount++;
                if (note.Type == SimaiNoteType.Touch) ObjectCounter.touchCount++;
                if (note.Type == SimaiNoteType.Slide)
                {
                    if (!note.IsSlideNoHead) ObjectCounter.tapCount++;
                    if (note.IsSlideBreak)
                        ObjectCounter.breakCount++;
                    else
                        ObjectCounter.slideCount++;
                }
            }
            else
            {
                if (note.Type == SimaiNoteType.Slide)
                {
                    if (!note.IsSlideNoHead) ObjectCounter.breakCount++;
                    if (note.IsSlideBreak)
                        ObjectCounter.breakCount++;
                    else
                        ObjectCounter.slideCount++;
                }
                else
                {
                    ObjectCounter.breakCount++;
                }
            }
    }

    private void InstantiateStarGroup(SimaiTimingPoint timing, SimaiNote note, int sort, double lastNoteTime)
    {
        string readSlideAnchor(string noteContent, ref int ptr)
        {
            if (isNonCTouchArea(noteContent[ptr]))
            {
                return noteContent[ptr++..++ptr];
            }
            return noteContent[ptr++].ToString();
        }

        var subSlide = new List<SimaiNote>();
        var subBarCount = new List<int>();
        var sumBarCount = 0;

        var noteContent = note.RawContent;
        var ptr = 0; // 指向目前处理的字符
        var latestStartIndex = readSlideAnchor(noteContent, ref ptr); // 存储上一个Slide的结尾 也就是下一个Slide的起点

        var specTimeFlag = 0; // 表示此组合slide是指定总时长 还是指定每一段的时长
        // 0-目前还没有读取 1-读取到了一个未指定时长的段落 2-读取到了一个指定时长的段落 3-（期望）读取到了最后一个时长指定

        while (ptr < noteContent.Length)
            if (!isSupportedSlideStart(noteContent[ptr]))
            {
                // Reading the slide type. Due to touch slide support, we can no longer assume the slide start is 1 character anymore
                var slideTypeChar = noteContent[ptr++].ToString();

                var slidePart = new SimaiNote();
                slidePart.Type = SimaiNoteType.Slide;
                slidePart.StartPosition = parseSlideAnchor(latestStartIndex);
                if (isTouch(latestStartIndex[0]))
                {
                    slidePart.TouchArea = latestStartIndex[0];
                }

                if (slideTypeChar == "V")
                {
                    // 转折星星
                    var middlePos = noteContent[ptr++];
                    var endPos = noteContent[ptr++];

                    slidePart.RawContent = latestStartIndex + slideTypeChar + middlePos + endPos;
                    // Temp: Unsure if V touch slide is supported
                    latestStartIndex = endPos.ToString();
                }
                else
                {
                    // 其他普通星星
                    // 额外检查pp和qq
                    if (noteContent[ptr] == slideTypeChar[0]) slideTypeChar += noteContent[ptr++];
                    var endPos = readSlideAnchor(noteContent, ref ptr);

                    // Special case if C is the start, use endPos for rotation
                    if (slidePart.StartPosition == 0)
                    {
                        slidePart.StartPosition = parseSlideAnchor(endPos);
                    }

                    slidePart.RawContent = latestStartIndex + slideTypeChar + endPos;
                    latestStartIndex = endPos.ToString();
                }

                if (noteContent[ptr] == '[')
                {
                    // 如果指定了速度
                    if (specTimeFlag == 0)
                        // 之前未读取过
                        specTimeFlag = 2;
                    else if (specTimeFlag == 1)
                        // 之前读取到的都是未指定时长的段落 那么将flag设为3 如果之后又读取到时长 则报错
                        specTimeFlag = 3;
                    else if (specTimeFlag == 3)
                        // 之前读取到了指定时长 并期待那个时长就是最终时长 但是又读取到一个新的时长 则报错
                        throw new Exception($"SLIDE ERROR: {note.RawContent}");

                    while (ptr < noteContent.Length && noteContent[ptr] != ']')
                        slidePart.RawContent += noteContent[ptr++];
                    slidePart.RawContent += noteContent[ptr++];
                }
                else
                {
                    // 没有指定速度
                    if (specTimeFlag == 0)
                        // 之前未读取过
                        specTimeFlag = 1;
                    else if (specTimeFlag == 2 || specTimeFlag == 3)
                        // 之前读取到指定时长的段落了 说明这一条组合星星有的指定时长 有的没指定 则需要报错
                        throw new Exception($"SLIDE ERROR: {note.RawContent}");
                }

                string slideShape = detectShapeFromText(slidePart.RawContent);
                slideShape = slideShape.TrimStart('*').TrimStart('^').TrimStart('-');
                int slideIndex = SLIDE_PREFAB_MAP[slideShape];
                if (slideIndex < 0) slideIndex = -slideIndex;

                var barCount = slidePrefab[slideIndex].transform.childCount;
                subBarCount.Add(barCount);
                sumBarCount += barCount;

                subSlide.Add(slidePart);
            }
            else
            {
                // 理论上来说 不应该读取到数字 因此如果读取到了 说明有语法错误
                throw new Exception($"SLIDE ERROR: {note.RawContent}");
            }

        subSlide.ForEach(o =>
        {
            o.IsBreak = note.IsBreak;
            o.IsEx = note.IsEx;
            o.IsSlideBreak = note.IsSlideBreak;
            o.IsMine = note.IsMine;
            o.IsMineSlide = note.IsMineSlide;
            o.IsSlideNoHead = true;
        });
        subSlide[0].IsSlideNoHead = note.IsSlideNoHead;

        if (specTimeFlag == 1 || specTimeFlag == 0)
            // 如果到结束还是1 那说明没有一个指定了时长 报错
            throw new Exception($"SLIDE ERROR: {note.RawContent}");
        // 此时 flag为2表示每条指定语法 为3表示整体指定语法

        var tempBarCount = 0;
        for (var i = 0; i < subSlide.Count; i++)
        {
            subSlide[i].SlideStartTime = note.SlideStartTime + (double)tempBarCount / sumBarCount * note.SlideTime;
            subSlide[i].SlideTime = (double)subBarCount[i] / sumBarCount * note.SlideTime;
            tempBarCount += subBarCount[i];
        }

        GameObject parent = null;
        List<SlideDrop> subSlides = new();
        float totalLen = (float)subSlide.Select(x => x.SlideTime).Sum();
        float totalSlideLen = 0;
        int totalJudgeAreaCount = 0;
        for (var i = 0; i <= subSlide.Count - 1; i++)
        {
            bool isConn = subSlide.Count != 1;
            bool isGroupHead = i == 0;
            bool isGroupEnd = i == subSlide.Count - 1;
            if (note.RawContent.Contains('w')) //wifi
            {
                if (isConn)
                    throw new InvalidOperationException("不允许Wifi Slide作为Connection Slide的一部分");
                InstantiateWifi(timing, subSlide[i]);
            }
            else
            {
                ConnSlideInfo info = new ConnSlideInfo()
                {
                    TotalLength = totalLen,
                    IsGroupPart = isConn,
                    IsGroupPartHead = isGroupHead,
                    IsGroupPartEnd = isGroupEnd,
                    Parent = parent
                };
                if (note.RawContent[0] >= 'A' && note.RawContent[0] <= 'E')
                {
                    parent = InstantiateTouchStar(timing, subSlide[i], info);
                }
                else
                {
                    parent = InstantiateStar(timing, subSlide[i], info);
                }
                subSlides.Add(parent.GetComponent<SlideDrop>());
            }
        }
        subSlides.ForEach(s =>
        {
            s.Initialize();
            totalSlideLen += s.GetSlideLength();
            totalJudgeAreaCount += s.judgeQueue.Count;
        });
        totalJudgeAreaCount -= (subSlides.Count - 1);
        subSlides.ForEach(s =>
        {
            s.ConnectInfo.TotalSlideLen = totalSlideLen;
            s.SetCanSkip(totalJudgeAreaCount);
        });
    }

    private GameObject InstantiateWifi(SimaiTimingPoint timing, SimaiNote note)
    {
        var str = note.RawContent.Substring(0, 3);
        var digits = str.Split('w');
        var startPos = int.Parse(digits[0]);
        var endPos = int.Parse(digits[1]);
        endPos = endPos - startPos;
        endPos = endPos < 0 ? endPos + 8 : endPos;
        endPos = endPos > 8 ? endPos - 8 : endPos;
        endPos++;

        var GOnote = Instantiate(starPrefab, notes.transform);
        var NDCompo = GOnote.GetComponent<StarDrop>();
        if (!note.IsSlideNoHead)
            noteManager.AddNote(GOnote, noteIndex[note.StartPosition]++);


        // note的图层顺序
        NDCompo.noteSortOrder = noteSortOrder;
        noteSortOrder -= NOTE_LAYER_COUNT[note.Type];

        NDCompo.tapSpr = customSkin.Star;
        NDCompo.eachSpr = customSkin.Star_Each;
        NDCompo.breakSpr = customSkin.Star_Break;
        NDCompo.exSpr = customSkin.Star_Ex;
        NDCompo.mineSpr = customSkin.Star_Mine;

        NDCompo.tapSpr_Double = customSkin.Star_Double;
        NDCompo.eachSpr_Double = customSkin.Star_Each_Double;
        NDCompo.breakSpr_Double = customSkin.Star_Break_Double;
        NDCompo.exSpr_Double = customSkin.Star_Ex_Double;
        NDCompo.mineSpr_Double = customSkin.Star_Mine_Double;

        NDCompo.BreakShine = BreakShine;

        NDCompo.rotateSpeed = (float)note.SlideTime;
        NDCompo.isEX = note.IsEx;
        NDCompo.isBreak = note.IsBreak;
        NDCompo.isMine = note.IsMine;

        if (NDCompo.isMine) NDCompo.tapLine = mineLine;

        var slideWifi = Instantiate(slidePrefab[SLIDE_PREFAB_MAP["wifi"]], notes.transform);
        slideWifi.SetActive(false);
        NDCompo.slide = slideWifi;
        var WifiCompo = slideWifi.GetComponent<WifiDrop>();

        WifiCompo.normalStar = customSkin.Star;
        WifiCompo.eachStar = customSkin.Star_Each;
        WifiCompo.breakStar = customSkin.Star_Break;
        WifiCompo.mineStar = customSkin.Star_Mine;
        WifiCompo.judgeBreakShine = JudgeBreakShine;
        WifiCompo.breakMaterial = breakMaterial;
        WifiCompo.slideShine = BreakShine;
        WifiCompo.areaStep = new List<int>(SLIDE_AREA_STEP_MAP["wifi"]);
        WifiCompo.judgeQueues = new(WIFISLIDE_JUDGE_QUEUE[startPos]);
        WifiCompo.slideConst = SLIDE_AREA_CONST["wifi"];
        WifiCompo.smoothSlideAnime = smoothSlideAnime;

        Array.Copy(customSkin.Wifi, WifiCompo.normalSlide, 11);
        Array.Copy(customSkin.Wifi_Each, WifiCompo.eachSlide, 11);
        Array.Copy(customSkin.Wifi_Break, WifiCompo.breakSlide, 11);
        Array.Copy(customSkin.Wifi_Mine, WifiCompo.mineSlide, 11);

        if (timing.Notes.Length > 1)
        {
            NDCompo.isEach = true;
            NDCompo.isDouble = false;
            var notes = timing.Notes.ToList();
            if (notes.FindAll(
                    o => o.Type == SimaiNoteType.Slide).Count
                > 1)
                WifiCompo.isEach = true;
            var count = notes.FindAll(
                o => o.Type == SimaiNoteType.Slide &&
                     o.StartPosition == note.StartPosition).Count;
            if (count > 1) //有同起点
            {
                NDCompo.isDouble = true;
                if (count == notes.Count)
                    NDCompo.isEach = false;
                else
                    NDCompo.isEach = true;
            }
        }

        WifiCompo.isBreak = note.IsSlideBreak;
        WifiCompo.isMine = note.IsMineSlide;

        NDCompo.isNoHead = note.IsSlideNoHead;
        NDCompo.time = (float)timing.Timing;
        NDCompo.startPosition = note.StartPosition;
        NDCompo.speed = noteSpeed * timing.HSpeed;

        WifiCompo.isJustR = detectJustType(note.RawContent, out endPos);
        WifiCompo.endPosition = endPos;
        WifiCompo.speed = noteSpeed * timing.HSpeed;
        WifiCompo.timeStart = (float)timing.Timing;
        WifiCompo.startPosition = note.StartPosition;
        WifiCompo.time = (float)note.SlideStartTime;
        WifiCompo.LastFor = (float)note.SlideTime;
        WifiCompo.sortIndex = slideLayer;
        slideLayer -= SLIDE_AREA_STEP_MAP["wifi"].Last();
        //slideLayer += 5;

        return slideWifi;
    }

    private GameObject InstantiateStar(SimaiTimingPoint timing, SimaiNote note, ConnSlideInfo info)
    {
        var GOnote = Instantiate(starPrefab, notes.transform);
        var NDCompo = GOnote.GetComponent<StarDrop>();
        if (!note.IsSlideNoHead)
            noteManager.AddNote(GOnote, noteIndex[note.StartPosition]++);
        // note的图层顺序
        NDCompo.noteSortOrder = noteSortOrder;
        noteSortOrder -= NOTE_LAYER_COUNT[note.Type];

        NDCompo.tapSpr = customSkin.Star;
        NDCompo.eachSpr = customSkin.Star_Each;
        NDCompo.breakSpr = customSkin.Star_Break;
        NDCompo.exSpr = customSkin.Star_Ex;
        NDCompo.mineSpr = customSkin.Star_Mine;

        NDCompo.tapSpr_Double = customSkin.Star_Double;
        NDCompo.eachSpr_Double = customSkin.Star_Each_Double;
        NDCompo.breakSpr_Double = customSkin.Star_Break_Double;
        NDCompo.exSpr_Double = customSkin.Star_Ex_Double;
        NDCompo.mineSpr_Double = customSkin.Star_Mine_Double;

        NDCompo.BreakShine = BreakShine;

        NDCompo.rotateSpeed = (float)note.SlideTime;
        NDCompo.isEX = note.IsEx;
        NDCompo.isBreak = note.IsBreak;
        NDCompo.isMine = note.IsMine;

        if (NDCompo.isMine) NDCompo.tapLine = mineLine;

        string slideShape = detectShapeFromText(note.RawContent);
        var isMirror = false;
        var isUDMirror = false;
        var isNoStartPositionRotation = false;
        if (slideShape.StartsWith("*"))
        {
            isNoStartPositionRotation = true;
            slideShape = slideShape.Substring(1);
        }
        if (slideShape.StartsWith("^"))
        {
            isUDMirror = true;
            slideShape = slideShape.Substring(1);
        }
        if (slideShape.StartsWith("-"))
        {
            isMirror = true;
            slideShape = slideShape.Substring(1);
        }
        int slideIndex = SLIDE_PREFAB_MAP[slideShape];

        var slide = Instantiate(slidePrefab[slideIndex], notes.transform);
        var slide_star = Instantiate(star_slidePrefab, notes.transform);
        slide_star.GetComponent<SpriteRenderer>().sprite = customSkin.Star;
        slide_star.SetActive(false);
        slide.SetActive(false);
        NDCompo.slide = slide;
        var SliCompo = slide.AddComponent<SlideDrop>();

        SliCompo.slideType = slideShape;
        SliCompo.spriteNormal = customSkin.Slide;
        SliCompo.spriteEach = customSkin.Slide_Each;
        SliCompo.spriteBreak = customSkin.Slide_Break;
        SliCompo.spriteMine = customSkin.Slide_Mine;
        SliCompo.slideShine = BreakShine;
        SliCompo.breakMaterial = breakMaterial;
        SliCompo.judgeBreakShine = JudgeBreakShine;
        SliCompo.areaStep = new List<int>(SLIDE_AREA_STEP_MAP[slideShape]);
        SliCompo.slideConst = SLIDE_AREA_CONST[slideShape];
        SliCompo.smoothSlideAnime = smoothSlideAnime;

        if (timing.Notes.Length > 1)
        {
            var notes = timing.Notes.ToList();
            NDCompo.isEach = true;
            if (notes.FindAll(o => o.Type == SimaiNoteType.Slide).Count > 1)
            {
                SliCompo.isEach = true;
                slide_star.GetComponent<SpriteRenderer>().sprite = customSkin.Star_Each;
            }

            var count = notes.FindAll(
                o => o.Type == SimaiNoteType.Slide &&
                     o.StartPosition == note.StartPosition).Count;
            if (count > 1)
            {
                NDCompo.isDouble = true;
                if (count == notes.Count)
                    NDCompo.isEach = false;
                else
                    NDCompo.isEach = true;
            }
        }

        SliCompo.ConnectInfo = info;
        SliCompo.isBreak = note.IsSlideBreak;
        SliCompo.isMine = note.IsMineSlide;
        if (note.IsSlideBreak) slide_star.GetComponent<SpriteRenderer>().sprite = customSkin.Star_Break;

        NDCompo.isNoHead = note.IsSlideNoHead;
        NDCompo.time = (float)timing.Timing;
        NDCompo.startPosition = note.StartPosition;
        NDCompo.speed = noteSpeed * timing.HSpeed;


        SliCompo.isMirror = isMirror;
        SliCompo.isUDMirror = isUDMirror;
        SliCompo.isNoStartPositionRotation = isNoStartPositionRotation;
        SliCompo.isJustR = detectJustType(note.RawContent, out int endPos);
        SliCompo.endPosition = endPos;
        if (slideIndex - 26 > 0 && slideIndex - 26 <= 8)
        {
            // known slide sprite issue
            //    1 2 3 4 5 6 7 8
            // p  X X X X X X O O
            // q  X O O X X X X X
            var pqEndPos = slideIndex - 26;
            SliCompo.isSpecialFlip = isMirror == (pqEndPos == 7 || pqEndPos == 8);
        }
        else
        {
            SliCompo.isSpecialFlip = isMirror;
        }
        SliCompo.speed = noteSpeed * timing.HSpeed;
        SliCompo.timeStart = (float)timing.Timing;
        SliCompo.areaPosition = note.TouchArea;
        SliCompo.startPosition = note.StartPosition;
        SliCompo.star_slide = slide_star;
        SliCompo.time = (float)note.SlideStartTime;
        SliCompo.LastFor = (float)note.SlideTime;
        //SliCompo.sortIndex = -7000 + (int)((lastNoteTime - timing.time) * -100) + sort * 5;
        SliCompo.sortIndex = slideLayer;
        slideLayer -= SLIDE_AREA_STEP_MAP[slideShape].Last();
        //slideLayer += 5;
        return slide;
    }

    private GameObject InstantiateTouchStar(SimaiTimingPoint timing, SimaiNote note, ConnSlideInfo info)
    {
        var GOnote = Instantiate(touchStarPrefab, notes.transform);
        if (!note.IsSlideNoHead)
            noteManager.AddTouch(GOnote, touchIndex[TouchBase.GetSensor(note.TouchArea, note.StartPosition)]++);
        var NDCompo = GOnote.GetComponent<TouchStarDrop>();

        // note的图层顺序
        NDCompo.noteSortOrder = noteSortOrder;
        noteSortOrder -= NOTE_LAYER_COUNT[note.Type];

        NDCompo.time = (float)timing.Timing;
        NDCompo.areaPosition = note.TouchArea;
        NDCompo.startPosition = note.StartPosition;

        NDCompo.fanNormalSprite = customSkin.TouchStar;
        NDCompo.fanEachSprite = customSkin.TouchStar_Each;
        NDCompo.fanBreakSprite = customSkin.TouchStar_Break;
        NDCompo.fanMineSprite = customSkin.TouchStar_Mine;
        NDCompo.pointNormalSprite = customSkin.TouchPoint;
        NDCompo.pointEachSprite = customSkin.TouchPoint_Each;
        NDCompo.pointBreakSprite = customSkin.TouchPoint_Break;
        NDCompo.pointMineSprite = customSkin.TouchPoint_Mine;
        NDCompo.justSprite = customSkin.TouchJust;
        Array.Copy(customSkin.TouchBorder, NDCompo.multTouchNormalSprite, 2);
        Array.Copy(customSkin.TouchBorder_Each, NDCompo.multTouchEachSprite, 2);
        Array.Copy(customSkin.TouchBorder_Break, NDCompo.multTouchBreakSprite, 2);
        Array.Copy(customSkin.TouchBorder_Mine, NDCompo.multTouchMineSprite, 2);
        NDCompo.speed = touchSpeed * timing.HSpeed;
        NDCompo.isFirework = note.IsHanabi;
        NDCompo.isBreak = note.IsBreak;
        NDCompo.isMine = note.IsMine;
        NDCompo.GroupInfo = null;

        string slideShape = detectShapeFromText(note.RawContent);
        var isMirror = false;
        var isUDMirror = false;
        var isNoStartPositionRotation = false;
        if (slideShape.StartsWith("*"))
        {
            isNoStartPositionRotation = true;
            slideShape = slideShape.Substring(1);
        }
        if (slideShape.StartsWith("^"))
        {
            isUDMirror = true;
            slideShape = slideShape.Substring(1);
        }
        if (slideShape.StartsWith("-"))
        {
            isMirror = true;
            slideShape = slideShape.Substring(1);
        }
        int slideIndex = SLIDE_PREFAB_MAP[slideShape];

        var slide = Instantiate(slidePrefab[slideIndex], notes.transform);
        var slide_star = Instantiate(star_slidePrefab, notes.transform);
        slide_star.GetComponent<SpriteRenderer>().sprite = customSkin.Star;
        slide_star.SetActive(false);
        slide.SetActive(false);
        NDCompo.slide = slide;
        var SliCompo = slide.AddComponent<SlideDrop>();

        SliCompo.slideType = slideShape;
        SliCompo.spriteNormal = customSkin.Slide;
        SliCompo.spriteEach = customSkin.Slide_Each;
        SliCompo.spriteBreak = customSkin.Slide_Break;
        SliCompo.spriteMine = customSkin.Slide_Mine;
        SliCompo.slideShine = BreakShine;
        SliCompo.breakMaterial = breakMaterial;
        SliCompo.judgeBreakShine = JudgeBreakShine;
        SliCompo.areaStep = new List<int>(SLIDE_AREA_STEP_MAP[slideShape]);
        SliCompo.slideConst = SLIDE_AREA_CONST[slideShape];
        SliCompo.smoothSlideAnime = smoothSlideAnime;

        if (timing.Notes.Length > 1)
        {
            var notes = timing.Notes.ToList();
            NDCompo.isEach = true;
            if (notes.FindAll(o => o.Type == SimaiNoteType.Slide).Count > 1)
            {
                SliCompo.isEach = true;
                slide_star.GetComponent<SpriteRenderer>().sprite = customSkin.Star_Each;
            }

            var count = notes.FindAll(
                o => o.Type == SimaiNoteType.Slide &&
                     o.StartPosition == note.StartPosition).Count;
            if (count > 1)
            {
                if (count == notes.Count)
                    NDCompo.isEach = false;
                else
                    NDCompo.isEach = true;
            }
        }

        SliCompo.ConnectInfo = info;
        SliCompo.isBreak = note.IsSlideBreak;
        SliCompo.isMine = note.IsMineSlide;
        if (note.IsSlideBreak) slide_star.GetComponent<SpriteRenderer>().sprite = customSkin.Star_Break;

        NDCompo.isNoHead = note.IsSlideNoHead;
        NDCompo.time = (float)timing.Timing;
        NDCompo.startPosition = note.StartPosition;
        NDCompo.speed = noteSpeed * timing.HSpeed;

        SliCompo.isMirror = isMirror;
        SliCompo.isUDMirror = isUDMirror;
        SliCompo.isNoStartPositionRotation = isNoStartPositionRotation;
        SliCompo.isJustR = detectJustType(note.RawContent, out int endPos);
        SliCompo.endPosition = endPos;
        if (slideIndex - 26 > 0 && slideIndex - 26 <= 8)
        {
            // known slide sprite issue
            //    1 2 3 4 5 6 7 8
            // p  X X X X X X O O
            // q  X O O X X X X X
            var pqEndPos = slideIndex - 26;
            SliCompo.isSpecialFlip = isMirror == (pqEndPos == 7 || pqEndPos == 8);
        }
        else
        {
            SliCompo.isSpecialFlip = isMirror;
        }
        SliCompo.speed = noteSpeed * timing.HSpeed;
        SliCompo.timeStart = (float)timing.Timing;
        SliCompo.startPosition = note.StartPosition;
        SliCompo.areaPosition = note.TouchArea;
        SliCompo.star_slide = slide_star;
        SliCompo.time = (float)note.SlideStartTime;
        SliCompo.LastFor = (float)note.SlideTime;
        //SliCompo.sortIndex = -7000 + (int)((lastNoteTime - timing.time) * -100) + sort * 5;
        SliCompo.sortIndex = slideLayer;
        slideLayer -= SLIDE_AREA_STEP_MAP[slideShape].Last();
        //slideLayer += 5;
        return slide;
    }

    private bool detectJustType(string content, out int endPos)
    {
        // > < ^ V w
        if (content.Contains('>'))
        {
            var slidePart = content.Split('>');
            endPos = parseSlideAnchor(slidePart[1]);
            var startPos = parseSlideAnchor(slidePart[0]);
            // Handle touch slide end in C
            if (slidePart[1].StartsWith("C")) endPos = parseSlideAnchor(slidePart[0]);
            if (isUpperHalf(startPos))
                return true;
            return false;
        }

        if (content.Contains('<'))
        {
            var slidePart = content.Split('<');
            endPos = parseSlideAnchor(slidePart[1]);
            var startPos = parseSlideAnchor(slidePart[0]);
            // Handle touch slide end in C
            if (slidePart[1].StartsWith("C")) endPos = parseSlideAnchor(slidePart[0]);
            if (!isUpperHalf(startPos))
                return true;
            return false;
        }

        if (content.Contains('^'))
        {
            var str = content.Substring(0, 3);
            var digits = str.Split('^');
            var startPos = int.Parse(digits[0]);
            endPos = int.Parse(digits[1]);
            endPos = endPos - startPos;
            endPos = endPos < 0 ? endPos + 8 : endPos;
            endPos = endPos > 8 ? endPos - 8 : endPos;

            if (endPos < 4)
            {
                endPos = int.Parse(digits[1]);
                return true;
            }
            if (endPos > 4)
            {
                endPos = int.Parse(digits[1]);
                return false;
            }
        }
        else if (content.Contains('V'))
        {
            var str = content.Substring(0, 4);
            var digits = str.Split('V');
            endPos = int.Parse(digits[1][1].ToString());

            if (isRightHalf(endPos))
                return true;
            return false;
        }
        else if (content.Contains('w'))
        {
            var str = content.Substring(0, 3);
            endPos = int.Parse(str.Substring(2, 1));
            if (isUpperHalf(endPos))
                return true;
            return false;
        }
        else
        {
            var slidePart = toSlidePart(content);
            endPos = parseSlideAnchor(slidePart[1]);
            // Handle touch slide end in C
            if (slidePart[1].StartsWith("C")) endPos = parseSlideAnchor(slidePart[0]);
            if (isRightHalf(endPos))
                return true;
            return false;
        }
        return true;
    }

    private string[] toSlidePart(string rawContent)
    {
        // Must have pp and qq before p and q
        var separators = new string[] { "pp", "qq", "-", "v", "s", "z", "p", "q" };
        foreach (var separator in separators)
        {
            if (rawContent.Contains(separator))
            {
                return rawContent.Split(separator);
            }
        }
        throw new InvalidOperationException($"Unable to get end position of {rawContent}");
    }

    private string detectShapeFromText(string content)
    {
        int getRelativeEndPos(int startPos, int endPos)
        {
            endPos = endPos - startPos;
            endPos = endPos < 0 ? endPos + 8 : endPos;
            endPos = endPos > 8 ? endPos - 8 : endPos;
            return endPos + 1;
        }

        char toDictName(char c)
        {
            if (c >= '1' && c <= '8') return '1';
            return c;
        }

        //print(content);
        if (content.Contains('-'))
        {
            // line
            var str = content.Split('[')[0]; // Length can now varied anywhere from 3 to 5, get substring until the first '[', if any
            var digits = str.Split('-');
            var startPos = parseSlideAnchor(digits[0]);
            var endPos = parseSlideAnchor(digits[1]);
            endPos = getRelativeEndPos(startPos, endPos);

            // If either start or end is in C position, single prefab for all 8 direction
            if (digits[0][0] == 'C' || digits[1][0] == 'C')
            {
                return $"{toDictName(digits[0][0])}{toDictName(digits[1][0])}_Line";
            }

            // If either start or end is non C touch
            if (isTouch(digits[0][0]) || isTouch(digits[1][0]))
            {
                return $"{toDictName(digits[0][0])}{toDictName(digits[1][0])}_Line_{endPos}";
            }

            // Normal slide logic
            if (endPos < 3 || endPos > 7) throw new Exception("-星星至少隔开一键\n-スライドエラー");
            return "line" + endPos;
        }

        if (content.Contains('>'))
        {
            // circle 默认顺时针
            var str = content.Split('[')[0];
            var digits = str.Split('>');
            var startPos = parseSlideAnchor(digits[0]);
            var endPos = parseSlideAnchor(digits[1]);
            endPos = getRelativeEndPos(startPos, endPos);

            if (digits[0][0] == 'C')
            {
                return $"{toDictName(digits[0][0])}{toDictName(digits[1][0])}_Circle_{endPos}";
            }

            if (digits[1][0] == 'C')
            {
                if (isUpperHalf(startPos))
                    return $"*{toDictName(digits[0][0])}{toDictName(digits[1][0])}_Circle_{startPos}";
                startPos = MirrorUDKeys(startPos);
                return $"*^{toDictName(digits[0][0])}{toDictName(digits[1][0])}_Circle_{startPos}";
            }

            if (isUpperHalf(startPos))
            {
                if (isTouch(digits[0][0]) || isTouch(digits[1][0]))
                {
                    return $"{toDictName(digits[0][0])}{toDictName(digits[1][0])}_Circle_{endPos}";
                }
                return "circle" + endPos;
            }

            endPos = MirrorKeys(endPos);
            // Because slide was generated for > and then mirrored for <, if slide ending in D or E, due to their offset from the line 1-5, they are off by one
            if (digits[1][0] is 'D' or 'E')
                endPos = endPos % 8 + 1;
            if (isTouch(digits[0][0]) || isTouch(digits[1][0]))
            {
                return $"-{toDictName(digits[0][0])}{toDictName(digits[1][0])}_Circle_{endPos}";
            }
            return "-circle" + endPos; //Mirror
        }

        if (content.Contains('<'))
        {
            // circle 默认顺时针
            var str = content.Split('[')[0];
            var digits = str.Split('<');
            var startPos = parseSlideAnchor(digits[0]);
            var endPos = parseSlideAnchor(digits[1]);
            endPos = getRelativeEndPos(startPos, endPos);

            if (digits[0][0] == 'C')
            {
                endPos = MirrorUDKeys(endPos);
                return $"^{toDictName(digits[0][0])}{toDictName(digits[1][0])}_Circle_{endPos}";
            }

            if (digits[1][0] == 'C')
            {
                if (!isUpperHalf(startPos))
                    return $"*{toDictName(digits[0][0])}{toDictName(digits[1][0])}_Circle_{startPos}";
                startPos = MirrorUDKeys(startPos);
                return $"*^{toDictName(digits[0][0])}{toDictName(digits[1][0])}_Circle_{startPos}";
            }

            // Otherwise, follow the mirroring logic
            if (!isUpperHalf(startPos))
            {
                if (isTouch(digits[0][0]) || isTouch(digits[1][0]))
                {
                    return $"{toDictName(digits[0][0])}{toDictName(digits[1][0])}_Circle_{endPos}";
                }
                return "circle" + endPos;
            }

            endPos = MirrorKeys(endPos);
            // Because slide was generated for > and then mirrored for <, if slide ending in D or E, due to their offset from the line 1-5, they are off by one
            if (digits[1][0] is 'D' or 'E')
                endPos = endPos % 8 + 1;
            if (isTouch(digits[0][0]) || isTouch(digits[1][0]))
            {
                return $"-{toDictName(digits[0][0])}{toDictName(digits[1][0])}_Circle_{endPos}";
            }
            return "-circle" + endPos; //Mirror
        }

        if (content.Contains('^'))
        {
            var str = content.Split('[')[0];
            if(str.Length > 3)
            {
                throw new Exception("Currently doesn't support touch slide with ^");
            }
            var digits = str.Split('^');
            var startPos = int.Parse(digits[0]);
            var endPos = int.Parse(digits[1]);
            endPos = getRelativeEndPos(startPos, endPos);

            if (endPos == 1 || endPos == 5)
            {
                throw new Exception("^星星不合法\n^スライドエラー");
            }

            if (endPos < 5)
            {
                return "circle" + endPos;
            }
            if (endPos > 5)
            {
                return "-circle" + MirrorKeys(endPos);
            }
        }

        if (content.Contains('v'))
        {
            // v
            var str = content.Substring(0, 3);
            var digits = str.Split('v');
            var startPos = int.Parse(digits[0]);
            var endPos = int.Parse(digits[1]);
            endPos = getRelativeEndPos(startPos, endPos);
            if (endPos == 5) throw new Exception("v星星不合法\nvスライドエラー");
            return "v" + endPos;
        }

        if (content.Contains("pp"))
        {
            // ppqq 默认为pp
            var str = content.Substring(0, 4);
            var digits = str.Split('p');
            var startPos = int.Parse(digits[0]);
            var endPos = int.Parse(digits[2]);
            endPos = getRelativeEndPos(startPos, endPos);
            return "ppqq" + endPos;
        }

        if (content.Contains("qq"))
        {
            // ppqq 默认为pp
            var str = content.Substring(0, 4);
            var digits = str.Split('q');
            var startPos = int.Parse(digits[0]);
            var endPos = int.Parse(digits[2]);
            endPos = getRelativeEndPos(startPos, endPos);
            endPos = MirrorKeys(endPos);
            return "-ppqq" + endPos;
        }

        if (content.Contains('p'))
        {
            // pq 默认为p
            var str = content.Substring(0, 3);
            var digits = str.Split('p');
            var startPos = int.Parse(digits[0]);
            var endPos = int.Parse(digits[1]);
            endPos = getRelativeEndPos(startPos, endPos);
            return "pq" + endPos;
        }

        if (content.Contains('q'))
        {
            // pq 默认为p
            var str = content.Substring(0, 3);
            var digits = str.Split('q');
            var startPos = int.Parse(digits[0]);
            var endPos = int.Parse(digits[1]);
            endPos = getRelativeEndPos(startPos, endPos);
            endPos = MirrorKeys(endPos);
            return "-pq" + endPos;
        }

        if (content.Contains('s'))
        {
            // s
            var str = content.Substring(0, 3);
            var digits = str.Split('s');
            var startPos = int.Parse(digits[0]);
            var endPos = int.Parse(digits[1]);
            endPos = getRelativeEndPos(startPos, endPos);
            if (endPos != 5) throw new Exception("s星星尾部错误\nsスライドエラー");
            return "s";
        }

        if (content.Contains('z'))
        {
            // s镜像
            var str = content.Substring(0, 3);
            var digits = str.Split('z');
            var startPos = int.Parse(digits[0]);
            var endPos = int.Parse(digits[1]);
            endPos = getRelativeEndPos(startPos, endPos);
            if (endPos != 5) throw new Exception("z星星尾部错误\nzスライドエラー");
            return "-s";
        }

        if (content.Contains('V'))
        {
            // L
            var str = content.Substring(0, 4);
            var digits = str.Split('V');
            var startPos = int.Parse(digits[0]);
            var turnPos = int.Parse(digits[1][0].ToString());
            var endPos = int.Parse(digits[1][1].ToString());

            turnPos = getRelativeEndPos(startPos, turnPos);
            endPos = getRelativeEndPos(startPos, endPos);
            if (turnPos == 7)
            {
                if (endPos < 2 || endPos > 5) throw new Exception("V星星终点不合法\nVスライドエラー");
                return "L" + endPos;
            }

            if (turnPos == 3)
            {
                if (endPos < 5) throw new Exception("V星星终点不合法\nVスライドエラー");
                return "-L" + MirrorKeys(endPos);
            }

            throw new Exception("V星星拐点只能隔开一键\nVスライドエラー");
        }

        if (content.Contains('w'))
        {
            // wifi
            var str = content.Substring(0, 3);
            var digits = str.Split('w');
            var startPos = int.Parse(digits[0]);
            var endPos = int.Parse(digits[1]);
            endPos = getRelativeEndPos(startPos, endPos);
            if (endPos != 5) throw new Exception("w星星尾部错误\nwスライドエラー");
            return "wifi";
        }

        return "";
    }

    private bool isUpperHalf(int key)
    {
        if (key == 7) return true;
        if (key == 8) return true;
        if (key == 1) return true;
        if (key == 2) return true;

        return false;
    }

    private bool isRightHalf(int key)
    {
        if (key == 1) return true;
        if (key == 2) return true;
        if (key == 3) return true;
        if (key == 4) return true;

        return false;
    }

    private int MirrorKeys(int key)
    {
        if (key == 1) return 1;
        if (key == 2) return 8;
        if (key == 3) return 7;
        if (key == 4) return 6;

        if (key == 5) return 5;
        if (key == 6) return 4;
        if (key == 7) return 3;
        if (key == 8) return 2;
        throw new Exception("Keys out of range: " + key);
    }

    private int MirrorUDKeys(int key)
    {
        if (key == 1) return 4;
        if (key == 2) return 3;
        if (key == 3) return 2;
        if (key == 4) return 1;

        if (key == 5) return 8;
        if (key == 6) return 7;
        if (key == 7) return 6;
        if (key == 8) return 5;
        throw new Exception("Keys out of range: " + key);
    }


    #region Small helpers
    private int charIntParse(char c)
    {
        return c - '0';
    }

    private int parseSlideAnchor(string anchor)
    {
        if (anchor.StartsWith('C')) return 1;
        if (anchor.Length == 1) return charIntParse(anchor[0]);
        return charIntParse(anchor[1]);
    }

    private bool isTouch(char c)
    {
        return (c >= 'A' && c <= 'E');
    }

    private bool isNonCTouchArea(char c)
    {
        return c != 'C' && isTouch(c);
    }

    private bool isSupportedSlideStart(char c)
    {
        return char.IsNumber(c) || isTouch(c);
    }
    #endregion Small helpers
}