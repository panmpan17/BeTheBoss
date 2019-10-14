using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
// using TMPro;

public class MainMenuMgr : MonoBehaviour
{
    [SerializeField]
    private string gameScene;
    [SerializeField]
    private Button startupSelectedButton;
    private bool loadingScene;

    // Start is called before the first frame update
    private void Start() {
        startupSelectedButton.Select();
        // startupSelectedButton.Sele
        // startupSelectedButton.
    }

    public void StartGame() {
        if (loadingScene) return;
        loadingScene = true;
        SceneManager.LoadSceneAsync(gameScene);
    }
}
