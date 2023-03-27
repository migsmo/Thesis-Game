using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FinalCutsceneScript : MonoBehaviour
{
    public TextMeshProUGUI Timer;
    private float timer = 10f;
    
    // Start is called before the first frame update
    void Start()
    {
        Timer.text = timer.ToString("f0") + "s";
    }

    // Update is called once per frame
    void Update()
    {
        timer -= 1 * Time.deltaTime;
        Timer.text = timer.ToString("f0") + "s";

        if (timer <= 0)
        {
            SceneManager.LoadScene("PostBattle");
        }
    }
}
