using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTestScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Vector2 playerSpritePosition = transform.GetChild(0).GetComponent<Transform>().transform.localPosition;
        Debug.Log(playerSpritePosition);
        playerSpritePosition += new Vector2 (playerSpritePosition.x, playerSpritePosition.y + 2);
        Debug.Log(playerSpritePosition);
        transform.GetChild(0).GetComponent<Transform>().transform.localPosition = playerSpritePosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
