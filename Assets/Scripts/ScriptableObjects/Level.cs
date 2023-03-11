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

    public string[] exerciseList;
}
