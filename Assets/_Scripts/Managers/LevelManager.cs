using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private static LevelManager instance;

    public static LevelManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("Level Manager is null.");
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadOptionsMenu()
    {

    }

    public void LoadTown()
    {
        SceneManager.LoadScene(3);
    }

    public void LoadLevelSelectionScene()
    {

    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }



}
