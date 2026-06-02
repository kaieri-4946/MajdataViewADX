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
        new SlideTable()
        {
            Name = "1D_PPQQ_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 4),
                BuildSlideArea(SensorType.C, 5, 8),
                BuildSlideArea(SensorType.B4, 9, 14),
                BuildSlideArea(SensorType.E4, 15, 17),
                BuildSlideArea(SensorType.A3, 18, 20),
                BuildSlideArea(SensorType.D3, 21, 24),
                BuildSlideArea(SensorType.A2, 25, 28),
                BuildSlideArea(SensorType.D2, 29, 32),
                BuildSlideArea(SensorType.A1, 33, 35),
                BuildSlideArea(SensorType.D1, 36, 42, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1D_PPQQ_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 4),
                BuildSlideArea(SensorType.C, 5, 8),
                BuildSlideArea(SensorType.B4, 9, 14),
                BuildSlideArea(SensorType.E4, 15, 17),
                BuildSlideArea(SensorType.A3, 18, 20),
                BuildSlideArea(SensorType.D3, 21, 23),
                BuildSlideArea(SensorType.A2, 24, 27),
                BuildSlideArea(SensorType.D2, 28, 34, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1D_PPQQ_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 4),
                BuildSlideArea(SensorType.C, 5, 7),
                BuildSlideArea(SensorType.B4, 8, 13),
                BuildSlideArea(SensorType.E4, 14, 16),
                BuildSlideArea(SensorType.A3, 17, 20),
                BuildSlideArea(SensorType.D3, 21, 26, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1D_PPQQ_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 4),
                BuildSlideArea(SensorType.C, 5, 8),
                BuildSlideArea(SensorType.B4, 9, 14),
                BuildSlideArea(SensorType.E4, 15, 17),
                BuildSlideArea(SensorType.A3, 18, 20),
                BuildSlideArea(SensorType.D3, 21, 24),
                BuildSlideArea(SensorType.A2, 25, 28),
                BuildSlideArea(SensorType.E2, 29, 33),
                BuildSlideArea(SensorType.B1, 34, 36),
                BuildSlideArea(SensorType.C, 37, 39),
                BuildSlideArea(SensorType.B4, 40, 45),
                BuildSlideArea(SensorType.E4, 46, 49),
                BuildSlideArea(SensorType.D4, 50, 54, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1D_PPQQ_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 4),
                BuildSlideArea(SensorType.C, 5, 8),
                BuildSlideArea(SensorType.B4, 9, 14),
                BuildSlideArea(SensorType.E4, 15, 17),
                BuildSlideArea(SensorType.A3, 18, 21),
                BuildSlideArea(SensorType.D3, 22, 24),
                BuildSlideArea(SensorType.A2, 25, 28),
                BuildSlideArea(SensorType.E2, 29, 33),
                BuildSlideArea(SensorType.B1, 34, 36),
                BuildSlideArea(SensorType.C, 37, 40),
                BuildSlideArea(SensorType.E5, 41, 48),
                BuildSlideArea(SensorType.D5, 49, 54, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1D_PPQQ_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 4),
                BuildSlideArea(SensorType.C, 5, 8),
                BuildSlideArea(SensorType.B4, 9, 14),
                BuildSlideArea(SensorType.E4, 15, 17),
                BuildSlideArea(SensorType.A3, 18, 20),
                BuildSlideArea(SensorType.D3, 21, 24),
                BuildSlideArea(SensorType.A2, 25, 28),
                BuildSlideArea(SensorType.E2, 29, 33),
                BuildSlideArea(SensorType.B1, 34, 36),
                BuildSlideArea(SensorType.C, 37, 39),
                BuildSlideArea(SensorType.B6, 40, 45),
                BuildSlideArea(SensorType.E6, 46, 47),
                BuildSlideArea(SensorType.D6, 48, 53, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1D_PPQQ_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 4),
                BuildSlideArea(SensorType.C, 5, 8),
                BuildSlideArea(SensorType.B4, 9, 14),
                BuildSlideArea(SensorType.E4, 15, 17),
                BuildSlideArea(SensorType.A3, 18, 21),
                BuildSlideArea(SensorType.D3, 22, 24),
                BuildSlideArea(SensorType.A2, 25, 28),
                BuildSlideArea(SensorType.E2, 29, 33),
                BuildSlideArea(SensorType.B1, 34, 36),
                BuildSlideArea(SensorType.B8, 37, 41),
                BuildSlideArea(SensorType.B7, 42, 43),
                BuildSlideArea(SensorType.D7, 44, 52, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1D_PPQQ_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 4),
                BuildSlideArea(SensorType.C, 5, 8),
                BuildSlideArea(SensorType.B4, 9, 14),
                BuildSlideArea(SensorType.E4, 15, 17),
                BuildSlideArea(SensorType.A3, 18, 21),
                BuildSlideArea(SensorType.D3, 22, 24),
                BuildSlideArea(SensorType.A2, 25, 28),
                BuildSlideArea(SensorType.E2, 29, 33),
                BuildSlideArea(SensorType.B1, 34, 37),
                BuildSlideArea(SensorType.E1, 38, 39),
                BuildSlideArea(SensorType.A8, 40, 43),
                BuildSlideArea(SensorType.D8, 44, 48, true, true)
            },
            Const = 0.1f
        },
new SlideTable()
        {
            Name = "1D_PQ_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 4),
                BuildSlideArea(SensorType.B8, 5, 6),
                BuildSlideArea(SensorType.B7, 7, 10),
                BuildSlideArea(SensorType.B6, 11, 13),
                BuildSlideArea(SensorType.B5, 14, 16),
                BuildSlideArea(SensorType.B4, 17, 20),
                BuildSlideArea(SensorType.B3, 21, 23),
                BuildSlideArea(SensorType.B2, 24, 26),
                BuildSlideArea(SensorType.B1, 27, 29),
                BuildSlideArea(SensorType.D1, 30, 38, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1D_PQ_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 4),
                BuildSlideArea(SensorType.B8, 5, 6),
                BuildSlideArea(SensorType.B7, 7, 10),
                BuildSlideArea(SensorType.B6, 11, 13),
                BuildSlideArea(SensorType.B5, 14, 16),
                BuildSlideArea(SensorType.B4, 17, 19),
                BuildSlideArea(SensorType.B3, 20, 22),
                BuildSlideArea(SensorType.B2, 23, 26),
                BuildSlideArea(SensorType.D2, 27, 34, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1D_PQ_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 4),
                BuildSlideArea(SensorType.B8, 5, 6),
                BuildSlideArea(SensorType.B7, 7, 10),
                BuildSlideArea(SensorType.B6, 11, 13),
                BuildSlideArea(SensorType.B5, 14, 16),
                BuildSlideArea(SensorType.B4, 17, 19),
                BuildSlideArea(SensorType.B3, 20, 23),
                BuildSlideArea(SensorType.D3, 24, 31, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1D_PQ_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 4),
                BuildSlideArea(SensorType.B8, 5, 6),
                BuildSlideArea(SensorType.B7, 7, 10),
                BuildSlideArea(SensorType.B6, 11, 13),
                BuildSlideArea(SensorType.B5, 14, 16),
                BuildSlideArea(SensorType.B4, 17, 20),
                BuildSlideArea(SensorType.D4, 21, 28, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1D_PQ_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 4),
                BuildSlideArea(SensorType.B8, 5, 6),
                BuildSlideArea(SensorType.B7, 7, 10),
                BuildSlideArea(SensorType.B6, 11, 13),
                BuildSlideArea(SensorType.B5, 14, 16),
                BuildSlideArea(SensorType.D5, 17, 24, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1D_PQ_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 4),
                BuildSlideArea(SensorType.B8, 5, 6),
                BuildSlideArea(SensorType.B7, 7, 10),
                BuildSlideArea(SensorType.B6, 11, 13),
                BuildSlideArea(SensorType.B5, 14, 17),
                BuildSlideArea(SensorType.B4, 18, 20),
                BuildSlideArea(SensorType.B3, 21, 23),
                BuildSlideArea(SensorType.B2, 24, 26),
                BuildSlideArea(SensorType.B1, 27, 30),
                BuildSlideArea(SensorType.B8, 31, 33),
                BuildSlideArea(SensorType.B7, 34, 36),
                BuildSlideArea(SensorType.B6, 37, 39),
                BuildSlideArea(SensorType.D6, 40, 48, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1D_PQ_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 4),
                BuildSlideArea(SensorType.B8, 5, 6),
                BuildSlideArea(SensorType.B7, 7, 10),
                BuildSlideArea(SensorType.B6, 11, 13),
                BuildSlideArea(SensorType.B5, 14, 16),
                BuildSlideArea(SensorType.B4, 17, 19),
                BuildSlideArea(SensorType.B3, 20, 23),
                BuildSlideArea(SensorType.B2, 24, 26),
                BuildSlideArea(SensorType.B1, 27, 29),
                BuildSlideArea(SensorType.B8, 30, 32),
                BuildSlideArea(SensorType.B7, 33, 36),
                BuildSlideArea(SensorType.D7, 37, 44, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1D_PQ_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 4),
                BuildSlideArea(SensorType.B8, 5, 6),
                BuildSlideArea(SensorType.B7, 7, 10),
                BuildSlideArea(SensorType.B6, 11, 13),
                BuildSlideArea(SensorType.B5, 14, 16),
                BuildSlideArea(SensorType.B4, 17, 20),
                BuildSlideArea(SensorType.B3, 21, 23),
                BuildSlideArea(SensorType.B2, 24, 26),
                BuildSlideArea(SensorType.B1, 27, 29),
                BuildSlideArea(SensorType.B8, 30, 32),
                BuildSlideArea(SensorType.D8, 33, 41, true, true)
            },
            Const = 0.1f
        },
