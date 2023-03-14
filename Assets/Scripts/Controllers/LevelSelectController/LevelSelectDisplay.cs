using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelSelectDisplay : MonoBehaviour
{

    public static int selectedLevel;
    public static int exerciseTimer;
    public static int restTimer;
    public static int setNo;
    public static string[] exerciseList;

    public Level level;
    public TMP_Text levelNo;
    public Button levelButton;
    public Image lockIcon;

    // Start is called before the first frame update
    void Start()
    {
        levelNo.text = level.levelNumber.ToString();
    }

    void Update()
    {
        if (!level.isUnlocked)
        {
            levelButton.enabled = false;
            levelNo.enabled = false;
            lockIcon.enabled = true;
        }
    }

    public void OpenScene()
    {
        selectedLevel = level.levelNumber;
        exerciseTimer = level.exerciseTimer;
        restTimer = level.restTimer;
        setNo = level.setNo;
        exerciseList = level.exerciseList;
        SceneManager.LoadScene("Pilot");
    }
}
