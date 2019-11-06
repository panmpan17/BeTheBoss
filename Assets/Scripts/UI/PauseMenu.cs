using UnityEngine;
using UnityEngine.SceneManagement;
using Audio;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private SelectableItem selected;
    private SettingMenu settingMenu;
    private bool usingSetting;

    private void Awake() {
        settingMenu = Instantiate(Resources.Load<GameObject>("Prefab/SettingMenu"), transform.parent).GetComponent<SettingMenu>();
        settingMenu.SetupCloseEvent(delegate { usingSetting = false; });
        settingMenu.gameObject.SetActive(false);
    }

    private void Start() {
        selected.Select();
    }

    public void Back() {
        AudioManager.ins.UnPause();
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    public void OpenSetting() {
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
            selected.Select();
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            selected.Selected = false;
            selected = selected.NavBottom;
            selected.Select();
        }
        else if (Input.GetButtonDown("Submit")) selected.Activate();
    }
}
