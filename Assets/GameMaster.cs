﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public static GameMaster instance;

    //private static bool isBossAlive;

    //public bool IsBossAlive { get { return isBossAlive; } set { isBossAlive = value; } }

    public bool IsBossAlive { get; set; }
    public bool HasMadeExchange { get; set; } = false;
    public int BossHealth { get; set; }
    public Vector2 BossDeathPosition { get; set; }

    public Vector2 LeftMinionBodyPosotion { get; set; }
    public Vector2 RightMinionBodyPosotion { get; set; }

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

}
