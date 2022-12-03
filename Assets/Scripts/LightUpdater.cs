using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
public class LightUpdater : MonoBehaviour
{
    [field: SerializeField] private List<Light2D> Lights { get; set; } = new List<Light2D>();
    [field: SerializeField] private DayTimeController DayTimeController { get; set; } = null;
    [field: SerializeField] private bool Inverted { get; set; } = false;

    private void Start()
    {
        DayTimeController = GameManager.Instance.DayTimeController;
    }

    private void Update()
    {
        foreach (Light2D light in Lights)
        {
            if (Inverted)
            {
                light.intensity = 1 - DayTimeController.DayTimeCurveValue;
            }
            else
            {
                light.intensity = DayTimeController.DayTimeCurveValue;
            }
        }
    }
}
