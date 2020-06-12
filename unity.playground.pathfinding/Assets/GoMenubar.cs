using System;
using UnityEngine;
using UnityEngine.UI;

public static class ExComponent
{
    public static void SetActive(this Component comp, bool isActive)
    {
        comp.gameObject.SetActive(isActive);
    }
}
public class GoMenubar : MonoBehaviour
{
    public Button btnBake;
    public Button btnFindPath;
    public Button btnReset;
    public Toggle optPrimary;
    public Toggle optStraight;
    public Toggle optDiagonal;
    public Toggle optWall;

    public Action FnBtnBake { get; set; }
    public Action FnBtnFindPath { get; set; }
    public Action FnBtnReset { get; set; }
    public Action<bool> FnOptPrimary { get; set; }
    public Action<bool> FnOptStraight { get; set; }
    public Action<bool> FnOptDiagonal { get; set; }
    public Action<bool> FnOptWall { get; set; }

    private void Awake()
    {
        btnBake.onClick.AddListener(() => FnBtnBake());
        btnFindPath.onClick.AddListener(() => FnBtnFindPath());
        btnReset.onClick.AddListener(() => FnBtnReset());
        optPrimary.onValueChanged.AddListener((isChecked) => FnOptPrimary(isChecked));
        optStraight.onValueChanged.AddListener((isChecked) => FnOptStraight(isChecked));
        optDiagonal.onValueChanged.AddListener((isChecked) => FnOptDiagonal(isChecked));
        optWall.onValueChanged.AddListener((isChecked) => FnOptWall(isChecked));

        Reset();
    }

    public void Reset()
    {
        btnBake.interactable = true;
        btnFindPath.interactable = false;
        optPrimary.isOn = true;
        optPrimary.interactable = false;
        optStraight.isOn = false;
        optStraight.interactable = false;
        optDiagonal.isOn = false;
        optDiagonal.interactable = false;
        optWall.isOn = false;
        optWall.interactable = false;
        btnReset.interactable = false;
    }

    internal void Baked()
    {
        optPrimary.interactable = true;
        optStraight.interactable = true;
        optDiagonal.interactable = true;
        optWall.interactable = true;
        btnFindPath.interactable = true;
        btnReset.interactable = true;
    }
}
