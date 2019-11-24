using TMPro;
using UnityEngine;
using MultiLanguage;
using Menu;

public class SettingMenu : MonoBehaviour
{
    [SerializeField]
    private Selectable selected;
    [SerializeField]
    private UnityEngine.UI.Image musicBar, soundBar;
    [SerializeField]
    private TextMeshProUGUI resolutionText, languageText;
    [SerializeField]
    private MultiLanguageText fullscreenMultiLan;

    private System.Action closeSettingEvent;

    private void Start() {
        selected.Select = true;

        ApplyPreference();
    }

    public void SetupCloseEvent(System.Action action) {
        closeSettingEvent = action;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) selected.Up(ref selected);
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) selected.Down(ref selected);
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) selected.Left(ref selected);
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) selected.Right(ref selected);
        else if (Input.GetButtonDown("Submit")) selected.Submit();
        else if (Input.GetButtonDown("Cancel")) Back();
    }

    public void MusicVolumeChoose(bool add) {
        PlayerPreference.MusicVolume += add? 0.1f: -0.1f;
        musicBar.fillAmount = PlayerPreference.MusicVolume;
        PlayerPreference.ApplyVolume();
    }
    public void SoundVolumeChoose(bool add) {
        PlayerPreference.SoundVolume += add ? 0.1f : -0.1f;
        soundBar.fillAmount = PlayerPreference.SoundVolume;
        PlayerPreference.ApplyVolume();
    }

    public void ResolutionChoose(bool right) {
        PlayerPreference.ResolutionIndex += right? 1: -1;
        Vector2Int _resolution = PlayerPreference.Resolution;
        resolutionText.text = string.Format("{0} x {1}", _resolution.x, _resolution.y);
        PlayerPreference.ApplyResolution();
    }

    public void ToggleFullscreenWindowed() {
        PlayerPreference.Fullscreen = !PlayerPreference.Fullscreen;
        fullscreenMultiLan.ChangeId(PlayerPreference.Fullscreen ? 8 : 9);
        PlayerPreference.ApplyResolution();
    }

    public void LanguageChoose(bool right) {
        PlayerPreference.LanguageIndex += right? 1: - 1;
        languageText.text = MultiLanguageMgr.GetLanguageName(PlayerPreference.Language);
        PlayerPreference.ApplyLanguage();
    }

    public void Back() {
        gameObject.SetActive(false);
        if (closeSettingEvent != null) closeSettingEvent();
        PlayerPreference.Save();
    }

    public void ApplyPreference() {
        musicBar.fillAmount = PlayerPreference.MusicVolume;
        soundBar.fillAmount = PlayerPreference.SoundVolume;
        Vector2 _resolution = PlayerPreference.Resolution;
        resolutionText.text = string.Format("{0} x {1}", _resolution.x, _resolution.y);
        fullscreenMultiLan.ChangeId(PlayerPreference.Fullscreen ? 8 : 9);
        languageText.text = MultiLanguageMgr.GetLanguageName(PlayerPreference.Language);
    }
}
