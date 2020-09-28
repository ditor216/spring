using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyNumber : MonoBehaviour
{
    public static int a;
    [SerializeField] private GameObject EnergyTank1_1;
    [SerializeField] private GameObject EnergyTank2_1;
    [SerializeField] private GameObject EnergyTank3_1;

    private void Start()
    {
        EnergyTank1_1.SetActive(false);
        EnergyTank2_1.SetActive(false);
        EnergyTank3_1.SetActive(false);
    }

    void Update()
    {
        if (a == 0)
        {
            EnergyTank1_1.SetActive(false);
            EnergyTank2_1.SetActive(false);
            EnergyTank3_1.SetActive(false);
        }
        if (a == 1)
        {
            EnergyTank1_1.SetActive(true);
            EnergyTank2_1.SetActive(false);
            EnergyTank3_1.SetActive(false);
        }
        if (a == 2)
        {
            EnergyTank1_1.SetActive(true);
            EnergyTank2_1.SetActive(true);
            EnergyTank3_1.SetActive(false);
        }
        if (a == 3)
        {
            EnergyTank1_1.SetActive(true);
            EnergyTank2_1.SetActive(true);
            EnergyTank3_1.SetActive(true);
        }
        if (CoreOfLife.Click1 == true)
        {
            a += 1;
            CoreOfLife.Click1 = false;
        }
        if(CoreOfLife.Click2 == true)
        {
            a -= 1;
            CoreOfLife.Click2 = false;
        }
    }
}
