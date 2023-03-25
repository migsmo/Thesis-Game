using System.IO;
using UnityEngine;

namespace Resources
{
    [System.Serializable]
    public class SaveManager
    {
        private string Directory = "/Resources/LevelData/";
        private string Filename = "";
        public Level[] loadedLevels;

        public class LevelData
        {
            public int levelNumber;
            public int exerciseTimer;
            public int restTimer;
            public int setNo;
            public int starsEarned;
            public int starsRequired;
            public int energyCost;
            public bool isUnlocked = false;
            public string[] exerciseList;
        }
        
        public void Save(Level level)
        {
            string savedObject = JsonUtility.ToJson(level);
            Filename = "/Level" + level.levelNumber + ".txt";
            File.WriteAllText(Application.streamingAssetsPath + Filename, savedObject);
        }

        public int Load(Level level)
        {
            LevelData temp = new LevelData();
            Filename = "/Level" + level.levelNumber + ".txt";
            string data = File.ReadAllText(Application.streamingAssetsPath + Filename);
            temp = JsonUtility.FromJson<LevelData>(data);
            return temp.starsEarned;
        }
    }
}