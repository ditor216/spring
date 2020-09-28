using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npc1 : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.name.Equals ("npc1"))
        {
            TextEffect.txt.SetActive(true);
        }
    }
}
