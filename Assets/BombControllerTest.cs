using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombControllerTest : MonoBehaviour
{
    [SerializeField] float bombSpeed = 1f;
    [SerializeField] float throwDistance = 1f;
    [SerializeField] float maximumHight = 1f;

    Collider2D myCollider;
    Animator myAnimator;
    Rigidbody2D myRigidbody2D;

    private Vector2 startPosition;
    private Vector2 endPosition;
    private float percentageCompleted = 0f;
    private float distanceTraveled = 0f;
    private float timeSinceThrowStarted = 0f;
    private bool startMoving = false;


    private bool oldMethod = true;
    private bool newMethod = false;

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
        //// Get a reference to where te bomb was thrown from
        //startPosition = transform.position;
        //// Arc animation variables
        //int randomIndex = Random.Range(0, 3);
        //randomlyPickedArcDuration = arcDuration[randomIndex];


    }

    public void SetUpThrowVariables()
    {
        startPosition = transform.position;
        endPosition = startPosition + Vector2.down * throwDistance;
        // endPosition = startPosition + Vector2.left * throwDistance;
        timeSinceThrowStarted = Time.time;
        startMoving = true;
    }

    private void FixedUpdate()
    {
        ControllBombDisplacementProgress();
    }

    private void ControllBombDisplacementProgress()
    {
        if (startMoving)
        {
            distanceTraveled = bombSpeed * (Time.time - timeSinceThrowStarted);
            percentageCompleted = distanceTraveled / throwDistance;
            DisplaceBomb();
            PlayArcAnimation();
            if (percentageCompleted > 1)
            {
                startMoving = false;
            }
        }
    }

    private void DisplaceBomb()
    {
        if (startMoving)
        {
            Vector2 currentPosition = Vector2.Lerp(startPosition, endPosition, percentageCompleted);
            myRigidbody2D.MovePosition(currentPosition);
        }
    }

    private void PlayArcAnimation()
    {
        if (oldMethod && !newMethod)
        {
            var hightIncrease = Mathf.Sin(percentageCompleted * Mathf.PI) * maximumHight;
            Vector2 bombSpritePosition = new Vector2(0, hightIncrease);
            transform.GetChild(0).GetComponent<Transform>().transform.localPosition = bombSpritePosition;
        }

        else if (!oldMethod && newMethod)
        {
            float arcStartPercent = (0.5f / maximumHight) * 0.5f;
            float arcPercent = arcStartPercent + Mathf.Lerp(1 - arcStartPercent, 0 - arcStartPercent, percentageCompleted);
            var hightIncrease = Mathf.Sin(arcPercent * Mathf.PI) * maximumHight;
            Vector2 bombSpritePosition = new Vector2(0, hightIncrease);
            transform.GetChild(0).GetComponent<Transform>().transform.localPosition = bombSpritePosition;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            oldMethod = true;
            newMethod = false;
            Debug.Log("Old Method: " + oldMethod);
            Debug.Log("New Method: " + newMethod);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            oldMethod = false;
            newMethod = true;
            Debug.Log("Old Method: " + oldMethod);
            Debug.Log("New Method: " + newMethod);
        }
    }
}
