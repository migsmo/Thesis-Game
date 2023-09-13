using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class PilotLogic : MonoBehaviour
{
    public TextMeshProUGUI Timer;
    public TextMeshProUGUI ExerciseLabel;
    public TextMeshProUGUI CurrentLevel;
    public TextMeshProUGUI SetLabel;
    public TextMeshProUGUI UpcomingExerciseLabel;
    public TextMeshProUGUI AveLabel;
    public TextMeshProUGUI ExerciseLabel2;
    public RawImage RestCutscenePanel;
    public VideoPlayer VideoPlayer;
    public Image SyncBar;
    public Image SideBarL;
    public Image SideBarR;
    public Image AveBar;
    public Image LeftContainer;
    public Image RightContainer;
    public RectTransform WarningScreen;
    public int SyncPercentage;
    public int AvePercentage;
    public AudioClip StartCue;
    public AudioClip StopCue;
    public AudioSource audioSource;
    public AudioClip CountDown;
    public AudioClip PowerUp;
    private float volume = 1;
    public static Report levelReport;
    public Animator transition;
    public float transitionTime;

    private string ExerciseName;
    private float CurrentTime = 0f;
    private float exerciseTimer = 0f;
    private int restTimer;
    private float setRestTimer=60f;
    private float transitionTimer;
    private string exerciseIndex;

    public bool getReady = true;
    public bool startExercise = false;
    public bool ExerciseDone = false;
    private bool SetDone = false;
    private bool startCutscene = false;
    private bool endCutscene = true;
    private bool inFrame = true;
    private bool audioPlayed1 = false;
    private bool audioPlayed2 = false;
    public bool transitionDone = false;

    private string[] exerciseList;
    private int selectedLevel;
    private int setNo;
    private int exerciseLength;
    public int currExercise = -1;
    private int currSet = 0;
    public int nextExercise = 0;
    private float rotate = 0;
    public int[] percentageList = new int[17];
    
    public VideoClip sumoSquat;
    public VideoClip staticLunge;
    public VideoClip gluteBridge;
    public VideoClip singleGlute;
    public VideoClip straightBridge;
    public VideoClip elbowPlanks;
    public VideoClip sidePlank;
    public VideoClip supermanHold;
    public VideoClip birdDog;
    public VideoClip highPlanks;
    public VideoClip pushupHold;
    public VideoClip easyPlanks;
    
    public GameObject pauseMenu;
    public bool isPaused;
    
    // Start is called before the first frame update
    void Awake()
    {
        selectedLevel = LevelSelectDisplay.selectedLevel;
        exerciseTimer = LevelSelectDisplay.exerciseTimer;
        restTimer = LevelSelectDisplay.restTimer;
        setNo = LevelSelectDisplay.setNo;
        exerciseList = LevelSelectDisplay.exerciseList;
        CurrentLevel.text = "Level " + selectedLevel;
        SetLabel.text = "Set " + (currSet + 1) + " / " + setNo;
        percentageList = new int[exerciseList.Length];
        exerciseLength = exerciseList.Length;
        transitionTimer = 2.5f;
        pauseMenu.SetActive(false);
        isPaused = false;
        
        if (!LevelSelectDisplay.currLevel.isGuided)
        {
            RightContainer.enabled = false;
            LeftContainer.enabled = false;
        }
        
        // Temporary code to be replaced to test post battle scene
        levelReport = new Report(exerciseList, percentageList);
        levelReport.levelNumber = selectedLevel;
        // levelReport.generateReport();
        // SceneManager.LoadScene("PostBattle");
    }

    void Update()
    {
        getSyncBar();
        getAveBar();
        setBar();
        if (SyncPercentage < 70)
        {
            rotate += 1 * Time.deltaTime;
            WarningScreen.Rotate(new Vector3(0, 0, rotate));
        }

        if (transitionTimer > 0)
        {
            transitionTimer -= Time.deltaTime;

            // When the transition timer reaches zero, set transitionDone to true
            if (transitionTimer <= 0)
            {
                transitionDone = true;
            }
        }
        if (inFrame && transitionDone)
        {
            if (currExercise < exerciseLength && currSet < setNo)
            {
                if (SetDone)
                {
                    Timer.text = CurrentTime.ToString("0");
                    CurrentTime -= 1 * Time.deltaTime;
                    if (CurrentTime <= 0)
                    {
                        SetDone = false;
                        startExercise = false;
                        getReady = true;
                    }
                }
                else
                    SetExerciseTimer();
            }
            else if (currExercise == exerciseList.Length && currSet < setNo)
            {
                Debug.LogWarning("ENTERED EQUALS");
                currSet++;
                if (currSet == setNo)
                {
                    levelReport.generateReport();
                    StartCoroutine(LoadLevel("FinalCutscene"));
                }
                else
                {
                    SetDone = true;
                    ExerciseLabel.text = "Set Rest";
                    SetLabel.text = "Set " + (currSet + 1) + " / " + setNo;
                    CurrentTime = setRestTimer;
                    currExercise = -1;
                    nextExercise = 0;
                }
            }
        }
        // else
        // {
        //     ExerciseLabel.text = "Body not in Frame";
        // }
        
        if (startCutscene)
        {
            switch (exerciseIndex)
            {
                case "Sumo Squat":
                    VideoPlayer.clip = sumoSquat;
                    break;
                case "Static Lunge (L)":
                    VideoPlayer.clip = staticLunge;
                    break;
                case "Static Lunge (R)":
                    VideoPlayer.clip = staticLunge;
                    break;
                case "Glute Bridge":
                    VideoPlayer.clip = gluteBridge;
                    break;
                case "Single Leg Glute Bridge (L)":
                    VideoPlayer.clip = singleGlute;
                    break;
                case "Single Leg Glute Bridge (R)":
                    VideoPlayer.clip = singleGlute;
                    break;
                case "Straight Bridge":
                    VideoPlayer.clip = straightBridge;
                    break;
                case "Elbow Planks":
                    VideoPlayer.clip = elbowPlanks;
                    break;
                case "Side Plank (L)":
                    VideoPlayer.clip = sidePlank;
                    break;
                case "Side Plank (R)":
                    VideoPlayer.clip = sidePlank;
                    break;
                case "Superman Hold":
                    VideoPlayer.clip = supermanHold;
                    break;
                case "Bird Dog (L)":
                    VideoPlayer.clip = birdDog;
                    break;
                case "Bird Dog (R)":
                    VideoPlayer.clip = birdDog;
                    break;
                case "High Planks":
                    VideoPlayer.clip = highPlanks;
                    break;
                case "Pushup Hold":
                    VideoPlayer.clip = pushupHold;
                    break;
                case "Side Plank (L) Easy":
                    VideoPlayer.clip = easyPlanks;
                    break;
                case "Side Plank (R) Easy":
                    VideoPlayer.clip = easyPlanks;
                    break;
            }
            RestCutscenePanel.CrossFadeAlpha(1f, 0.5f, false);
            if (!isPaused)
            {
                VideoPlayer.Play();
            }
        }
        
        if (endCutscene)
        {
            RestCutscenePanel.CrossFadeAlpha(0f, 0.5f, false);
        }

        if (isPaused)
        {
            audioSource.Pause();
            if (startCutscene)
            {  
                VideoPlayer.Pause();
            }
        }
        else
        {
            audioSource.Play();
            if (startCutscene)
            {
                VideoPlayer.Play();
            }
        }
    }

    public void getSyncBar()
    {
        SyncBar.fillAmount = (float)SyncPercentage / 100;
    }

    public void getAveBar()
    {
        AveBar.fillAmount = (float)AvePercentage / 100;
        AveLabel.text = AvePercentage.ToString() + "%";
    }

    public void setLabel(string Label)
    {
        exerciseIndex = ExerciseName;
        ExerciseName = Label;
        ExerciseLabel.text = ExerciseName;
        ExerciseLabel2.text = ExerciseName;
    }

    public void setBar()
    {
        if (SyncPercentage >= 75)
        {
            SideBarL.color = new Color32(71, 198, 83, 255);
            SideBarR.color = new Color32(71, 198, 83, 255);
        }
        else if (SyncPercentage < 75 && SyncPercentage >= 50)
        {
            SideBarL.color = new Color32(241, 146, 66, 255);
            SideBarR.color = new Color32(241, 146, 66, 255);
        }
        else
        {
            SideBarL.color = new Color32(238, 63, 63, 255);
            SideBarR.color = new Color32(238, 63, 63, 255);
        }

    }

    public void SetExerciseTimer()
    {
        Timer.text = CurrentTime.ToString("0");
        if (!ExerciseDone)
        {
            if (getReady)
            {
                audioSource.PlayOneShot(PowerUp, volume);
                getReady = false;
                startCutscene = false;
                endCutscene = true;
                // if (currExercise == 0)
                // {
                //     UpcomingExerciseLabel.text = "Upcoming Exercise: " + exerciseList[currExercise];
                // }

                if (nextExercise < exerciseLength)
                {
                    setLabel("Get Ready");
                    CurrentTime = 5f;
                    UpcomingExerciseLabel.text = "Upcoming Exercise: " + exerciseList[nextExercise];
                    currExercise = nextExercise;
                }
            }
            if (!getReady && CurrentTime <= 0 && !startExercise)
            {
                CurrentTime = exerciseTimer + 0.3f;
                startExercise = true;
                currExercise = nextExercise;
                nextExercise++;
                Debug.LogWarning("CurrExercise" + currExercise);
                Debug.LogWarning("exLength" + exerciseLength);
                if (currExercise < exerciseLength)
                {
                    setLabel(exerciseList[currExercise]);
                }
                UpcomingExerciseLabel.text = "";
                audioSource.PlayOneShot(StartCue, volume);
            }

            if (!getReady && (CurrentTime <= 3) && !startExercise && !audioPlayed1)
            {
                audioPlayed1 = true;
                audioSource.PlayOneShot(CountDown, volume);
            }

            if (startExercise && CurrentTime <= 3 && !audioPlayed2)
            {
                audioPlayed2 = true;
                audioSource.PlayOneShot(CountDown, volume);
            }
            if (startExercise && CurrentTime <= 0)
            {
                ExerciseDone = true;
                startExercise = false;
                audioSource.PlayOneShot(StopCue, volume);
                audioPlayed2 = false;
                audioPlayed1 = false;
                currExercise = -1;
                // nextExercise++;
                if (nextExercise < exerciseLength)
                {
                    UpcomingExerciseLabel.text = "Upcoming Exercise: " + exerciseList[nextExercise];
                    CurrentTime = restTimer;
                    setLabel("Rest");
                    startCutscene = true;
                    endCutscene = false;
                }
                else if (nextExercise == exerciseLength)
                {
                    UpcomingExerciseLabel.text = "";
                }
            }
        }
        else
        {
            if (CurrentTime <= 0)
            {
                if (nextExercise < exerciseLength)
                {
                    CurrentTime = exerciseTimer;
                }
                // currExercise++;
                // nextExercise++;
                getReady = true;
                ExerciseDone = false;
            }
        }
        CurrentTime -= 1 * Time.deltaTime;
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void QuitGame()
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




