using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyBarController : MonoBehaviour
{
    Slider energySlider;

    private void Start()
    {
        energySlider = GetComponent<Slider>();
    }

    public void SetEnergy(int energy)
    {
        energySlider.value = energy;
    }  
}
