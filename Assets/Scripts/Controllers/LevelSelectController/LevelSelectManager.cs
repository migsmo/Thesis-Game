using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectManager : MonoBehaviour
{
    public Level[] levels;
    private int total_stars = 0;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < levels.Length; i++)
        {
            total_stars += levels[i].starsEarned;
        }

        for (int i = 0; i < levels.Length; i++)
        {
            if (total_stars >= levels[i].starsRequired)
            {
                levels[i].isUnlocked = true;
            }
            else
            {
                levels[i].isUnlocked = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
