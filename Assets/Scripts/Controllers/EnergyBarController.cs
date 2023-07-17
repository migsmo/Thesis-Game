using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.WellKnownTypes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnergyBarController : MonoBehaviour
{
    private static Slider energySlider;
    private static string energyKey = "Energy";
    public TMP_Text energyCount;
    private static string lastResetDateKey = "LastEnergyResetDate";


    void Start()
    {
        energySlider = GetComponent<Slider>();
        if (IsNewDay())
        {
            SetEnergy(100);
        }
        else
        {

            // Load the energy value from PlayerPrefs
            int energy = PlayerPrefs.GetInt(energyKey, 100);

            //energy = 100;

            if (energySlider != null)
            {
                energySlider.value = energy;
                energyCount.text = energy.ToString();
            }
        }

    }

    private void Update()
    {
        if (IsNewDay())
        {
            SetEnergy(100);
        }
    }

    private bool IsNewDay()
    {
        string savedDate = PlayerPrefs.GetString(lastResetDateKey);

        string currentDate = System.DateTime.Now.ToString("yyyyMMdd");

        return (savedDate != currentDate);
    }

    public void SetEnergy(int energy)
    {

        energySlider.value = energy;
        energyCount.text = energy.ToString();

        // Save the energy value to PlayerPrefs
        PlayerPrefs.SetInt(energyKey, energy);
        PlayerPrefs.Save();

        // Save the current date for energy reset
        string currentDate = System.DateTime.Now.ToString("yyyyMMdd");
        PlayerPrefs.SetString(lastResetDateKey, currentDate);

    }

    public int GetEnergy()
    {
        return (int)energySlider.value;
    }
}




// Test script rests at 60s
//using System.Collections;
//using System.Collections.Generic;
//using Google.Protobuf.WellKnownTypes;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//public class EnergyBarController : MonoBehaviour
//{
//    private static Slider energySlider;
//    private static string energyKey = "Energy";
//    private static string lastResetTimeKey = "LastEnergyResetTime";
//    public TMP_Text energyCount;

//    private float resetInterval = 60f; // Reset interval in seconds
//    private float nextResetTime; // Time of the next reset

//    void Start()
//    {
//        energySlider = GetComponent<Slider>();

//        // Set the initial reset time
//        nextResetTime = Time.time + resetInterval;

//        // Load the energy value from PlayerPrefs
//        int energy = PlayerPrefs.GetInt(energyKey, 100);

//        if (energySlider != null)
//        {
//            energySlider.value = energy;
//            energyCount.text = energy.ToString();
//        }
//    }

//    private void Update()
//    {
//        // Check if it's time for a reset
//        if (Time.time >= nextResetTime)
//        {
//            // Reset energy to 100
//            SetEnergy(100);

//            // Calculate the time of the next reset
//            nextResetTime = Time.time + resetInterval;
//        }
//    }

//    public void SetEnergy(int energy)
//    {
//        energySlider.value = energy;
//        energyCount.text = energy.ToString();

//        // Save the energy value to PlayerPrefs
//        PlayerPrefs.SetInt(energyKey, energy);
//        PlayerPrefs.Save();
//    }

//    public int GetEnergy()
//    {
//        return (int)energySlider.value;
//    }
//}
