using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextEffect : MonoBehaviour
{
    public static GameObject txt;

    private void Start()
    {
        txt = GameObject.Find("TextEffect");
        txt.SetActive(false);
    }
}
