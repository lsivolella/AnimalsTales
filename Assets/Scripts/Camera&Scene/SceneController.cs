using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class SceneController : MonoBehaviour
{
    public static SceneController instance { get; private set; }

    [SerializeField] Camera mainCamera;
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] Vector2 townSpawnPoint;
    [SerializeField] Vector2 caveSpawnPoint;

    GameObject playerGameObject;
    private static bool firstTimePlayed = true;
    private static bool cinematicsOn = false;
    private static bool cinematicsPlayed = false;

    public bool CinematicsOn { get { return cinematicsOn; } set { cinematicsOn = value; } }
    public bool CinematicsPlayed { get {return cinematicsPlayed; } set {cinematicsPlayed = value; } }

    //private GameObject playerGameObject;
    private string activeSceneName;

    private void Awake()
    {
        SetUpSingleton();
    }

    private void SetUpSingleton()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance.gameObject);
        }
        else if (instance != null)
            Destroy(instance.gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GetActiveScene();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void GetActiveScene()
    {
        activeSceneName = SceneManager.GetActiveScene().name;
    }

    public void LoadCaveScene()
    {
        SceneManager.LoadScene("CaveScene");
    }

    public void LoadTownScene()
    {
        SceneManager.LoadScene("TownScene");
    }

    public void ActivateVituralCamera2()
    {
        Debug.Log("Activate VCAM 2");

    }

    public void StartBlackCatDialogue()
    {
        Debug.Log("Black Cat Dialogue");
    }

    public void StartCinematics()
    {
        cinematicsOn = true;
    }

    public void TerminateCinamatics()
    {
        cinematicsOn = false;
    }
}
