using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YarnBallController : MonoBehaviour
{
    [Header("Motion")]
    [SerializeField] float ballSpeed = 1f;
    [SerializeField] float rotationSpeed = 1f;
    [Header("Arc Animation")]
    [SerializeField] float yarnBallThrowingSpeed = 1f;
    [SerializeField] float maximumArcHight = 1f;
    [Header("SelfDestruction")]
    [SerializeField] float yarnBallFadeDuration = 0.5f;

    // Cached enum
    public enum YarnBallState { NotThrown, FlyingToPlayer, GroundedAtPlayerArea, FlyingToEnemy, GroundedAtEnemyArea };
    YarnBallState yarnBallState;

    // Cached References
    Rigidbody2D myRigidbody;
    Collider2D myCollider;
    SpriteRenderer[] spriteRenderers;

    // Cached Variables
    private Vector2 movementVector;
    private Vector2 positionWhenThrownByEnemy;
    private Vector2 positionWhenLanded;
    private float timeWhenWasThrown = 0f;
    private float distanceToBeTraveled = 0f;
    private float distanceTraveled = 0f;
    private float percentualDistanceTravaled;
    private float yarnBallDirectionOfMovement;
    private float yarnBallRotationDirection;
    private float yarnBallFadeTimer;
    private bool isFacingRight;

    private void Awake()
    {
        GetAccessToComponents();
    }

    private void GetAccessToComponents()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        SetUpDefaultVariables();
    }

    private void SetUpDefaultVariables()
    {
        // Prevent Yarn Ball from colliding with Enemy before it is thrown
        myCollider.enabled = false;
        // Set the Bomb initial state
        yarnBallState = YarnBallState.NotThrown;
    }

    private void Update()
    {
        ControlFading();
    }

    public void SetFadeAway()
    {
        yarnBallFadeTimer = yarnBallFadeDuration;
        Invoke("SelfDestruct", yarnBallFadeDuration);
    }

    private void SelfDestruct()
    {
        Destroy(gameObject);
    }

    private void ControlFading()
    {
        if (yarnBallFadeTimer > 0)
        {
            yarnBallFadeTimer -= Time.deltaTime;

            float alphaFactor = yarnBallFadeTimer / yarnBallFadeDuration;
            Debug.Log("Fade Timer: " + yarnBallFadeTimer);
            Debug.Log("Fade Duration: " + yarnBallFadeDuration);
            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f * alphaFactor);
            }
        }
    }


    private void FixedUpdate()
    {
        ProcessMovementAndRotation();
        ControllYarnBallDisplacementProgress();
        ThrowAgainstPlayer();
        TravelBackToCaster();
    }

    private void ProcessMovementAndRotation()
    {
        if (yarnBallState == YarnBallState.GroundedAtPlayerArea)
        {
            myRigidbody.MovePosition(myRigidbody.position + movementVector * ballSpeed * Time.fixedDeltaTime);
            myRigidbody.MoveRotation(myRigidbody.rotation + yarnBallRotationDirection * rotationSpeed * Time.fixedDeltaTime);
        }
    }

    private void BounceYarnBall()
    {
        yarnBallDirectionOfMovement *= -1;
        movementVector *= -1;
        yarnBallRotationDirection *= -1;
    }

    public void SetpUpThrowAgainstPlayer()
    {
        positionWhenThrownByEnemy = transform.position;
        if (isFacingRight)
            positionWhenLanded = new Vector2(transform.position.x + 1.8f, transform.position.y - 0.4f);
        else
            positionWhenLanded = new Vector2(transform.position.x - 1.8f, transform.position.y - 0.4f);
        DefineYarnBallOriginalDirection(isFacingRight);
        // Get a Time reference for the moment the action begun
        timeWhenWasThrown = Time.time;
        yarnBallState = YarnBallState.FlyingToPlayer;
    }

    public void DefineYarnBallOriginalDirection(bool isFacingRight)
    {
        this.isFacingRight = isFacingRight;

        if (isFacingRight)
        {
            yarnBallDirectionOfMovement = 1;
            yarnBallRotationDirection = -1;
            movementVector = new Vector2(1, 0);
        }
        else
        {
            yarnBallDirectionOfMovement = -1;
            yarnBallRotationDirection = 1;
            movementVector = new Vector2(-1, 0);
        }
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

    private void SetMovementRestrictions()
    {
        myRigidbody.constraints = RigidbodyConstraints2D.FreezePositionY;
    }

    private void RemoveMovementRestrictions()
    {
        myRigidbody.constraints = RigidbodyConstraints2D.None;
    }

    private void PrepareYarnBallToRoll()
    {
        if (yarnBallState == YarnBallState.FlyingToPlayer)
        {
            myCollider.enabled = true;
            yarnBallState = YarnBallState.GroundedAtPlayerArea;
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
        RemoveMovementRestrictions();
        // positionWhenLanded = transform.position;
        myCollider.isTrigger = true;
        // Get a Time reference for the moment the action begun
        timeWhenWasThrown = Time.time;
        yarnBallState = YarnBallState.FlyingToEnemy;
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
        if (collision.gameObject.CompareTag("Player"))
        {
            float collisisonDirection = transform.position.x - collision.gameObject.transform.position.x;
            if (Mathf.Sign(collisisonDirection) != Mathf.Sign(yarnBallDirectionOfMovement) && yarnBallState == YarnBallState.GroundedAtPlayerArea)
                BounceYarnBall();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("LeftSideCollider"))
        {
            if (isFacingRight && yarnBallState == YarnBallState.GroundedAtPlayerArea)
            {
                SetUpTravelBackToCaster();
            }
            else
                BounceYarnBall();
        }

        if (collision.gameObject.CompareTag("RightSideCollider"))
        {
            if (!isFacingRight && yarnBallState == YarnBallState.GroundedAtPlayerArea)
            {
                SetUpTravelBackToCaster();
            }
            else
                BounceYarnBall();
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
