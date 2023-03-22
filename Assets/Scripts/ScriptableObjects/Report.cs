using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Report
{
    public string[] exerciseList;
    public int[] percentageList;
    public int earnedStars = 0;
    public float totalPercentage = 0;

    public Report(string[] exerciseList, int[] percentageList)
    {
        this.exerciseList = exerciseList;
        this.percentageList = percentageList;
    }

    public void generateReport()
    {
        for (int i = 0; i < exerciseList.Length; i++)
        {
            totalPercentage += percentageList[i];
        }

        Debug.Log($"");
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

    public int getExerciseLength()
    {
        return exerciseList.Length;
    }
}