new SlideTable()
        {
            Name = "11_S_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 4),
                BuildSlideArea(SensorType.B8, 5, 6),
                BuildSlideArea(SensorType.B7, 7, 10),
                BuildSlideArea(SensorType.B8, 11, 13),
                BuildSlideArea(SensorType.E1, 14, 17),
                BuildSlideArea(SensorType.A1, 18, 23, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "11_S_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 4),
                BuildSlideArea(SensorType.B8, 5, 6),
                BuildSlideArea(SensorType.B7, 7, 10),
                BuildSlideArea(SensorType.B8, 11, 13),
                BuildSlideArea(SensorType.B1, 14, 17),
                BuildSlideArea(SensorType.E2, 18, 20),
                BuildSlideArea(SensorType.A2, 21, 27, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "11_S_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 4),
                BuildSlideArea(SensorType.B8, 5, 6),
                BuildSlideArea(SensorType.B7, 7, 10),
                BuildSlideArea(SensorType.B1, 11, 16),
                BuildSlideArea(SensorType.B2, 17, 20),
                BuildSlideArea(SensorType.E3, 21, 23),
                BuildSlideArea(SensorType.A3, 24, 30, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "11_S_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 4),
                BuildSlideArea(SensorType.B8, 5, 6),
                BuildSlideArea(SensorType.B7, 7, 10),
                BuildSlideArea(SensorType.C, 11, 13),
                BuildSlideArea(SensorType.B2, 14, 18),
                BuildSlideArea(SensorType.B3, 19, 22),
                BuildSlideArea(SensorType.E4, 23, 25),
                BuildSlideArea(SensorType.A4, 26, 32, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "11_S_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 4),
                BuildSlideArea(SensorType.B8, 5, 6),
                BuildSlideArea(SensorType.B7, 7, 10),
                BuildSlideArea(SensorType.C, 11, 13),
                BuildSlideArea(SensorType.B3, 14, 19),
                BuildSlideArea(SensorType.B4, 20, 22),
                BuildSlideArea(SensorType.E5, 23, 26),
                BuildSlideArea(SensorType.A5, 27, 32, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "11_S_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 4),
                BuildSlideArea(SensorType.B8, 5, 6),
                BuildSlideArea(SensorType.B7, 7, 10),
                BuildSlideArea(SensorType.C, 11, 13),
                BuildSlideArea(SensorType.B4, 14, 18),
                BuildSlideArea(SensorType.B5, 19, 22),
                BuildSlideArea(SensorType.E6, 23, 25),
                BuildSlideArea(SensorType.A6, 26, 32, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "11_S_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 4),
                BuildSlideArea(SensorType.B8, 5, 6),
                BuildSlideArea(SensorType.B7, 7, 10),
                BuildSlideArea(SensorType.B5, 11, 16),
                BuildSlideArea(SensorType.B6, 17, 20),
                BuildSlideArea(SensorType.E7, 21, 23),
                BuildSlideArea(SensorType.A7, 24, 30, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "11_S_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 4),
                BuildSlideArea(SensorType.B8, 5, 6),
                BuildSlideArea(SensorType.B7, 7, 10),
                BuildSlideArea(SensorType.B6, 11, 13),
                BuildSlideArea(SensorType.B7, 14, 17),
                BuildSlideArea(SensorType.E8, 18, 20),
                BuildSlideArea(SensorType.A8, 21, 27, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1D_S_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 4),
                BuildSlideArea(SensorType.B8, 5, 6),
                BuildSlideArea(SensorType.B7, 7, 10),
                BuildSlideArea(SensorType.B8, 11, 17),
                BuildSlideArea(SensorType.D1, 18, 25, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1D_S_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 4),
                BuildSlideArea(SensorType.B8, 5, 6),
                BuildSlideArea(SensorType.B7, 7, 10),
                BuildSlideArea(SensorType.B8, 11, 13),
                BuildSlideArea(SensorType.B1, 14, 17),
                BuildSlideArea(SensorType.D2, 18, 25, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1D_S_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 4),
                BuildSlideArea(SensorType.B8, 5, 6),
                BuildSlideArea(SensorType.B7, 7, 10),
                BuildSlideArea(SensorType.B8, 11, 13),
                BuildSlideArea(SensorType.B1, 14, 16),
                BuildSlideArea(SensorType.B2, 17, 20),
                BuildSlideArea(SensorType.D3, 21, 28, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1D_S_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 4),
                BuildSlideArea(SensorType.B8, 5, 6),
                BuildSlideArea(SensorType.B7, 7, 10),
                BuildSlideArea(SensorType.C, 11, 14),
                BuildSlideArea(SensorType.B1, 15, 17),
                BuildSlideArea(SensorType.B2, 18, 19),
                BuildSlideArea(SensorType.B3, 20, 23),
                BuildSlideArea(SensorType.D4, 24, 31, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1D_S_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 4),
                BuildSlideArea(SensorType.B8, 5, 6),
                BuildSlideArea(SensorType.B7, 7, 10),
                BuildSlideArea(SensorType.C, 11, 13),
                BuildSlideArea(SensorType.B3, 14, 20),
                BuildSlideArea(SensorType.B4, 21, 24),
                BuildSlideArea(SensorType.D5, 25, 32, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1D_S_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 4),
                BuildSlideArea(SensorType.B8, 5, 6),
                BuildSlideArea(SensorType.B7, 7, 10),
                BuildSlideArea(SensorType.C, 11, 13),
                BuildSlideArea(SensorType.B4, 14, 20),
                BuildSlideArea(SensorType.B5, 21, 24),
                BuildSlideArea(SensorType.D6, 25, 32, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1D_S_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 4),
                BuildSlideArea(SensorType.B8, 5, 6),
                BuildSlideArea(SensorType.B7, 7, 10),
                BuildSlideArea(SensorType.C, 11, 14),
                BuildSlideArea(SensorType.B5, 15, 17),
                BuildSlideArea(SensorType.B6, 18, 23),
                BuildSlideArea(SensorType.D7, 24, 31, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1D_S_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 4),
                BuildSlideArea(SensorType.B8, 5, 6),
                BuildSlideArea(SensorType.B7, 7, 10),
                BuildSlideArea(SensorType.B6, 11, 13),
                BuildSlideArea(SensorType.B7, 14, 20),
                BuildSlideArea(SensorType.D8, 21, 28, true, true)
            },
            Const = 0.1f
        },
new SlideTable()
        {
            Name = "1A_Line_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1A_Line_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 7, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1A_Line_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 2),
                BuildSlideArea(SensorType.E2, 3, 3),
                BuildSlideArea(SensorType.E3, 4, 8),
                BuildSlideArea(SensorType.A3, 9, 13, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1A_Line_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 4),
                BuildSlideArea(SensorType.B2, 5, 6),
                BuildSlideArea(SensorType.B3, 7, 10),
                BuildSlideArea(SensorType.A4, 11, 18, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1A_Line_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 4),
                BuildSlideArea(SensorType.C, 5, 7),
                BuildSlideArea(SensorType.B5, 8, 13),
                BuildSlideArea(SensorType.A5, 14, 19, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1A_Line_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 4),
                BuildSlideArea(SensorType.B8, 5, 6),
                BuildSlideArea(SensorType.B7, 7, 10),
                BuildSlideArea(SensorType.A6, 11, 18, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1A_Line_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D1, 0, 2),
                BuildSlideArea(SensorType.E1, 3, 3),
                BuildSlideArea(SensorType.E8, 4, 8),
                BuildSlideArea(SensorType.A7, 9, 13, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1A_Line_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D1, 0, 1),
                BuildSlideArea(SensorType.A8, 2, 7, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1B_Line_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1B_Line_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 3),
                BuildSlideArea(SensorType.B2, 4, 7, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1B_Line_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 3),
                BuildSlideArea(SensorType.B2, 4, 6),
                BuildSlideArea(SensorType.B3, 7, 11, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1B_Line_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.C, 0, 9),
                BuildSlideArea(SensorType.B4, 10, 14, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1B_Line_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 4),
                BuildSlideArea(SensorType.C, 5, 7),
                BuildSlideArea(SensorType.B5, 8, 15, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1B_Line_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.C, 0, 9),
                BuildSlideArea(SensorType.B6, 10, 14, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1B_Line_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 3),
                BuildSlideArea(SensorType.B8, 4, 6),
                BuildSlideArea(SensorType.B7, 7, 11, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1B_Line_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 3),
                BuildSlideArea(SensorType.B8, 4, 7, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1C_Line",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 4),
                BuildSlideArea(SensorType.C, 5, 10, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1D_Line_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D1, 0, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1D_Line_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1D_Line_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 5),
                BuildSlideArea(SensorType.D3, 6, 10, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1D_Line_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 3),
                BuildSlideArea(SensorType.B2, 4, 6),
                BuildSlideArea(SensorType.B3, 7, 10),
                BuildSlideArea(SensorType.D4, 11, 16, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1D_Line_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 4),
                BuildSlideArea(SensorType.C, 5, 8),
                BuildSlideArea(SensorType.B4, 9, 12),
                BuildSlideArea(SensorType.E5, 13, 15),
                BuildSlideArea(SensorType.D5, 16, 19, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1D_Line_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 4),
                BuildSlideArea(SensorType.C, 5, 8),
                BuildSlideArea(SensorType.B6, 9, 12),
                BuildSlideArea(SensorType.E6, 13, 15),
                BuildSlideArea(SensorType.D6, 16, 19, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1D_Line_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 3),
                BuildSlideArea(SensorType.B8, 4, 6),
                BuildSlideArea(SensorType.B7, 7, 10),
                BuildSlideArea(SensorType.D7, 11, 16, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1D_Line_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D1, 0, 1),
                BuildSlideArea(SensorType.A8, 2, 5),
                BuildSlideArea(SensorType.D8, 6, 10, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1E_Line_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1E_Line_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1E_Line_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 2),
                BuildSlideArea(SensorType.E2, 3, 3),
                BuildSlideArea(SensorType.E3, 4, 9, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1E_Line_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 3),
                BuildSlideArea(SensorType.B2, 4, 6),
                BuildSlideArea(SensorType.B3, 7, 9),
                BuildSlideArea(SensorType.E4, 10, 14, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1E_Line_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 4),
                BuildSlideArea(SensorType.C, 5, 8),
                BuildSlideArea(SensorType.B4, 9, 13),
                BuildSlideArea(SensorType.E5, 14, 17, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1E_Line_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 4),
                BuildSlideArea(SensorType.C, 5, 8),
                BuildSlideArea(SensorType.B6, 9, 13),
                BuildSlideArea(SensorType.E6, 14, 17, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1E_Line_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 3),
                BuildSlideArea(SensorType.B8, 4, 6),
                BuildSlideArea(SensorType.B7, 7, 9),
                BuildSlideArea(SensorType.E7, 10, 14, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1E_Line_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D1, 0, 2),
                BuildSlideArea(SensorType.E1, 3, 3),
                BuildSlideArea(SensorType.E8, 4, 9, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "A1_Line_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "A1_Line_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 7, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "A1_Line_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 2),
                BuildSlideArea(SensorType.E3, 3, 7),
                BuildSlideArea(SensorType.D3, 8, 8),
                BuildSlideArea(SensorType.A3, 9, 13, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "A1_Line_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B2, 0, 5),
                BuildSlideArea(SensorType.B3, 6, 8),
                BuildSlideArea(SensorType.E4, 9, 12),
                BuildSlideArea(SensorType.A4, 13, 18, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "A1_Line_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 2),
                BuildSlideArea(SensorType.C, 3, 6),
                BuildSlideArea(SensorType.B5, 7, 11),
                BuildSlideArea(SensorType.A5, 12, 19, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "A1_Line_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B8, 0, 5),
                BuildSlideArea(SensorType.B7, 6, 8),
                BuildSlideArea(SensorType.E7, 9, 12),
                BuildSlideArea(SensorType.A6, 13, 18, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "A1_Line_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 2),
                BuildSlideArea(SensorType.E8, 3, 7),
                BuildSlideArea(SensorType.D8, 8, 8),
                BuildSlideArea(SensorType.A7, 9, 13, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "A1_Line_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D1, 0, 1),
                BuildSlideArea(SensorType.A8, 2, 7, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AA_Line_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 6, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AA_Line_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 2),
                BuildSlideArea(SensorType.E3, 3, 7),
                BuildSlideArea(SensorType.A3, 8, 12, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AA_Line_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B2, 0, 5),
                BuildSlideArea(SensorType.B3, 6, 8),
                BuildSlideArea(SensorType.A4, 9, 16, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AA_Line_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 2),
                BuildSlideArea(SensorType.C, 3, 6),
                BuildSlideArea(SensorType.B5, 7, 12),
                BuildSlideArea(SensorType.A5, 13, 18, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AA_Line_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B8, 0, 5),
                BuildSlideArea(SensorType.B7, 6, 8),
                BuildSlideArea(SensorType.A6, 9, 16, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AA_Line_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 2),
                BuildSlideArea(SensorType.E8, 3, 7),
                BuildSlideArea(SensorType.A7, 8, 12, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AA_Line_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D1, 0, 1),
                BuildSlideArea(SensorType.A8, 2, 6, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AB_Line_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AB_Line_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 2),
                BuildSlideArea(SensorType.B2, 3, 5, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AB_Line_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 2),
                BuildSlideArea(SensorType.B2, 3, 4),
                BuildSlideArea(SensorType.B3, 5, 10, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AB_Line_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 3),
                BuildSlideArea(SensorType.C, 4, 7),
                BuildSlideArea(SensorType.B4, 8, 13, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AB_Line_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 2),
                BuildSlideArea(SensorType.C, 3, 6),
                BuildSlideArea(SensorType.B5, 7, 14, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AB_Line_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 3),
                BuildSlideArea(SensorType.C, 4, 7),
                BuildSlideArea(SensorType.B6, 8, 13, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AB_Line_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 2),
                BuildSlideArea(SensorType.B8, 3, 4),
                BuildSlideArea(SensorType.B7, 5, 10, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AB_Line_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 2),
                BuildSlideArea(SensorType.B8, 3, 5, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AC_Line",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 2),
                BuildSlideArea(SensorType.C, 3, 8, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AD_Line_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D1, 0, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AD_Line_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AD_Line_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 2),
                BuildSlideArea(SensorType.A2, 3, 4),
                BuildSlideArea(SensorType.D3, 5, 9, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AD_Line_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 2),
                BuildSlideArea(SensorType.B2, 3, 5),
                BuildSlideArea(SensorType.B3, 6, 9),
                BuildSlideArea(SensorType.D4, 10, 15, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AD_Line_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 2),
                BuildSlideArea(SensorType.C, 3, 6),
                BuildSlideArea(SensorType.B4, 7, 11),
                BuildSlideArea(SensorType.E5, 12, 13),
                BuildSlideArea(SensorType.D5, 14, 17, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AD_Line_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 2),
                BuildSlideArea(SensorType.C, 3, 6),
                BuildSlideArea(SensorType.B6, 7, 11),
                BuildSlideArea(SensorType.E6, 12, 13),
                BuildSlideArea(SensorType.D6, 14, 17, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AD_Line_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 2),
                BuildSlideArea(SensorType.B8, 3, 5),
                BuildSlideArea(SensorType.B7, 6, 9),
                BuildSlideArea(SensorType.D7, 10, 15, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AD_Line_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D1, 0, 2),
                BuildSlideArea(SensorType.A8, 3, 4),
                BuildSlideArea(SensorType.D8, 5, 9, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AE_Line_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AE_Line_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AE_Line_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 2),
                BuildSlideArea(SensorType.E3, 3, 8, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AE_Line_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 2),
                BuildSlideArea(SensorType.B2, 3, 4),
                BuildSlideArea(SensorType.B3, 5, 7),
                BuildSlideArea(SensorType.E4, 8, 12, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AE_Line_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 2),
                BuildSlideArea(SensorType.C, 3, 6),
                BuildSlideArea(SensorType.B4, 7, 11),
                BuildSlideArea(SensorType.E5, 12, 15, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AE_Line_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 2),
                BuildSlideArea(SensorType.C, 3, 6),
                BuildSlideArea(SensorType.B6, 7, 11),
                BuildSlideArea(SensorType.E6, 12, 15, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AE_Line_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 2),
                BuildSlideArea(SensorType.B8, 3, 4),
                BuildSlideArea(SensorType.B7, 5, 7),
                BuildSlideArea(SensorType.E7, 8, 12, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AE_Line_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 2),
                BuildSlideArea(SensorType.E8, 3, 8, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "B1_Line_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "B1_Line_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 7, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "B1_Line_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B2, 0, 2),
                BuildSlideArea(SensorType.E3, 3, 5),
                BuildSlideArea(SensorType.A3, 6, 11, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "B1_Line_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.C, 0, 3),
                BuildSlideArea(SensorType.A4, 4, 14, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "B1_Line_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.C, 0, 2),
                BuildSlideArea(SensorType.B5, 3, 7),
                BuildSlideArea(SensorType.A5, 8, 15, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "B1_Line_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.C, 0, 3),
                BuildSlideArea(SensorType.A6, 4, 14, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "B1_Line_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B8, 0, 2),
                BuildSlideArea(SensorType.E8, 3, 5),
                BuildSlideArea(SensorType.A7, 6, 11, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "B1_Line_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 1),
                BuildSlideArea(SensorType.A8, 2, 7, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BA_Line_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BA_Line_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 5, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BA_Line_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B2, 0, 2),
                BuildSlideArea(SensorType.E3, 3, 5),
                BuildSlideArea(SensorType.A3, 6, 10, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BA_Line_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.C, 0, 3),
                BuildSlideArea(SensorType.B4, 4, 7),
                BuildSlideArea(SensorType.A4, 8, 13, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BA_Line_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.C, 0, 2),
                BuildSlideArea(SensorType.B5, 3, 8),
                BuildSlideArea(SensorType.A5, 9, 14, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BA_Line_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.C, 0, 3),
                BuildSlideArea(SensorType.B6, 4, 7),
                BuildSlideArea(SensorType.A6, 8, 13, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BA_Line_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B8, 0, 2),
                BuildSlideArea(SensorType.E8, 3, 5),
                BuildSlideArea(SensorType.A7, 6, 10, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BA_Line_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 1),
                BuildSlideArea(SensorType.A8, 2, 5, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BB_Line_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B2, 0, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BB_Line_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B2, 0, 2),
                BuildSlideArea(SensorType.B3, 3, 6, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BB_Line_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.C, 0, 2),
                BuildSlideArea(SensorType.B4, 3, 8, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BB_Line_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.C, 0, 2),
                BuildSlideArea(SensorType.B5, 3, 9, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BB_Line_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.C, 0, 2),
                BuildSlideArea(SensorType.B6, 3, 8, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BB_Line_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B8, 0, 2),
                BuildSlideArea(SensorType.B7, 3, 6, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BB_Line_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B8, 0, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BC_Line",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.C, 0, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BD_Line_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 1),
                BuildSlideArea(SensorType.D1, 2, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BD_Line_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 1),
                BuildSlideArea(SensorType.D2, 2, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BD_Line_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B2, 0, 2),
                BuildSlideArea(SensorType.D3, 3, 8, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BD_Line_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B2, 0, 2),
                BuildSlideArea(SensorType.B3, 3, 4),
                BuildSlideArea(SensorType.E4, 5, 8),
                BuildSlideArea(SensorType.D4, 9, 11, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BD_Line_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.C, 0, 2),
                BuildSlideArea(SensorType.B4, 3, 7),
                BuildSlideArea(SensorType.E5, 8, 9),
                BuildSlideArea(SensorType.D5, 10, 13, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BD_Line_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.C, 0, 2),
                BuildSlideArea(SensorType.B6, 3, 7),
                BuildSlideArea(SensorType.E6, 8, 9),
                BuildSlideArea(SensorType.D6, 10, 13, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BD_Line_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B8, 0, 2),
                BuildSlideArea(SensorType.B7, 3, 4),
                BuildSlideArea(SensorType.E7, 5, 8),
                BuildSlideArea(SensorType.D7, 9, 11, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BD_Line_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B8, 0, 2),
                BuildSlideArea(SensorType.D8, 3, 8, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BE_Line_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BE_Line_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BE_Line_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B2, 0, 2),
                BuildSlideArea(SensorType.E3, 3, 6, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BE_Line_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B3, 0, 5),
                BuildSlideArea(SensorType.E4, 6, 9, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BE_Line_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.C, 0, 2),
                BuildSlideArea(SensorType.B4, 3, 8),
                BuildSlideArea(SensorType.E5, 9, 11, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BE_Line_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.C, 0, 2),
                BuildSlideArea(SensorType.B6, 3, 8),
                BuildSlideArea(SensorType.E6, 9, 11, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BE_Line_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B7, 0, 5),
                BuildSlideArea(SensorType.E7, 6, 9, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BE_Line_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B8, 0, 2),
                BuildSlideArea(SensorType.E8, 3, 6, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "C1_Line",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 3),
                BuildSlideArea(SensorType.A1, 4, 10, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "CA_Line",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 2),
                BuildSlideArea(SensorType.A1, 3, 8, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "CB_Line",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "CD_Line",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 4),
                BuildSlideArea(SensorType.D1, 5, 8, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "CE_Line",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 6, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "D1_Line_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "D1_Line_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 1),
                BuildSlideArea(SensorType.D2, 2, 4),
                BuildSlideArea(SensorType.A2, 5, 10, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "D1_Line_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 4),
                BuildSlideArea(SensorType.B2, 5, 7),
                BuildSlideArea(SensorType.E3, 8, 9),
                BuildSlideArea(SensorType.A3, 10, 16, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "D1_Line_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 1),
                BuildSlideArea(SensorType.B1, 2, 3),
                BuildSlideArea(SensorType.C, 4, 6),
                BuildSlideArea(SensorType.B4, 7, 12),
                BuildSlideArea(SensorType.A4, 13, 19, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "D1_Line_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 1),
                BuildSlideArea(SensorType.B8, 2, 3),
                BuildSlideArea(SensorType.C, 4, 6),
                BuildSlideArea(SensorType.B5, 7, 12),
                BuildSlideArea(SensorType.A5, 13, 19, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "D1_Line_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B8, 0, 4),
                BuildSlideArea(SensorType.B7, 5, 7),
                BuildSlideArea(SensorType.E7, 8, 9),
                BuildSlideArea(SensorType.A6, 10, 16, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "D1_Line_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A8, 0, 1),
                BuildSlideArea(SensorType.D8, 2, 4),
                BuildSlideArea(SensorType.A7, 5, 10, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "D1_Line_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A8, 0, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DA_Line_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DA_Line_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 1),
                BuildSlideArea(SensorType.D2, 2, 5),
                BuildSlideArea(SensorType.A2, 6, 9, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DA_Line_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 3),
                BuildSlideArea(SensorType.B2, 4, 6),
                BuildSlideArea(SensorType.E3, 7, 10),
                BuildSlideArea(SensorType.A3, 11, 15, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DA_Line_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 1),
                BuildSlideArea(SensorType.B1, 2, 3),
                BuildSlideArea(SensorType.C, 4, 6),
                BuildSlideArea(SensorType.B4, 7, 11),
                BuildSlideArea(SensorType.A4, 12, 17, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DA_Line_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 1),
                BuildSlideArea(SensorType.B8, 2, 3),
                BuildSlideArea(SensorType.C, 4, 6),
                BuildSlideArea(SensorType.B5, 7, 11),
                BuildSlideArea(SensorType.A5, 12, 17, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DA_Line_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B8, 0, 3),
                BuildSlideArea(SensorType.B7, 4, 6),
                BuildSlideArea(SensorType.E7, 7, 10),
                BuildSlideArea(SensorType.A6, 11, 15, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DA_Line_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A8, 0, 1),
                BuildSlideArea(SensorType.D8, 2, 5),
                BuildSlideArea(SensorType.A7, 6, 9, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DA_Line_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A8, 0, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DB_Line_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 1),
                BuildSlideArea(SensorType.B1, 2, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DB_Line_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 3),
                BuildSlideArea(SensorType.B2, 4, 8, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DB_Line_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 1),
                BuildSlideArea(SensorType.B1, 2, 3),
                BuildSlideArea(SensorType.B2, 4, 6),
                BuildSlideArea(SensorType.B3, 7, 11, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DB_Line_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 1),
                BuildSlideArea(SensorType.B1, 2, 3),
                BuildSlideArea(SensorType.C, 4, 6),
                BuildSlideArea(SensorType.B4, 7, 13, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DB_Line_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 1),
                BuildSlideArea(SensorType.B8, 2, 3),
                BuildSlideArea(SensorType.C, 4, 6),
                BuildSlideArea(SensorType.B5, 7, 13, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DB_Line_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 1),
                BuildSlideArea(SensorType.B8, 2, 3),
                BuildSlideArea(SensorType.B7, 4, 6),
                BuildSlideArea(SensorType.B6, 7, 11, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DB_Line_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B8, 0, 3),
                BuildSlideArea(SensorType.B7, 4, 8, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DB_Line_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 1),
                BuildSlideArea(SensorType.B8, 2, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DC_Line",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 1),
                BuildSlideArea(SensorType.C, 2, 8, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DD_Line_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 1),
                BuildSlideArea(SensorType.D2, 2, 6, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DD_Line_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 4),
                BuildSlideArea(SensorType.D3, 5, 12, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DD_Line_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 1),
                BuildSlideArea(SensorType.B1, 2, 3),
                BuildSlideArea(SensorType.B2, 4, 7),
                BuildSlideArea(SensorType.B3, 8, 9),
                BuildSlideArea(SensorType.E4, 10, 13),
                BuildSlideArea(SensorType.D4, 14, 16, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DD_Line_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 1),
                BuildSlideArea(SensorType.C, 2, 6),
                BuildSlideArea(SensorType.E5, 7, 14),
                BuildSlideArea(SensorType.D5, 15, 18, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DD_Line_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 1),
                BuildSlideArea(SensorType.B8, 2, 3),
                BuildSlideArea(SensorType.B7, 4, 7),
                BuildSlideArea(SensorType.B6, 8, 9),
                BuildSlideArea(SensorType.E6, 10, 13),
                BuildSlideArea(SensorType.D6, 14, 16, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DD_Line_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E8, 0, 4),
                BuildSlideArea(SensorType.D7, 5, 12, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DD_Line_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A8, 0, 1),
                BuildSlideArea(SensorType.D8, 2, 6, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DE_Line_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DE_Line_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 5, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DE_Line_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 3),
                BuildSlideArea(SensorType.B2, 4, 6),
                BuildSlideArea(SensorType.E3, 7, 10, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DE_Line_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 1),
                BuildSlideArea(SensorType.B1, 2, 3),
                BuildSlideArea(SensorType.B3, 4, 9),
                BuildSlideArea(SensorType.E4, 10, 14, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DE_Line_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 1),
                BuildSlideArea(SensorType.C, 2, 6),
                BuildSlideArea(SensorType.E5, 7, 15, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DE_Line_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 1),
                BuildSlideArea(SensorType.B8, 2, 3),
                BuildSlideArea(SensorType.B6, 4, 9),
                BuildSlideArea(SensorType.E6, 10, 14, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DE_Line_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B8, 0, 3),
                BuildSlideArea(SensorType.B7, 4, 6),
                BuildSlideArea(SensorType.E7, 7, 10, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DE_Line_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E8, 0, 5, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "E1_Line_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "E1_Line_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 3),
                BuildSlideArea(SensorType.D2, 4, 5),
                BuildSlideArea(SensorType.A2, 6, 9, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "E1_Line_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 1),
                BuildSlideArea(SensorType.B2, 2, 4),
                BuildSlideArea(SensorType.E3, 5, 8),
                BuildSlideArea(SensorType.A3, 9, 14, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "E1_Line_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 1),
                BuildSlideArea(SensorType.C, 2, 4),
                BuildSlideArea(SensorType.B4, 5, 9),
                BuildSlideArea(SensorType.A4, 10, 17, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "E1_Line_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B8, 0, 1),
                BuildSlideArea(SensorType.C, 2, 4),
                BuildSlideArea(SensorType.B5, 5, 9),
                BuildSlideArea(SensorType.A5, 10, 17, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "E1_Line_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B8, 0, 1),
                BuildSlideArea(SensorType.B7, 2, 4),
                BuildSlideArea(SensorType.E7, 5, 8),
                BuildSlideArea(SensorType.A6, 9, 14, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "E1_Line_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E8, 0, 3),
                BuildSlideArea(SensorType.D8, 4, 5),
                BuildSlideArea(SensorType.A7, 6, 9, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "E1_Line_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A8, 0, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EA_Line_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EA_Line_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 3),
                BuildSlideArea(SensorType.A2, 4, 8, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EA_Line_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 1),
                BuildSlideArea(SensorType.B2, 2, 4),
                BuildSlideArea(SensorType.E3, 5, 8),
                BuildSlideArea(SensorType.A3, 9, 12, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EA_Line_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 1),
                BuildSlideArea(SensorType.C, 2, 4),
                BuildSlideArea(SensorType.B4, 5, 9),
                BuildSlideArea(SensorType.A4, 10, 15, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EA_Line_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B8, 0, 1),
                BuildSlideArea(SensorType.C, 2, 4),
                BuildSlideArea(SensorType.B5, 5, 9),
                BuildSlideArea(SensorType.A5, 10, 15, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EA_Line_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B8, 0, 1),
                BuildSlideArea(SensorType.B7, 2, 4),
                BuildSlideArea(SensorType.E7, 5, 8),
                BuildSlideArea(SensorType.A6, 9, 12, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EA_Line_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E8, 0, 3),
                BuildSlideArea(SensorType.A7, 4, 8, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EA_Line_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A8, 0, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EB_Line_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EB_Line_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 1),
                BuildSlideArea(SensorType.B2, 2, 6, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EB_Line_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 1),
                BuildSlideArea(SensorType.B3, 2, 9, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EB_Line_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 1),
                BuildSlideArea(SensorType.C, 2, 3),
                BuildSlideArea(SensorType.B4, 4, 11, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EB_Line_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B8, 0, 1),
                BuildSlideArea(SensorType.C, 2, 3),
                BuildSlideArea(SensorType.B5, 4, 11, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EB_Line_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B8, 0, 1),
                BuildSlideArea(SensorType.B6, 2, 9, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EB_Line_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B8, 0, 1),
                BuildSlideArea(SensorType.B7, 2, 6, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EB_Line_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B8, 0, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EC_Line",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.C, 0, 6, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "ED_Line_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D1, 0, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "ED_Line_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 5, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "ED_Line_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 1),
                BuildSlideArea(SensorType.B2, 2, 5),
                BuildSlideArea(SensorType.D3, 6, 10, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "ED_Line_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 1),
                BuildSlideArea(SensorType.B3, 2, 7),
                BuildSlideArea(SensorType.E4, 8, 11),
                BuildSlideArea(SensorType.D4, 12, 14, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "ED_Line_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.C, 0, 3),
                BuildSlideArea(SensorType.E5, 4, 11),
                BuildSlideArea(SensorType.D5, 12, 15, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "ED_Line_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B8, 0, 1),
                BuildSlideArea(SensorType.B6, 2, 7),
                BuildSlideArea(SensorType.E6, 8, 11),
                BuildSlideArea(SensorType.D6, 12, 14, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "ED_Line_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B8, 0, 1),
                BuildSlideArea(SensorType.B7, 2, 5),
                BuildSlideArea(SensorType.D7, 6, 10, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "ED_Line_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D8, 0, 5, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EE_Line_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 1),
                BuildSlideArea(SensorType.E2, 2, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EE_Line_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 1),
                BuildSlideArea(SensorType.B2, 2, 4),
                BuildSlideArea(SensorType.E3, 5, 8, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EE_Line_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 1),
                BuildSlideArea(SensorType.C, 2, 5),
                BuildSlideArea(SensorType.B3, 6, 8),
                BuildSlideArea(SensorType.E4, 9, 12, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EE_Line_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.C, 0, 3),
                BuildSlideArea(SensorType.E5, 4, 13, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EE_Line_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B8, 0, 1),
                BuildSlideArea(SensorType.C, 2, 5),
                BuildSlideArea(SensorType.B6, 6, 8),
                BuildSlideArea(SensorType.E6, 9, 12, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EE_Line_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B8, 0, 1),
                BuildSlideArea(SensorType.B7, 2, 4),
                BuildSlideArea(SensorType.E7, 5, 8, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EE_Line_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B8, 0, 1),
                BuildSlideArea(SensorType.E8, 2, 4, true, true)
            },
            Const = 0.1f
        },
new SlideTable()
        {
            Name = "1C_Circle_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 0),
                BuildSlideArea(SensorType.E2, 1, 1),
                BuildSlideArea(SensorType.B2, 2, 2),
                BuildSlideArea(SensorType.C, 3, 5, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1C_Circle_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B2, 0, 0),
                BuildSlideArea(SensorType.C, 1, 2, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1C_Circle_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D4, 0, 1),
                BuildSlideArea(SensorType.A4, 2, 5),
                BuildSlideArea(SensorType.D5, 6, 8),
                BuildSlideArea(SensorType.A5, 9, 11),
                BuildSlideArea(SensorType.E6, 12, 14),
                BuildSlideArea(SensorType.E7, 15, 19),
                BuildSlideArea(SensorType.B7, 20, 21),
                BuildSlideArea(SensorType.B8, 22, 24),
                BuildSlideArea(SensorType.C, 25, 30, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1C_Circle_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D5, 0, 1),
                BuildSlideArea(SensorType.A5, 2, 5),
                BuildSlideArea(SensorType.D6, 6, 9),
                BuildSlideArea(SensorType.A6, 10, 11),
                BuildSlideArea(SensorType.E7, 12, 14),
                BuildSlideArea(SensorType.B7, 15, 16),
                BuildSlideArea(SensorType.B8, 17, 20),
                BuildSlideArea(SensorType.C, 21, 27, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1C_Circle_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D6, 0, 1),
                BuildSlideArea(SensorType.A6, 2, 4),
                BuildSlideArea(SensorType.D7, 5, 8),
                BuildSlideArea(SensorType.E7, 9, 9),
                BuildSlideArea(SensorType.E8, 10, 13),
                BuildSlideArea(SensorType.B8, 14, 14),
                BuildSlideArea(SensorType.C, 15, 22, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1C_Circle_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D7, 0, 1),
                BuildSlideArea(SensorType.A7, 2, 3),
                BuildSlideArea(SensorType.E8, 4, 6),
                BuildSlideArea(SensorType.B8, 7, 9),
                BuildSlideArea(SensorType.B1, 10, 12),
                BuildSlideArea(SensorType.C, 13, 18, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1C_Circle_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D8, 0, 1),
                BuildSlideArea(SensorType.A8, 2, 5),
                BuildSlideArea(SensorType.E1, 6, 7),
                BuildSlideArea(SensorType.B1, 8, 9),
                BuildSlideArea(SensorType.C, 10, 15, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1C_Circle_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D1, 0, 1),
                BuildSlideArea(SensorType.B1, 2, 5),
                BuildSlideArea(SensorType.C, 6, 11, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AC_Circle_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B2, 0, 2),
                BuildSlideArea(SensorType.C, 3, 5, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AC_Circle_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B2, 0, 0),
                BuildSlideArea(SensorType.C, 1, 2, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AC_Circle_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D4, 0, 1),
                BuildSlideArea(SensorType.A4, 2, 4),
                BuildSlideArea(SensorType.E5, 5, 7),
                BuildSlideArea(SensorType.E6, 8, 12),
                BuildSlideArea(SensorType.B6, 13, 14),
                BuildSlideArea(SensorType.B7, 15, 17),
                BuildSlideArea(SensorType.C, 18, 26, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AC_Circle_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D5, 0, 1),
                BuildSlideArea(SensorType.A5, 2, 4),
                BuildSlideArea(SensorType.E6, 5, 7),
                BuildSlideArea(SensorType.B7, 8, 13),
                BuildSlideArea(SensorType.B8, 14, 17),
                BuildSlideArea(SensorType.C, 18, 23, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AC_Circle_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D6, 0, 1),
                BuildSlideArea(SensorType.A6, 2, 4),
                BuildSlideArea(SensorType.E7, 5, 7),
                BuildSlideArea(SensorType.B7, 8, 10),
                BuildSlideArea(SensorType.B8, 11, 13),
                BuildSlideArea(SensorType.C, 14, 20, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AC_Circle_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D7, 0, 1),
                BuildSlideArea(SensorType.E8, 2, 6),
                BuildSlideArea(SensorType.B8, 7, 8),
                BuildSlideArea(SensorType.B1, 9, 11),
                BuildSlideArea(SensorType.C, 12, 16, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AC_Circle_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D8, 0, 1),
                BuildSlideArea(SensorType.E8, 2, 2),
                BuildSlideArea(SensorType.B1, 3, 6),
                BuildSlideArea(SensorType.C, 7, 12, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AC_Circle_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 2),
                BuildSlideArea(SensorType.B1, 3, 4),
                BuildSlideArea(SensorType.C, 5, 10, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BC_Circle_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.C, 0, 5, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BC_Circle_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.C, 0, 2, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BC_Circle_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B4, 0, 2),
                BuildSlideArea(SensorType.B5, 3, 5),
                BuildSlideArea(SensorType.C, 6, 15, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BC_Circle_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B5, 0, 2),
                BuildSlideArea(SensorType.B6, 3, 5),
                BuildSlideArea(SensorType.C, 6, 13, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BC_Circle_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B6, 0, 2),
                BuildSlideArea(SensorType.B7, 3, 5),
                BuildSlideArea(SensorType.C, 6, 12, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BC_Circle_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B7, 0, 1),
                BuildSlideArea(SensorType.C, 2, 9, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BC_Circle_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B8, 0, 2),
                BuildSlideArea(SensorType.C, 3, 8, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BC_Circle_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 1),
                BuildSlideArea(SensorType.C, 2, 5, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DC_Circle_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B2, 0, 4),
                BuildSlideArea(SensorType.C, 5, 8, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DC_Circle_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B2, 0, 0),
                BuildSlideArea(SensorType.C, 1, 2, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DC_Circle_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A3, 0, 1),
                BuildSlideArea(SensorType.D4, 2, 4),
                BuildSlideArea(SensorType.E4, 5, 5),
                BuildSlideArea(SensorType.A4, 6, 8),
                BuildSlideArea(SensorType.E5, 9, 10),
                BuildSlideArea(SensorType.E6, 11, 15),
                BuildSlideArea(SensorType.B6, 16, 16),
                BuildSlideArea(SensorType.B7, 17, 20),
                BuildSlideArea(SensorType.C, 21, 28, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DC_Circle_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A4, 0, 1),
                BuildSlideArea(SensorType.D5, 2, 4),
                BuildSlideArea(SensorType.E6, 5, 9),
                BuildSlideArea(SensorType.B6, 10, 12),
                BuildSlideArea(SensorType.B7, 13, 15),
                BuildSlideArea(SensorType.B8, 16, 18),
                BuildSlideArea(SensorType.C, 19, 24, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DC_Circle_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A5, 0, 1),
                BuildSlideArea(SensorType.E6, 2, 5),
                BuildSlideArea(SensorType.E7, 6, 9),
                BuildSlideArea(SensorType.B7, 10, 11),
                BuildSlideArea(SensorType.B8, 12, 15),
                BuildSlideArea(SensorType.C, 16, 22, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DC_Circle_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A6, 0, 1),
                BuildSlideArea(SensorType.E7, 2, 4),
                BuildSlideArea(SensorType.B8, 5, 10),
                BuildSlideArea(SensorType.C, 11, 18, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DC_Circle_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A7, 0, 0),
                BuildSlideArea(SensorType.E8, 1, 3),
                BuildSlideArea(SensorType.B8, 4, 5),
                BuildSlideArea(SensorType.B1, 6, 9),
                BuildSlideArea(SensorType.C, 10, 14, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DC_Circle_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A8, 0, 1),
                BuildSlideArea(SensorType.E1, 2, 5),
                BuildSlideArea(SensorType.B1, 6, 6),
                BuildSlideArea(SensorType.C, 7, 11, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EC_Circle_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 1),
                BuildSlideArea(SensorType.C, 2, 6, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EC_Circle_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B2, 0, 0),
                BuildSlideArea(SensorType.C, 1, 2, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EC_Circle_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E4, 0, 3),
                BuildSlideArea(SensorType.B4, 4, 5),
                BuildSlideArea(SensorType.B5, 6, 9),
                BuildSlideArea(SensorType.B6, 10, 12),
                BuildSlideArea(SensorType.C, 13, 21, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EC_Circle_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B4, 0, 2),
                BuildSlideArea(SensorType.B5, 3, 5),
                BuildSlideArea(SensorType.B6, 6, 8),
                BuildSlideArea(SensorType.B7, 9, 12),
                BuildSlideArea(SensorType.C, 13, 18, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EC_Circle_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B5, 0, 1),
                BuildSlideArea(SensorType.B6, 2, 5),
                BuildSlideArea(SensorType.B7, 6, 8),
                BuildSlideArea(SensorType.C, 9, 16, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EC_Circle_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B6, 0, 1),
                BuildSlideArea(SensorType.B7, 2, 4),
                BuildSlideArea(SensorType.B8, 5, 7),
                BuildSlideArea(SensorType.C, 8, 13, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EC_Circle_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B7, 0, 1),
                BuildSlideArea(SensorType.B8, 2, 4),
                BuildSlideArea(SensorType.C, 5, 11, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EC_Circle_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B8, 0, 1),
                BuildSlideArea(SensorType.B1, 2, 3),
                BuildSlideArea(SensorType.C, 4, 8, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "C1_Circle_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B5, 0, 3),
                BuildSlideArea(SensorType.B6, 4, 6),
                BuildSlideArea(SensorType.E7, 7, 8),
                BuildSlideArea(SensorType.A7, 9, 10),
                BuildSlideArea(SensorType.D8, 11, 12),
                BuildSlideArea(SensorType.A8, 13, 13),
                BuildSlideArea(SensorType.D1, 14, 15),
                BuildSlideArea(SensorType.A1, 16, 17, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "C1_Circle_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B5, 0, 2),
                BuildSlideArea(SensorType.B6, 3, 3),
                BuildSlideArea(SensorType.A8, 4, 5),
                BuildSlideArea(SensorType.A1, 6, 6),
                BuildSlideArea(SensorType.D2, 7, 8, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "C1_Circle_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B3, 0, 2),
                BuildSlideArea(SensorType.A3, 3, 8, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "C1_Circle_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B3, 0, 3),
                BuildSlideArea(SensorType.E4, 4, 6),
                BuildSlideArea(SensorType.D4, 7, 7),
                BuildSlideArea(SensorType.A4, 8, 10, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "C1_Circle_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B4, 0, 4),
                BuildSlideArea(SensorType.D5, 5, 8),
                BuildSlideArea(SensorType.A5, 9, 12, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "C1_Circle_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B4, 0, 3),
                BuildSlideArea(SensorType.E5, 4, 6),
                BuildSlideArea(SensorType.A5, 7, 9),
                BuildSlideArea(SensorType.D6, 10, 10),
                BuildSlideArea(SensorType.A6, 11, 14, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "C1_Circle_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B4, 0, 3),
                BuildSlideArea(SensorType.B5, 4, 5),
                BuildSlideArea(SensorType.E6, 6, 8),
                BuildSlideArea(SensorType.A6, 9, 10),
                BuildSlideArea(SensorType.D7, 11, 12),
                BuildSlideArea(SensorType.A7, 13, 18, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "C1_Circle_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B5, 0, 4),
                BuildSlideArea(SensorType.E7, 5, 11),
                BuildSlideArea(SensorType.D7, 12, 12),
                BuildSlideArea(SensorType.A7, 13, 13),
                BuildSlideArea(SensorType.D8, 14, 16),
                BuildSlideArea(SensorType.A8, 17, 20, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "CA_Circle_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B5, 0, 3),
                BuildSlideArea(SensorType.B6, 4, 5),
                BuildSlideArea(SensorType.E8, 6, 10),
                BuildSlideArea(SensorType.A8, 11, 12),
                BuildSlideArea(SensorType.D1, 13, 14),
                BuildSlideArea(SensorType.A1, 15, 16, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "CA_Circle_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B6, 0, 3),
                BuildSlideArea(SensorType.B7, 4, 4),
                BuildSlideArea(SensorType.D2, 5, 8, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "CA_Circle_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B3, 0, 3),
                BuildSlideArea(SensorType.A3, 4, 8, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "CA_Circle_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B3, 0, 2),
                BuildSlideArea(SensorType.E4, 3, 5),
                BuildSlideArea(SensorType.A4, 6, 9, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "CA_Circle_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B4, 0, 4),
                BuildSlideArea(SensorType.E5, 5, 7),
                BuildSlideArea(SensorType.A5, 8, 11, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "CA_Circle_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B4, 0, 3),
                BuildSlideArea(SensorType.D6, 4, 10),
                BuildSlideArea(SensorType.A6, 11, 13, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "CA_Circle_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B4, 0, 3),
                BuildSlideArea(SensorType.B5, 4, 4),
                BuildSlideArea(SensorType.E6, 5, 7),
                BuildSlideArea(SensorType.D7, 8, 11),
                BuildSlideArea(SensorType.A7, 12, 14, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "CA_Circle_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B5, 0, 4),
                BuildSlideArea(SensorType.B6, 5, 6),
                BuildSlideArea(SensorType.E7, 7, 8),
                BuildSlideArea(SensorType.A7, 9, 11),
                BuildSlideArea(SensorType.D8, 12, 13),
                BuildSlideArea(SensorType.A8, 14, 17, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "CB_Circle_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B7, 0, 5),
                BuildSlideArea(SensorType.B8, 6, 7),
                BuildSlideArea(SensorType.B1, 8, 11, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "CB_Circle_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B8, 0, 4),
                BuildSlideArea(SensorType.B1, 5, 5),
                BuildSlideArea(SensorType.B2, 6, 7, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "CB_Circle_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B3, 0, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "CB_Circle_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B4, 0, 5, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "CB_Circle_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B4, 0, 3),
                BuildSlideArea(SensorType.B5, 4, 6, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "CB_Circle_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B5, 0, 4),
                BuildSlideArea(SensorType.B6, 5, 8, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "CB_Circle_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B6, 0, 5),
                BuildSlideArea(SensorType.B7, 6, 9, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "CB_Circle_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B7, 0, 6),
                BuildSlideArea(SensorType.B8, 7, 10, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "CD_Circle_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B5, 0, 4),
                BuildSlideArea(SensorType.B6, 5, 5),
                BuildSlideArea(SensorType.E7, 6, 8),
                BuildSlideArea(SensorType.E8, 9, 12),
                BuildSlideArea(SensorType.A8, 13, 14),
                BuildSlideArea(SensorType.D1, 15, 17, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "CD_Circle_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B5, 0, 3),
                BuildSlideArea(SensorType.B6, 4, 4),
                BuildSlideArea(SensorType.B7, 5, 7),
                BuildSlideArea(SensorType.E8, 8, 8),
                BuildSlideArea(SensorType.A1, 9, 11),
                BuildSlideArea(SensorType.D2, 12, 13, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "CD_Circle_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B6, 0, 5),
                BuildSlideArea(SensorType.B7, 6, 8),
                BuildSlideArea(SensorType.E1, 9, 15),
                BuildSlideArea(SensorType.A1, 16, 19),
                BuildSlideArea(SensorType.D2, 20, 21),
                BuildSlideArea(SensorType.A2, 22, 23),
                BuildSlideArea(SensorType.D3, 24, 28, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "CD_Circle_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B3, 0, 2),
                BuildSlideArea(SensorType.D4, 3, 7, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "CD_Circle_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D5, 0, 10, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "CD_Circle_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B4, 0, 3),
                BuildSlideArea(SensorType.E5, 4, 6),
                BuildSlideArea(SensorType.A5, 7, 9),
                BuildSlideArea(SensorType.D6, 10, 12, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "CD_Circle_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B4, 0, 3),
                BuildSlideArea(SensorType.B5, 4, 5),
                BuildSlideArea(SensorType.E6, 6, 8),
                BuildSlideArea(SensorType.A6, 9, 11),
                BuildSlideArea(SensorType.D7, 12, 14, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "CD_Circle_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B5, 0, 4),
                BuildSlideArea(SensorType.E7, 5, 10),
                BuildSlideArea(SensorType.A7, 11, 11),
                BuildSlideArea(SensorType.D8, 12, 16, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "CE_Circle_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B6, 0, 5),
                BuildSlideArea(SensorType.B7, 6, 7),
                BuildSlideArea(SensorType.B8, 8, 10),
                BuildSlideArea(SensorType.E1, 11, 13, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "CE_Circle_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B6, 0, 3),
                BuildSlideArea(SensorType.B7, 4, 5),
                BuildSlideArea(SensorType.B8, 6, 11, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "CE_Circle_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B7, 0, 6),
                BuildSlideArea(SensorType.B8, 7, 9),
                BuildSlideArea(SensorType.B1, 10, 13),
                BuildSlideArea(SensorType.E2, 14, 16),
                BuildSlideArea(SensorType.E3, 17, 21, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "CE_Circle_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B3, 0, 2),
                BuildSlideArea(SensorType.E4, 3, 6, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "CE_Circle_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B4, 0, 7, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "CE_Circle_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B4, 0, 3),
                BuildSlideArea(SensorType.B5, 4, 5),
                BuildSlideArea(SensorType.E6, 6, 9, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "CE_Circle_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B5, 0, 4),
                BuildSlideArea(SensorType.B6, 5, 7),
                BuildSlideArea(SensorType.E7, 8, 11, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "CE_Circle_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B5, 0, 4),
                BuildSlideArea(SensorType.B6, 5, 6),
                BuildSlideArea(SensorType.B7, 7, 10),
                BuildSlideArea(SensorType.E8, 11, 12, true, true)
            },
            Const = 0.1f
        },
new SlideTable()
        {
            Name = "1A_Circle_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 5),
                BuildSlideArea(SensorType.D3, 6, 8),
                BuildSlideArea(SensorType.A3, 9, 11),
                BuildSlideArea(SensorType.D4, 12, 14),
                BuildSlideArea(SensorType.A4, 15, 18),
                BuildSlideArea(SensorType.D5, 19, 21),
                BuildSlideArea(SensorType.A5, 22, 24),
                BuildSlideArea(SensorType.D6, 25, 28),
                BuildSlideArea(SensorType.A6, 29, 31),
                BuildSlideArea(SensorType.D7, 32, 34),
                BuildSlideArea(SensorType.A7, 35, 38),
                BuildSlideArea(SensorType.D8, 39, 41),
                BuildSlideArea(SensorType.A8, 42, 44),
                BuildSlideArea(SensorType.D1, 45, 48),
                BuildSlideArea(SensorType.A1, 49, 53, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1A_Circle_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1A_Circle_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 4),
                BuildSlideArea(SensorType.D3, 5, 8),
                BuildSlideArea(SensorType.A3, 9, 13, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1A_Circle_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 4),
                BuildSlideArea(SensorType.D3, 5, 7),
                BuildSlideArea(SensorType.A3, 8, 10),
                BuildSlideArea(SensorType.D4, 11, 13),
                BuildSlideArea(SensorType.A4, 14, 17, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1A_Circle_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 5),
                BuildSlideArea(SensorType.D3, 6, 9),
                BuildSlideArea(SensorType.A3, 10, 13),
                BuildSlideArea(SensorType.D4, 14, 17),
                BuildSlideArea(SensorType.A4, 18, 21),
                BuildSlideArea(SensorType.D5, 22, 25),
                BuildSlideArea(SensorType.A5, 26, 29, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1A_Circle_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 5),
                BuildSlideArea(SensorType.D3, 6, 8),
                BuildSlideArea(SensorType.A3, 9, 11),
                BuildSlideArea(SensorType.D4, 12, 14),
                BuildSlideArea(SensorType.A4, 15, 18),
                BuildSlideArea(SensorType.D5, 19, 21),
                BuildSlideArea(SensorType.A5, 22, 24),
                BuildSlideArea(SensorType.D6, 25, 28),
                BuildSlideArea(SensorType.A6, 29, 33, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1A_Circle_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 4),
                BuildSlideArea(SensorType.D3, 5, 6),
                BuildSlideArea(SensorType.A3, 7, 9),
                BuildSlideArea(SensorType.D4, 10, 12),
                BuildSlideArea(SensorType.A4, 13, 15),
                BuildSlideArea(SensorType.D5, 16, 17),
                BuildSlideArea(SensorType.A5, 18, 20),
                BuildSlideArea(SensorType.D6, 21, 23),
                BuildSlideArea(SensorType.A6, 24, 26),
                BuildSlideArea(SensorType.D7, 27, 28),
                BuildSlideArea(SensorType.A7, 29, 33, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1A_Circle_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 3),
                BuildSlideArea(SensorType.D3, 4, 5),
                BuildSlideArea(SensorType.A3, 6, 7),
                BuildSlideArea(SensorType.D4, 8, 9),
                BuildSlideArea(SensorType.A4, 10, 12),
                BuildSlideArea(SensorType.D5, 13, 14),
                BuildSlideArea(SensorType.A5, 15, 16),
                BuildSlideArea(SensorType.D6, 17, 18),
                BuildSlideArea(SensorType.A6, 19, 20),
                BuildSlideArea(SensorType.D7, 21, 23),
                BuildSlideArea(SensorType.A7, 24, 25),
                BuildSlideArea(SensorType.D8, 26, 27),
                BuildSlideArea(SensorType.A8, 28, 31, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1B_Circle_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 4),
                BuildSlideArea(SensorType.D3, 5, 8),
                BuildSlideArea(SensorType.A3, 9, 11),
                BuildSlideArea(SensorType.D4, 12, 14),
                BuildSlideArea(SensorType.A4, 15, 18),
                BuildSlideArea(SensorType.D5, 19, 21),
                BuildSlideArea(SensorType.A5, 22, 24),
                BuildSlideArea(SensorType.E6, 25, 27),
                BuildSlideArea(SensorType.E7, 28, 30),
                BuildSlideArea(SensorType.E8, 31, 34),
                BuildSlideArea(SensorType.B8, 35, 35),
                BuildSlideArea(SensorType.B1, 36, 39, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1B_Circle_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 0),
                BuildSlideArea(SensorType.E2, 1, 2),
                BuildSlideArea(SensorType.B2, 3, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1B_Circle_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 4),
                BuildSlideArea(SensorType.E3, 5, 8),
                BuildSlideArea(SensorType.B3, 9, 12, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1B_Circle_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 4),
                BuildSlideArea(SensorType.D3, 5, 7),
                BuildSlideArea(SensorType.E3, 8, 9),
                BuildSlideArea(SensorType.E4, 10, 13),
                BuildSlideArea(SensorType.B4, 14, 16, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1B_Circle_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 5),
                BuildSlideArea(SensorType.D3, 6, 9),
                BuildSlideArea(SensorType.A3, 10, 11),
                BuildSlideArea(SensorType.E4, 12, 14),
                BuildSlideArea(SensorType.E5, 15, 19),
                BuildSlideArea(SensorType.B5, 20, 22, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1B_Circle_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 4),
                BuildSlideArea(SensorType.D3, 5, 8),
                BuildSlideArea(SensorType.A3, 9, 11),
                BuildSlideArea(SensorType.D4, 12, 15),
                BuildSlideArea(SensorType.E4, 16, 16),
                BuildSlideArea(SensorType.A4, 17, 18),
                BuildSlideArea(SensorType.E5, 19, 20),
                BuildSlideArea(SensorType.B5, 21, 23),
                BuildSlideArea(SensorType.B6, 24, 27, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1B_Circle_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 4),
                BuildSlideArea(SensorType.D3, 5, 6),
                BuildSlideArea(SensorType.A3, 7, 9),
                BuildSlideArea(SensorType.D4, 10, 12),
                BuildSlideArea(SensorType.A4, 13, 15),
                BuildSlideArea(SensorType.E5, 16, 18),
                BuildSlideArea(SensorType.E6, 19, 23),
                BuildSlideArea(SensorType.B6, 24, 26),
                BuildSlideArea(SensorType.B7, 27, 30, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1B_Circle_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 3),
                BuildSlideArea(SensorType.D3, 4, 5),
                BuildSlideArea(SensorType.A3, 6, 7),
                BuildSlideArea(SensorType.D4, 8, 9),
                BuildSlideArea(SensorType.A4, 10, 12),
                BuildSlideArea(SensorType.D5, 13, 14),
                BuildSlideArea(SensorType.A5, 15, 16),
                BuildSlideArea(SensorType.E6, 17, 19),
                BuildSlideArea(SensorType.E7, 20, 23),
                BuildSlideArea(SensorType.B7, 24, 25),
                BuildSlideArea(SensorType.B8, 26, 30, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1D_Circle_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 5),
                BuildSlideArea(SensorType.D3, 6, 9),
                BuildSlideArea(SensorType.A3, 10, 13),
                BuildSlideArea(SensorType.D4, 14, 17),
                BuildSlideArea(SensorType.A4, 18, 21),
                BuildSlideArea(SensorType.D5, 22, 25),
                BuildSlideArea(SensorType.A5, 26, 28),
                BuildSlideArea(SensorType.D6, 29, 32),
                BuildSlideArea(SensorType.A6, 33, 36),
                BuildSlideArea(SensorType.D7, 37, 40),
                BuildSlideArea(SensorType.A7, 41, 44),
                BuildSlideArea(SensorType.D8, 45, 48),
                BuildSlideArea(SensorType.A8, 49, 50),
                BuildSlideArea(SensorType.D1, 51, 53, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1D_Circle_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 2, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1D_Circle_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 5),
                BuildSlideArea(SensorType.D3, 6, 10, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1D_Circle_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 4),
                BuildSlideArea(SensorType.D3, 5, 7),
                BuildSlideArea(SensorType.A3, 8, 10),
                BuildSlideArea(SensorType.D4, 11, 15, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1D_Circle_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 2),
                BuildSlideArea(SensorType.A2, 3, 6),
                BuildSlideArea(SensorType.D3, 7, 10),
                BuildSlideArea(SensorType.A3, 11, 13),
                BuildSlideArea(SensorType.D4, 14, 16),
                BuildSlideArea(SensorType.A4, 17, 19),
                BuildSlideArea(SensorType.D5, 20, 23, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1D_Circle_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 5),
                BuildSlideArea(SensorType.D3, 6, 8),
                BuildSlideArea(SensorType.A3, 9, 12),
                BuildSlideArea(SensorType.D4, 13, 16),
                BuildSlideArea(SensorType.A4, 17, 19),
                BuildSlideArea(SensorType.D5, 20, 23),
                BuildSlideArea(SensorType.A5, 24, 26),
                BuildSlideArea(SensorType.D6, 27, 32, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1D_Circle_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 4),
                BuildSlideArea(SensorType.D3, 5, 7),
                BuildSlideArea(SensorType.A3, 8, 10),
                BuildSlideArea(SensorType.D4, 11, 13),
                BuildSlideArea(SensorType.A4, 14, 16),
                BuildSlideArea(SensorType.D5, 17, 19),
                BuildSlideArea(SensorType.A5, 20, 22),
                BuildSlideArea(SensorType.D6, 23, 25),
                BuildSlideArea(SensorType.A6, 26, 28),
                BuildSlideArea(SensorType.D7, 29, 33, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1D_Circle_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 3),
                BuildSlideArea(SensorType.D3, 4, 6),
                BuildSlideArea(SensorType.A3, 7, 8),
                BuildSlideArea(SensorType.D4, 9, 11),
                BuildSlideArea(SensorType.A4, 12, 13),
                BuildSlideArea(SensorType.D5, 14, 16),
                BuildSlideArea(SensorType.A5, 17, 18),
                BuildSlideArea(SensorType.D6, 19, 21),
                BuildSlideArea(SensorType.A6, 22, 23),
                BuildSlideArea(SensorType.D7, 24, 26),
                BuildSlideArea(SensorType.A7, 27, 28),
                BuildSlideArea(SensorType.D8, 29, 32, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1E_Circle_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 5),
                BuildSlideArea(SensorType.D3, 6, 9),
                BuildSlideArea(SensorType.A3, 10, 13),
                BuildSlideArea(SensorType.D4, 14, 17),
                BuildSlideArea(SensorType.A4, 18, 20),
                BuildSlideArea(SensorType.D5, 21, 22),
                BuildSlideArea(SensorType.A5, 23, 23),
                BuildSlideArea(SensorType.D6, 24, 26),
                BuildSlideArea(SensorType.A6, 27, 27),
                BuildSlideArea(SensorType.D7, 28, 30),
                BuildSlideArea(SensorType.A7, 31, 32),
                BuildSlideArea(SensorType.E8, 33, 34),
                BuildSlideArea(SensorType.E1, 35, 38, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1E_Circle_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 2, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1E_Circle_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 5),
                BuildSlideArea(SensorType.E3, 6, 9, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1E_Circle_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 4),
                BuildSlideArea(SensorType.D3, 5, 7),
                BuildSlideArea(SensorType.A3, 8, 11),
                BuildSlideArea(SensorType.E4, 12, 15, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1E_Circle_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 5),
                BuildSlideArea(SensorType.D3, 6, 8),
                BuildSlideArea(SensorType.A3, 9, 11),
                BuildSlideArea(SensorType.D4, 12, 14),
                BuildSlideArea(SensorType.E4, 15, 15),
                BuildSlideArea(SensorType.E5, 16, 21, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1E_Circle_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 5),
                BuildSlideArea(SensorType.D3, 6, 8),
                BuildSlideArea(SensorType.A3, 9, 12),
                BuildSlideArea(SensorType.D4, 13, 16),
                BuildSlideArea(SensorType.A4, 17, 18),
                BuildSlideArea(SensorType.E5, 19, 21),
                BuildSlideArea(SensorType.E6, 22, 27, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1E_Circle_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 4),
                BuildSlideArea(SensorType.D3, 5, 7),
                BuildSlideArea(SensorType.A3, 8, 10),
                BuildSlideArea(SensorType.D4, 11, 13),
                BuildSlideArea(SensorType.A4, 14, 16),
                BuildSlideArea(SensorType.D5, 17, 20),
                BuildSlideArea(SensorType.A5, 21, 22),
                BuildSlideArea(SensorType.E6, 23, 26),
                BuildSlideArea(SensorType.E7, 27, 31, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "1E_Circle_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 3),
                BuildSlideArea(SensorType.D3, 4, 6),
                BuildSlideArea(SensorType.A3, 7, 8),
                BuildSlideArea(SensorType.D4, 9, 11),
                BuildSlideArea(SensorType.A4, 12, 13),
                BuildSlideArea(SensorType.D5, 14, 16),
                BuildSlideArea(SensorType.A5, 17, 18),
                BuildSlideArea(SensorType.D6, 19, 21),
                BuildSlideArea(SensorType.A6, 22, 23),
                BuildSlideArea(SensorType.E7, 24, 26),
                BuildSlideArea(SensorType.E8, 27, 32, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "A1_Circle_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 4),
                BuildSlideArea(SensorType.D3, 5, 8),
                BuildSlideArea(SensorType.A3, 9, 11),
                BuildSlideArea(SensorType.D4, 12, 14),
                BuildSlideArea(SensorType.A4, 15, 18),
                BuildSlideArea(SensorType.D5, 19, 21),
                BuildSlideArea(SensorType.A5, 22, 24),
                BuildSlideArea(SensorType.D6, 25, 28),
                BuildSlideArea(SensorType.A6, 29, 31),
                BuildSlideArea(SensorType.D7, 32, 34),
                BuildSlideArea(SensorType.A7, 35, 38),
                BuildSlideArea(SensorType.D8, 39, 41),
                BuildSlideArea(SensorType.A8, 42, 44),
                BuildSlideArea(SensorType.D1, 45, 48),
                BuildSlideArea(SensorType.A1, 49, 53, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "A1_Circle_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 0),
                BuildSlideArea(SensorType.A2, 1, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "A1_Circle_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 4),
                BuildSlideArea(SensorType.D3, 5, 8),
                BuildSlideArea(SensorType.A3, 9, 13, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "A1_Circle_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 4),
                BuildSlideArea(SensorType.D3, 5, 7),
                BuildSlideArea(SensorType.A3, 8, 10),
                BuildSlideArea(SensorType.D4, 11, 13),
                BuildSlideArea(SensorType.A4, 14, 17, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "A1_Circle_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 4),
                BuildSlideArea(SensorType.D3, 5, 7),
                BuildSlideArea(SensorType.A3, 8, 11),
                BuildSlideArea(SensorType.D4, 12, 15),
                BuildSlideArea(SensorType.A4, 16, 19),
                BuildSlideArea(SensorType.D5, 20, 23),
                BuildSlideArea(SensorType.A5, 24, 29, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "A1_Circle_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 4),
                BuildSlideArea(SensorType.D3, 5, 8),
                BuildSlideArea(SensorType.A3, 9, 11),
                BuildSlideArea(SensorType.D4, 12, 14),
                BuildSlideArea(SensorType.A4, 15, 18),
                BuildSlideArea(SensorType.D5, 19, 21),
                BuildSlideArea(SensorType.A5, 22, 24),
                BuildSlideArea(SensorType.D6, 25, 28),
                BuildSlideArea(SensorType.A6, 29, 33, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "A1_Circle_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 3),
                BuildSlideArea(SensorType.D3, 4, 6),
                BuildSlideArea(SensorType.A3, 7, 9),
                BuildSlideArea(SensorType.D4, 10, 12),
                BuildSlideArea(SensorType.A4, 13, 15),
                BuildSlideArea(SensorType.D5, 16, 17),
                BuildSlideArea(SensorType.A5, 18, 20),
                BuildSlideArea(SensorType.D6, 21, 23),
                BuildSlideArea(SensorType.A6, 24, 26),
                BuildSlideArea(SensorType.D7, 27, 29),
                BuildSlideArea(SensorType.A7, 30, 33, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "A1_Circle_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 0),
                BuildSlideArea(SensorType.A2, 1, 3),
                BuildSlideArea(SensorType.D3, 4, 5),
                BuildSlideArea(SensorType.A3, 6, 7),
                BuildSlideArea(SensorType.D4, 8, 9),
                BuildSlideArea(SensorType.A4, 10, 12),
                BuildSlideArea(SensorType.D5, 13, 14),
                BuildSlideArea(SensorType.A5, 15, 16),
                BuildSlideArea(SensorType.D6, 17, 18),
                BuildSlideArea(SensorType.A6, 19, 21),
                BuildSlideArea(SensorType.D7, 22, 23),
                BuildSlideArea(SensorType.A7, 24, 25),
                BuildSlideArea(SensorType.D8, 26, 27),
                BuildSlideArea(SensorType.A8, 28, 31, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AA_Circle_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 4),
                BuildSlideArea(SensorType.D3, 5, 8),
                BuildSlideArea(SensorType.A3, 9, 11),
                BuildSlideArea(SensorType.D4, 12, 14),
                BuildSlideArea(SensorType.A4, 15, 18),
                BuildSlideArea(SensorType.D5, 19, 21),
                BuildSlideArea(SensorType.A5, 22, 24),
                BuildSlideArea(SensorType.D6, 25, 28),
                BuildSlideArea(SensorType.A6, 29, 31),
                BuildSlideArea(SensorType.D7, 32, 34),
                BuildSlideArea(SensorType.A7, 35, 38),
                BuildSlideArea(SensorType.D8, 39, 41),
                BuildSlideArea(SensorType.A8, 42, 44),
                BuildSlideArea(SensorType.D1, 45, 48),
                BuildSlideArea(SensorType.A1, 49, 53, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AA_Circle_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 0),
                BuildSlideArea(SensorType.A2, 1, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AA_Circle_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 4),
                BuildSlideArea(SensorType.D3, 5, 8),
                BuildSlideArea(SensorType.A3, 9, 13, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AA_Circle_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 4),
                BuildSlideArea(SensorType.D3, 5, 7),
                BuildSlideArea(SensorType.A3, 8, 10),
                BuildSlideArea(SensorType.D4, 11, 13),
                BuildSlideArea(SensorType.A4, 14, 17, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AA_Circle_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 3),
                BuildSlideArea(SensorType.D3, 4, 6),
                BuildSlideArea(SensorType.A3, 7, 8),
                BuildSlideArea(SensorType.D4, 9, 11),
                BuildSlideArea(SensorType.A4, 12, 14),
                BuildSlideArea(SensorType.D5, 15, 16),
                BuildSlideArea(SensorType.A5, 17, 20, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AA_Circle_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 4),
                BuildSlideArea(SensorType.D3, 5, 8),
                BuildSlideArea(SensorType.A3, 9, 11),
                BuildSlideArea(SensorType.D4, 12, 14),
                BuildSlideArea(SensorType.A4, 15, 18),
                BuildSlideArea(SensorType.D5, 19, 21),
                BuildSlideArea(SensorType.A5, 22, 24),
                BuildSlideArea(SensorType.D6, 25, 28),
                BuildSlideArea(SensorType.A6, 29, 33, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AA_Circle_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 3),
                BuildSlideArea(SensorType.D3, 4, 6),
                BuildSlideArea(SensorType.A3, 7, 9),
                BuildSlideArea(SensorType.D4, 10, 12),
                BuildSlideArea(SensorType.A4, 13, 15),
                BuildSlideArea(SensorType.D5, 16, 17),
                BuildSlideArea(SensorType.A5, 18, 20),
                BuildSlideArea(SensorType.D6, 21, 23),
                BuildSlideArea(SensorType.A6, 24, 26),
                BuildSlideArea(SensorType.D7, 27, 29),
                BuildSlideArea(SensorType.A7, 30, 33, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AA_Circle_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 0),
                BuildSlideArea(SensorType.A2, 1, 3),
                BuildSlideArea(SensorType.D3, 4, 5),
                BuildSlideArea(SensorType.A3, 6, 7),
                BuildSlideArea(SensorType.D4, 8, 9),
                BuildSlideArea(SensorType.A4, 10, 12),
                BuildSlideArea(SensorType.D5, 13, 14),
                BuildSlideArea(SensorType.A5, 15, 16),
                BuildSlideArea(SensorType.D6, 17, 18),
                BuildSlideArea(SensorType.A6, 19, 20),
                BuildSlideArea(SensorType.D7, 21, 23),
                BuildSlideArea(SensorType.A7, 24, 25),
                BuildSlideArea(SensorType.D8, 26, 27),
                BuildSlideArea(SensorType.A8, 28, 31, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AB_Circle_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 4),
                BuildSlideArea(SensorType.D3, 5, 8),
                BuildSlideArea(SensorType.A3, 9, 11),
                BuildSlideArea(SensorType.D4, 12, 15),
                BuildSlideArea(SensorType.A4, 16, 16),
                BuildSlideArea(SensorType.E5, 17, 18),
                BuildSlideArea(SensorType.E6, 19, 21),
                BuildSlideArea(SensorType.E7, 22, 25),
                BuildSlideArea(SensorType.B7, 26, 27),
                BuildSlideArea(SensorType.E8, 28, 28),
                BuildSlideArea(SensorType.B8, 29, 29),
                BuildSlideArea(SensorType.B1, 30, 34, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AB_Circle_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 1),
                BuildSlideArea(SensorType.B2, 2, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AB_Circle_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.E3, 2, 7),
                BuildSlideArea(SensorType.B3, 8, 11, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AB_Circle_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 4),
                BuildSlideArea(SensorType.E3, 5, 7),
                BuildSlideArea(SensorType.E4, 8, 12),
                BuildSlideArea(SensorType.B4, 13, 15, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AB_Circle_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 3),
                BuildSlideArea(SensorType.E3, 4, 7),
                BuildSlideArea(SensorType.E4, 8, 11),
                BuildSlideArea(SensorType.B4, 12, 15),
                BuildSlideArea(SensorType.B5, 16, 19, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AB_Circle_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 4),
                BuildSlideArea(SensorType.D3, 5, 8),
                BuildSlideArea(SensorType.E3, 9, 9),
                BuildSlideArea(SensorType.A3, 10, 11),
                BuildSlideArea(SensorType.E4, 12, 13),
                BuildSlideArea(SensorType.E5, 14, 17),
                BuildSlideArea(SensorType.B5, 18, 20),
                BuildSlideArea(SensorType.B6, 21, 25, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AB_Circle_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 3),
                BuildSlideArea(SensorType.D3, 4, 7),
                BuildSlideArea(SensorType.A3, 8, 9),
                BuildSlideArea(SensorType.E4, 10, 13),
                BuildSlideArea(SensorType.E5, 14, 18),
                BuildSlideArea(SensorType.E6, 19, 22),
                BuildSlideArea(SensorType.B6, 23, 24),
                BuildSlideArea(SensorType.B7, 25, 29, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AB_Circle_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 3),
                BuildSlideArea(SensorType.D3, 4, 5),
                BuildSlideArea(SensorType.A3, 6, 7),
                BuildSlideArea(SensorType.D4, 8, 10),
                BuildSlideArea(SensorType.A4, 11, 12),
                BuildSlideArea(SensorType.E5, 13, 14),
                BuildSlideArea(SensorType.E6, 15, 19),
                BuildSlideArea(SensorType.E7, 20, 23),
                BuildSlideArea(SensorType.B7, 24, 25),
                BuildSlideArea(SensorType.B8, 26, 30, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AD_Circle_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 0),
                BuildSlideArea(SensorType.A2, 1, 2),
                BuildSlideArea(SensorType.D3, 3, 4),
                BuildSlideArea(SensorType.A3, 5, 6),
                BuildSlideArea(SensorType.D4, 7, 8),
                BuildSlideArea(SensorType.A4, 9, 10),
                BuildSlideArea(SensorType.D5, 11, 12),
                BuildSlideArea(SensorType.A5, 13, 14),
                BuildSlideArea(SensorType.D6, 15, 16),
                BuildSlideArea(SensorType.A6, 17, 18),
                BuildSlideArea(SensorType.D7, 19, 20),
                BuildSlideArea(SensorType.A7, 21, 22),
                BuildSlideArea(SensorType.D8, 23, 24),
                BuildSlideArea(SensorType.A8, 25, 26),
                BuildSlideArea(SensorType.D1, 27, 29, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AD_Circle_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 2, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AD_Circle_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 4),
                BuildSlideArea(SensorType.D3, 5, 10, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AD_Circle_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 4),
                BuildSlideArea(SensorType.D3, 5, 7),
                BuildSlideArea(SensorType.A3, 8, 10),
                BuildSlideArea(SensorType.D4, 11, 15, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AD_Circle_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 3),
                BuildSlideArea(SensorType.D3, 4, 6),
                BuildSlideArea(SensorType.A3, 7, 9),
                BuildSlideArea(SensorType.D4, 10, 12),
                BuildSlideArea(SensorType.A4, 13, 15),
                BuildSlideArea(SensorType.D5, 16, 19, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AD_Circle_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 5),
                BuildSlideArea(SensorType.D3, 6, 8),
                BuildSlideArea(SensorType.A3, 9, 12),
                BuildSlideArea(SensorType.D4, 13, 16),
                BuildSlideArea(SensorType.A4, 17, 19),
                BuildSlideArea(SensorType.D5, 20, 23),
                BuildSlideArea(SensorType.A5, 24, 26),
                BuildSlideArea(SensorType.D6, 27, 32, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AD_Circle_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 4),
                BuildSlideArea(SensorType.D3, 5, 7),
                BuildSlideArea(SensorType.A3, 8, 10),
                BuildSlideArea(SensorType.D4, 11, 13),
                BuildSlideArea(SensorType.A4, 14, 16),
                BuildSlideArea(SensorType.D5, 17, 19),
                BuildSlideArea(SensorType.A5, 20, 22),
                BuildSlideArea(SensorType.D6, 23, 25),
                BuildSlideArea(SensorType.A6, 26, 28),
                BuildSlideArea(SensorType.D7, 29, 33, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AD_Circle_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 3),
                BuildSlideArea(SensorType.D3, 4, 6),
                BuildSlideArea(SensorType.A3, 7, 8),
                BuildSlideArea(SensorType.D4, 9, 11),
                BuildSlideArea(SensorType.A4, 12, 13),
                BuildSlideArea(SensorType.D5, 14, 16),
                BuildSlideArea(SensorType.A5, 17, 18),
                BuildSlideArea(SensorType.D6, 19, 21),
                BuildSlideArea(SensorType.A6, 22, 23),
                BuildSlideArea(SensorType.D7, 24, 26),
                BuildSlideArea(SensorType.A7, 27, 28),
                BuildSlideArea(SensorType.D8, 29, 32, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AE_Circle_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 0),
                BuildSlideArea(SensorType.A2, 1, 2),
                BuildSlideArea(SensorType.D3, 3, 4),
                BuildSlideArea(SensorType.A3, 5, 6),
                BuildSlideArea(SensorType.D4, 7, 8),
                BuildSlideArea(SensorType.A4, 9, 10),
                BuildSlideArea(SensorType.D5, 11, 12),
                BuildSlideArea(SensorType.A5, 13, 14),
                BuildSlideArea(SensorType.E6, 15, 16),
                BuildSlideArea(SensorType.A6, 17, 18),
                BuildSlideArea(SensorType.E7, 19, 20),
                BuildSlideArea(SensorType.E8, 21, 24),
                BuildSlideArea(SensorType.E1, 25, 29, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AE_Circle_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 2, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AE_Circle_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 5),
                BuildSlideArea(SensorType.E3, 6, 8, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AE_Circle_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 4),
                BuildSlideArea(SensorType.E3, 5, 8),
                BuildSlideArea(SensorType.E4, 9, 14, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AE_Circle_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 3),
                BuildSlideArea(SensorType.D3, 4, 7),
                BuildSlideArea(SensorType.A3, 8, 9),
                BuildSlideArea(SensorType.E4, 10, 12),
                BuildSlideArea(SensorType.E5, 13, 19, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AE_Circle_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 5),
                BuildSlideArea(SensorType.D3, 6, 8),
                BuildSlideArea(SensorType.A3, 9, 10),
                BuildSlideArea(SensorType.D4, 11, 13),
                BuildSlideArea(SensorType.A4, 14, 15),
                BuildSlideArea(SensorType.E5, 16, 17),
                BuildSlideArea(SensorType.E6, 18, 23, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AE_Circle_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 4),
                BuildSlideArea(SensorType.D3, 5, 7),
                BuildSlideArea(SensorType.A3, 8, 10),
                BuildSlideArea(SensorType.D4, 11, 14),
                BuildSlideArea(SensorType.A4, 15, 17),
                BuildSlideArea(SensorType.E5, 18, 20),
                BuildSlideArea(SensorType.A5, 21, 23),
                BuildSlideArea(SensorType.E6, 24, 25),
                BuildSlideArea(SensorType.E7, 26, 30, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "AE_Circle_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 3),
                BuildSlideArea(SensorType.D3, 4, 6),
                BuildSlideArea(SensorType.A3, 7, 8),
                BuildSlideArea(SensorType.D4, 9, 11),
                BuildSlideArea(SensorType.A4, 12, 13),
                BuildSlideArea(SensorType.D5, 14, 16),
                BuildSlideArea(SensorType.E5, 17, 17),
                BuildSlideArea(SensorType.A5, 18, 19),
                BuildSlideArea(SensorType.E6, 20, 21),
                BuildSlideArea(SensorType.E7, 22, 26),
                BuildSlideArea(SensorType.E8, 27, 32, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "B1_Circle_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B2, 0, 2),
                BuildSlideArea(SensorType.E3, 3, 4),
                BuildSlideArea(SensorType.E4, 5, 7),
                BuildSlideArea(SensorType.E5, 8, 11),
                BuildSlideArea(SensorType.A5, 12, 12),
                BuildSlideArea(SensorType.D6, 13, 14),
                BuildSlideArea(SensorType.A6, 15, 17),
                BuildSlideArea(SensorType.D7, 18, 20),
                BuildSlideArea(SensorType.A7, 21, 24),
                BuildSlideArea(SensorType.D8, 25, 27),
                BuildSlideArea(SensorType.A8, 28, 30),
                BuildSlideArea(SensorType.D1, 31, 34),
                BuildSlideArea(SensorType.A1, 35, 39, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "B1_Circle_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 1),
                BuildSlideArea(SensorType.D2, 2, 2),
                BuildSlideArea(SensorType.A2, 3, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "B1_Circle_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 4),
                BuildSlideArea(SensorType.D3, 5, 7),
                BuildSlideArea(SensorType.A3, 8, 12, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "B1_Circle_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 1),
                BuildSlideArea(SensorType.E3, 2, 6),
                BuildSlideArea(SensorType.D3, 7, 7),
                BuildSlideArea(SensorType.A3, 8, 8),
                BuildSlideArea(SensorType.D4, 9, 11),
                BuildSlideArea(SensorType.A4, 12, 16, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "B1_Circle_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 1),
                BuildSlideArea(SensorType.E3, 2, 5),
                BuildSlideArea(SensorType.A3, 6, 8),
                BuildSlideArea(SensorType.D4, 9, 10),
                BuildSlideArea(SensorType.A4, 11, 13),
                BuildSlideArea(SensorType.D5, 14, 16),
                BuildSlideArea(SensorType.A5, 17, 22, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "B1_Circle_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B2, 0, 3),
                BuildSlideArea(SensorType.E3, 4, 5),
                BuildSlideArea(SensorType.A3, 6, 8),
                BuildSlideArea(SensorType.D4, 9, 10),
                BuildSlideArea(SensorType.A4, 11, 12),
                BuildSlideArea(SensorType.D5, 13, 16),
                BuildSlideArea(SensorType.A5, 17, 19),
                BuildSlideArea(SensorType.D6, 20, 22),
                BuildSlideArea(SensorType.A6, 23, 27, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "B1_Circle_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B2, 0, 2),
                BuildSlideArea(SensorType.E3, 3, 4),
                BuildSlideArea(SensorType.E4, 5, 9),
                BuildSlideArea(SensorType.A4, 10, 12),
                BuildSlideArea(SensorType.D5, 13, 15),
                BuildSlideArea(SensorType.A5, 16, 17),
                BuildSlideArea(SensorType.D6, 18, 20),
                BuildSlideArea(SensorType.A6, 21, 23),
                BuildSlideArea(SensorType.D7, 24, 26),
                BuildSlideArea(SensorType.A7, 27, 30, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "B1_Circle_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B2, 0, 2),
                BuildSlideArea(SensorType.E3, 3, 5),
                BuildSlideArea(SensorType.E4, 6, 9),
                BuildSlideArea(SensorType.A4, 10, 12),
                BuildSlideArea(SensorType.D5, 13, 14),
                BuildSlideArea(SensorType.A5, 15, 16),
                BuildSlideArea(SensorType.D6, 17, 18),
                BuildSlideArea(SensorType.A6, 19, 20),
                BuildSlideArea(SensorType.D7, 21, 22),
                BuildSlideArea(SensorType.A7, 23, 24),
                BuildSlideArea(SensorType.D8, 25, 27),
                BuildSlideArea(SensorType.A8, 28, 30, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BA_Circle_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B2, 0, 2),
                BuildSlideArea(SensorType.E3, 3, 4),
                BuildSlideArea(SensorType.E4, 5, 7),
                BuildSlideArea(SensorType.E5, 8, 10),
                BuildSlideArea(SensorType.E6, 11, 14),
                BuildSlideArea(SensorType.A6, 15, 16),
                BuildSlideArea(SensorType.D7, 17, 17),
                BuildSlideArea(SensorType.A7, 18, 19),
                BuildSlideArea(SensorType.D8, 20, 22),
                BuildSlideArea(SensorType.A8, 23, 25),
                BuildSlideArea(SensorType.D1, 26, 29),
                BuildSlideArea(SensorType.A1, 30, 34, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BA_Circle_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BA_Circle_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 1),
                BuildSlideArea(SensorType.D3, 2, 6),
                BuildSlideArea(SensorType.A3, 7, 11, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BA_Circle_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 1),
                BuildSlideArea(SensorType.E3, 2, 5),
                BuildSlideArea(SensorType.A3, 6, 8),
                BuildSlideArea(SensorType.D4, 9, 11),
                BuildSlideArea(SensorType.A4, 12, 15, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BA_Circle_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B2, 0, 3),
                BuildSlideArea(SensorType.E3, 4, 5),
                BuildSlideArea(SensorType.E4, 6, 10),
                BuildSlideArea(SensorType.A4, 11, 13),
                BuildSlideArea(SensorType.D5, 14, 15),
                BuildSlideArea(SensorType.A5, 16, 19, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BA_Circle_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B2, 0, 2),
                BuildSlideArea(SensorType.E3, 3, 5),
                BuildSlideArea(SensorType.E4, 6, 9),
                BuildSlideArea(SensorType.A4, 10, 12),
                BuildSlideArea(SensorType.D5, 13, 14),
                BuildSlideArea(SensorType.A5, 15, 16),
                BuildSlideArea(SensorType.D6, 17, 19),
                BuildSlideArea(SensorType.A6, 20, 24, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BA_Circle_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B2, 0, 2),
                BuildSlideArea(SensorType.E3, 3, 4),
                BuildSlideArea(SensorType.E4, 5, 8),
                BuildSlideArea(SensorType.E5, 9, 13),
                BuildSlideArea(SensorType.A5, 14, 16),
                BuildSlideArea(SensorType.D6, 17, 19),
                BuildSlideArea(SensorType.A6, 20, 21),
                BuildSlideArea(SensorType.D7, 22, 24),
                BuildSlideArea(SensorType.A7, 25, 28, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BA_Circle_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B2, 0, 2),
                BuildSlideArea(SensorType.E3, 3, 5),
                BuildSlideArea(SensorType.E4, 6, 9),
                BuildSlideArea(SensorType.E5, 10, 14),
                BuildSlideArea(SensorType.A5, 15, 16),
                BuildSlideArea(SensorType.E6, 17, 18),
                BuildSlideArea(SensorType.A6, 19, 20),
                BuildSlideArea(SensorType.D7, 21, 23),
                BuildSlideArea(SensorType.A7, 24, 24),
                BuildSlideArea(SensorType.D8, 25, 27),
                BuildSlideArea(SensorType.A8, 28, 30, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BB_Circle_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B2, 0, 2),
                BuildSlideArea(SensorType.B3, 3, 5),
                BuildSlideArea(SensorType.B4, 6, 8),
                BuildSlideArea(SensorType.B5, 9, 12),
                BuildSlideArea(SensorType.B6, 13, 15),
                BuildSlideArea(SensorType.B7, 16, 18),
                BuildSlideArea(SensorType.B8, 19, 22),
                BuildSlideArea(SensorType.B1, 23, 26, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BB_Circle_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B2, 0, 2, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BB_Circle_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B2, 0, 2),
                BuildSlideArea(SensorType.B3, 3, 8, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BB_Circle_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B2, 0, 2),
                BuildSlideArea(SensorType.B3, 3, 5),
                BuildSlideArea(SensorType.B4, 6, 10, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BB_Circle_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B2, 0, 2),
                BuildSlideArea(SensorType.B3, 3, 6),
                BuildSlideArea(SensorType.B4, 7, 10),
                BuildSlideArea(SensorType.B5, 11, 15, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BB_Circle_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B2, 0, 2),
                BuildSlideArea(SensorType.B3, 3, 5),
                BuildSlideArea(SensorType.B4, 6, 8),
                BuildSlideArea(SensorType.B5, 9, 12),
                BuildSlideArea(SensorType.B6, 13, 16, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BB_Circle_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B2, 0, 2),
                BuildSlideArea(SensorType.B3, 3, 6),
                BuildSlideArea(SensorType.B4, 7, 9),
                BuildSlideArea(SensorType.B5, 10, 13),
                BuildSlideArea(SensorType.B6, 14, 17),
                BuildSlideArea(SensorType.B7, 18, 22, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BB_Circle_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B2, 0, 1),
                BuildSlideArea(SensorType.B3, 2, 4),
                BuildSlideArea(SensorType.B4, 5, 7),
                BuildSlideArea(SensorType.B5, 8, 10),
                BuildSlideArea(SensorType.B6, 11, 13),
                BuildSlideArea(SensorType.B7, 14, 16),
                BuildSlideArea(SensorType.B8, 17, 20, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BD_Circle_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B2, 0, 2),
                BuildSlideArea(SensorType.E3, 3, 5),
                BuildSlideArea(SensorType.E4, 6, 8),
                BuildSlideArea(SensorType.E5, 9, 12),
                BuildSlideArea(SensorType.E6, 13, 16),
                BuildSlideArea(SensorType.A6, 17, 18),
                BuildSlideArea(SensorType.D7, 19, 20),
                BuildSlideArea(SensorType.A7, 21, 22),
                BuildSlideArea(SensorType.D8, 23, 24),
                BuildSlideArea(SensorType.A8, 25, 26),
                BuildSlideArea(SensorType.D1, 27, 29, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BD_Circle_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 2, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BD_Circle_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 1),
                BuildSlideArea(SensorType.A2, 2, 4),
                BuildSlideArea(SensorType.D3, 5, 8, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BD_Circle_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 1),
                BuildSlideArea(SensorType.E3, 2, 5),
                BuildSlideArea(SensorType.A3, 6, 8),
                BuildSlideArea(SensorType.D4, 9, 13, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BD_Circle_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E3, 0, 5),
                BuildSlideArea(SensorType.A3, 6, 9),
                BuildSlideArea(SensorType.D4, 10, 11),
                BuildSlideArea(SensorType.A4, 12, 13),
                BuildSlideArea(SensorType.D5, 14, 17, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BD_Circle_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B2, 0, 2),
                BuildSlideArea(SensorType.E3, 3, 5),
                BuildSlideArea(SensorType.E4, 6, 10),
                BuildSlideArea(SensorType.A4, 11, 13),
                BuildSlideArea(SensorType.D5, 14, 15),
                BuildSlideArea(SensorType.A5, 16, 17),
                BuildSlideArea(SensorType.D6, 18, 22, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BD_Circle_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B2, 0, 2),
                BuildSlideArea(SensorType.E3, 3, 5),
                BuildSlideArea(SensorType.E4, 6, 9),
                BuildSlideArea(SensorType.E5, 10, 14),
                BuildSlideArea(SensorType.A5, 15, 17),
                BuildSlideArea(SensorType.D6, 18, 20),
                BuildSlideArea(SensorType.A6, 21, 22),
                BuildSlideArea(SensorType.D7, 23, 27, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BD_Circle_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B2, 0, 2),
                BuildSlideArea(SensorType.E3, 3, 4),
                BuildSlideArea(SensorType.E4, 5, 8),
                BuildSlideArea(SensorType.E5, 9, 13),
                BuildSlideArea(SensorType.A5, 14, 16),
                BuildSlideArea(SensorType.D6, 17, 19),
                BuildSlideArea(SensorType.A6, 20, 21),
                BuildSlideArea(SensorType.D7, 22, 23),
                BuildSlideArea(SensorType.A7, 24, 26),
                BuildSlideArea(SensorType.D8, 27, 30, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BE_Circle_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B2, 0, 2),
                BuildSlideArea(SensorType.B3, 3, 6),
                BuildSlideArea(SensorType.B4, 7, 10),
                BuildSlideArea(SensorType.E5, 11, 13),
                BuildSlideArea(SensorType.B5, 14, 14),
                BuildSlideArea(SensorType.E6, 15, 16),
                BuildSlideArea(SensorType.B6, 17, 18),
                BuildSlideArea(SensorType.E7, 19, 20),
                BuildSlideArea(SensorType.E8, 21, 24),
                BuildSlideArea(SensorType.E1, 25, 29, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BE_Circle_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 1, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BE_Circle_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B2, 0, 3),
                BuildSlideArea(SensorType.E3, 4, 6, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BE_Circle_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B2, 0, 2),
                BuildSlideArea(SensorType.E3, 3, 5),
                BuildSlideArea(SensorType.E4, 6, 11, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BE_Circle_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B2, 0, 2),
                BuildSlideArea(SensorType.E3, 3, 5),
                BuildSlideArea(SensorType.B3, 6, 7),
                BuildSlideArea(SensorType.E4, 8, 9),
                BuildSlideArea(SensorType.E5, 10, 14, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BE_Circle_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B2, 0, 2),
                BuildSlideArea(SensorType.B3, 3, 6),
                BuildSlideArea(SensorType.E4, 7, 9),
                BuildSlideArea(SensorType.B4, 10, 11),
                BuildSlideArea(SensorType.E5, 12, 13),
                BuildSlideArea(SensorType.E6, 14, 19, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BE_Circle_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B2, 0, 2),
                BuildSlideArea(SensorType.B3, 3, 6),
                BuildSlideArea(SensorType.E4, 7, 9),
                BuildSlideArea(SensorType.B4, 10, 11),
                BuildSlideArea(SensorType.E5, 12, 13),
                BuildSlideArea(SensorType.E6, 14, 17),
                BuildSlideArea(SensorType.E7, 18, 22, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "BE_Circle_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B2, 0, 2),
                BuildSlideArea(SensorType.B3, 3, 5),
                BuildSlideArea(SensorType.B4, 6, 9),
                BuildSlideArea(SensorType.E5, 10, 11),
                BuildSlideArea(SensorType.B5, 12, 12),
                BuildSlideArea(SensorType.E6, 13, 15),
                BuildSlideArea(SensorType.E7, 16, 19),
                BuildSlideArea(SensorType.E8, 20, 26, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "D1_Circle_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 3, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "D1_Circle_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 1),
                BuildSlideArea(SensorType.D2, 2, 3),
                BuildSlideArea(SensorType.A2, 4, 8, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "D1_Circle_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 1),
                BuildSlideArea(SensorType.D2, 2, 5),
                BuildSlideArea(SensorType.A2, 6, 8),
                BuildSlideArea(SensorType.D3, 9, 12),
                BuildSlideArea(SensorType.A3, 13, 17, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "D1_Circle_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 1),
                BuildSlideArea(SensorType.D2, 2, 4),
                BuildSlideArea(SensorType.A2, 5, 7),
                BuildSlideArea(SensorType.D3, 8, 10),
                BuildSlideArea(SensorType.A3, 11, 13),
                BuildSlideArea(SensorType.D4, 14, 17),
                BuildSlideArea(SensorType.A4, 18, 22, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "D1_Circle_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 1),
                BuildSlideArea(SensorType.D2, 2, 3),
                BuildSlideArea(SensorType.A2, 4, 6),
                BuildSlideArea(SensorType.D3, 7, 9),
                BuildSlideArea(SensorType.A3, 10, 12),
                BuildSlideArea(SensorType.D4, 13, 15),
                BuildSlideArea(SensorType.A4, 16, 19),
                BuildSlideArea(SensorType.D5, 20, 23),
                BuildSlideArea(SensorType.A5, 24, 30, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "D1_Circle_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 1),
                BuildSlideArea(SensorType.D2, 2, 5),
                BuildSlideArea(SensorType.A2, 6, 8),
                BuildSlideArea(SensorType.D3, 9, 12),
                BuildSlideArea(SensorType.A3, 13, 15),
                BuildSlideArea(SensorType.D4, 16, 19),
                BuildSlideArea(SensorType.A4, 20, 23),
                BuildSlideArea(SensorType.D5, 24, 26),
                BuildSlideArea(SensorType.A5, 27, 30),
                BuildSlideArea(SensorType.D6, 31, 34),
                BuildSlideArea(SensorType.A6, 35, 39, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "D1_Circle_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 1),
                BuildSlideArea(SensorType.D2, 2, 4),
                BuildSlideArea(SensorType.A2, 5, 7),
                BuildSlideArea(SensorType.D3, 8, 10),
                BuildSlideArea(SensorType.A3, 11, 13),
                BuildSlideArea(SensorType.D4, 14, 16),
                BuildSlideArea(SensorType.A4, 17, 19),
                BuildSlideArea(SensorType.D5, 20, 22),
                BuildSlideArea(SensorType.A5, 23, 25),
                BuildSlideArea(SensorType.D6, 26, 28),
                BuildSlideArea(SensorType.A6, 29, 32),
                BuildSlideArea(SensorType.D7, 33, 35),
                BuildSlideArea(SensorType.A7, 36, 39, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "D1_Circle_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 1),
                BuildSlideArea(SensorType.D2, 2, 3),
                BuildSlideArea(SensorType.A2, 4, 6),
                BuildSlideArea(SensorType.D3, 7, 8),
                BuildSlideArea(SensorType.A3, 9, 11),
                BuildSlideArea(SensorType.D4, 12, 13),
                BuildSlideArea(SensorType.A4, 14, 16),
                BuildSlideArea(SensorType.D5, 17, 18),
                BuildSlideArea(SensorType.A5, 19, 21),
                BuildSlideArea(SensorType.D6, 22, 23),
                BuildSlideArea(SensorType.A6, 24, 26),
                BuildSlideArea(SensorType.D7, 27, 28),
                BuildSlideArea(SensorType.A7, 29, 31),
                BuildSlideArea(SensorType.D8, 32, 33),
                BuildSlideArea(SensorType.A8, 34, 37, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DA_Circle_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 1, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DA_Circle_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 1),
                BuildSlideArea(SensorType.D2, 2, 4),
                BuildSlideArea(SensorType.A2, 5, 8, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DA_Circle_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 1),
                BuildSlideArea(SensorType.D2, 2, 5),
                BuildSlideArea(SensorType.A2, 6, 8),
                BuildSlideArea(SensorType.D3, 9, 12),
                BuildSlideArea(SensorType.A3, 13, 17, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DA_Circle_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 1),
                BuildSlideArea(SensorType.D2, 2, 4),
                BuildSlideArea(SensorType.A2, 5, 7),
                BuildSlideArea(SensorType.D3, 8, 10),
                BuildSlideArea(SensorType.A3, 11, 13),
                BuildSlideArea(SensorType.D4, 14, 17),
                BuildSlideArea(SensorType.A4, 18, 22, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DA_Circle_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 1),
                BuildSlideArea(SensorType.D2, 2, 4),
                BuildSlideArea(SensorType.A2, 5, 6),
                BuildSlideArea(SensorType.D3, 7, 9),
                BuildSlideArea(SensorType.A3, 10, 12),
                BuildSlideArea(SensorType.D4, 13, 15),
                BuildSlideArea(SensorType.A4, 16, 17),
                BuildSlideArea(SensorType.D5, 18, 20),
                BuildSlideArea(SensorType.A5, 21, 25, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DA_Circle_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 1),
                BuildSlideArea(SensorType.D2, 2, 5),
                BuildSlideArea(SensorType.A2, 6, 8),
                BuildSlideArea(SensorType.D3, 9, 12),
                BuildSlideArea(SensorType.A3, 13, 15),
                BuildSlideArea(SensorType.D4, 16, 19),
                BuildSlideArea(SensorType.A4, 20, 23),
                BuildSlideArea(SensorType.D5, 24, 26),
                BuildSlideArea(SensorType.A5, 27, 30),
                BuildSlideArea(SensorType.D6, 31, 34),
                BuildSlideArea(SensorType.A6, 35, 39, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DA_Circle_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 1),
                BuildSlideArea(SensorType.D2, 2, 4),
                BuildSlideArea(SensorType.A2, 5, 7),
                BuildSlideArea(SensorType.D3, 8, 10),
                BuildSlideArea(SensorType.A3, 11, 13),
                BuildSlideArea(SensorType.D4, 14, 16),
                BuildSlideArea(SensorType.A4, 17, 19),
                BuildSlideArea(SensorType.D5, 20, 22),
                BuildSlideArea(SensorType.A5, 23, 25),
                BuildSlideArea(SensorType.D6, 26, 28),
                BuildSlideArea(SensorType.A6, 29, 31),
                BuildSlideArea(SensorType.D7, 32, 34),
                BuildSlideArea(SensorType.A7, 35, 39, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DA_Circle_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 1),
                BuildSlideArea(SensorType.D2, 2, 3),
                BuildSlideArea(SensorType.A2, 4, 6),
                BuildSlideArea(SensorType.D3, 7, 8),
                BuildSlideArea(SensorType.A3, 9, 10),
                BuildSlideArea(SensorType.D4, 11, 13),
                BuildSlideArea(SensorType.A4, 14, 16),
                BuildSlideArea(SensorType.D5, 17, 18),
                BuildSlideArea(SensorType.A5, 19, 20),
                BuildSlideArea(SensorType.D6, 21, 23),
                BuildSlideArea(SensorType.A6, 24, 26),
                BuildSlideArea(SensorType.D7, 27, 28),
                BuildSlideArea(SensorType.A7, 29, 31),
                BuildSlideArea(SensorType.D8, 32, 33),
                BuildSlideArea(SensorType.A8, 34, 37, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DB_Circle_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 3, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DB_Circle_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 1),
                BuildSlideArea(SensorType.E2, 2, 4),
                BuildSlideArea(SensorType.B2, 5, 7, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DB_Circle_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 1),
                BuildSlideArea(SensorType.E2, 2, 5),
                BuildSlideArea(SensorType.E3, 6, 10),
                BuildSlideArea(SensorType.B3, 11, 13, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DB_Circle_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 1),
                BuildSlideArea(SensorType.D2, 2, 5),
                BuildSlideArea(SensorType.A2, 6, 8),
                BuildSlideArea(SensorType.E3, 9, 10),
                BuildSlideArea(SensorType.E4, 11, 15),
                BuildSlideArea(SensorType.B4, 16, 18, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DB_Circle_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 1),
                BuildSlideArea(SensorType.D2, 2, 4),
                BuildSlideArea(SensorType.A2, 5, 7),
                BuildSlideArea(SensorType.E3, 8, 10),
                BuildSlideArea(SensorType.E4, 11, 15),
                BuildSlideArea(SensorType.B4, 16, 18),
                BuildSlideArea(SensorType.B5, 19, 22, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DB_Circle_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 1),
                BuildSlideArea(SensorType.D2, 2, 5),
                BuildSlideArea(SensorType.A2, 6, 7),
                BuildSlideArea(SensorType.E3, 8, 10),
                BuildSlideArea(SensorType.E4, 11, 15),
                BuildSlideArea(SensorType.E5, 16, 19),
                BuildSlideArea(SensorType.B5, 20, 22),
                BuildSlideArea(SensorType.B6, 23, 27, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DB_Circle_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 1),
                BuildSlideArea(SensorType.D2, 2, 4),
                BuildSlideArea(SensorType.A2, 5, 7),
                BuildSlideArea(SensorType.D3, 8, 11),
                BuildSlideArea(SensorType.A3, 12, 14),
                BuildSlideArea(SensorType.E4, 15, 17),
                BuildSlideArea(SensorType.E5, 18, 21),
                BuildSlideArea(SensorType.E6, 22, 25),
                BuildSlideArea(SensorType.B6, 26, 27),
                BuildSlideArea(SensorType.B7, 28, 32, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DB_Circle_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 1),
                BuildSlideArea(SensorType.D2, 2, 3),
                BuildSlideArea(SensorType.A2, 4, 6),
                BuildSlideArea(SensorType.D3, 7, 9),
                BuildSlideArea(SensorType.A3, 10, 11),
                BuildSlideArea(SensorType.E4, 12, 14),
                BuildSlideArea(SensorType.E5, 15, 18),
                BuildSlideArea(SensorType.E6, 19, 23),
                BuildSlideArea(SensorType.B6, 24, 27),
                BuildSlideArea(SensorType.E7, 28, 29),
                BuildSlideArea(SensorType.B7, 30, 30),
                BuildSlideArea(SensorType.B8, 31, 34, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DD_Circle_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 0),
                BuildSlideArea(SensorType.D2, 1, 3),
                BuildSlideArea(SensorType.A2, 4, 5),
                BuildSlideArea(SensorType.D3, 6, 7),
                BuildSlideArea(SensorType.A3, 8, 9),
                BuildSlideArea(SensorType.D4, 10, 12),
                BuildSlideArea(SensorType.A4, 13, 14),
                BuildSlideArea(SensorType.D5, 15, 16),
                BuildSlideArea(SensorType.A5, 17, 18),
                BuildSlideArea(SensorType.D6, 19, 20),
                BuildSlideArea(SensorType.A6, 21, 23),
                BuildSlideArea(SensorType.D7, 24, 25),
                BuildSlideArea(SensorType.A7, 26, 27),
                BuildSlideArea(SensorType.D8, 28, 29),
                BuildSlideArea(SensorType.A8, 30, 31),
                BuildSlideArea(SensorType.D1, 32, 35, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DD_Circle_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 1),
                BuildSlideArea(SensorType.D2, 2, 6, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DD_Circle_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 1),
                BuildSlideArea(SensorType.D2, 2, 5),
                BuildSlideArea(SensorType.A2, 6, 8),
                BuildSlideArea(SensorType.D3, 9, 14, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DD_Circle_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 1),
                BuildSlideArea(SensorType.D2, 2, 4),
                BuildSlideArea(SensorType.A2, 5, 8),
                BuildSlideArea(SensorType.D3, 9, 11),
                BuildSlideArea(SensorType.A3, 12, 14),
                BuildSlideArea(SensorType.D4, 15, 20, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DD_Circle_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 1),
                BuildSlideArea(SensorType.D2, 2, 4),
                BuildSlideArea(SensorType.A2, 5, 7),
                BuildSlideArea(SensorType.D3, 8, 10),
                BuildSlideArea(SensorType.A3, 11, 13),
                BuildSlideArea(SensorType.D4, 14, 16),
                BuildSlideArea(SensorType.A4, 17, 19),
                BuildSlideArea(SensorType.D5, 20, 23, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DD_Circle_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 1),
                BuildSlideArea(SensorType.D2, 2, 3),
                BuildSlideArea(SensorType.A2, 4, 6),
                BuildSlideArea(SensorType.D3, 7, 8),
                BuildSlideArea(SensorType.A3, 9, 11),
                BuildSlideArea(SensorType.D4, 12, 14),
                BuildSlideArea(SensorType.A4, 15, 16),
                BuildSlideArea(SensorType.D5, 17, 19),
                BuildSlideArea(SensorType.A5, 20, 21),
                BuildSlideArea(SensorType.D6, 22, 25, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DD_Circle_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 1),
                BuildSlideArea(SensorType.D2, 2, 4),
                BuildSlideArea(SensorType.A2, 5, 8),
                BuildSlideArea(SensorType.D3, 9, 11),
                BuildSlideArea(SensorType.A3, 12, 14),
                BuildSlideArea(SensorType.D4, 15, 18),
                BuildSlideArea(SensorType.A4, 19, 21),
                BuildSlideArea(SensorType.D5, 22, 24),
                BuildSlideArea(SensorType.A5, 25, 27),
                BuildSlideArea(SensorType.D6, 28, 31),
                BuildSlideArea(SensorType.A6, 32, 34),
                BuildSlideArea(SensorType.D7, 35, 40, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DD_Circle_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 1),
                BuildSlideArea(SensorType.D2, 2, 4),
                BuildSlideArea(SensorType.A2, 5, 6),
                BuildSlideArea(SensorType.D3, 7, 9),
                BuildSlideArea(SensorType.A3, 10, 12),
                BuildSlideArea(SensorType.D4, 13, 15),
                BuildSlideArea(SensorType.A4, 16, 17),
                BuildSlideArea(SensorType.D5, 18, 20),
                BuildSlideArea(SensorType.A5, 21, 23),
                BuildSlideArea(SensorType.D6, 24, 26),
                BuildSlideArea(SensorType.A6, 27, 28),
                BuildSlideArea(SensorType.D7, 29, 31),
                BuildSlideArea(SensorType.A7, 32, 34),
                BuildSlideArea(SensorType.D8, 35, 38, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DE_Circle_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 0),
                BuildSlideArea(SensorType.D2, 1, 3),
                BuildSlideArea(SensorType.A2, 4, 5),
                BuildSlideArea(SensorType.D3, 6, 7),
                BuildSlideArea(SensorType.A3, 8, 9),
                BuildSlideArea(SensorType.D4, 10, 12),
                BuildSlideArea(SensorType.A4, 13, 14),
                BuildSlideArea(SensorType.D5, 15, 17),
                BuildSlideArea(SensorType.A5, 18, 19),
                BuildSlideArea(SensorType.E6, 20, 21),
                BuildSlideArea(SensorType.A6, 22, 23),
                BuildSlideArea(SensorType.E7, 24, 25),
                BuildSlideArea(SensorType.E8, 26, 30),
                BuildSlideArea(SensorType.E1, 31, 35, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DE_Circle_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 5, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DE_Circle_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 1),
                BuildSlideArea(SensorType.D2, 2, 5),
                BuildSlideArea(SensorType.E3, 6, 11, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DE_Circle_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 1),
                BuildSlideArea(SensorType.D2, 2, 5),
                BuildSlideArea(SensorType.A2, 6, 8),
                BuildSlideArea(SensorType.E3, 9, 11),
                BuildSlideArea(SensorType.E4, 12, 17, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DE_Circle_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 1),
                BuildSlideArea(SensorType.D2, 2, 4),
                BuildSlideArea(SensorType.A2, 5, 7),
                BuildSlideArea(SensorType.D3, 8, 10),
                BuildSlideArea(SensorType.E3, 11, 11),
                BuildSlideArea(SensorType.A3, 12, 13),
                BuildSlideArea(SensorType.E4, 14, 16),
                BuildSlideArea(SensorType.E5, 17, 22, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DE_Circle_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 1),
                BuildSlideArea(SensorType.D2, 2, 3),
                BuildSlideArea(SensorType.A2, 4, 6),
                BuildSlideArea(SensorType.D3, 7, 9),
                BuildSlideArea(SensorType.A3, 10, 11),
                BuildSlideArea(SensorType.E4, 12, 14),
                BuildSlideArea(SensorType.A4, 15, 17),
                BuildSlideArea(SensorType.E5, 18, 19),
                BuildSlideArea(SensorType.E6, 20, 25, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DE_Circle_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 1),
                BuildSlideArea(SensorType.D2, 2, 4),
                BuildSlideArea(SensorType.A2, 5, 8),
                BuildSlideArea(SensorType.D3, 9, 11),
                BuildSlideArea(SensorType.A3, 12, 14),
                BuildSlideArea(SensorType.D4, 15, 18),
                BuildSlideArea(SensorType.A4, 19, 20),
                BuildSlideArea(SensorType.E5, 21, 23),
                BuildSlideArea(SensorType.A5, 24, 25),
                BuildSlideArea(SensorType.E6, 26, 27),
                BuildSlideArea(SensorType.E7, 28, 32, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "DE_Circle_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 1),
                BuildSlideArea(SensorType.D2, 2, 4),
                BuildSlideArea(SensorType.A2, 5, 6),
                BuildSlideArea(SensorType.D3, 7, 9),
                BuildSlideArea(SensorType.A3, 10, 12),
                BuildSlideArea(SensorType.D4, 13, 15),
                BuildSlideArea(SensorType.A4, 16, 18),
                BuildSlideArea(SensorType.D5, 19, 21),
                BuildSlideArea(SensorType.A5, 22, 23),
                BuildSlideArea(SensorType.E6, 24, 26),
                BuildSlideArea(SensorType.E7, 27, 32),
                BuildSlideArea(SensorType.E8, 33, 38, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "E1_Circle_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 3, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "E1_Circle_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 1),
                BuildSlideArea(SensorType.D2, 2, 4),
                BuildSlideArea(SensorType.A2, 5, 8, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "E1_Circle_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 2),
                BuildSlideArea(SensorType.D2, 3, 4),
                BuildSlideArea(SensorType.A2, 5, 6),
                BuildSlideArea(SensorType.D3, 7, 10),
                BuildSlideArea(SensorType.A3, 11, 15, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "E1_Circle_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.D2, 0, 4),
                BuildSlideArea(SensorType.A2, 5, 6),
                BuildSlideArea(SensorType.D3, 7, 10),
                BuildSlideArea(SensorType.A3, 11, 12),
                BuildSlideArea(SensorType.D4, 13, 16),
                BuildSlideArea(SensorType.A4, 17, 21, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "E1_Circle_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 4),
                BuildSlideArea(SensorType.A2, 5, 7),
                BuildSlideArea(SensorType.D3, 8, 9),
                BuildSlideArea(SensorType.A3, 10, 12),
                BuildSlideArea(SensorType.D4, 13, 15),
                BuildSlideArea(SensorType.A4, 16, 17),
                BuildSlideArea(SensorType.D5, 18, 20),
                BuildSlideArea(SensorType.A5, 21, 27, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "E1_Circle_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 3),
                BuildSlideArea(SensorType.A2, 4, 6),
                BuildSlideArea(SensorType.D3, 7, 8),
                BuildSlideArea(SensorType.A3, 9, 10),
                BuildSlideArea(SensorType.D4, 11, 13),
                BuildSlideArea(SensorType.A4, 14, 16),
                BuildSlideArea(SensorType.D5, 17, 20),
                BuildSlideArea(SensorType.A5, 21, 23),
                BuildSlideArea(SensorType.D6, 24, 27),
                BuildSlideArea(SensorType.A6, 28, 33, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "E1_Circle_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 3),
                BuildSlideArea(SensorType.A2, 4, 6),
                BuildSlideArea(SensorType.D3, 7, 9),
                BuildSlideArea(SensorType.A3, 10, 11),
                BuildSlideArea(SensorType.D4, 12, 14),
                BuildSlideArea(SensorType.A4, 15, 17),
                BuildSlideArea(SensorType.D5, 18, 20),
                BuildSlideArea(SensorType.A5, 21, 23),
                BuildSlideArea(SensorType.D6, 24, 26),
                BuildSlideArea(SensorType.A6, 27, 29),
                BuildSlideArea(SensorType.D7, 30, 33),
                BuildSlideArea(SensorType.A7, 34, 37, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "E1_Circle_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 3),
                BuildSlideArea(SensorType.A2, 4, 6),
                BuildSlideArea(SensorType.D3, 7, 9),
                BuildSlideArea(SensorType.A3, 10, 11),
                BuildSlideArea(SensorType.D4, 12, 14),
                BuildSlideArea(SensorType.A4, 15, 16),
                BuildSlideArea(SensorType.D5, 17, 18),
                BuildSlideArea(SensorType.A5, 19, 21),
                BuildSlideArea(SensorType.D6, 22, 23),
                BuildSlideArea(SensorType.A6, 24, 26),
                BuildSlideArea(SensorType.D7, 27, 28),
                BuildSlideArea(SensorType.A7, 29, 31),
                BuildSlideArea(SensorType.D8, 32, 33),
                BuildSlideArea(SensorType.A8, 34, 37, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EA_Circle_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 2, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EA_Circle_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 2),
                BuildSlideArea(SensorType.D2, 3, 4),
                BuildSlideArea(SensorType.A2, 5, 8, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EA_Circle_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 4),
                BuildSlideArea(SensorType.A2, 5, 6),
                BuildSlideArea(SensorType.D3, 7, 9),
                BuildSlideArea(SensorType.A3, 10, 14, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EA_Circle_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 3),
                BuildSlideArea(SensorType.A2, 4, 6),
                BuildSlideArea(SensorType.D3, 7, 9),
                BuildSlideArea(SensorType.A3, 10, 12),
                BuildSlideArea(SensorType.D4, 13, 15),
                BuildSlideArea(SensorType.A4, 16, 20, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EA_Circle_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 4),
                BuildSlideArea(SensorType.A2, 5, 7),
                BuildSlideArea(SensorType.D3, 8, 10),
                BuildSlideArea(SensorType.A3, 11, 12),
                BuildSlideArea(SensorType.D4, 13, 15),
                BuildSlideArea(SensorType.A4, 16, 17),
                BuildSlideArea(SensorType.D5, 18, 20),
                BuildSlideArea(SensorType.A5, 21, 25, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EA_Circle_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 3),
                BuildSlideArea(SensorType.A2, 4, 6),
                BuildSlideArea(SensorType.E3, 7, 8),
                BuildSlideArea(SensorType.A3, 9, 11),
                BuildSlideArea(SensorType.D4, 12, 13),
                BuildSlideArea(SensorType.A4, 14, 15),
                BuildSlideArea(SensorType.D5, 16, 18),
                BuildSlideArea(SensorType.A5, 19, 20),
                BuildSlideArea(SensorType.D6, 21, 23),
                BuildSlideArea(SensorType.A6, 24, 28, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EA_Circle_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 3),
                BuildSlideArea(SensorType.E3, 4, 7),
                BuildSlideArea(SensorType.A3, 8, 10),
                BuildSlideArea(SensorType.D4, 11, 14),
                BuildSlideArea(SensorType.A4, 15, 16),
                BuildSlideArea(SensorType.D5, 17, 19),
                BuildSlideArea(SensorType.A5, 20, 22),
                BuildSlideArea(SensorType.D6, 23, 25),
                BuildSlideArea(SensorType.A6, 26, 28),
                BuildSlideArea(SensorType.D7, 29, 31),
                BuildSlideArea(SensorType.A7, 32, 36, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EA_Circle_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 3),
                BuildSlideArea(SensorType.E3, 4, 9),
                BuildSlideArea(SensorType.A3, 10, 11),
                BuildSlideArea(SensorType.E4, 12, 14),
                BuildSlideArea(SensorType.A4, 15, 16),
                BuildSlideArea(SensorType.D5, 17, 19),
                BuildSlideArea(SensorType.A5, 20, 21),
                BuildSlideArea(SensorType.D6, 22, 23),
                BuildSlideArea(SensorType.A6, 24, 26),
                BuildSlideArea(SensorType.D7, 27, 28),
                BuildSlideArea(SensorType.A7, 29, 31),
                BuildSlideArea(SensorType.D8, 32, 33),
                BuildSlideArea(SensorType.A8, 34, 37, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EB_Circle_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E1, 0, 1, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EB_Circle_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.B1, 0, 1),
                BuildSlideArea(SensorType.B2, 2, 4, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EB_Circle_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 4),
                BuildSlideArea(SensorType.B2, 5, 5),
                BuildSlideArea(SensorType.B3, 6, 11, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EB_Circle_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 3),
                BuildSlideArea(SensorType.B2, 4, 6),
                BuildSlideArea(SensorType.E3, 7, 8),
                BuildSlideArea(SensorType.B3, 9, 10),
                BuildSlideArea(SensorType.B4, 11, 15, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EB_Circle_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 3),
                BuildSlideArea(SensorType.B2, 4, 5),
                BuildSlideArea(SensorType.E3, 6, 7),
                BuildSlideArea(SensorType.B3, 8, 9),
                BuildSlideArea(SensorType.B4, 10, 13),
                BuildSlideArea(SensorType.B5, 14, 18, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EB_Circle_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 3),
                BuildSlideArea(SensorType.E3, 4, 8),
                BuildSlideArea(SensorType.B3, 9, 11),
                BuildSlideArea(SensorType.E4, 12, 13),
                BuildSlideArea(SensorType.B4, 14, 15),
                BuildSlideArea(SensorType.B5, 16, 18),
                BuildSlideArea(SensorType.B6, 19, 23, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EB_Circle_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 3),
                BuildSlideArea(SensorType.E3, 4, 7),
                BuildSlideArea(SensorType.B3, 8, 9),
                BuildSlideArea(SensorType.E4, 10, 11),
                BuildSlideArea(SensorType.B4, 12, 13),
                BuildSlideArea(SensorType.B5, 14, 17),
                BuildSlideArea(SensorType.B6, 18, 21),
                BuildSlideArea(SensorType.B7, 22, 26, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EB_Circle_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 3),
                BuildSlideArea(SensorType.E3, 4, 9),
                BuildSlideArea(SensorType.B3, 10, 12),
                BuildSlideArea(SensorType.E4, 13, 14),
                BuildSlideArea(SensorType.B4, 15, 16),
                BuildSlideArea(SensorType.E5, 17, 18),
                BuildSlideArea(SensorType.B5, 19, 19),
                BuildSlideArea(SensorType.B6, 20, 22),
                BuildSlideArea(SensorType.B7, 23, 25),
                BuildSlideArea(SensorType.B8, 26, 30, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "ED_Circle_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 3),
                BuildSlideArea(SensorType.E3, 4, 8),
                BuildSlideArea(SensorType.A3, 9, 10),
                BuildSlideArea(SensorType.E4, 11, 12),
                BuildSlideArea(SensorType.A4, 13, 14),
                BuildSlideArea(SensorType.D5, 15, 17),
                BuildSlideArea(SensorType.A5, 18, 18),
                BuildSlideArea(SensorType.D6, 19, 21),
                BuildSlideArea(SensorType.A6, 22, 23),
                BuildSlideArea(SensorType.D7, 24, 25),
                BuildSlideArea(SensorType.A7, 26, 27),
                BuildSlideArea(SensorType.D8, 28, 29),
                BuildSlideArea(SensorType.A8, 30, 32),
                BuildSlideArea(SensorType.D1, 33, 35, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "ED_Circle_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 1),
                BuildSlideArea(SensorType.D2, 2, 5, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "ED_Circle_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.A1, 0, 2),
                BuildSlideArea(SensorType.E2, 3, 4),
                BuildSlideArea(SensorType.D2, 5, 5),
                BuildSlideArea(SensorType.A2, 6, 6),
                BuildSlideArea(SensorType.D3, 7, 11, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "ED_Circle_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 4),
                BuildSlideArea(SensorType.A2, 5, 6),
                BuildSlideArea(SensorType.D3, 7, 9),
                BuildSlideArea(SensorType.A3, 10, 12),
                BuildSlideArea(SensorType.D4, 13, 17, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "ED_Circle_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 3),
                BuildSlideArea(SensorType.A2, 4, 7),
                BuildSlideArea(SensorType.D3, 8, 10),
                BuildSlideArea(SensorType.A3, 11, 12),
                BuildSlideArea(SensorType.D4, 13, 15),
                BuildSlideArea(SensorType.A4, 16, 18),
                BuildSlideArea(SensorType.D5, 19, 22, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "ED_Circle_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 4),
                BuildSlideArea(SensorType.A2, 5, 7),
                BuildSlideArea(SensorType.E3, 8, 9),
                BuildSlideArea(SensorType.A3, 10, 11),
                BuildSlideArea(SensorType.D4, 12, 14),
                BuildSlideArea(SensorType.A4, 15, 16),
                BuildSlideArea(SensorType.D5, 17, 19),
                BuildSlideArea(SensorType.A5, 20, 21),
                BuildSlideArea(SensorType.D6, 22, 25, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "ED_Circle_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 3),
                BuildSlideArea(SensorType.E3, 4, 8),
                BuildSlideArea(SensorType.A3, 9, 10),
                BuildSlideArea(SensorType.D4, 11, 12),
                BuildSlideArea(SensorType.A4, 13, 14),
                BuildSlideArea(SensorType.D5, 15, 17),
                BuildSlideArea(SensorType.A5, 18, 20),
                BuildSlideArea(SensorType.D6, 21, 24),
                BuildSlideArea(SensorType.A6, 25, 27),
                BuildSlideArea(SensorType.D7, 28, 32, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "ED_Circle_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 4),
                BuildSlideArea(SensorType.E3, 5, 10),
                BuildSlideArea(SensorType.A3, 11, 13),
                BuildSlideArea(SensorType.E4, 14, 15),
                BuildSlideArea(SensorType.D4, 16, 16),
                BuildSlideArea(SensorType.A4, 17, 18),
                BuildSlideArea(SensorType.D5, 19, 21),
                BuildSlideArea(SensorType.A5, 22, 23),
                BuildSlideArea(SensorType.D6, 24, 26),
                BuildSlideArea(SensorType.A6, 27, 28),
                BuildSlideArea(SensorType.D7, 29, 31),
                BuildSlideArea(SensorType.A7, 32, 34),
                BuildSlideArea(SensorType.D8, 35, 38, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EE_Circle_1",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 3),
                BuildSlideArea(SensorType.E3, 4, 7),
                BuildSlideArea(SensorType.E4, 8, 12),
                BuildSlideArea(SensorType.E5, 13, 16),
                BuildSlideArea(SensorType.E6, 17, 21),
                BuildSlideArea(SensorType.E7, 22, 25),
                BuildSlideArea(SensorType.E8, 26, 30),
                BuildSlideArea(SensorType.E1, 31, 35, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EE_Circle_2",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 3, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EE_Circle_3",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 3),
                BuildSlideArea(SensorType.E3, 4, 8, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EE_Circle_4",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 3),
                BuildSlideArea(SensorType.E3, 4, 8),
                BuildSlideArea(SensorType.E4, 9, 15, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EE_Circle_5",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 3),
                BuildSlideArea(SensorType.E3, 4, 7),
                BuildSlideArea(SensorType.E4, 8, 12),
                BuildSlideArea(SensorType.E5, 13, 17, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EE_Circle_6",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 4),
                BuildSlideArea(SensorType.E3, 5, 9),
                BuildSlideArea(SensorType.E4, 10, 14),
                BuildSlideArea(SensorType.E5, 15, 19),
                BuildSlideArea(SensorType.E6, 20, 25, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EE_Circle_7",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 3),
                BuildSlideArea(SensorType.E3, 4, 7),
                BuildSlideArea(SensorType.E4, 8, 12),
                BuildSlideArea(SensorType.E5, 13, 16),
                BuildSlideArea(SensorType.E6, 17, 21),
                BuildSlideArea(SensorType.E7, 22, 26, true, true)
            },
            Const = 0.1f
        },

        new SlideTable()
        {
            Name = "EE_Circle_8",
            JudgeQueue = new SlideArea[]
            {
                BuildSlideArea(SensorType.E2, 0, 2),
                BuildSlideArea(SensorType.E3, 3, 6),
                BuildSlideArea(SensorType.E4, 7, 10),
                BuildSlideArea(SensorType.E5, 11, 14),
                BuildSlideArea(SensorType.E6, 15, 17),
                BuildSlideArea(SensorType.E7, 18, 25, true, true)
            },
            Const = 0.1f
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