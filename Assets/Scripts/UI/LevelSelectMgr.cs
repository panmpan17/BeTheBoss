﻿using UnityEngine;
using UnityEngine.SceneManagement;
using Menu;
using Setting;
using Setting.Data;

public class LevelSelectMgr : MonoBehaviour
{
    static public LevelSelectMgr ins;
    private const string LEVELSCENE = "Level";

    [SerializeField]
    private Selectable selected;
    [SerializeField]
    private Vector3 boundMin, boundMax;
    private Vector3 targetPos { get {
        return new Vector3(Mathf.Clamp(selected.transform.position.x, boundMin.x, boundMax.x), Mathf.Clamp(selected.transform.position.y, boundMin.y, boundMax.y), boundMin.z);
    } }
    [SerializeField]
    private float smoothTime;
    private Vector3 moveSpeed;

    private LevelSelectable[] levelSelectables;
    private SettingMenu settingMenu;
    private bool usingSetting;

    private void Awake() {
        ins = this;
        transform.position = targetPos;

        MainMenuMgr.InitialLoading();

        settingMenu = Instantiate(Resources.Load<SettingMenu>("Prefab/SettingMenu"));
        levelSelectables = FindObjectsOfType<LevelSelectable>();
    }

    private void Start() {
        selected.Select = true;

        if (SettingReader.TryLoadLevelSetting())
        {
            for (int i = 0; i < levelSelectables.Length; i++)
            {
                LevelSetting setting;
                if (SettingReader.TryGetLevelSetting(levelSelectables[i].ID, out setting))
                {
                    levelSelectables[i].ApplySetting(setting);
                }
            }
        }
    }

    public void Update()
    {
        if (settingMenu.enabled) return;

        if ((transform.position - targetPos).sqrMagnitude >= 0.01f)
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref moveSpeed, smoothTime);
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) selected.Up(ref selected);
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) selected.Down(ref selected);
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) selected.Left(ref selected);
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) selected.Right(ref selected);
        else if (Input.GetButtonDown("Submit")) selected.Submit();
        else if (Input.GetButtonDown("Cancel")) settingMenu.Activate();
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((boundMin + boundMax) / 2, boundMax - boundMin);
    }

    static public void LoadLevel(byte levelId) {
        SceneManager.LoadScene(LEVELSCENE + levelId);
    }

    static public bool LevelHasUnlock(byte levelId) {
        if (levelId <= 0) return true;
        return false;
    }
}
