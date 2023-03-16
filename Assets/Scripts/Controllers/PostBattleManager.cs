using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PostBattleManager : MonoBehaviour
{

    public Image TotalBar;
    public Sprite CompletedStar;
    public Image star1;
    public Image star2;
    public Image star3;
    public TextMeshProUGUI PercentageLabel;
    private int TotalPercentage = 0;
    public GameObject ExerciseRating;
    public TextMeshProUGUI ExerciseName;
    public TextMeshProUGUI Percentage;
    public ExerciseRatingDisplay Labels;
    private string[] exerciseList = new string[11] { "Wall Sit", "Sumo Squat", "Static Lunge(L)", "Static Lunge(R)",
                                      "Glute Bridge", "Single Leg Glute Bridge(L)", "Single Leg Glute Bridge(R)",
                                      "Straight Bridge", "Single Leg Glute Bridge(L)", "Single Leg Glute Bridge(R)",
                                      "Straight Bridge"};
    private int[] percentageList = new int[11] {60, 70, 100, 50, 50, 60, 80, 70, 100, 100, 100 };
    private int total_stars = 0;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < exerciseList.Length; i++)
        {
            ExerciseRating.transform.GetChild(0).GetChild(0).name = exerciseList[i];
            ExerciseRating.transform.GetChild(0).GetChild(1).name = percentageList[i].ToString();
            TotalPercentage += percentageList[i];
            Debug.Log(TotalPercentage);
            GameObject exerciseElement = Instantiate(ExerciseRating, transform.position, transform.rotation) as GameObject;
            if (i < 9)
                exerciseElement.transform.SetParent(GameObject.FindGameObjectWithTag("BD1").transform, false);
            else
                exerciseElement.transform.SetParent(GameObject.FindGameObjectWithTag("BD2").transform, false);
        }
        TotalPercentage = TotalPercentage / exerciseList.Length;
        Debug.Log(TotalPercentage);
        PercentageLabel.text = TotalPercentage.ToString() + "%";
        TotalBar.fillAmount = (float)TotalPercentage / 100;
        if (TotalPercentage >= 75)
        {
            total_stars = 3;
            star3.GetComponent<Image>().sprite = CompletedStar;
            star2.GetComponent<Image>().sprite = CompletedStar;
            star1.GetComponent<Image>().sprite = CompletedStar;
        }
        else if (TotalPercentage < 75 && TotalPercentage >= 50)
        {
            total_stars = 2;
            star2.GetComponent<Image>().sprite = CompletedStar;
            star1.GetComponent<Image>().sprite = CompletedStar;

        }
        else
        {
            total_stars = 1;
            star1.GetComponent<Image>().sprite = CompletedStar;
        }
    }
}
