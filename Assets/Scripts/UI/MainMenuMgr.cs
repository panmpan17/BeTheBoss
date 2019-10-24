﻿#pragma warning disable 649

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

        public void Update() {
            if (loadingScene) return;

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
                selectedItem.Selected = false;
                selectedItem = selectedItem.NavTop;
                selectedItem.Selected = true;
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
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