using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class EnergyBarOverlay : MonoBehaviour
{
    private EnergyBarController energyBarController;
    private PopupWindowController popupWindowController;
    private static string lastEnergyDeductedKey = "LastEnergyDeducted";

    void Start()
    {
     
        SceneManager.LoadSceneAsync("EnergyBar", LoadSceneMode.Additive);
        StartCoroutine(FindEnergyBarController());
    }

    private IEnumerator FindEnergyBarController()
    {
        Scene energyBarScene = SceneManager.GetSceneByName("EnergyBar");
        yield return new WaitUntil(() => energyBarScene.isLoaded);

        EnergyBarController[] energyController = energyBarScene.GetRootGameObjects()
            .SelectMany(go => go.GetComponentsInChildren<EnergyBarController>(true))
            .ToArray();

        
        if (energyController.Length > 0 )
        {
            if (energyController.Length > 1) 
            {
                Debug.LogWarning("Multiple EnergyBarController components found in EnergyBar scene. Using first one.");
            }
          
            energyBarController = energyController[0];
        }
        else
        {
            Debug.LogError("Failed to find EnergyBarController component in EnergyBar scene.");
        }

        PopupWindowController[] popController = energyBarScene.GetRootGameObjects()
         .SelectMany(go => go.GetComponentsInChildren<PopupWindowController>(true))
         .ToArray();

        if ( popController.Length > 0)
        {
            if (popController.Length > 1)
            {
                Debug.LogWarning("Multiple PopupWindowController components found in EnergyBar scene. Using first one.");
            }
            popupWindowController = popController[0];

        }
        else
        {
            Debug.LogError("Failed to find PopupWindowController component in EnergyBar scene.");
        }



    }

    public bool DecreaseEnergy(int energy)
    {
        print("called" + energy);
        print("energyBarController" + energyBarController);

        if (energy > energyBarController.GetEnergy())
        {
            popupWindowController.AddToQueue("Not enough energy");
            return false;
        }

        PlayerPrefs.SetInt(lastEnergyDeductedKey, energy);
        PlayerPrefs.Save();

        energyBarController.SetEnergy(energyBarController.GetEnergy() - energy);

        return true;
    }
}
