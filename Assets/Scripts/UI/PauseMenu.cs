using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private SelectableItem selected;
    private SettingMenu settingMenu;
    private bool usingSetting;

    private void Awake() {
        selected.Select();
        settingMenu = Instantiate(Resources.Load<GameObject>("Prefab/SettingMenu"), transform.parent).GetComponent<SettingMenu>();
        settingMenu.SetupCloseEvent(delegate { usingSetting = false; });
        settingMenu.gameObject.SetActive(false);
    }

    public void Back() {
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    public void OpenSetting() {
        Debug.Log("open setting");
        settingMenu.gameObject.SetActive(true);
        usingSetting = true;
    }

    public void BackToMenu() {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    void Update()
    {
        if (usingSetting) return;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            selected.Selected = false;
            selected = selected.NavTop;
            selected.Selected = true;
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            selected.Selected = false;
            selected = selected.NavBottom;
            selected.Selected = true;
        }
        else if (Input.GetButtonDown("Submit")) selected.Activate();
    }
}
