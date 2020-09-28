using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public static CameraControl singleton;
    public Transform Player;
    public Vector3 Offset_ToPlayer;
    public float Force_Shock;

    [Header(" ")]
    [Range(0.0f, 1.0f)] public float Lerp;

    public void Awake()
    {
        singleton = this;   // make anyone can use it without GetComponent<>()
    }

    public void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, Player.position + Offset_ToPlayer, Lerp);     // follow player with the smoooth move
    }

    public void CameraShock()
    {
        transform.position += Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)) * new Vector3(Force_Shock, 0.0f, 0.0f);  // set camera's position to random to make the shock effect
    }
}
