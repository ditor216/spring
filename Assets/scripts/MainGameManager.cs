using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainGameManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public int Tab = 0;

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            pauseMenu.SetActive(true);
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
    public void Cancel()
    {
        SceneManager.LoadScene("spring");
    }


}
