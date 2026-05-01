using System;
using UnityEngine;

public partial class ObjectCounter : MonoBehaviour
{
    private void UpdateTimeOutput()
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
}