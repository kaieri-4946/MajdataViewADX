#nullable enable

#region

using System;
using System.Linq;
using UnityEngine;

#endregion

public class SlideArea
{
    public SensorType[] Areas { get; init; }
    public int ArrowProgressWhenOn { get; init; }
    public int ArrowProgressWhenFinished { get; init; }
    public bool IsSkippable { get; set; }
    public bool IsLast { get; set; }

    public bool On { get; set; }
    public bool Off { get; set; }
    public bool IsFinished
    {
        get
        {
            if (IsLast)
                return On;

            return On && Off;
        }
    }

    public void SetIsLast() => IsLast = true;
    public void SetNonLast() => IsLast = false;

    public SlideArea Clone()
    {
        return new SlideArea
        {
            Areas = (SensorType[])Areas.Clone(),
            ArrowProgressWhenOn = ArrowProgressWhenOn,
            ArrowProgressWhenFinished = ArrowProgressWhenFinished,
            IsSkippable = IsSkippable,
            IsLast = IsLast,
            On = false,
            Off = false
        };
    }

    public void Mirror(SensorType baseLine)
    {
        for (var i = 0; i < Areas.Length; i++)
        {
            Areas[i] = Areas[i].Mirror(baseLine);
        }
    }

    public void Diff(int diff)
    {
        for (var i = 0; i < Areas.Length; i++)
        {
            Areas[i] = Areas[i].Diff(diff);
        }
    }

    public void Judge(bool status)
    {
        if (status)
        {
            On = true;
        }
        else
        {
            if (On)
            {
                Off = true;
            }
        }
    }
}

public class SlideTable
{
    public string Name { get; init; } = string.Empty;
    public SlideArea[] JudgeQueue { get; init; } = Array.Empty<SlideArea>();
    public float Const { get; init; } = 0f;

    public SlideTable Clone()
    {
        return new SlideTable()
        {
            Name = Name,
            JudgeQueue = JudgeQueue.Select(area => area.Clone()).ToArray(),
            Const = Const
        };
    }

    public void Mirror(SensorType baseLine)
    {
        foreach (var area in JudgeQueue)
        {
            area.Mirror(baseLine);
        }
    }

    public void Diff(int diff)
    {
        foreach (var area in JudgeQueue)
        {
            area.Diff(diff);
        }
    }
}

public class WifiTable
{
    public string Name { get; init; } = string.Empty;
    public SlideArea[] Left { get; init; } = Array.Empty<SlideArea>();
    public SlideArea[] Center { get; init; } = Array.Empty<SlideArea>();
    public SlideArea[] Right { get; init; } = Array.Empty<SlideArea>();
    public float Const { get; init; } = 0f;
    public WifiTable Clone()
    {
        return new WifiTable()
        {
            Name = Name,
            Left = Left.Select(area => area.Clone()).ToArray(),
            Center = Center.Select(area => area.Clone()).ToArray(),
            Right = Right.Select(area => area.Clone()).ToArray(),
            Const = Const
        };
    }
    public void Diff(int diff)
    {
        foreach (var area in Left) area.Diff(diff);
        foreach (var area in Center) area.Diff(diff);
        foreach (var area in Right) area.Diff(diff);
    }
}

