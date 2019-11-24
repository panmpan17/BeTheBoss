using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

namespace Audio {
    public enum AudioEnum
    {
        MenuBGM,
        InGameBGM,
        UISelect,
        UIClick,
        BulletShoot
    }

    public class AudioManager : MonoBehaviour
    {
        static private AudioManager _ins;
        static public AudioManager ins { get {
            if (_ins == null) _ins = Instantiate(Resources.Load<AudioManager>("Prefab/AudioManager"));
            return _ins;
        } }
        private AudioSource bgmSrc, soundSrc;
        private float musicVolume, soundVolume;
        private Audio currentBGM;

        [SerializeField]
        private Audio[] audios;

        private void Awake() {
            DontDestroyOnLoad(gameObject);
            bgmSrc = gameObject.AddComponent<AudioSource>();
            soundSrc = gameObject.AddComponent<AudioSource>();

            for (int i = 0; i < audios.Length; i++) Audio.Register(audios[i]);
        }

        public void ApplyVolume(float soundVolume, float musicVolume) {
            this.musicVolume = soundVolume;
            this.soundVolume = musicVolume;
            soundSrc.volume = soundVolume;

            if (currentBGM == null) bgmSrc.volume = musicVolume;
            else bgmSrc.volume = currentBGM.DefaultVolume * musicVolume;
        }

        public void PlayerSound(AudioEnum _type) {
            Audio audio;
            if (Audio.TryGet(_type, out audio)) soundSrc.PlayOneShot(audio.Clip, audio.DefaultVolume);
        }

        public void PlayBGM(AudioEnum _type) {
            Audio audio;
            if (Audio.TryGet(_type, out audio)) {
                bgmSrc.clip = audio.Clip;
                bgmSrc.volume = audio.DefaultVolume * musicVolume;
                bgmSrc.Play();
                currentBGM = audio;
            }
        }

        public void Pause()
        {
            soundSrc.Pause();
        }

        public void UnPause()
        {
            soundSrc.UnPause();
        }

        [System.Serializable]
        public class Audio {
            static private Dictionary<AudioEnum, Audio> dict = new Dictionary<AudioEnum, Audio>();

            static public void Register(Audio audio) {
                dict.Add(audio.Type, audio);
            }
            
            static public bool TryGet(AudioEnum type, out Audio audio) {
                return dict.TryGetValue(type, out audio);
            }

            [SerializeField]
            private AudioClip clip;
            public AudioClip Clip { get { return clip; } }
            [SerializeField]
            private AudioEnum type;
            public AudioEnum Type { get { return type; } }
            [SerializeField]
            private float defaultVolume;
            public float DefaultVolume { get { return defaultVolume; } }
        }

    #if UNITY_EDITOR
        [CustomEditor(typeof(AudioManager))]
        public class _Editor : Editor {
            AudioManager manager;
            ReorderableList audiosList;

            private void OnEnable() {
                manager = target as AudioManager;

                audiosList = new ReorderableList(serializedObject, serializedObject.FindProperty("audios"),
                    false, true, true, true);
                audiosList.drawHeaderCallback = (Rect rect) => {
                    EditorGUI.LabelField(rect, "Audios");
                };
                audiosList.elementHeightCallback = (int index) => { return 62; };
                audiosList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                    SerializedProperty ele = audiosList.serializedProperty.GetArrayElementAtIndex(index);
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y + 0, rect.width, 18), ele.FindPropertyRelative("clip"));
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y + 19, rect.width, 18), ele.FindPropertyRelative("type"));

                    SerializedProperty defaultVolume = ele.FindPropertyRelative("defaultVolume");
                    defaultVolume.floatValue = EditorGUI.Slider(new Rect(rect.x, rect.y + 38, rect.width, 18), "Default Volume", defaultVolume.floatValue, 0, 1);
                };
            }

            public override void OnInspectorGUI() {
                GUILayout.Space(8);
                serializedObject.Update();
                audiosList.DoLayoutList();
                serializedObject.ApplyModifiedProperties();
            }
        }
    #endif
    }
}