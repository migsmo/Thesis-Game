using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Resources
{
    [System.Serializable]
    public class SaveManager
    {
        private string Directory = "/LevelData/";
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

        public void generateLog(Report report){
            string time = DateTime.Now.ToString("HH-mm-ss");
            string fileName = DateTime.Now.Month + "-" +
                              DateTime.Now.Day + "-" +
                              DateTime.Now.Year + "-" +
                              time + "-" +
                              "Level" + report.levelNumber + ".txt";
            string path = Application.streamingAssetsPath + Directory + fileName;
            string data = "";
            string newLine = "\n";

            DateTime currentDateTime = DateTime.Now;
            
            File.AppendAllText(path, currentDateTime.ToString() + newLine);
            File.AppendAllText(path, "Level " + report.levelNumber + " Performace Breakdown:" + newLine);

            for (int i = 0; i < report.exerciseList.Length; i++)
            {
                File.AppendAllText(path, report.exerciseList[i] + " " + report.percentageList[i] + "%" + newLine);
            }
            
            File.AppendAllText(path, "Total Percentage: " + report.totalPercentage + "%");
            File.AppendAllText(path, "Total Stars Earned: " + report.earnedStars);
        }
    }
}