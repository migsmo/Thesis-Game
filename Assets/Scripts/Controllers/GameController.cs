using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public StoryScene currentScene;
    public SpeechBarController speechBar;
    public SpriteSwitcher backgroundController;

    // Start is called before the first frame update
    void Start()
    {
        speechBar.PlayScene(currentScene);
        backgroundController.SetImage(currentScene.background);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if(speechBar.IsCompleted())
            {
                if(speechBar.IsLastSentence())
                {
                    currentScene = currentScene.nextScene;
                    speechBar.PlayScene(currentScene);
                    backgroundController.SwitchImage(currentScene.background);
                } else
                {
                    speechBar.PlayNextSentence();

                }
            }
        }
    }
}
