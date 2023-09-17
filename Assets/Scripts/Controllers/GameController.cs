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
    public Animator transition;
    public float transitionTime;

    // Start is called before the first frame update
    void Start()
    {
        postBattleIndex = level.postIndex;
        postScene = level.postScene;
        // isPostBattle = level.isPostBattle;
        if (isPostBattle)
        {
            Debug.LogWarning("Entered Is Post Battle");
            for (int i = 1; i < postScene; i++)
            {
                postBattleIndex -= currentScene.sentences.Count;
                currentScene = currentScene.nextScene;
                Debug.LogWarning(postBattleIndex + "POSTBATTLEINDEX");
            }
        }
        speechBar.PlayScene(currentScene);
        Debug.LogWarning(currentScene);
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
            if(speechBar.IsCompleted())
            {
                if(speechBar.IsLastSentence())
                {
                    if (currentScene.nextScene == null)
                        if (isBattleEnd)
                        {
                            StartCoroutine(LoadLevel("CameraSpace"));    
                        }
                        else
                            StartCoroutine(LoadLevel(level.nextLevel)); 
                    else
                    {
                        postBattleIndex -= currentScene.sentences.Count;
                        currentScene = currentScene.nextScene;
                        speechBar.PlayScene(currentScene);
                        backgroundController.SwitchImage(currentScene.background);
                        postScene++;
                    }
                } else
                {
                    Debug.LogWarning(SpeechBarController.sentenceIndex + "sentenceIndex" + postBattleIndex + "postIndex");

                    if (SpeechBarController.sentenceIndex == postBattleIndex - 1 && !isBattleEnd)
                        StartCoroutine(LoadLevel("CameraSpace"));        
                    else
                        speechBar.PlayNextSentence();


                }
            }
        }
    }

    IEnumerator LoadLevel(string sceneName)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneName);
    }
    
    public bool getIsBattleEnd()
    {
        return isBattleEnd;
    }
}