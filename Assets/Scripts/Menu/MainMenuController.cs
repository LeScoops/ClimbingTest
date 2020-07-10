using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] string sceneToLoad;

    public void StartGame() { SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single); }
    public void Instructions() { SceneManager.LoadScene("Instructions", LoadSceneMode.Single); }
    public void MainMenu() { SceneManager.LoadScene("MainMenu", LoadSceneMode.Single); }
    public void QuitGame() { Application.Quit(); }
}
