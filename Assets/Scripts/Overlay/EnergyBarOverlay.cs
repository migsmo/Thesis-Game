using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class EnergyBarOverlay : MonoBehaviour
{
    private EnergyBarController energyBarController;
    

    void Start()
    {
     
        SceneManager.LoadSceneAsync("EnergyBar", LoadSceneMode.Additive);
        StartCoroutine(FindEnergyBarController());
    }

    private IEnumerator FindEnergyBarController()
    {
        // Wait until the EnergyBar scene has finished loading
        Scene energyBarScene = SceneManager.GetSceneByName("EnergyBar");
        yield return new WaitUntil(() => energyBarScene.isLoaded);

        // Find the EnergyBarController component in the EnergyBar scene
        EnergyBarController[] controllers = energyBarScene.GetRootGameObjects()
            .SelectMany(go => go.GetComponentsInChildren<EnergyBarController>(true))
            .ToArray();

        if (controllers.Length > 0)
        {
            // If there is more than one EnergyBarController in the scene, warn about it
            if (controllers.Length > 1)
            {
                Debug.LogWarning("Multiple EnergyBarController components found in EnergyBar scene. Using first one.");
            }

            // Use the first EnergyBarController found
            energyBarController = controllers[0];
        }
        else
        {
            Debug.LogError("Failed to find EnergyBarController component in EnergyBar scene.");
        }


    }

    public bool DecreaseEnergy(int energy)
    {
        print("called" + energy);
        print("energyBarController" + energyBarController);

        if (energy > energyBarController.GetEnergy())
        {
            return false;
        }

        energyBarController.SetEnergy(energyBarController.GetEnergy() - energy);

        return true;
    }
}
