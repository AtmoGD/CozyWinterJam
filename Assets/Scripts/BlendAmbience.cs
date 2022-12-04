using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendAmbience : MonoBehaviour
{
    [SerializeField] private GameManager Manager;
    [SerializeField] private UIController UIController = null;
    [SerializeField] private string ambienceNoPeople = "AmbienceNoPeople";
    [SerializeField] private string ambienceFewPeople = "AmbienceFewPeople";
    [SerializeField] private string ambienceManyPeople = "AmbienceManyPeople";
    [SerializeField] private int noPeopleThreshold = 0;
    [SerializeField] private int fewPeopleThreshold = 10;
    [SerializeField] private int manyPeopleThreshold = 20;

    private void Update()
    {
        int peopleCount = Manager.customers.Count;

        float noPeopleVolume = 0f;
        float fewPeopleVolume = 0f;
        float manyPeopleVolume = 0f;

        if (peopleCount <= noPeopleThreshold)
        {
            noPeopleVolume = 1f;
        }
        else if (peopleCount <= fewPeopleThreshold)
        {
            noPeopleVolume = 1f - (peopleCount - noPeopleThreshold) / (fewPeopleThreshold - noPeopleThreshold);
            fewPeopleVolume = 1f - noPeopleVolume;
        }
        else if (peopleCount <= manyPeopleThreshold)
        {
            fewPeopleVolume = 1f - (peopleCount - fewPeopleThreshold) / (manyPeopleThreshold - fewPeopleThreshold);
            manyPeopleVolume = 1f - fewPeopleVolume;
        }
        else
        {
            manyPeopleVolume = 1f;
        }

        noPeopleVolume *= UIController.SFXSlider.value;
        fewPeopleVolume *= UIController.SFXSlider.value;
        manyPeopleVolume *= UIController.SFXSlider.value;

        AudioManager.Instance.SetVolume(ambienceNoPeople, noPeopleVolume);
        AudioManager.Instance.SetVolume(ambienceFewPeople, fewPeopleVolume);
        AudioManager.Instance.SetVolume(ambienceManyPeople, manyPeopleVolume);
    }
}
