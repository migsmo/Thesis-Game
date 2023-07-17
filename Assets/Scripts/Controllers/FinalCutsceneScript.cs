using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FinalCutsceneScript : MonoBehaviour
{
    private float timer = 10f;
    public Animator transition;
    public float transitionTime;
    private float transitionTimer;
    public bool transitionDone = false;

    // Start is called before the first frame update
    void Start()
    {
        // Timer.text = timer.ToString("f0") + "s";
        // transitionTimer = 2.5f;
    }

    // Update is called once per frame
    void Update()
    {
        if (transitionTimer > 0)
        {
            transitionTimer -= Time.deltaTime;
        
            // When the transition timer reaches zero, set transitionDone to true
            if (transitionTimer <= 0)
            {
                transitionDone = true;
            }
        }
        else
        {
            timer -= 1 * Time.deltaTime;

            if (timer <= 0)
            {
                StartCoroutine(LoadLevel("PostBattle"));
            }
        }

        IEnumerator LoadLevel(string sceneName)
        {
            transition.SetTrigger("Start");

            yield return new WaitForSeconds(transitionTime);

            SceneManager.LoadScene(sceneName);
        }
    }
}
