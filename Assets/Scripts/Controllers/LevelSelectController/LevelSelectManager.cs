using System.Collections;
using System.Collections.Generic;
using Resources;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour
{ 
    public Level[] levels;
    private int total_stars = 0;
    public static int calculatedStars;
    public Animator transition;
    public float transitionTime;
    public bool isStoryMode;

    // Start is called before the first frame update
    void Start()
    {
        if (isStoryMode)
        {
            for (int i = 1; i < levels.Length; i++)
            {
                if (levels[i - 1].starsEarned >= 1)
                {
                    levels[i].isUnlocked = true;
                }
            }
        }
        else
        {
            for (int i = 0; i < levels.Length; i++)
            {
                total_stars += levels[i].starsEarned;
            }

            calculatedStars = total_stars;

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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Return()
    {
        StartCoroutine(LoadLevel("MainMenu"));
    }

    IEnumerator LoadLevel(string sceneName)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneName);
    }
}
