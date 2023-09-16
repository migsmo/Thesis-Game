using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator transition;
    public float transitionTime;

    public void StoryMode()
    {
        LevelSelectDisplay.fromSim = false;
        StartCoroutine(LoadLevel("StorySelect"));
    }

    // Update is called once per frame
    public void ArcadeMode()
    {
        LevelSelectDisplay.fromSim = true;
        StartCoroutine(LoadLevel("LevelSelect"));
    }

    IEnumerator LoadLevel(string sceneName)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneName);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
