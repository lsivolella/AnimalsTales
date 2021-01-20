using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YarnBallController : MonoBehaviour
{
    [Header("Motion")]
    [SerializeField] Vector2 movementDirection = new Vector2(1, 0);     // Positive is Right/Up, Negative is Left/Down
    [SerializeField] float ballSpeed = 1f;
    [SerializeField] float rotationSpeed = 1f;
    [Header("Arc Animation")]
    [SerializeField] float yarnBallThrowingSpeed = 1f;
    [SerializeField] float minimumBombSpeed = 1f;
    [SerializeField] float maximumArcHight = 1f;

    // Cached enum
    public enum YarnBallState { NotThrown, FlyingToPlayer, GroundedAtPlayerArea, FlyingToEnemy, GroundedAtEnemyArea };
    YarnBallState yarnBallState;

    // Cached References
    Rigidbody2D myRigidbody;
    Collider2D myCollider;

    // Cached Variables
    private float rotationDirection = -1;

    // Cached Variables
    private Vector2 positionWhenThrownByEnemy;
    private Vector2 positionWhenLanded;
    private float timeWhenWasThrown = 0f;
    private float distanceToBeTraveled = 0f;
    private float distanceTraveled = 0f;
    private float percentualDistanceTravaled;
    private int numberOfBounces;

    // Properties
    public YarnBallState YarnBallStateGet { get { return yarnBallState; } }
    public float BallSpeed { get { return ballSpeed; } }
    public float RotationSpeed { get { return rotationSpeed; } }
    public Vector2 MovementDirection { get { return movementDirection; } }
    public float RotationDirection { get { return rotationDirection; } }

    private void Awake()
    {
        GetAccessToComponents();
        SetUpDefaultVariables();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    private void GetAccessToComponents()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
    }

    private void SetUpDefaultVariables()
    {
        // Prevent Yarn Ball from colliding with Enemy before it is thrown
        myCollider.enabled = false;
        // Set the Bomb initial state
        yarnBallState = YarnBallState.NotThrown;
    }

    private void SetMovementRestrictions()
    {
        if (movementDirection.x > movementDirection.y)
            myRigidbody.constraints = RigidbodyConstraints2D.FreezePositionY;
        else
            myRigidbody.constraints = RigidbodyConstraints2D.FreezePositionX;
    }

    private void RemoveMovementRestrictions()
    {
        myRigidbody.constraints = RigidbodyConstraints2D.None;
    }

    public void SetpUpThrowAgainstPlayer()
    {
        positionWhenThrownByEnemy = transform.position;
        positionWhenLanded = new Vector2 (transform.position.x - 1.8f, transform.position.y - 0.4f);  // transform.position.x - 1.75f
        // Get a Time reference for the moment the action begun
        timeWhenWasThrown = Time.time;
        yarnBallState = YarnBallState.FlyingToPlayer;
    }

    private void FixedUpdate()
    {
        ProcessMovementAndRotation();
        ControllYarnBallDisplacementProgress();
        ThrowAgainstPlayer();
        SetUpTravelBackToCaster();
        TravelBackToCaster();
    }

    private void ProcessMovementAndRotation()
    {
        if (yarnBallState == YarnBallState.GroundedAtPlayerArea)
        {
            myRigidbody.MovePosition(myRigidbody.position + movementDirection * ballSpeed * Time.fixedDeltaTime);
            myRigidbody.MoveRotation(myRigidbody.rotation + rotationDirection * rotationSpeed * Time.fixedDeltaTime); 
        }
    }

    private void ChangeDirectionOfMovement(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Objects") || yarnBallState == YarnBallState.GroundedAtPlayerArea)
        {
            BounceYarnBall();
            numberOfBounces++;
        }
    }

    private void BounceYarnBall()
    {
        movementDirection *= -1;
        rotationDirection *= -1;
    }

    private void ControllYarnBallDisplacementProgress()
    {
        if (yarnBallState == YarnBallState.FlyingToPlayer || yarnBallState == YarnBallState.FlyingToEnemy)
        {
            distanceTraveled = yarnBallThrowingSpeed * (Time.time - timeWhenWasThrown);
            distanceToBeTraveled = (positionWhenThrownByEnemy - positionWhenLanded).magnitude;
            percentualDistanceTravaled = distanceTraveled / distanceToBeTraveled; // 1.75f
            PlayArcAnimation();

            if (percentualDistanceTravaled >= 1f)
            {
                SetMovementRestrictions();
                PrepareYarnBallToRoll();
                ResetArcProgress();

                if (yarnBallState == YarnBallState.FlyingToPlayer) yarnBallState = YarnBallState.GroundedAtPlayerArea;
                else if (yarnBallState == YarnBallState.FlyingToEnemy)
                {
                    yarnBallState = YarnBallState.NotThrown;
                }
            }
        }
    }

    private void PrepareYarnBallToRoll()
    {
        if (yarnBallState == YarnBallState.FlyingToPlayer && numberOfBounces < 2)
        {
            numberOfBounces = 0;
            myCollider.enabled = true;
            yarnBallState = YarnBallState.GroundedAtPlayerArea;
            float getArcDirection = (positionWhenLanded - positionWhenThrownByEnemy).x;
            Debug.Log(getArcDirection);
            if (getArcDirection < 0)
                BounceYarnBall();
        }
    }

    private void ResetArcProgress()
    {
        distanceToBeTraveled = 0;
        distanceTraveled = 0;
        timeWhenWasThrown = 0;
        percentualDistanceTravaled = 0;
    }
    private void ThrowAgainstPlayer()
    {
        // This is initiated when the cat throw the yarn ball towards the player
        if (yarnBallState == YarnBallState.FlyingToPlayer)
        {
            Vector2 currentPosition = Vector2.Lerp(positionWhenThrownByEnemy, positionWhenLanded, percentualDistanceTravaled);
            myRigidbody.MovePosition(currentPosition);
        }
    }

    private void PlayArcAnimation()
    {
        if (yarnBallState == YarnBallState.FlyingToPlayer)
        {
            // float arcStartPercent = (0.5f / maximumArcHight) * 0.5f;
            float arcStartPercent = 0f;
            float arcPercent = arcStartPercent + Mathf.Lerp(1 - arcStartPercent, 0 - arcStartPercent, percentualDistanceTravaled);
            var hightIncrease = Mathf.Sin(arcPercent * Mathf.PI) * maximumArcHight;
            Vector2 ballSpritePosition = new Vector2(0, hightIncrease);
            transform.GetChild(0).GetComponent<Transform>().transform.localPosition = ballSpritePosition;
        }
        else if (yarnBallState == YarnBallState.FlyingToEnemy)
        {
            var hightIncrease = Mathf.Sin(percentualDistanceTravaled * Mathf.PI) * maximumArcHight;
            Vector2 ballSpritePosition = new Vector2(0, hightIncrease);
            transform.GetChild(0).GetComponent<Transform>().transform.localPosition = ballSpritePosition;
        }
    }

    private void SetUpTravelBackToCaster()
    {
        if (numberOfBounces == 2)
        {
            RemoveMovementRestrictions();
            // positionWhenLanded = transform.position;
            myCollider.isTrigger = true;
            // Get a Time reference for the moment the action begun
            timeWhenWasThrown = Time.time;
            yarnBallState = YarnBallState.FlyingToEnemy;
            numberOfBounces++;
        }
    }

    private void TravelBackToCaster()
    {
        // This is initiated when the yarn ball bounces back to the minion cat
        if (yarnBallState == YarnBallState.FlyingToEnemy)
        {
            Vector2 currentPosition = Vector2.Lerp(positionWhenLanded, positionWhenThrownByEnemy, percentualDistanceTravaled);
            myRigidbody.MovePosition(currentPosition);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ChangeDirectionOfMovement(collision);

        if (collision.gameObject.CompareTag("Player"))
            GetComponent<CircleCollider2D>().enabled = false;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            GetComponent<CircleCollider2D>().enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("LeftSideCollider"))
        {
            SetUpTravelBackToCaster()
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            GameObject reddishMinion = collision.gameObject;
            reddishMinion.GetComponent<ReddishMinionMovementController>().CanMove = true;
            reddishMinion.GetComponent<ReddishMinionMovementController>().IsMoving = true;
            reddishMinion.GetComponent<ReddishMinionCombatController>().IsAttacking = false;
            Destroy(gameObject);
        }
    }
}
