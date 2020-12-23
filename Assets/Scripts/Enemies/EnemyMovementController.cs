using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementController : MonoBehaviour
{
    // Serializes Parameters
    [Header("Movement")]
    [SerializeField] float movementSpeed = 1f;

    // Cached References
    Rigidbody2D myRigidbody;
    Animator myAnimator;

    // Cached Movement Variables
    private Vector2 movementDirection; // The direction the enemy has to move towards (-1 is left);
    //private bool canMove = true;
    private bool canMove = true;
    private bool isMoving = true;
    private float durationOfMovement = 2f;
    private float movementDurationMeter; // The distance the enemy has walked

    // Properties
    public bool CanMove { get { return canMove; } set { canMove = value; } }
    public bool IsMoving { get { return isMoving; } set { isMoving = value;} }


    private void Awake()
    {
        GetAccessToComponents();
        GenerateRandomMovementVector();
    }

    private void GetAccessToComponents()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            if (!isMoving)
            {
                GenerateRandomMovementVector();
            }

            if (isMoving)
            {
                AnimateMovement();
                movementDurationMeter += Time.deltaTime;
                if (movementDurationMeter >= durationOfMovement)
                {
                    isMoving = false;
                }
            } 
        }
    }

    private void GenerateRandomMovementVector()
    {
        // Picks a random direction for the Enemy to move towards
        movementDirection = new Vector2(Random.Range(-1f, 1f), 0f).normalized;
        // Resets any residual velocity
        myRigidbody.velocity = Vector2.zero; 
        isMoving = true;
        movementDurationMeter = 0;
    }

    private void MoveToOpositeDirection(Vector2 direction)
    {
        movementDirection = direction;
        isMoving = true;
        movementDurationMeter = 0;
    }

    private void AnimateMovement()
    {
        if (!Mathf.Approximately(movementDirection.x, 0.0f))
        {
            myAnimator.SetFloat("Look X", movementDirection.x);
            myAnimator.SetFloat("Speed", movementDirection.magnitude);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Objects") || collision.gameObject.CompareTag("Bomb"))
        {
            MoveToOpositeDirection(collision.GetContact(0).normal);
        }
    }

    private void FixedUpdate()
    {
        MoveEnemy();
    }

    private void MoveEnemy()
    {
        if (canMove)
        {
            if (isMoving)
            {
                Vector2 position = myRigidbody.position;
                position += movementDirection * movementSpeed * Time.fixedDeltaTime;
                myRigidbody.MovePosition(position);
            } 
        }
    }
}
