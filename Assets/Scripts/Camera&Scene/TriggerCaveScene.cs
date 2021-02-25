using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerCaveScene : MonoBehaviour
{
    [SerializeField] SceneController sceneController;

    private Vector2 playerSpawnPosition;

    private void OnDrawGizmos()
    {
        playerSpawnPosition = new Vector2(transform.position.x, transform.position.y + 0.5f);
        Gizmos.DrawWireSphere(playerSpawnPosition, 0.1f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("TestObject"))
            sceneController.LoadTownScene();
    }
}
