using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelStarsScript : MonoBehaviour
{
    public Level level;
    public Sprite CompletedStar;
    public Image star1;
    public Image star2;
    public Image star3;


    // Start is called before the first frame update
    void Start()
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
