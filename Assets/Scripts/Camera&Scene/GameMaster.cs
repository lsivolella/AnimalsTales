using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public static GameMaster instance;

    #region Boss and Minions Variables

    public bool IsBossAlive { get; set; }
    public bool BossHealthSetUp { get; set; }
    public int BossMaxHealth { get; set; }
    public int BossHealth { get; set; }
    public Vector2 BossDeathPosition { get; set; }
    public Vector2 LeftMinionBodyPosotion { get; set; }
    public Vector2 RightMinionBodyPosotion { get; set; }

    #endregion

    #region Player Variables

    public Vector2 PlayerPosition { get; set; }
    public Vector2 PlayerDefaultPosition { get; set; }
    public Vector2 PlayerSpawnPosition { get; set; }
    public int PlayerHealth { get; set; }
    public int PlayerMaxHealth { get; set; }
    public bool PlayerHealthSetUp { get; set; }
    public bool PlayerMovementSetUp { get; set; }
    public bool PlayerSpawnSetUp { get; set; }

    #endregion

    #region Game Variables

    public enum HatStatus { NotDropped, AtFlor, Inventory, Delivered }
    public HatStatus hatStatus;

    #endregion

    private void Awake()
    {
        SetUpSingleton();
    }

    private void SetUpSingleton()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != null)
            Destroy(gameObject);
    }

    public void ResetGame()
    {
        // Boss & Minions Variables
        IsBossAlive = true;
        BossHealthSetUp = false;
        BossHealth = BossMaxHealth;
        BossDeathPosition = Vector2.zero;
        LeftMinionBodyPosotion = Vector2.zero;
        RightMinionBodyPosotion = Vector2.zero;

        // Player Variables
        PlayerPosition = PlayerDefaultPosition;
        PlayerSpawnPosition = PlayerDefaultPosition;
        PlayerHealth = PlayerMaxHealth;
        PlayerHealthSetUp = false;
        PlayerMovementSetUp = false;
        PlayerSpawnSetUp = false;

        // Game Variables
        hatStatus = HatStatus.NotDropped;
        SceneController.instance.CinematicsPlayed = false;
    }
}
