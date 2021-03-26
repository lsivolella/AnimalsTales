using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnController : MonoBehaviour
{
    [System.Serializable]
    public class SpawnPoint
    {
        public string name;
        public Vector2 coordinates;
    }

    // Serialized Parameters
    [Header("Spawn Points")]
    [SerializeField] List<SpawnPoint> spawnPoints;

    // Cached References
    GameMaster gameMaster;

    private void Awake()
    {
        GetAccessToComponents();
        SetDefaultVariables();
    }

    private void GetAccessToComponents()
    {
        gameMaster = GameMaster.instance;
    }

    private void SetDefaultVariables()
    {
        if (!gameMaster.PlayerSpawnSetUp)
        {
            gameMaster.PlayerDefaultPosition = transform.position;
            gameMaster.PlayerSpawnSetUp = true;
        }
        else if (gameMaster.PlayerSpawnSetUp)
            transform.position = gameMaster.PlayerSpawnPosition;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "CaveSceneTrigger")
        {
            gameMaster.PlayerSpawnPosition = spawnPoints[0].coordinates;
        }
        else if (collision.gameObject.name == "TownSceneTrigger")
        {
            gameMaster.PlayerSpawnPosition = spawnPoints[1].coordinates;
        }
    }
}
