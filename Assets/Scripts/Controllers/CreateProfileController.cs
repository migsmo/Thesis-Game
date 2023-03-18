using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreateProfileController : MonoBehaviour
{
    // Start is called before the first frame update
    public void Confirm()
    {
        SceneManager.LoadScene("Intro");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
