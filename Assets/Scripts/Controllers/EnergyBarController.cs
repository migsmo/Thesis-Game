using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;
using UnityEngine.UI;

public class EnergyBarController : MonoBehaviour
{
    private static  Slider energySlider;
    private static string energyKey = "Energy";

    void Start()
    {
        energySlider = GetComponent<Slider>();


        // Load the energy value from PlayerPrefs
        int energy = PlayerPrefs.GetInt(energyKey, 100);

        if (energySlider != null)
        {
            energySlider.value = energy;
        }
       
    }

    public void SetEnergy(int energy)
    {
        
        energySlider.value = energy;

        // Save the energy value to PlayerPrefs
        PlayerPrefs.SetInt(energyKey, energy);
        PlayerPrefs.Save();
      
    }

    public int GetEnergy()
    {
        return (int)energySlider.value;
    }
}
