using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class CoreOfLife : MonoBehaviour
{
    int lifeFlower;
    public static bool Click1 = false;
    public static bool Click2 = false;
    public static bool aniOn = false;
    public static bool aniOff = false;
    private int hp = 1;
    private bool Contact = false;
    private bool isOver = false;
    [SerializeField] private string selectableTag = "lifeFlower";

    private void Start()
    {
        lifeFlower = LayerMask.NameToLayer("lifeFlower");
    }
    private void Update()
    {
        ClickSelect();
        touch();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.CompareTag(selectableTag))
        {
            Contact = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(selectableTag))
        {
            Contact = false;
            if (hp == 1 && Input.GetMouseButtonUp(1))
            {
                Click2 = false;
            }
        }
        if (other.gameObject.CompareTag(selectableTag))
        {
            Contact = false;
            if (hp == 0 && Input.GetMouseButtonUp(0))
            {
                Click1 = false;
            }
        }
    }
    private void touch()
    {
        if (isOver == true)
        {
            if (Contact == true)
            {
                if (Input.GetMouseButtonDown(0) && EnergyNumber.a <= 2 && EnergyNumber.a >= 0 && hp == 1)
                {
                    Click1 = true;
                    aniOn = true;
                    hp -= 1;
                }
                if (Input.GetMouseButtonDown(1) && EnergyNumber.a >= 1 && EnergyNumber.a <= 3 && hp == 0)
                {
                    Click2 = true;
                    aniOff = true;
                    hp += 1;
                }
            }
        }
    }
    GameObject ClickSelect()
    {
        Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero, 0f,LayerMask.GetMask("lifeFlower"));
        var selection = hit.transform;
        if (selection.CompareTag(selectableTag))
        {
            isOver = true;
            return hit.transform.gameObject;
        }
        else
        {
            isOver = false;
            return null;
        }
    }
}
