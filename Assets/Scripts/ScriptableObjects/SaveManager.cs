using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Resources
{
    [System.Serializable]
    public class SaveManager
    {
        private string ArcadeDirectory = "/ArcadeData/";
        private string StoryModeDirectory = "/StoryModeData/";
        private string LogsDirectory = "/Logs/";
        private string Filename = "";
        public Level[] loadedLevels;

        public class LevelData
        {
            public int levelNumber;
            public string levelName;
            public string nextLevel;
            public int postScene;
            public int postIndex;
            public int exerciseTimer;
            public int restTimer;
            public int setNo;
            public int starsEarned;
            public int starsRequired;
            public int energyCost;
            public bool isUnlocked;
            public bool isGuided;
            public bool isBattleEnd;
            public bool isPostBattle;
            public bool isStoryMode;
            public string[] exerciseList;
        }
        
        public void Save(Level level)
        {
            string savedObject = JsonUtility.ToJson(level);
            string directoryPath = "";
            
            if (LevelSelectDisplay.isStoryMode)
            {
                directoryPath = Application.persistentDataPath + StoryModeDirectory;
            }
            else
            {
                directoryPath = Application.persistentDataPath + ArcadeDirectory;
            }
            

            // Create the base directory if it doesn't exist
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string filename = "Level" + level.levelNumber + ".txt";
            string filePath = Path.Combine(directoryPath, filename);

            // Save the file
            File.WriteAllText(filePath, savedObject);
        }

        public void SaveStoryMode(Level level)
        {
            string savedObject = JsonUtility.ToJson(level);
            string directoryPath = Application.persistentDataPath + StoryModeDirectory;

            // Create the base directory if it doesn't exist
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string filename = "Level" + level.levelNumber + ".txt";
            string filePath = Path.Combine(directoryPath, filename);

            // Save the file
            File.WriteAllText(filePath, savedObject);
        }

        public int Load(Level level)
        {
            var temp = new LevelData();
            Filename = "/Level" + level.levelNumber + ".txt";
            var data = "";
            string filePath = "";

            if (level.isStoryMode)
            {
                filePath = Application.persistentDataPath + StoryModeDirectory + Filename;
            }
            else
            {
                filePath = Application.persistentDataPath + ArcadeDirectory + Filename;
            }

            // Check if the file exists
            if (File.Exists(filePath))
            {
                data = File.ReadAllText(filePath);
            }
            else
            {
                Save(level);
                return 0;
            }
  
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
            string directoryPath = Application.persistentDataPath + LogsDirectory;
            string path = Path.Combine(directoryPath, fileName);
            string data = "";
            string newLine = "\n";

            DateTime currentDateTime = DateTime.Now;

            // Check if the directory exists, and create it if it doesn't
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            
            Debug.Log(directoryPath + Filename);

            // Create or append to the log file
            using (StreamWriter writer = File.AppendText(path))
            {
                writer.WriteLine(currentDateTime.ToString());
                writer.WriteLine("Level " + report.levelNumber + " Performance Breakdown:");

                for (int i = 0; i < report.exerciseList.Length; i++)
                {
                    writer.WriteLine(report.exerciseList[i] + " " + report.percentageList[i] + "%");
                }

                writer.WriteLine("Total Percentage: " + report.totalPercentage + "%");
                writer.WriteLine("Total Stars Earned: " + report.earnedStars);
            }
        }
    }
}