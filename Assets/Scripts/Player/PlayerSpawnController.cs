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
    Rigidbody2D myRigidbody;

    private static Vector2 spawnPosition;
    private static bool firstTimePlayed = true;

    public bool FirstTimePlayed { get { return firstTimePlayed; } }

    private void Awake()
    {
        GetAccessToComponents();
        if (!firstTimePlayed)
            transform.position = spawnPosition;
            //myRigidbody.MovePosition(spawnPosition);
        firstTimePlayed = false;
    }

    private void GetAccessToComponents()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "CaveSceneTrigger")
        {
            spawnPosition = spawnPoints[0].coordinates;
        }
        else if (collision.gameObject.name == "TownSceneTrigger")
        {
            spawnPosition = spawnPoints[1].coordinates;
        }
    }
}
