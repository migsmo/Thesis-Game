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
        private string StoryModeDirectory = "/StoryModeData/StoryProgress.txt";
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

        // Makes a save file that stores the current level completed and total stars earned
        public void SaveStoryProgress(StoryProgress story)
        {
            string savedObject = JsonUtility.ToJson(story); ;
            File.WriteAllText(Application.streamingAssetsPath + StoryModeDirectory, savedObject);
        }

        // returns StoryProgress object from text file that has the last level completed and total stars earned
        public StoryProgress LoadStoryProgress()
        {
            StoryProgress temp = new StoryProgress();

            try
            {
                string data = File.ReadAllText(Application.streamingAssetsPath + StoryModeDirectory);
                temp = JsonUtility.FromJson<StoryProgress>(data);
            }
            catch (Exception e)
            {
                Debug.Log("No story mode text file found. Creating one now");
            }
            
            return temp;
        }

        public int Load(Level level)
        {
            var temp = new LevelData();
            Filename = "/Level" + level.levelNumber + ".txt";
            var data = File.ReadAllText(Application.streamingAssetsPath + Filename);
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
            
            File.AppendAllText(path, "Total Percentage: " + report.totalPercentage + "%" + newLine);
            File.AppendAllText(path, "Total Stars Earned: " + report.earnedStars);
        }
    }
}