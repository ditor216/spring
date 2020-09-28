using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class imageSize : MonoBehaviour
{
    Image Image;
    void Start()
    {
        Image = this.transform.GetComponent<Image>();
    }
    void Update()
    {
        Image.SetNativeSize();
    }
}
