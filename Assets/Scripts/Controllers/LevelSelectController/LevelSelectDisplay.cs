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
    public static Level currLevel;

    public Level level;
    public TMP_Text levelNo;
    public Button levelButton;
    public Image lockIcon;
    public GameObject Panel;

    public Sprite CompletedStar;
    public Image star1;
    public Image star2;
    public Image star3;

    public TextMeshProUGUI LevelName;
    public TextMeshProUGUI EnergyCost;
    public TextMeshProUGUI Exercises;
    public TextMeshProUGUI Sets;
    public TextMeshProUGUI PlayTime;

    // Start is called before the first frame update
    void Start()
    {
        levelNo.text = level.levelNumber.ToString();
        Panel.SetActive(false);
        int time = 0;

        // Initialize Panel
        LevelName.text = "Level " + level.levelNumber;
        EnergyCost.text = "Energy Cost: " + level.energyCost;
        Sets.text = "Sets: " + level.setNo;
        Exercises.text = "Exercises (" + level.exerciseList.Length + ")";

        time = ((level.exerciseTimer + level.restTimer) * (level.exerciseList.Length * level.setNo) + (60 * (level.setNo - 1))) / 60;
        PlayTime.text = "Play Time: " + time + "min";
    }

    void OnEnable()
    {
        switch (level.starsEarned)
        {
            case 3:
                star3.GetComponent<Image>().sprite = CompletedStar;
                star2.GetComponent<Image>().sprite = CompletedStar;
                star1.GetComponent<Image>().sprite = CompletedStar;
                break;
            case 2:
                star2.GetComponent<Image>().sprite = CompletedStar;
                star1.GetComponent<Image>().sprite = CompletedStar;
                break;
            case 1:
                star1.GetComponent<Image>().sprite = CompletedStar;
                break;
        }

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
        currLevel = level;
        SceneManager.LoadScene("Pilot");
    }

    public void OnMouseOver()
    {
        if (level.isUnlocked)
        {
            Panel.SetActive(true);
        }
    }

    public void OnMouseExit()
    {
        Panel.SetActive(false);
    }
}
