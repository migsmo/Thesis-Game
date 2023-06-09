using System.Collections;
using System.Collections.Generic;
using Resources;
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

    public EnergyBarOverlay energyBarOverlay;
    public static Level currLevel;

    public Level level;
    public TMP_Text levelNo;
    public Button levelButton;
    public Image lockIcon;
    public GameObject Panel;
    public TMP_Text requiredStarsLabel;

    public Sprite CompletedStar;
    public Image star1;
    public Image star2;
    public Image star3;

    public TextMeshProUGUI LevelName;
    public TextMeshProUGUI EnergyCost;
    public TextMeshProUGUI Exercises;
    public TextMeshProUGUI Sets;
    public TextMeshProUGUI PlayTime;

    private int starRemainder;

    // Start is called before the first frame update
    void Awake()
    {
        levelNo.text = level.levelNumber.ToString();
        Panel.SetActive(false);
        int time = 0;
        
        SaveManager saveManager = new SaveManager();
        level.starsEarned = saveManager.Load(level);

        // Initialize Panel
        LevelName.text = "Level " + level.levelNumber;
        EnergyCost.text = "Energy Cost: " + level.energyCost;
        Sets.text = "Sets: " + level.setNo;
        Exercises.text = "Exercises (" + level.exerciseList.Length + ")";

        time = ((level.exerciseTimer + level.restTimer) * (level.exerciseList.Length * level.setNo) + (60 * (level.setNo - 1))) / 60;
        PlayTime.text = "Play Time: " + time + "min";
       
    }

    void Update()
    {
        if (!level.isUnlocked)
        {
            levelButton.enabled = false;
            levelNo.enabled = false;
            lockIcon.enabled = true;
            requiredStarsLabel.enabled = true;
            star1.enabled = false;
            star2.enabled = false;
            star3.enabled = false;
            starRemainder = level.starsRequired - LevelSelectManager.calculatedStars;

            if (starRemainder > 1)
            {
                requiredStarsLabel.text = "You need " + starRemainder + " more stars to unlock";
            }
            else
            {
                requiredStarsLabel.text = "You need " + starRemainder + " more star to unlock";
            }
        }
        else
        {
            switch (level.starsEarned)
            {
                case 3:
                    star3.sprite = CompletedStar;
                    star2.sprite = CompletedStar;
                    star1.sprite = CompletedStar;
                    break;
                case 2:
                    star2.sprite = CompletedStar;
                    star1.sprite = CompletedStar;
                    break;
                case 1:
                    star1.sprite = CompletedStar;
                    break;
            }
        }
    }

    public void OpenScene()
    {
        Debug.LogWarning("level.energyCost" + level.energyCost);
        bool isEnergyDeducted = energyBarOverlay.DecreaseEnergy(level.energyCost);
        if(!isEnergyDeducted)
        {
            return;
        }

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
