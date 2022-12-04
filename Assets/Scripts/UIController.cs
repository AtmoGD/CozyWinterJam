using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [field: SerializeField] public GameManager Manager { get; private set; } = null;
    [field: SerializeField] public Animator StartAnimator { get; private set; } = null;
    [field: SerializeField] public GameObject GameUI { get; private set; } = null;
    [field: SerializeField] public GameObject PauseUI { get; private set; } = null;
    [field: SerializeField] public Slider MusicSlider { get; private set; } = null;
    [field: SerializeField] public Slider SFXSlider { get; private set; } = null;
    [field: SerializeField] public TMPro.TMP_Text MoneyText { get; private set; } = null;
    [field: SerializeField] public GameObject BuildingUI { get; private set; } = null;
    [field: SerializeField] public GameObject BuildingsPanel { get; private set; } = null;
    [field: SerializeField] public GameObject DecorationPanel { get; private set; } = null;
    [field: SerializeField] public GameObject UpgradePanel { get; private set; } = null;
    private bool startGameButtonClicked = false;

    public bool IsBuildingUIActive
    {
        get
        {
            return BuildingUI.activeSelf;
        }
    }

    private void Start()
    {
        GameUI.SetActive(true);
        BuildingUI.SetActive(false);
        PauseUI.SetActive(false);
        OpenBuildingsPanel();
        ChangeMusicVolume();
    }

    public void PlayClickSound()
    {
        AudioManager.Instance.Play("Click");
    }

    public void PlayDeleteSound()
    {
        AudioManager.Instance.Play("Delete");
    }

    public void ChangeMusicVolume()
    {
        AudioManager.Instance.SetVolume(SoundType.Music, MusicSlider.value);
        AudioManager.Instance.SetVolume(SoundType.SFX, SFXSlider.value);
    }

    public void StartButtonClicked()
    {
        if (startGameButtonClicked) return;

        startGameButtonClicked = true;
        StartAnimator.SetTrigger("FadeOut");
    }

    public void StartGame()
    {
        Manager.StartGame();
    }

    private void Update()
    {
        MoneyText.text = Manager.Money.ToString();
    }

    public void PauseGame()
    {
        Manager.PauseGame();
        PauseUI.SetActive(true);
    }

    public void ResumeGame()
    {
        Manager.UnpauseGame();
        PauseUI.SetActive(false);
    }

    public void OpenBuildingsUI()
    {
        BuildingUI.SetActive(true);
    }

    public void CloseBuildingsUI()
    {
        BuildingUI.SetActive(false);
    }

    public void OpenBuildingsPanel()
    {
        BuildingsPanel.SetActive(true);
        DecorationPanel.SetActive(false);
        UpgradePanel.SetActive(false);
    }

    public void OpenDecorationPanel()
    {
        BuildingsPanel.SetActive(false);
        DecorationPanel.SetActive(true);
        UpgradePanel.SetActive(false);
    }

    public void OpenUpgradePanel()
    {
        BuildingsPanel.SetActive(false);
        DecorationPanel.SetActive(false);
        UpgradePanel.SetActive(true);
    }

    public void PlaceObject(Placeable obj)
    {
        if (Manager.Money >= obj.Cost)
        {
            Manager.StartPlacingObject(obj);
            CloseBuildingsUI();
        }
    }
}
