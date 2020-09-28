using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lifeFlower : MonoBehaviour
{
    public Animator ani;

    void Update()
    {
        if(CoreOfLife.aniOn == true)
        {
            ani.SetBool("click", true);
            CoreOfLife.aniOn = false;
        }
        if(CoreOfLife.aniOff == true)
        {
            ani.SetBool("click", false);
            CoreOfLife.aniOff = false;
        }
    }
}
