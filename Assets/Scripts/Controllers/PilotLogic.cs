using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PilotLogic : MonoBehaviour
{
    public TextMeshProUGUI Timer;
    public TextMeshProUGUI ExerciseLabel;
    public TextMeshProUGUI CurrentLevel;
    public TextMeshProUGUI SetLabel;
    public TextMeshProUGUI UpcomingExerciseLabel;
    public TextMeshProUGUI AveLabel;
    public TextMeshProUGUI ExerciseLabel2;
    public Image SyncBar;
    public Image SideBarL;
    public Image SideBarR;
    public Image AveBar;
    public int SyncPercentage;
    public int AvePercentage;
    public AudioClip StartCue;
    public AudioClip StopCue;
    public AudioSource audioSource;
    private float volume = 1;
    public static Report levelReport;

    private string ExerciseName;
    private float CurrentTime = 0f;
    private float exerciseTimer = 0f;
    private int restTimer;
    private float setRestTimer=60f;

    private bool getReady = true;
    private bool startExercise = false;
    private bool ExerciseDone = false;
    private bool SetDone = false;
    private bool startCutscene = false;
    private bool endCutscene = false;
    private bool inFrame = false;

    private string[] exerciseList;
    private int selectedLevel;
    private int setNo;
    private int exerciseLength;
    public int currExercise = -1;
    private int currSet = 0;
    public int nextExercise = 0;
    
    public int[] percentageList = new int[17];

    // Start is called before the first frame update
    void Start()
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

        if (inFrame)
        {
            if (currExercise < exerciseList.Length && currSet < setNo)
            {
                if (SetDone)
                {
                    Timer.text = CurrentTime.ToString("0");
                    CurrentTime -= 1 * Time.deltaTime;
                    if (CurrentTime <= 0)
                    {
                        SetDone = false;
                    }
                }
                else
                    SetExerciseTimer();
            }
            else if (currExercise == exerciseList.Length && currSet < setNo)
            {
                currSet++;
                if (currSet == setNo)
                {
                    ExerciseLabel.text = "Level Complete!";
                    levelReport.generateReport();
                    SceneManager.LoadScene("PostBattle");
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
        else
        {
            ExerciseLabel.text = "Body not in Frame";
        }

        if (startCutscene)
        {
            if (RestCutscenePanel.alpha < 1)
            {
                RestCutscenePanel.alpha += Time.deltaTime;
                if (RestCutscenePanel.alpha >= 1)
                {
                    startCutscene = false;
                }
            }
        }
        
        if (endCutscene)
        {
            if (RestCutscenePanel.alpha >= 0)
            {
                RestCutscenePanel.alpha -= Time.deltaTime;
                if (RestCutscenePanel.alpha == 0)
                {
                    endCutscene = false;
                }
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
        if (SetDone)
        {
            Debug.Log("In 2");
            CurrentTime = setRestTimer;
            SetDone = false;
        }

        if (!ExerciseDone)
        {
            if (getReady)
            {
                CurrentTime = 5f;
                getReady = false;
                setLabel("Get Ready");

                // if (currExercise == 0)
                // {
                //     UpcomingExerciseLabel.text = "Upcoming Exercise: " + exerciseList[currExercise];
                // }
                
                UpcomingExerciseLabel.text = "Upcoming Exercise: " + exerciseList[nextExercise];
            }
            if (!getReady && CurrentTime <= 0 && !startExercise)
            {
                CurrentTime = exerciseTimer + 0.3f;
                startExercise = true;
                currExercise = nextExercise;
                nextExercise++;
                setLabel(exerciseList[currExercise]);
                UpcomingExerciseLabel.text = "";
                audioSource.PlayOneShot(StartCue, volume);
            }
            if (startExercise && CurrentTime <= 0)
            {
                startExercise = false;
                ExerciseDone = true;
                CurrentTime = restTimer;
                audioSource.PlayOneShot(StopCue, volume);
                setLabel("Rest");
                currExercise = -1;
                // nextExercise++;
                if (nextExercise < exerciseLength)
                {
                    UpcomingExerciseLabel.text = "Upcoming Exercise: " + exerciseList[nextExercise];
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
                ExerciseDone = false;
                CurrentTime = exerciseTimer;
                // currExercise++;
                // nextExercise++;
                currExercise = -1;
                getReady = true;
            }
        }
        CurrentTime -= 1 * Time.deltaTime;
    }
}




