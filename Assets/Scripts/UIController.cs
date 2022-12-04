using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [field: SerializeField] public GameManager Manager { get; private set; } = null;
    [field: SerializeField] public GameObject GameUI { get; private set; } = null;
    [field: SerializeField] public GameObject PauseUI { get; private set; } = null;
    [field: SerializeField] public TMPro.TMP_Text MoneyText { get; private set; } = null;
    [field: SerializeField] public GameObject BuildingUI { get; private set; } = null;
    [field: SerializeField] public GameObject BuildingsPanel { get; private set; } = null;
    [field: SerializeField] public GameObject DecorationPanel { get; private set; } = null;
    [field: SerializeField] public GameObject UpgradePanel { get; private set; } = null;
    [field: SerializeField] public AudioSource ClickSound { get; private set; } = null;

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
    }

    public void PlayClickSound()
    {
        ClickSound.Play();
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
