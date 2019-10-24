#pragma warning disable 649

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ReleaseVersion.UI;

namespace ReleaseVersion {
    public class MainMenuMgr : MonoBehaviour
    {
        private const string gameScene = "GameRelease";
        private bool loadingScene;
        [SerializeField]
        private SelectableItem selectedItem;

        private void Start() {
            selectedItem.Selected = true;
        }

        public void Play() {
            loadingScene = true;
            SceneManager.LoadSceneAsync(gameScene);
        }

        public void OpenSetting() {

        }

        public void Quit() {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void Update() {
            if (loadingScene) return;

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
                selectedItem.Selected = false;
                selectedItem = selectedItem.NavTop;
                selectedItem.Selected = true;
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
                selectedItem.Selected = false;
                selectedItem = selectedItem.NavBottom;
                selectedItem.Selected = true;
            }
            else if (Input.GetButton("Submit")) selectedItem.Activate();
        }
    }
}