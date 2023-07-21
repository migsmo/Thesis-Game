using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Level", menuName = "Level")] 
public class Level : ScriptableObject
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
    public bool isUnlocked = false;
    public bool isGuided;
    public bool isBattleEnd;
    public bool isPostBattle;

    public string[] exerciseList;
}