public static class SlideTables
{
    static readonly SlideTable[] SLIDE_TABLES = new SlideTable[]
    {
        new SlideTable()
        {
            Name = "circle2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 3, false),
                BuildSlideArea(SensorType.A2, 5, 7, true, true)
            },
            Const = 0.465f
        },
        new SlideTable()
        {
            Name = "circle3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 3),
                BuildSlideArea(SensorType.A2, 7, 11, false),
                BuildSlideArea(SensorType.A3, 13, 15, true, true)
            },
            Const = 0.233f
        },
        new SlideTable()
        {
            Name = "circle4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 3),
                BuildSlideArea(SensorType.A2, 7, 11),
                BuildSlideArea(SensorType.A3, 14, 19),
                BuildSlideArea(SensorType.A4, 21, 23, true, true)
            },
            Const = 0.155f
        },
        new SlideTable()
        {
            Name = "circle5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 3),
                BuildSlideArea(SensorType.A2, 7, 11),
                BuildSlideArea(SensorType.A3, 14, 19),
                BuildSlideArea(SensorType.A4, 23, 27),
                BuildSlideArea(SensorType.A5, 29, 31, true, true)
            },
            Const = 0.116f
        },
        new SlideTable()
        {
            Name = "circle6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 3),
                BuildSlideArea(SensorType.A2, 7, 11),
                BuildSlideArea(SensorType.A3, 14, 19),
                BuildSlideArea(SensorType.A4, 23, 27),
                BuildSlideArea(SensorType.A5, 31, 35),
                BuildSlideArea(SensorType.A6, 37, 39, true, true)
            },
            Const = 0.093f
        },
        new SlideTable()
        {
            Name = "circle7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 3),
                BuildSlideArea(SensorType.A2, 7, 11),
                BuildSlideArea(SensorType.A3, 14, 19),
                BuildSlideArea(SensorType.A4, 23, 27),
                BuildSlideArea(SensorType.A5, 31, 35),
                BuildSlideArea(SensorType.A6, 39, 43),
                BuildSlideArea(SensorType.A7, 45, 47, true, true)
            },
            Const = 0.078f
        },
        new SlideTable()
        {
            Name = "circle8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 3),
                BuildSlideArea(SensorType.A2, 7, 11),
                BuildSlideArea(SensorType.A3, 14, 19),
                BuildSlideArea(SensorType.A4, 23, 27),
                BuildSlideArea(SensorType.A5, 31, 35),
                BuildSlideArea(SensorType.A6, 39, 43),
                BuildSlideArea(SensorType.A7, 46, 51),
                BuildSlideArea(SensorType.A8, 53, 55, true, true)
            },
            Const = 0.066f
        },
        new SlideTable()
        {
            Name = "circle1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 3),
                BuildSlideArea(SensorType.A2, 7, 11),
                BuildSlideArea(SensorType.A3, 14, 19),
                BuildSlideArea(SensorType.A4, 23, 27),
                BuildSlideArea(SensorType.A5, 31, 35),
                BuildSlideArea(SensorType.A6, 39, 43),
                BuildSlideArea(SensorType.A7, 46, 51),
                BuildSlideArea(SensorType.A8, 54, 59),
                BuildSlideArea(SensorType.A1, 61, 63, true, true)
            },
            Const = 0.058f
        },
        new SlideTable()
        {
            Name = "line3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 3),
                BuildSlideArea(new SensorType[] { SensorType.A2, SensorType.B2 }, 6, 9, false),
                BuildSlideArea(SensorType.A3, 10, 13, true, true)
            },
            Const = 0.182f
        },
        new SlideTable()
        {
            Name = "line4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 4),
                BuildSlideArea(SensorType.B2, 6, 9),
                BuildSlideArea(SensorType.B3, 11, 14),
                BuildSlideArea(SensorType.A4, 15, 18, true, true)
            },
            Const = 0.19f
        },
        new SlideTable()
        {
            Name = "line5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 4),
                BuildSlideArea(SensorType.B1, 5, 7),
                BuildSlideArea(SensorType.C, 10, 12),
                BuildSlideArea(SensorType.B5, 13, 16),
                BuildSlideArea(SensorType.A5, 17, 19, true, true)
            },
            Const = 0.152f
        },
        new SlideTable()
        {
            Name = "line6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 4),
                BuildSlideArea(SensorType.B8, 6, 9),
                BuildSlideArea(SensorType.B7, 11, 14),
                BuildSlideArea(SensorType.A6, 15, 18, true, true)
            },
            Const = 0.19f
        },
        new SlideTable()
        {
            Name = "line7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 3),
                BuildSlideArea(new SensorType[] { SensorType.A8, SensorType.B8 }, 6, 9, false),
                BuildSlideArea(SensorType.A7, 10, 13, true, true)
            },
            Const = 0.182f
        },
        new SlideTable()
        {
            Name = "v1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 3),
                BuildSlideArea(SensorType.B1, 4, 7),
                BuildSlideArea(SensorType.C, 8, 13),
                BuildSlideArea(SensorType.B1, 14, 16),
                BuildSlideArea(SensorType.A1, 17, 19, true, true)
            },
            Const = 0.185f
        },
        new SlideTable()
        {
            Name = "v2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 3),
                BuildSlideArea(SensorType.B1, 4, 7),
                BuildSlideArea(SensorType.C, 8, 13),
                BuildSlideArea(SensorType.B2, 14, 16),
                BuildSlideArea(SensorType.A2, 17, 19, true, true)
            },
            Const = 0.15f
        },
        new SlideTable()
        {
            Name = "v3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 3),
                BuildSlideArea(SensorType.B1, 4, 7),
                BuildSlideArea(SensorType.C, 8, 13),
                BuildSlideArea(SensorType.B3, 14, 16),
                BuildSlideArea(SensorType.A3, 17, 19, true, true)
            },
            Const = 0.158f
        },
        new SlideTable()
        {
            Name = "v4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 3),
                BuildSlideArea(SensorType.B1, 4, 7),
                BuildSlideArea(SensorType.C, 8, 13),
                BuildSlideArea(SensorType.B4, 14, 16),
                BuildSlideArea(SensorType.A4, 17, 19, true, true)
            },
            Const = 0.158f
        },
        new SlideTable()
        {
            Name = "v6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 3),
                BuildSlideArea(SensorType.B1, 4, 7),
                BuildSlideArea(SensorType.C, 8, 13),
                BuildSlideArea(SensorType.B6, 14, 16),
                BuildSlideArea(SensorType.A6, 17, 19, true, true)
            },
            Const = 0.158f
        },
        new SlideTable()
        {
            Name = "v7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 3),
                BuildSlideArea(SensorType.B1, 4, 7),
                BuildSlideArea(SensorType.C, 8, 13),
                BuildSlideArea(SensorType.B7, 14, 16),
                BuildSlideArea(SensorType.A7, 17, 19, true, true)
            },
            Const = 0.158f
        },
        new SlideTable()
        {
            Name = "v8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 3),
                BuildSlideArea(SensorType.B1, 4, 7),
                BuildSlideArea(SensorType.C, 8, 13),
                BuildSlideArea(SensorType.B8, 14, 16),
                BuildSlideArea(SensorType.A8, 17, 19, true, true)
            },
            Const = 0.154f
        },
        new SlideTable()
        {
            Name = "ppqq1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 3),
                BuildSlideArea(SensorType.B1, 5, 7),
                BuildSlideArea(SensorType.C, 10, 13),
                BuildSlideArea(SensorType.B4, 15, 17),
                BuildSlideArea(SensorType.A3, 21, 26),
                BuildSlideArea(SensorType.A2, 29, 32),
                BuildSlideArea(SensorType.A1, 33, 35, true, true)
            },
            Const = 0.065f

        },
        new SlideTable()
        {
            Name = "ppqq2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 3),
                BuildSlideArea(SensorType.B1, 5, 7),
                BuildSlideArea(SensorType.C, 9, 13),
                BuildSlideArea(SensorType.B4, 14, 17),
                BuildSlideArea(SensorType.A3, 20, 25),
                BuildSlideArea(SensorType.A2, 26, 28, true, true),
            },
            Const = 0.086f
        },
        new SlideTable()
        {
            Name = "ppqq3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 3),
                BuildSlideArea(SensorType.B1, 4, 7),
                BuildSlideArea(SensorType.C, 9, 13),
                BuildSlideArea(SensorType.B4, 14, 17),
                BuildSlideArea(SensorType.A3, 19, 22, true, true),
            },
            Const = 0.157f
        },
        new SlideTable()
        {
            Name = "ppqq4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 3),
                BuildSlideArea(SensorType.B1, 5, 7),
                BuildSlideArea(SensorType.C, 9, 13),
                BuildSlideArea(SensorType.B4, 14, 17),
                BuildSlideArea(SensorType.A3, 20, 25),
                BuildSlideArea(SensorType.A2, 28, 33),
                BuildSlideArea(SensorType.B1, 34, 37),
                BuildSlideArea(SensorType.C, 39, 43),
                BuildSlideArea(SensorType.B4, 44, 46),
                BuildSlideArea(SensorType.A4, 47, 49, true, true),
            },
            Const = 0.065f
        },
        new SlideTable()
        {
            Name = "ppqq5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 3),
                BuildSlideArea(SensorType.B1, 5, 7),
                BuildSlideArea(SensorType.C, 9, 13),
                BuildSlideArea(SensorType.B4, 14, 17),
                BuildSlideArea(SensorType.A3, 20, 25),
                BuildSlideArea(SensorType.A2, 28, 33),
                BuildSlideArea(SensorType.B1, 34, 37),
                BuildSlideArea(SensorType.C, 39, 43),
                BuildSlideArea(SensorType.B5, 44, 46),
                BuildSlideArea(SensorType.A5, 47, 49, true, true),
            },
            Const = 0.065f
        },
        new SlideTable()
        {
            Name = "ppqq6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 3),
                BuildSlideArea(SensorType.B1, 5, 7),
                BuildSlideArea(SensorType.C, 9, 13),
                BuildSlideArea(SensorType.B4, 14, 17),
                BuildSlideArea(SensorType.A3, 20, 25),
                BuildSlideArea(SensorType.A2, 28, 33),
                BuildSlideArea(SensorType.B1, 34, 37),
                BuildSlideArea(new SensorType[] { SensorType.C, SensorType.B8 }, 38, 40),
                BuildSlideArea(new SensorType[] { SensorType.B7, SensorType.B6 }, 42, 44),
                BuildSlideArea(SensorType.A6, 46, 48, true, true),
            },
            Const = 0.067f
        },
        new SlideTable()
        {
            Name = "ppqq7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 3),
                BuildSlideArea(SensorType.B1, 5, 7),
                BuildSlideArea(SensorType.C, 9, 13),
                BuildSlideArea(SensorType.B4, 14, 17),
                BuildSlideArea(SensorType.A3, 20, 25),
                BuildSlideArea(SensorType.A2, 28, 33),
                BuildSlideArea(SensorType.B1, 34, 37),
                BuildSlideArea(SensorType.B8, 38, 42),
                BuildSlideArea(SensorType.A7, 43, 46, true, true),
            },
            Const = 0.079f
        },
        new SlideTable()
        {
            Name = "ppqq8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 3),
                BuildSlideArea(SensorType.B1, 5, 7),
                BuildSlideArea(SensorType.C, 9, 13),
                BuildSlideArea(SensorType.B4, 14, 17),
                BuildSlideArea(SensorType.A3, 20, 25),
                BuildSlideArea(SensorType.A2, 28, 33),
                BuildSlideArea(new SensorType[] { SensorType.B1, SensorType.A1 }, 35, 37),
                BuildSlideArea(SensorType.A8, 38, 41, true, true),
            },
            Const = 0.0626f
        },
        new SlideTable()
        {
            Name = "L2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 3),
                BuildSlideArea(new SensorType[] { SensorType.B8, SensorType.A8 }, 6, 10, false),
                BuildSlideArea(SensorType.A7, 12, 19),
                BuildSlideArea(SensorType.B8, 21, 24),
                BuildSlideArea(SensorType.B1, 25, 28),
                BuildSlideArea(SensorType.A2, 29, 32, true, true),
            },
            Const = 0.1f
        },
        new SlideTable()
        {
            Name = "L3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 3),
                BuildSlideArea(new SensorType[] { SensorType.B8, SensorType.A8 }, 6, 10, false),
                BuildSlideArea(SensorType.A7, 12, 18),
                BuildSlideArea(SensorType.B7, 20, 22),
                BuildSlideArea(SensorType.C, 25, 27),
                BuildSlideArea(SensorType.B3, 28, 31),
                BuildSlideArea(SensorType.A3, 32, 34, true, true),
            },
            Const = 0.104f
        },
        new SlideTable()
        {
            Name = "L4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 3),
                BuildSlideArea(new SensorType[] { SensorType.B8, SensorType.A8 }, 6, 10, false),
                BuildSlideArea(SensorType.A7, 12, 19),
                BuildSlideArea(SensorType.B6, 21, 24),
                BuildSlideArea(SensorType.B5, 25, 28),
                BuildSlideArea(SensorType.A4, 29, 32, true, true),
            },
            Const = 0.098f
        },
        new SlideTable()
        {
            Name = "L5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 3),
                BuildSlideArea(new SensorType[] { SensorType.B8, SensorType.A8 }, 6, 10, false),
                BuildSlideArea(SensorType.A7, 12, 18),
                BuildSlideArea(new SensorType[] { SensorType.B6, SensorType.A6 }, 21, 24, false),
                BuildSlideArea(SensorType.A5, 27, 28, true, true),
            },
            Const = 0.105f
        },
        new SlideTable()
        {
            Name = "s",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 4),
                BuildSlideArea(SensorType.B8, 7, 9),
                BuildSlideArea(SensorType.B7, 10, 12),
                BuildSlideArea(SensorType.C, 14, 17),
                BuildSlideArea(SensorType.B3, 19, 21),
                BuildSlideArea(SensorType.B4, 22, 25),
                BuildSlideArea(SensorType.A5, 27, 30, true, true),
            },
            Const = 0.13f
        },
        new SlideTable()
        {
            Name = "pq1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 4),
                BuildSlideArea(SensorType.B8, 5, 8),
                BuildSlideArea(SensorType.B7, 9, 11),
                BuildSlideArea(SensorType.B6, 12, 14),
                BuildSlideArea(SensorType.B5, 15, 17),
                BuildSlideArea(SensorType.B4, 19, 21),
                BuildSlideArea(SensorType.B3, 22, 24),
                BuildSlideArea(SensorType.B2, 25, 29),
                BuildSlideArea(SensorType.A1, 30, 33, true, true),
            },
            Const = 0.095f
        },
        new SlideTable()
        {
            Name = "pq2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 4),
                BuildSlideArea(SensorType.B8, 5, 8),
                BuildSlideArea(SensorType.B7, 9, 11),
                BuildSlideArea(SensorType.B6, 12, 14),
                BuildSlideArea(SensorType.B5, 16, 18),
                BuildSlideArea(SensorType.B4, 19, 21),
                BuildSlideArea(SensorType.B3, 22, 26),
                BuildSlideArea(SensorType.A2, 27, 30, true, true),
            },
            Const = 0.112f
        },
        new SlideTable()
        {
            Name = "pq3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 4),
                BuildSlideArea(SensorType.B8, 5, 8),
                BuildSlideArea(SensorType.B7, 9, 11),
                BuildSlideArea(SensorType.B6, 12, 14),
                BuildSlideArea(SensorType.B5, 16, 18),
                BuildSlideArea(SensorType.B4, 20, 23),
                BuildSlideArea(SensorType.A3, 25, 27, true, true),
            },
            Const = 0.125f
        },
        new SlideTable()
        {
            Name = "pq4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 4),
                BuildSlideArea(SensorType.B8, 5, 8),
                BuildSlideArea(SensorType.B7, 9, 11),
                BuildSlideArea(SensorType.B6, 12, 14),
                BuildSlideArea(SensorType.B5, 16, 20),
                BuildSlideArea(SensorType.A4, 22, 24, true, true),
            },
            Const = 0.139f
        },
        new SlideTable()
        {
            Name = "pq5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 4),
                BuildSlideArea(SensorType.B8, 5, 8),
                BuildSlideArea(SensorType.B7, 9, 12),
                BuildSlideArea(SensorType.B6, 14, 17),
                BuildSlideArea(SensorType.A5, 19, 21, true, true),
            },
            Const = 0.160f
        },
        new SlideTable()
        {
            Name = "pq6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 4),
                BuildSlideArea(SensorType.B8, 5, 8),
                BuildSlideArea(SensorType.B7, 9, 11),
                BuildSlideArea(SensorType.B6, 13, 15),
                BuildSlideArea(SensorType.B5, 16, 18),
                BuildSlideArea(SensorType.B4, 19, 21),
                BuildSlideArea(SensorType.B3, 22, 24),
                BuildSlideArea(SensorType.B2, 25, 27),
                BuildSlideArea(SensorType.B1, 28, 30),
                BuildSlideArea(SensorType.B8, 31, 33),
                BuildSlideArea(SensorType.B7, 35, 38),
                BuildSlideArea(SensorType.A6, 40, 42, true, true),
            },
            Const = 0.080f
        },
        new SlideTable()
        {
            Name = "pq7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 4),
                BuildSlideArea(SensorType.B8, 7, 9),
                BuildSlideArea(SensorType.B7, 10, 12),
                BuildSlideArea(SensorType.B6, 13, 15),
                BuildSlideArea(SensorType.B5, 16, 18),
                BuildSlideArea(SensorType.B4, 20, 22),
                BuildSlideArea(SensorType.B3, 23, 25),
                BuildSlideArea(SensorType.B2, 26, 28),
                BuildSlideArea(SensorType.B1, 30, 32),
                BuildSlideArea(SensorType.B8, 33, 36),
                BuildSlideArea(SensorType.A7, 37, 40, true, true),
            },
            Const = 0.084f
        },
        new SlideTable()
        {
            Name = "pq8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 4),
                BuildSlideArea(SensorType.B8, 5, 8),
                BuildSlideArea(SensorType.B7, 9, 11),
                BuildSlideArea(SensorType.B6, 12, 14),
                BuildSlideArea(SensorType.B5, 15, 17),
                BuildSlideArea(SensorType.B4, 19, 21),
                BuildSlideArea(SensorType.B3, 22, 24),
                BuildSlideArea(SensorType.B2, 25, 27),
                BuildSlideArea(SensorType.B1, 28, 32),
                BuildSlideArea(SensorType.A8, 33, 36, true, true),
            },
            Const = 0.0895f
        },
    };

    static readonly WifiTable WIFI_TABLE = new WifiTable
    {
        Name = "wifi",
        Left = new SlideArea[] // L
        {
            BuildSlideArea(SensorType.A1,0),
            BuildSlideArea(SensorType.B8,2),
            BuildSlideArea(SensorType.B7,4),
            BuildSlideArea(new SensorType[] { SensorType.A6 , SensorType.D6 },7,true,true)
        },
        Center = new SlideArea[] // Center
        {
            BuildSlideArea(SensorType.A1,0),
            BuildSlideArea(SensorType.B1,2),
            BuildSlideArea(SensorType.C,4),
            BuildSlideArea(new SensorType[] { SensorType.A5 , SensorType.B5 },7,true,true)
        },
        Right = new SlideArea[] // R
        {
            BuildSlideArea(SensorType.A1,0),
            BuildSlideArea(SensorType.B2,2),
            BuildSlideArea(SensorType.B3,4),
            BuildSlideArea(new SensorType[] { SensorType.A4 , SensorType.D5 },7,true,true)
        },
        Const = 0.162870f
    };

    public static SlideTable? FindTableByName(string prefabName)
    {
        //Temp
        var a = Array.Find(SLIDE_TABLES, x => x.Name == prefabName)?.Clone();
        if (a is null)
        {
            return new SlideTable()
            {
                Name = prefabName,
                JudgeQueue = new SlideArea[]
                {
                BuildSlideArea(SensorType.A1, 0, 4),
                BuildSlideArea(SensorType.B8, 5, 8),
                BuildSlideArea(SensorType.B7, 9, 11, true, true),
                },
                Const = 0.0895f
            };
        }
        return a;
    }

    public static WifiTable GetWifiTable(int startPos)
    {
        var table = WIFI_TABLE.Clone();
        var diff = Math.Abs(1 - startPos);
        if (diff != 0)
        {
            table.Diff(diff);
        }
        return table;
    }

    static SlideArea BuildSlideArea(SensorType type, int arrowProgress,
        bool isSkippable = true, bool isLast = false)
    {
        return new SlideArea()
        {
            Areas = new[] { type },
            ArrowProgressWhenOn = arrowProgress,
            ArrowProgressWhenFinished = arrowProgress,
            IsSkippable = isSkippable,
            IsLast = isLast
        };
    }

    static SlideArea BuildSlideArea(SensorType type, int progressWhenOn, int progressWhenFinished,
        bool isSkippable = true, bool isLast = false)
    {
        return new SlideArea()
        {
            Areas = new[] { type },
            ArrowProgressWhenOn = progressWhenOn,
            ArrowProgressWhenFinished = progressWhenFinished,
            IsSkippable = isSkippable,
            IsLast = isLast
        };
    }

    static SlideArea BuildSlideArea(SensorType[] type, int arrowProgress,
        bool isSkippable = true, bool isLast = false)
    {
        return new SlideArea()
        {
            Areas = type,
            ArrowProgressWhenOn = arrowProgress,
            ArrowProgressWhenFinished = arrowProgress,
            IsSkippable = isSkippable,
            IsLast = isLast
        };
    }

    static SlideArea BuildSlideArea(SensorType[] type, int progressWhenOn,
        int progressWhenFinished, bool isSkippable = true, bool isLast = false)
    {
        return new SlideArea()
        {
            Areas = type,
            ArrowProgressWhenOn = progressWhenOn,
            ArrowProgressWhenFinished = progressWhenFinished,
            IsSkippable = isSkippable,
            IsLast = isLast
        };
    }
}