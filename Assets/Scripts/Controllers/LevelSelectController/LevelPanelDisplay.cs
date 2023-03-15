using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelPanelDisplay : MonoBehaviour
{
    public Level level;
    public TextMeshProUGUI LevelName;
    public TextMeshProUGUI EnergyCost;
    public TextMeshProUGUI Exercises;
    public TextMeshProUGUI Sets;

    // Start is called before the first frame update
    void Start()
    {
        LevelName.text = "Level " + level.levelNumber;
        EnergyCost.text = "Energy Cost: " + level.energyCost;
        Exercises.text = "Exercises: " + level.exerciseList.Length;
        Sets.text = "Sets: " + level.setNo;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
