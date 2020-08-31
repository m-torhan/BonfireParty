using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
   public void PlayGame ()
    {
        GameManager.Instance.LoadMainMap();
    }

    public void QuitGame ()
    {
        Application.Quit();
    }

}
