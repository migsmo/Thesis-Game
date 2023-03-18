using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Level")] 
public class Level : ScriptableObject
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

    int getStarts()
    {
        return starsRequired;
    }

    void setStars(int num)
    {
        starsEarned = num;
    }
}
