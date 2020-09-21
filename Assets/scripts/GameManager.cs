using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject illustratedMenu;
    public GameObject plant1;
    public GameObject plant2;
    public GameObject plant3;
    public GameObject plant4;
    public GameObject plant5;
    public GameObject plant6;
    public GameObject page11;
    public GameObject page12;
    public GameObject page13;

    public int Tab = 0;


    void Update()
    {
        print (Tab);
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            pauseMenu.SetActive(true);
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            Tab += 1;
        }
        if (Tab % 2 == 0)
        {
            illustratedMenu.SetActive(false);
        }
        else
        {
            illustratedMenu.SetActive(true);
        }
    }
    public void StartGame()
    {
        SceneManager.LoadScene("game-1");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    public void Restart()
    {
        Application.LoadLevel(Application.loadedLevel);
    }
    public void Plant1()
    {
        plant1.SetActive(true);
    }
    public void Page11()
    {
        page11.SetActive(true);
        page12.SetActive(false);
        page13.SetActive(false);
    }
    public void Page12()
    {
        page12.SetActive(true);
        page11.SetActive(false);
        page13.SetActive(false);
    }
    public void Page13()
    {
        page13.SetActive(true);
        page11.SetActive(false);
        page12.SetActive(false);
    }
}
