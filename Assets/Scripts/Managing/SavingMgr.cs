using System.Collections;
using System.Collections.Generic;
// using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Saving {
    public class SavingMgr
    {
        private const string SAVING_PATH = "Saving.data";
        static public Saving saving;
        static public bool Loaded { get { return saving != null; } }
        
        static public void LoadSaving() {
            string path = Application.persistentDataPath + "/" + SAVING_PATH;
            if (!File.Exists(path)) {
                saving = new Saving();
                return;
            }

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            saving = formatter.Deserialize(stream) as Saving;
            stream.Close();
            saving.ToList();
        }

        static public void Save() {
            string path = Application.persistentDataPath + "/" + SAVING_PATH;
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Create);
            saving.ToArray();
            formatter.Serialize(stream, saving);
            stream.Close();
        }
    }

    [System.Serializable]
    public class Saving {
        public int[] PlayedLevel;
        public int[] UnlockedBoss;
        public int[] UnlockedPlayer;
        public int[] UnlockedAchievement;

        private List<int> playedLevel, unlockedBoss, unlockedPlayer, unlockedAchievement;

        public Saving() {
            playedLevel = new List<int>();
            unlockedBoss = new List<int>();
            unlockedPlayer = new List<int>();
            unlockedAchievement = new List<int>();
        }

        public void ToArray() {
            PlayedLevel = playedLevel.ToArray();
            UnlockedBoss = unlockedBoss.ToArray();
            UnlockedPlayer = unlockedPlayer.ToArray();
            UnlockedAchievement = unlockedAchievement.ToArray();
        }
        public void ToList() {
            playedLevel = new List<int>(PlayedLevel);
            unlockedBoss = new List<int>(UnlockedBoss);
            unlockedPlayer = new List<int>(UnlockedPlayer);
            unlockedAchievement = new List<int>(UnlockedAchievement);
        }
    }
}