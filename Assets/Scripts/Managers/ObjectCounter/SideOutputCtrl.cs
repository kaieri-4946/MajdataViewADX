using System;
using System.Linq;
using UnityEngine;

public partial class ObjectCounter : MonoBehaviour
{
    //run once when play
    private void StartSideOutput()
    {
        if (CurrentUIType is UIType.TrgUI)
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
    }
    //run in update
    private void UpdateSideOutput()
    {
        var allCount = tapCount + holdCount + slideCount + touchCount + breakCount;
        var allSum = tapSum + holdSum + slideSum + touchSum + breakSum;
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
                tapCount, holdCount, slideCount, touchCount, breakCount,
                tapSum, holdSum, slideSum, touchSum, breakSum,
                allCount, allSum,
                Majdata<InputManager>.Instance!.Mode
            );
            
            objectRate.text = string.Format(
                "FiNALE  Rate:\n" +
                "{0:000.00}   %\n" +
                "DELUXE Rate:\n" +
                "{1:000.0000} % ",
                Math.Truncate((float)FiNowScore() / FiSumScore() * 10000) / 100,
                Math.Truncate(((float)DxNowScore() / DxSumScore() * 100 + BreakRate()) * 10000) / 10000
            );
            
            var fast = totalJudgedCount
                .Where(x => x.Key > JudgeType.Perfect && x.Key != JudgeType.Miss)
                .Select(x => x.Value)
                .Sum();
            var late = totalJudgedCount
                .Where(x => x.Key < JudgeType.Perfect && x.Key != JudgeType.Miss)
                .Select(x => x.Value)
                .Sum();
            judgeResultCount.text = $"{cPerfectCount}\n{perfectCount}\n{greatCount}\n{goodCount}\n{missCount}\n\n{fast}\n{late}";
        }
        else
        {
            objNoteCount.text = 
                $"{tapCount} / {tapSum}\n" +
                $"{holdCount} / {holdSum}\n" +
                $"{slideCount} / {slideSum}\n" +
                $"{touchCount} / {touchSum}\n" +
                $"{breakCount} / {breakSum}\n" +
                $"{allCount} / {allSum}";
            
            var rate = Math.Truncate(((float)DxNowScore() / DxSumScore() * 100 + BreakRate()) * 10000) / 10000;
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
}