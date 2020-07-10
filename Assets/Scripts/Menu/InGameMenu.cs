using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{
    [SerializeField] GameObject MenuBoard;
    bool isActive = false;

    private void Start()
    {
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleActive();
        }
    }

    public void ToggleActive()
    {
        if (isActive)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isActive = false;
            MenuBoard.SetActive(false);
            Time.timeScale = 1.0f;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            isActive = true;
            MenuBoard.SetActive(true);
            Time.timeScale = 0.0f;
        }
    }

    public void MainMenu() { Time.timeScale = 1.0f; SceneManager.LoadScene("MainMenu", LoadSceneMode.Single); }
    public void QuitGame() { Application.Quit(); }
}
