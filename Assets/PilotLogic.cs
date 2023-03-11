using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PilotLogic : MonoBehaviour
{
    public TextMeshProUGUI Timer;
    public TextMeshProUGUI ExerciseLabel;
    public Image SyncBar;
    public Image SideBarL;
    public Image SideBarR;
    public int SyncPercentage;
    public float ExerciseTimer = 0f;
    public string ExerciseName = "Static Lunge";
    private float CurrentTime = 0f;
    private float RestTimer = 10.3f;
    private bool ExerciseDone = false;
    private bool getReady = true;
    private bool startExercise = false;
    private  string[] ExerciseList = new string[8] { "Wall Sit", "Sumo Squat", "Static Lunge(L)", "Static Lunge(R)",
                                      "Glute Bridge", "Single Leg Glute Bridge(L)", "Single Leg Glute Bridge(R)",
                                      "Straight Bridge"};
    private int currExercise=0;
    // Start is called before the first frame update
    void Start()
    {
        setLabel(ExerciseList[currExercise]);
    

        private int selectedLevel;
        private int exerciseTimer;
        private int restTimer;
        private int setNo;
        private int currentExercise;
        private string[] exerciseList;
        private int exerciseLength;

        public TMP_Text exerciseLabel;
        public TMP_Text currentLevel;

    // Start is called before the first frame update
    void Start()
    {
        selectedLevel = LevelSelectDisplay.selectedLevel;
        exerciseTimer = LevelSelectDisplay.exerciseTimer;
        restTimer = LevelSelectDisplay.restTimer;
        setNo = LevelSelectDisplay.setNo;
        exerciseList = LevelSelectDisplay.exerciseList;
        currentLevel.text = "Level " + selectedLevel;

        exerciseLength = exerciseList.Length;
        currentExercise = 0;
    }

    // Update is called once per frame
    void Update()
    {
        getSyncBar();
        getLabel();
        setBar();
        if (currExercise < ExerciseList.Length)
        {
            SetExerciseTimer();
        }
    }

    IEnumerator ExerciseCoroutine()
    {
        while(currentExercise < exerciseLength)
        {
            yield return new WaitForSeconds(2f);
            currentExercise++;
            Debug.Log(exerciseList[currentExercise]);
        }
    }

    public void getSyncBar()
    {
        SyncBar.fillAmount = (float)SyncPercentage / 100;
    }

    public void setLabel(string Label)
    {
        ExerciseName = Label;
        exerciseLabel.text = exerciseList[currentExercise];
    }

    public void getLabel()
    {
        exerciseLabel.text = exerciseList[currentExercise];
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
            if(!getReady && CurrentTime <=0 && !startExercise)
            {
                CurrentTime = ExerciseTimer + 0.3f;
                startExercise = true;
                setLabel(ExerciseList[currExercise]);
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
                CurrentTime = ExerciseTimer;
                currExercise++;
                getReady = true;
            }
        }
        CurrentTime -= 1 * Time.deltaTime;
    }
}
