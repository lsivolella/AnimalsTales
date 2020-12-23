using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    SceneController[] sceneController;

    private int currentScene;
        
    private void Awake()
    {
        SetUpSingleton();
        GetCurrentScene();
    }

    private void SetUpSingleton()
    {
        sceneController = FindObjectsOfType<SceneController>();

        if (sceneController.Length > 1)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void GetCurrentScene()
    {
        currentScene = SceneManager.GetActiveScene().buildIndex;
    }

    public void LoadCaveScene()
    {
        SceneManager.LoadScene("CaveScene");
    }

    public void LoadTownScene()
    {
        SceneManager.LoadScene("TownScene");
    }

    // TODO: fix the place where the player is spawned when he exits the cave scene and enter the town scene
}
