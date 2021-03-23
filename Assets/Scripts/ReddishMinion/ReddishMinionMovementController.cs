using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReddishMinionMovementController : MonoBehaviour
{
    // Serialized Parameters
    [Header("Movement")]
    [SerializeField] float movementSpeed = 1f;
    [SerializeField] float durationOfMovement = 0.2f;

    // Cached References
    Rigidbody2D myRigidbody;
    Animator myAnimator;
    ReddishMinionAnimationController reddishMinionAnimationController;
    ReddishMinionCombatController reddishMinionCombatController;

    // Cached Movement Variables
    private Vector2 movementDirection; // The direction the enemy has to move towards (-1 is left/down);
    private bool canMove;
    private bool isMoving;
    private float movementDurationMeter; // The time the enemy has walked a certain direction
    private Vector2 deadBodyPosition;

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
        myAnimator = GetComponentInChildren<Animator>();
        reddishMinionAnimationController = GetComponentInChildren<ReddishMinionAnimationController>();
        reddishMinionCombatController = GetComponentInChildren<ReddishMinionCombatController>();
    }

    private void Start()
    {
        SetDefaultVariables();
    }

    public void SetDefaultVariables()
    {
        if (!GameMaster.instance.HasMadeExchange)
        {
            canMove = true;
            isMoving = false;
        }
        else if (GameMaster.instance.HasMadeExchange && !GameMaster.instance.IsBossAlive)
        {
            canMove = false ;
            isMoving = false;
            myAnimator.SetTrigger("beginDead");

            if (reddishMinionAnimationController.IsFacingRight)
                transform.position = GameMaster.instance.LeftMinionBodyPosotion;
            else if (!reddishMinionAnimationController.IsFacingRight)
                transform.position = GameMaster.instance.RightMinionBodyPosotion;
        }
    }

    // Update is called once per frame
    void Update()
    {
        ControlMovement();
    }

    private void ControlMovement()
    {
        if (canMove && !SceneController.instance.CinematicsOn && !reddishMinionCombatController.OnHold && GameMaster.instance.IsBossAlive)
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
        if (canMove && !SceneController.instance.CinematicsOn && !reddishMinionCombatController.OnHold && GameMaster.instance.IsBossAlive)
        {
            if (isMoving)
            {
                Vector2 position = myRigidbody.position;
                position += movementDirection * movementSpeed * Time.fixedDeltaTime;
                myRigidbody.MovePosition(position);
            }
        }
    }

    public void StoreDeadBodyPosition()
    {
        if (reddishMinionAnimationController.IsFacingRight)
            GameMaster.instance.LeftMinionBodyPosotion = transform.position;
        else if (!reddishMinionAnimationController.IsFacingRight)
            GameMaster.instance.RightMinionBodyPosotion = transform.position;
    }
}
