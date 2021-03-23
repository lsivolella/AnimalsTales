using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static YarnBallController;

public class YarnBallShadowSpriteController : MonoBehaviour
{
    // Cached References
    Rigidbody2D myRigidbody;
    YarnBallController yarnBallController;

    private float groundPosition;

    void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        yarnBallController = GetComponentInParent<YarnBallController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (yarnBallController.yarnBallState == YarnBallState.PickUp)
        {
            myRigidbody.position = yarnBallController.transform.position;
            groundPosition = myRigidbody.position.y;
        }
        else if (yarnBallController.yarnBallState == YarnBallState.Lifted || 
                yarnBallController.yarnBallState == YarnBallState.FlyingToPlayer || 
                yarnBallController.yarnBallState == YarnBallState.FlyingToEnemy)
        {
            Vector2 newPosition = new Vector2(yarnBallController.transform.position.x, groundPosition);
            Debug.Log(newPosition);
            myRigidbody.position = new Vector2(yarnBallController.transform.position.x, groundPosition);
        }
        else
            myRigidbody.position = yarnBallController.transform.position;
    }
}
