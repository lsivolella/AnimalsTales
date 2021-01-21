using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerCaveScene : MonoBehaviour
{
    [SerializeField] SceneController sceneController;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (SceneManager.GetActiveScene().name == "TownScene")
            sceneController.LoadCaveScene();
        else if (SceneManager.GetActiveScene().name == "CaveScene")
            sceneController.LoadTownScene();
    }
}
