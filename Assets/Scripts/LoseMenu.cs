using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseMenu : MonoBehaviour
{
    
    public void LoadMenu()
    {
        GameManager.Instance.LoadMainMenu();
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
