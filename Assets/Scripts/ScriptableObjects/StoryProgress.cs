using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryProgress
{
    public int CurrentLevel = 0;
    public int TotalStarsEarned = 0;
    
    // Intended to be called during PostBattle Scene during Story Mode
    public void CompletedLevel()
    {
        CurrentLevel++;
    }

    public void AddStars(int stars)
    {
        TotalStarsEarned += stars;
    }
}
