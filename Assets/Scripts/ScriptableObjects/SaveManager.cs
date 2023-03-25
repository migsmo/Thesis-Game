using System;
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

        public void generateLog(Report report){
            string time = DateTime.Now.ToString("HH:mm:ss");
            string fileName = DateTime.Now.Month + "-" + 
                            DateTime.Now.Day + "-" + 
                            DateTime.Now.Year + "-" + 
                            time + "-" +
                            "Level" + report.levelNumber;
            string path = Application.dataPath + Directory + "/" + fileName;
            
            DateTime currentDateTime = DateTime.Now;
            // Create a new file and write data to it
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.WriteLine(currentDateTime.ToString());
                writer.WriteLine("Level " + report.levelNumber + " Performace Breakdown:");

                for(int i = 0; i < report.exerciseList.Length; i++)
                {
                    writer.WriteLine(report.exerciseList[i] + " " + report.percentageList[i] + "%");
                }
                writer.WriteLine("Total Percentage: " + report.totalPercentage + "%");
                writer.WriteLine("Total Stars Earned: " + report.earnedStars); 
            }
        }
    }
}