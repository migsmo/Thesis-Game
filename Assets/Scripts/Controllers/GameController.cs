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
    public static string sceneName;
    public static string nextName;
    public static bool isBattleEnd;
    public static bool isPostBattle;
    public static int postBattleIndex;
    public static int selectedLevel;
    public static int exerciseTimer;
    public static int restTimer;
    public static int setNo;
    public static string[] exerciseList;
    public static int postScene;

    // Start is called before the first frame update
    void Start()
    {
        postBattleIndex = level.postIndex;
        postScene = level.postScene;
        if (isPostBattle)
        {
            Debug.LogWarning(isPostBattle + " = IsPostBattle2");
            Debug.LogWarning("Entered Is Post Battle");
            currentScene = currentScene.nextScene;
        }
        speechBar.PlayScene(currentScene);
        backgroundController.SetImage(currentScene.background);
        LevelSelectDisplay.selectedLevel = level.levelNumber;
        LevelSelectDisplay.exerciseTimer = level.exerciseTimer;
        LevelSelectDisplay.restTimer = level.restTimer;
        LevelSelectDisplay.setNo = level.setNo;
        LevelSelectDisplay.exerciseList = level.exerciseList;
        LevelSelectDisplay.currLevel = level;
        sceneName = level.levelName;
        nextName = level.nextLevel;
        isBattleEnd = level.isBattleEnd;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            Debug.LogWarning(isPostBattle + " = IsPostBattle2");
            if(speechBar.IsCompleted())
            {
                if(speechBar.IsLastSentence())
                {
                    if (currentScene.nextScene == null)
                        if (isBattleEnd)
                        {
                            SceneManager.LoadScene("CameraSpace");
                        }
                        else
                        {
                            if (isPostBattle)
                            {
                                isPostBattle = false;
                            } 
                            SceneManager.LoadScene("StorySelect");
                        }
                    else
                    {
                        if (isPostBattle)
                        {
                            currentScene = currentScene.nextScene;
                            speechBar.PlayScene(currentScene);
                            backgroundController.SetImage(currentScene.background);
                        }
                        else 
                            if(!isBattleEnd)
                                SceneManager.LoadScene("CameraSpace");
                            else
                            {
                                currentScene = currentScene.nextScene;
                                speechBar.PlayScene(currentScene);
                                backgroundController.SetImage(currentScene.background);
                            }
                    }
                } else
                {
                    Debug.LogWarning(SpeechBarController.sentenceIndex + "sentenceIndex" + postBattleIndex + "postIndex");
                    speechBar.PlayNextSentence();


                }
            }
        }
    }

    public bool getIsBattleEnd()
    {
        return isBattleEnd;
    }
}
