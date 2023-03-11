using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PilotLogic : MonoBehaviour
{
    public TextMeshProUGUI Timer;
    public TextMeshProUGUI ExerciseLabel;
    public TextMeshProUGUI CurrentLevel;
    public Image SyncBar;
    public Image SideBarL;
    public Image SideBarR;
    public int SyncPercentage;

    private string ExerciseName;
    private float CurrentTime = 0f;
    private float RestTimer = 10.3f;
    private float exerciseTimer = 0f;

    private bool getReady = true;
    private bool startExercise = false;
    private bool ExerciseDone = false;

    private string[] exerciseList;
    private int selectedLevel;
    private int restTimer;
    private int setNo;
    private int exerciseLength;
    private int currExercise = 0;

    // Start is called before the first frame update
    void Start() 
    {
        selectedLevel = LevelSelectDisplay.selectedLevel;
        exerciseTimer = LevelSelectDisplay.exerciseTimer;
        restTimer = LevelSelectDisplay.restTimer;
        setNo = LevelSelectDisplay.setNo;
        exerciseList = LevelSelectDisplay.exerciseList;
        CurrentLevel.text = "Level " + selectedLevel;
        exerciseLength = exerciseList.Length;
    }

    void Update()
    {
        getSyncBar();
        setBar();

        if (currExercise < exerciseList.Length)
        {
            SetExerciseTimer();
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
        if (!ExerciseDone)
        {
            if (getReady)
            {
                CurrentTime = 5f;
                getReady = false;
                setLabel("Get Ready");
            }
            if (!getReady && CurrentTime <= 0 && !startExercise)
            {
                CurrentTime = exerciseTimer + 0.3f;
                startExercise = true;
                setLabel(exerciseList[currExercise]);
            }
            if (startExercise && CurrentTime <= 0)
            {
                startExercise = false;
                ExerciseDone = true;
                CurrentTime = RestTimer;
                setLabel("Rest");
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




