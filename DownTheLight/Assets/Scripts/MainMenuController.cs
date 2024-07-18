using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    SceneManager sceneManager;
    [SerializeField] GameObject _startButton;

    private void Awake()
    {
        sceneManager = new SceneManager();
        EventSystem.current.SetSelectedGameObject(_startButton);
    }
    public void LoadLevel()
    {
        SceneManager.LoadScene("Prototype");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
