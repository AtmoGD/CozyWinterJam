using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DayTime
{
    public float Hour;
    public float Minute;
}

public class DayTimeController : MonoBehaviour
{
    [field: SerializeField] public DayTime DayTime { get; private set; } = new DayTime();
    [field: SerializeField] public float MinuteLength { get; private set; } = 1f;
    [field: SerializeField] public GameManager GameManager { get; private set; } = null;
    [field: SerializeField] public TMP_Text HourText { get; private set; } = null;
    [field: SerializeField] public TMP_Text MinuteText { get; private set; } = null;
    [SerializeField] public AnimationCurve DayTimeCurve = null;
    public float DayTimePercentage
    {
        get
        {
            return (DayTime.Hour * 60 + DayTime.Minute) / (24 * 60);
        }
    }

    public float DayTimeCurveValue
    {
        get
        {
            return DayTimeCurve.Evaluate(DayTimePercentage);
        }
    }


    private void Update()
    {
        if (!GameManager.GameStarted) return;

        DayTime.Minute += (Time.deltaTime / MinuteLength) * GameManager.WorldTimeScale;

        if (DayTime.Minute >= 59f)
        {
            DayTime.Minute = 0f;
            DayTime.Hour++;
            if (DayTime.Hour >= 23f)
            {
                DayTime.Hour = 0f;
            }
        }

        HourText.text = DayTime.Hour.ToString("00");
        MinuteText.text = DayTime.Minute.ToString("00");
    }
}
