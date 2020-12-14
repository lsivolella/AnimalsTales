using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteLayerController : MonoBehaviour
{
    private SpriteRenderer spriteTransform;

    // Start is called before the first frame update
    void Start()
    {
        if (CompareTag("Enemy"))
            spriteTransform = transform.GetChild(0).GetComponent<SpriteRenderer>();
        else
            spriteTransform = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player)
        {
            if (player.transform.position.y > this.gameObject.transform.position.y)
            {
                spriteTransform.sortingOrder = 1;
            }
            else if (player.transform.position.y < this.gameObject.transform.position.y)
            {
                spriteTransform.sortingOrder = 0;
            }
        }
    }
}
