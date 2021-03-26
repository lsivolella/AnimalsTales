using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReddishCatMovementController : MonoBehaviour
{
    // Serialized Parameters
    [Header("Movement")]
    [SerializeField] float movementSpeed = 1f;
    [SerializeField] float durationOfMovement = 0.2f;

    // Cached References
    Rigidbody2D myRigidbody;
    Collider2D myCollider;
    ReddishCatAnimationController reddishCatAnimationController;
    ReddishCatHealthController reddishCatHealthController;
    ReddishCatCombatController reddishCatCombatController;

    // Cached Movement Variables
    private Vector2 movementDirection; // The direction the enemy has to move towards (-1 is left/down);
    private bool canMove;
    private bool isMoving;
    private float movementDurationMeter; // The time the enemy has walked a certain direction

    // Properties
    public bool CanMove { get { return canMove; } set { canMove = value; } }
    public bool IsMoving { get { return isMoving; } set { isMoving = value; } }


    private void Awake()
    {
        GetAccessToComponents();
    }

    private void GetAccessToComponents()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
        reddishCatAnimationController = GetComponentInChildren<ReddishCatAnimationController>();
        reddishCatHealthController = GetComponent<ReddishCatHealthController>();
        reddishCatCombatController = GetComponent<ReddishCatCombatController>();
    }

    private void Start()
    {
        SetDefaultVariables();
    }

    private void SetDefaultVariables()
    {
        canMove = false;
        isMoving = false;
    }

    // Update is called once per frame
    void Update()
    {
        ControlMovement();
        ControlOnHoldState();
    }

    private void ControlMovement()
    {
        if (canMove && !SceneController.instance.CinematicsOn && !reddishCatCombatController.OnHold && reddishCatHealthController.IsAlive)
        {
            if (!isMoving)
            {
                GenerateRandomMovementVector();
            }

            if (isMoving)
            {
                reddishCatAnimationController.AnimateMovement(movementDirection);
                movementDurationMeter += Time.deltaTime;
                if (movementDurationMeter >= durationOfMovement)
                {
                    isMoving = false;
                }
            }
        }
    }

    private void ControlOnHoldState()
    {
        if (reddishCatCombatController.OnHold)
        {
            canMove = false;
            isMoving = false;
        }
    }

    public void GenerateRandomMovementVector()
    {
        // Picks a random direction for the Enemy to move towards
        movementDirection = new Vector2(Random.Range(-1f, 1f), 0f).normalized;
        // Resets any residual velocity
        myRigidbody.velocity = Vector2.zero;
        canMove = true;
        isMoving = true;
        movementDurationMeter = 0;
    }

    private void MoveToOpositeDirection(Vector2 direction)
    {
        movementDirection = new Vector2(direction.x, movementDirection.y);
        isMoving = true;
        movementDurationMeter = 0;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Objects") || collision.gameObject.CompareTag("Bomb"))
        {
            if (collision.gameObject.CompareTag("Bomb") && reddishCatHealthController.IsInvincible)
            {
                Physics2D.IgnoreCollision(myCollider, collision.gameObject.GetComponent<Collider2D>(), true);
            }
            MoveToOpositeDirection(collision.GetContact(0).normal);
        }
    }

    private void FixedUpdate()
    {
        MoveEnemy();
    }

    private void MoveEnemy()
    {
        if (canMove && !SceneController.instance.CinematicsOn && !reddishCatCombatController.OnHold && reddishCatHealthController.IsAlive)
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
