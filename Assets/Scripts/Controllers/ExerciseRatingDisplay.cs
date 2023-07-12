using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExerciseRatingDisplay : MonoBehaviour
{
    public TextMeshProUGUI ExerciseLabel;
    public TextMeshProUGUI PercentageLabel;
    public GameObject NameCont;
    public GameObject PercentageCont;
    public Image RatingBar;

    void Start()
    {
        ExerciseLabel.text = NameCont.name;
        PercentageLabel.text = PercentageCont.name + "%";
        RatingBar.fillAmount = float.Parse(PercentageCont.name)/100;
    }
}
