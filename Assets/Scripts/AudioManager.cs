using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AudioType { Select, Click }

public class AudioManager : MonoBehaviour
{
    static public AudioManager ins;

    [SerializeField]
    private AudioClip[] audios;

    private void Awake() {
        ins = this;
    }

    public void PlayerSound(AudioSource src, AudioType _type) {
        src.clip = audios[(int) _type];
        src.Play();
    }
}
