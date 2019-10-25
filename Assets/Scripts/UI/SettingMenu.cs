using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using MultiLanguage;

public class SettingMenu : MonoBehaviour
{
    [SerializeField]
    private SelectableItem selected, resolutionSelectable, languageSelectable;
    [SerializeField]
    private MultiLanguageText fullscreenMultiLan;

    private System.Action closeSettingEvent;

    private void Start() {
        selected.Selected = true;

        Vector2 _resolution = PlayerPreference.Resolution;
        resolutionSelectable.GetComponent<TextMeshProUGUI>().text = string.Format("{0} x {1}", _resolution.x, _resolution.y);
        fullscreenMultiLan.ChangeId(PlayerPreference.Fullscreen? 8: 9);
        languageSelectable.GetComponent<TextMeshProUGUI>().text = MultiLanguageMgr.GetLanguageName(PlayerPreference.Language);
    }

    public void SetupCloseEvent(System.Action action) {
        closeSettingEvent = action;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (selected.NavTop) {
                selected.Selected = false;
                selected = selected.NavTop;
                selected.Select();
            }
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (selected.NavBottom) {
                selected.Selected = false;
                selected = selected.NavBottom;
                selected.Select();
            }
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (selected.NavLeft)
            {
                selected.Selected = false;
                selected = selected.NavLeft;
                selected.Select();
            }
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (selected.NavRight)
            {
                selected.Selected = false;
                selected = selected.NavRight;
                selected.Select();
            }
        }
        else if (Input.GetButtonDown("Submit")) selected.Activate();
    }

    public void ChangeResolution() {
        Vector2Int _resolution = PlayerPreference.Resolution;
        Screen.SetResolution(_resolution.x, _resolution.y, PlayerPreference.Fullscreen? FullScreenMode.FullScreenWindow: FullScreenMode.Windowed);
    }
    public void ResolutionNavLeft() {
        selected.Selected = false;
        selected = resolutionSelectable;
        selected.Select();

        PlayerPreference.ResolutionIndex -= 1;
        Vector2Int _resolution = PlayerPreference.Resolution;
        resolutionSelectable.GetComponent<TextMeshProUGUI>().text = string.Format("{0} x {1}", _resolution.x, _resolution.y);
        ChangeResolution();
    }
    public void ResolutionNavRight() {
        selected.Selected = false;
        selected = resolutionSelectable;
        selected.Select();

        PlayerPreference.ResolutionIndex += 1;
        Vector2Int _resolution = PlayerPreference.Resolution;
        resolutionSelectable.GetComponent<TextMeshProUGUI>().text = string.Format("{0} x {1}", _resolution.x, _resolution.y);
        ChangeResolution();
    }
    public void ToggleFullscreenWindowed() {
        PlayerPreference.Fullscreen = !PlayerPreference.Fullscreen;
        fullscreenMultiLan.ChangeId(PlayerPreference.Fullscreen ? 8 : 9);
        ChangeResolution();
    }
    public void LanguageNavLeft() {
        selected.Selected = false;
        selected = languageSelectable;
        selected.Select();

        PlayerPreference.LanguageIndex -= 1;
        languageSelectable.GetComponent<TextMeshProUGUI>().text = MultiLanguageMgr.GetLanguageName(PlayerPreference.Language);

        MultiLanguageMgr.SwitchAllTextsLanguage(PlayerPreference.Language);
    }
    public void LanguageNavRight() {
        selected.Selected = false;
        selected = languageSelectable;
        selected.Select();

        PlayerPreference.LanguageIndex += 1;
        languageSelectable.GetComponent<TextMeshProUGUI>().text = MultiLanguageMgr.GetLanguageName(PlayerPreference.Language); ;

        MultiLanguageMgr.SwitchAllTextsLanguage(PlayerPreference.Language);
    }
    public void Back() {
        gameObject.SetActive(false);
        if (closeSettingEvent != null) closeSettingEvent();
        PlayerPreference.Save();
    }
}
