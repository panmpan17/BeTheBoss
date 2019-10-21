using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ReleaseVersion.UI;

namespace ReleaseVersion {
    public class MainMenuMgr : MonoBehaviour
    {
        [SerializeField]
        private string gameScene;
        private bool loadingScene;
        [SerializeField]
        private SelectableItem selectedItem;

        private void Start() {
            selectedItem.Selected = true;
        }

        public void Update() {
            if (loadingScene) return;

            if (Input.GetKeyDown(KeyCode.W)) {
                selectedItem.Selected = false;
                selectedItem = selectedItem.NavTop;
                selectedItem.Selected = true;
            }
            else if (Input.GetKeyDown(KeyCode.S)) {
                selectedItem.Selected = false;
                selectedItem = selectedItem.NavBottom;
                selectedItem.Selected = true;
            }
            else if (Input.GetButton("Submit")) {
                switch (selectedItem.Arg) {
                    case "play":
                        loadingScene = true;
                        SceneManager.LoadSceneAsync(gameScene);
                        break;
                    case "option":
                        break;
                    case "quit":
                        Application.Quit();
                        break;
                }
            }
        }
    }
}