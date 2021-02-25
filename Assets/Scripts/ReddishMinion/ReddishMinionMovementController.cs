using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReddishMinionMovementController : MonoBehaviour
{
    // Serialized Parameters
    [Header("Movement")]
    [SerializeField] float movementSpeed = 1f;

    // Cached References
    Rigidbody2D myRigidbody;
    Collider2D myCollider;
    Animator myAnimator;
    ReddishMinionAnimationController reddishMinionAnimationController;
    ReddishMinionCombatController reddishMinionCombatController;

    // Cached Movement Variables
    private Vector2 movementDirection; // The direction the enemy has to move towards (-1 is left/down);
    private bool canMove;
    private bool isMoving;
    private float durationOfMovement = 0.2f;
    private float movementDurationMeter; // The time the enemy has walked a certain direction

    // Properties
    public bool CanMove { get { return canMove; } set { canMove = value; } }
    public bool IsMoving { get { return isMoving; } set { isMoving = value; } }

    private void Awake()
    {
        GetAccessToComponents();
    }

    private void Start()
    {
        SetDefaultVariables();
        GenerateRandomMovementVector();
    }

    public void SetDefaultVariables()
    {
        canMove = true;
        isMoving = true;
    }

    private void GetAccessToComponents()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
        reddishMinionAnimationController = GetComponentInChildren<ReddishMinionAnimationController>();
        reddishMinionCombatController = GetComponentInChildren<ReddishMinionCombatController>();
    }

    // Update is called once per frame
    void Update()
    {
        ControlMovement();
    }

    private void ControlMovement()
    {
        if (canMove && !SceneController.instance.CinematicsOn && !reddishMinionCombatController.OnHold)
        {
            if (!isMoving)
            {
                GenerateRandomMovementVector();
            }

            if (isMoving)
            {
                reddishMinionAnimationController.AnimateMovement(movementDirection);
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
        movementDirection = new Vector2(0f, Random.Range(-1f, 1f)).normalized;
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Objects"))
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
        if (canMove && !SceneController.instance.CinematicsOn && !reddishMinionCombatController.OnHold)
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
