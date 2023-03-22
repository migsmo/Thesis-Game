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
    public Image SyncBar;
    public Image SideBarL;
    public Image SideBarR;
    public int SyncPercentage;
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

    private string[] exerciseList;
    private int selectedLevel;
    private int setNo;
    private int exerciseLength;
    public int currExercise = 0;
    private int currSet = 0;
    
    public int[] percentageList = new int[17] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };

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
        exerciseLength = exerciseList.Length;
        
        // Temporary code to be replaced to test post battle scene
        levelReport = new Report(exerciseList, percentageList);
        levelReport.generateReport();
    }

    void Update()
    {
        getSyncBar();
        setBar();

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
                SceneManager.LoadScene("PostBattle");
            }
            else
            {
                SetDone = true;
                SetLabel.text = "Set " + (currSet + 1) + " / " + setNo;
                CurrentTime = setRestTimer;
                currExercise = 0;
            }

        }
    }

    public void getSyncBar()
    {
        SyncBar.fillAmount = (float)SyncPercentage / 100;
    }

    public void setLabel(string Label)
    {
        ExerciseName = Label;
        ExerciseLabel.text = ExerciseName;
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

                if (currExercise == 0)
                {
                    UpcomingExerciseLabel.text = "Upcoming Exercise: " + exerciseList[currExercise];
                }

            }
            if (!getReady && CurrentTime <= 0 && !startExercise)
            {
                CurrentTime = exerciseTimer + 0.3f;
                startExercise = true;
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
                if (currExercise + 1 < exerciseLength)
                {
                    UpcomingExerciseLabel.text = "Upcoming Exercise: " + exerciseList[currExercise + 1];
                }
                else if (currExercise == exerciseLength)
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
                currExercise++;
                getReady = true;
            }
        }
        CurrentTime -= 1 * Time.deltaTime;
    }
}




