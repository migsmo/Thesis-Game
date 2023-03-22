using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public Level level;
    public StoryScene currentScene;
    public SpeechBarController speechBar;
    public SpriteSwitcher backgroundController;
    public static int selectedLevel;
    public static int exerciseTimer;
    public static int restTimer;
    public static int setNo;
    public static string[] exerciseList;

    // Start is called before the first frame update
    void Start()
    {
        speechBar.PlayScene(currentScene);
        backgroundController.SetImage(currentScene.background);
        LevelSelectDisplay.selectedLevel = level.levelNumber;
        LevelSelectDisplay.exerciseTimer = level.exerciseTimer;
        LevelSelectDisplay.restTimer = level.restTimer;
        LevelSelectDisplay.setNo = level.setNo;
        LevelSelectDisplay.exerciseList = level.exerciseList;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if(speechBar.IsCompleted())
            {
                if(speechBar.IsLastSentence())
                {
                    if (currentScene.nextScene == null)
                        SceneManager.LoadScene("Pilot");
                    else
                    {
                        currentScene = currentScene.nextScene;
                        speechBar.PlayScene(currentScene);
                        backgroundController.SwitchImage(currentScene.background);
                    }
                } else
                {
                    speechBar.PlayNextSentence();

                }
            }
        }
    }
}
