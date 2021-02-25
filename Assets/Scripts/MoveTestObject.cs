using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveTestObject : MonoBehaviour
{
    public static MoveTestObject instance;

    private static Vector2 spawnPosition;
    private static bool firstTimePlayedTest = true;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(instance.gameObject);

        if (!firstTimePlayedTest)
        {
            transform.position = spawnPosition;
        }

        firstTimePlayedTest = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 movementDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        transform.Translate(movementDirection * Time.deltaTime * 5);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "CaveSceneTrigger")
            spawnPosition = new Vector2(-1.5f, -5.25f);
        else if (collision.gameObject.name == "TownSceneTrigger")
            spawnPosition = new Vector2(5.5f, 10.8f);
    }
}
