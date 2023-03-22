using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Report", menuName = "Report")] 
public class Report : ScriptableObject
{
    public string[] exerciseList;
    public float[] percentageList;
    public int earnedStars;
    public float totalPercentage = 0;

    public void generateReport()
    {
        for (int i = 0; i < exerciseList.Length; i++){
            totalPercentage += percentageList[i];
        }
        totalPercentage = totalPercentage / exerciseList.Length;

        if (totalPercentage >= 75)
        {
            earnedStars = 3;
        }
        else if (totalPercentage < 75 && totalPercentage >= 50)
        {
            earnedStars = 2;
        }
        else
        {
            earnedStars = 1;
        }
    }

    
}
