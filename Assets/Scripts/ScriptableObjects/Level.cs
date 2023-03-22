using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
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
}
