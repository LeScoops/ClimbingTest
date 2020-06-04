using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void StartGame() { SceneManager.LoadScene("TestScene01", LoadSceneMode.Single); }
    public void Instructions() { SceneManager.LoadScene("Instructions", LoadSceneMode.Single); }
    public void MainMenu() { SceneManager.LoadScene("MainMenu", LoadSceneMode.Single); }
    public void QuitGame() { Application.Quit(); }
}
