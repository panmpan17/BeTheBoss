using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MultiLanguage;

public class IntroAnimation : MonoBehaviour
{
    static private int[] lines = new int[] {
        101, 102, 103, 104, 105, 106, 107, 108
    };
    private int linesIndex;

    [SerializeField]
    private TextMeshProUGUI text;
    [SerializeField]
    private float startDelayTime, linesTime, nextDelayTime, fadeInTime, fadeOutTime, endFadeTime;
    private Color fadeFrom, fadeTo;
    private Timer linesTimer, delayTimer, fadeTimer;
    private bool delay, fadding, ending;
    private System.Action fadeCallback;

    private string Line { set {
        text.text = value;
    } }

    private void Awake() {
        Line = "";
    }

    private void Start() {
        delayTimer = new Timer(startDelayTime);
        linesTimer = new Timer(linesTime);

        delay = true;
    }

    private void Update() {

        if (delay) {
            if (!delayTimer.UpdateEnd) return;
            delay = false;
            Line = MultiLanguageMgr.GetTextById(PlayerPreference.Language, lines[linesIndex]);

            fadeFrom = new Color(1, 1, 1, 0);
            fadeTo = Color.white;
            fadeTimer = new Timer(fadeInTime);
            fadeCallback = null;
            fadding = true;
        }

        if (fadding) {
            if (!fadeTimer.UpdateEnd) {
                text.color = Color.Lerp(fadeFrom, fadeTo, fadeTimer.Progress);
                return;
            } else {
                text.color = fadeTo;
                fadding = false;
                if (fadeCallback != null) fadeCallback();
                return;
            }
        }

        if (linesTimer.UpdateEnd) {
            linesTimer.Reset();

            fadeFrom = Color.white;
            fadeTo = new Color(1, 1, 1, 0);
            fadding = true;
            fadeTimer = new Timer(fadeOutTime);
            fadeCallback = delegate {
                linesIndex++;
                delayTimer = new Timer(nextDelayTime);
                delay = true;

                if (linesIndex >= lines.Length) {
                    gameObject.SetActive(false);
                    PlayerPreference.SkipIntro = true;
                    PlayerPreference.Save();
                }
            };
        }
    }
}
