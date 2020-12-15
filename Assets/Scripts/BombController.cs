using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombController : MonoBehaviour
{
    [Header("Arc Animation")]
    [SerializeField] float minimumThrowDistance = 1f;
    [SerializeField] float maximumThrowDistance = 1f;
    [SerializeField] float minimumBombSpeed = 1f;
    [SerializeField] float maximumArcHight = 1f;
    [Header("Bomb Detonation")]
    [SerializeField] public Transform colliderLeftPivot;
    [SerializeField] public Transform colliderUpPivot;
    [SerializeField] public Transform colliderRightPivot;
    [SerializeField] public Transform colliderDownPivot;
    [SerializeField] public float colliderRange = 1f;

    // Cached References
    Collider2D myCollider;
    Animator myAnimator;
    Rigidbody2D myRigidbody2D;

    private Vector2 startPosition;
    private Vector2 endPosition;
    private bool startMoving = false;
    private bool travelBack = false;
    private float randomlyPickedThrowDistance;
    private float timeWhenWasThrown = 0f;
    private float distanceTraveled = 0f;
    private float percentualDistanceTravaled;
    private float bombSpeed;

    public bool StartMoving { get { return startMoving; } }
    public Vector2 EndPosition { get { return endPosition; } }


    private void Awake()
    {
        GetAccessToComponents();
        SetDefaultVariables();
    }

    private void GetAccessToComponents()
    {
        myCollider = GetComponent<Collider2D>();
        myAnimator = GetComponentInChildren<Animator>();
        myRigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void SetDefaultVariables()
    {
        // Prevents bomb form colliding with enemy before it is thorwn
        myCollider.enabled = false;
        // Arc animation variables
        //int randomIndex = Random.Range(0, 3);
        //randomlyPickedThrowDistance = throwDistance[randomIndex];
        randomlyPickedThrowDistance = Random.Range(minimumThrowDistance, maximumThrowDistance);
        bombSpeed = minimumBombSpeed * randomlyPickedThrowDistance / minimumThrowDistance;
    }

    public void SetUpThrowAgainstPlayer()
    {
        startPosition = transform.position;
        endPosition = startPosition + Vector2.down * randomlyPickedThrowDistance;
        timeWhenWasThrown = Time.time;
        startMoving = true;
    }

    public void SetUpThrowAgainstCaster()
    {
        // Adjusts the start position so that the bomb falls in the same y coordinate as the enemy, blocking his movements
        startPosition = startPosition + Vector2.down * 0.5f;
        timeWhenWasThrown = Time.time;
        travelBack = true;
        myAnimator.SetBool("holdDetonation", true);
    }

    private void FixedUpdate()
    {
        ControllBombDisplacementProgress();
        ThrowAgainstPlayer();
        ThrowAgainstCaster();
    }

    private void ControllBombDisplacementProgress()
    {
        if (startMoving || travelBack)
        {
            distanceTraveled = bombSpeed * (Time.time - timeWhenWasThrown);
            percentualDistanceTravaled = distanceTraveled / randomlyPickedThrowDistance;
            PlayArcAnimation();

            if (percentualDistanceTravaled >= 1f)
            {
                myCollider.enabled = true;
                startMoving = false;
                travelBack = false;
                myAnimator.SetBool("holdDetonation", false);
                myAnimator.SetTrigger("detonation");
                ResetArcProgress();
            }
        }
    }

    private void ResetArcProgress()
    {
        distanceTraveled = 0;
        timeWhenWasThrown = 0;
        percentualDistanceTravaled = 0;
    }

    private void ThrowAgainstPlayer()
    {
        // This is initiated when the cat throw the bomb towards the player
        if (startMoving)
        {
            Vector2 currentPosition = Vector2.Lerp(startPosition, endPosition, percentualDistanceTravaled);
            myRigidbody2D.MovePosition(currentPosition);
        }
    }

    public void ThrowAgainstCaster()
    {
        // This is initiated when the player successfully hit the bomb from the bottom
        if (travelBack)
        {
            Vector2 startPositionAdjusted = new Vector2(startPosition.x, startPosition.y - 0.5f);
            Vector2 positionVector = Vector2.Lerp(endPosition, startPosition, percentualDistanceTravaled);
            myRigidbody2D.MovePosition(positionVector);
        }
    }

    private void PlayArcAnimation()
    {
        if (startMoving)
        {
            float arcStartPercent = (0.5f / maximumArcHight) * 0.5f;
            float arcPercent = arcStartPercent + Mathf.Lerp(1 - arcStartPercent, 0 - arcStartPercent, percentualDistanceTravaled);
            var hightIncrease = Mathf.Sin(arcPercent * Mathf.PI) * maximumArcHight;
            Vector2 bombSpritePosition = new Vector2(0, hightIncrease);
            transform.GetChild(0).GetComponent<Transform>().transform.localPosition = bombSpritePosition; 
        }
        else if (travelBack)
        {
            var hightIncrease = Mathf.Sin(percentualDistanceTravaled * Mathf.PI) * maximumArcHight;
            Vector2 bombSpritePosition = new Vector2(0, hightIncrease);
            transform.GetChild(0).GetComponent<Transform>().transform.localPosition = bombSpritePosition;
        }
    }

    public void SelfDestruct()
    {
        Destroy(gameObject);
    }
}