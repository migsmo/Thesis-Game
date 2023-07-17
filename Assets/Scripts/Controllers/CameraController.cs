using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    private WebCamTexture webcamTexture;

    private void Start()
    {
        // Check if webcam is available
        if (WebCamTexture.devices.Length == 0)
        {
            Debug.Log("No webcam found!");
            return;
        }

        // Get the default webcam
        WebCamDevice defaultCamera = WebCamTexture.devices[0];

        // Create a new WebCamTexture using the default camera
        webcamTexture = new WebCamTexture(defaultCamera.name);

        // Set the texture of a material or UI element to display the webcam feed
        // For example, if you have a RawImage component attached to the same GameObject:
        RawImage rawImage = GetComponent<RawImage>();
        rawImage.texture = webcamTexture;

        // Start the webcam feed
        webcamTexture.Play();
    }
}
