using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowSpriteController : MonoBehaviour
{
    // Cached References
    Rigidbody2D myRigidbody;
    YarnBallController yarnBallController;

    // enum
    public enum ShadowState { NotThrown, FlyingToPlayer, GroundedAtPlayerArea, FlyingToEnemy, GroundedAtEnemyArea };
    ShadowState shadowState;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        yarnBallController = GetComponentInParent<YarnBallController>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = Vector2.zero;
       // TODO: use the Rigidbody instead of the transform
    }
}
