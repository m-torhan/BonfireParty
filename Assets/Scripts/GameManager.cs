using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

    private Scene? currentlyLoadedScene;

    private void Awake()
    {
        Instance = this;
        LoadMainMenu();

        SceneManager.sceneLoaded += this.OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        // Debug.Log("New scene loaded: " + scene);
        currentlyLoadedScene = scene;
        SceneManager.SetActiveScene(scene);
    }


    public void LoadMainMenu()
    {
        UnloadCurrentScene();
        SceneManager.LoadScene("Menu", LoadSceneMode.Additive);
    }

    public void LoadMainMap()
    {
        UnloadCurrentScene();
        SceneManager.LoadScene("Map", LoadSceneMode.Additive);
    }

    public void LoadLoseScreen()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        UnloadCurrentScene();
        SceneManager.LoadScene("Lose", LoadSceneMode.Additive);
    }

    public void LoadWinScreen()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        UnloadCurrentScene();
        SceneManager.LoadScene("Win", LoadSceneMode.Additive);
    }

    private void UnloadCurrentScene()
    {
        if(currentlyLoadedScene != null)
        {
            SceneManager.UnloadSceneAsync((Scene)currentlyLoadedScene);
            currentlyLoadedScene = null;
        }
    }
}
