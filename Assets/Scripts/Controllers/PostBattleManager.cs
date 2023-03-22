using System.Collections;
using System.Collections.Generic;
using Resources;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PostBattleManager : MonoBehaviour
{
    public Image TotalBar;
    public Sprite CompletedStar;
    public Image star1;
    public Image star2;
    public Image star3;
    public TextMeshProUGUI PercentageLabel;
    public GameObject ExerciseRating;
    public TextMeshProUGUI ExerciseName;
    public TextMeshProUGUI Percentage;
    public ExerciseRatingDisplay Labels;
    private int total_stars = 0;
    private Report report;

    // Start is called before the first frame update
    void Start()
    {
        SaveManager saveManager = new SaveManager();
        report = PilotLogic.levelReport;

        for (int i = 0; i < PilotLogic.levelReport.getExerciseLength(); i++)
        {
            ExerciseRating.transform.GetChild(0).GetChild(0).name = report.exerciseList[i];
            ExerciseRating.transform.GetChild(0).GetChild(1).name = report.percentageList[i].ToString();
            GameObject exerciseElement = Instantiate(ExerciseRating, transform.position, transform.rotation) as GameObject;
            if (i < 9)
                exerciseElement.transform.SetParent(GameObject.FindGameObjectWithTag("BD1").transform, false);
            else
                exerciseElement.transform.SetParent(GameObject.FindGameObjectWithTag("BD2").transform, false);
        }

        PercentageLabel.text = report.totalPercentage.ToString("F2") + "%";
        TotalBar.fillAmount = (float)report.totalPercentage / 100;
        switch (report.earnedStars)
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

        if (report.earnedStars <= LevelSelectDisplay.currLevel.starsEarned) return;
        LevelSelectDisplay.currLevel.starsEarned = report.earnedStars;
        saveManager.Save(LevelSelectDisplay.currLevel);
    }

    public void Return()
    {
        SceneManager.LoadScene("LevelSelect");
    }
}
