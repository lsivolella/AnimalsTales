using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReddishMinionAnimationController : MonoBehaviour
{
    //Serialized Parameters
    [Header("Bomb Attack")]
    [SerializeField] GameObject yarnBallPrefab;
    [SerializeField] bool isFacingRight;

    // Cached References
    ReddishMinionMovementController reddishMinionMovementController;
    ReddishMinionCombatController reddishMinionCombatController;
    Animator myAnimator;
    GameObject yarnBall;

    public bool IsFacingRight { get { return isFacingRight; }}

    //public bool IsFacingRight { get; private set; }

    private void Awake()
    {
        GetAccessToComponents();
    }

    private void GetAccessToComponents()
    {
        reddishMinionMovementController = GetComponentInParent<ReddishMinionMovementController>();
        reddishMinionCombatController = GetComponentInParent<ReddishMinionCombatController>();
        myAnimator = GetComponent<Animator>();
    }

    public void AnimateMovement(Vector2 movementDirection)
    {
        if (!Mathf.Approximately(0.0f, movementDirection.y))
        {
            myAnimator.SetFloat("Look Y", movementDirection.y);
            myAnimator.SetFloat("Speed", movementDirection.magnitude);
        }
    }

    private void DefineSpriteDirection()
    {
        if (isFacingRight)
            myAnimator.SetFloat("Look X", 1);
        else
            myAnimator.SetFloat("Look X", -1);
    }

    public void PrepareYarnBall()
    {
        DefineSpriteDirection();
        myAnimator.SetBool("pickYarnBall", true);
        reddishMinionMovementController.CanMove = false;
        reddishMinionMovementController.IsMoving = false;
        reddishMinionCombatController.IsAttacking = true;

        Vector2 pickUpPosition;
        if (isFacingRight)
            pickUpPosition = new Vector2(transform.position.x + 0.35f, transform.position.y + 0.15f);
        else
            pickUpPosition = new Vector2(transform.position.x - 0.35f, transform.position.y + 0.15f);

        yarnBall = Instantiate(yarnBallPrefab, pickUpPosition, Quaternion.identity);
        yarnBall.GetComponent<YarnBallController>().DefineYarnBallOriginalDirection(isFacingRight);
    }

    private void LiftYarnBall()
    {
        DefineSpriteDirection();
        myAnimator.SetBool("pickYarnBall", false);
        myAnimator.SetBool("liftYarnBall", true);
        yarnBall.GetComponent<YarnBallController>().LiftYarnBall();

        if (isFacingRight)
            yarnBall.GetComponent<Rigidbody2D>().MovePosition(new Vector2(transform.position.x - 0.05f, transform.position.y + 0.58f));
        else
            yarnBall.GetComponent<Rigidbody2D>().MovePosition(new Vector2(transform.position.x + 0.05f, transform.position.y + 0.58f));
    }

    private void ThrowYarnBall()
    {
        DefineSpriteDirection();
        myAnimator.SetFloat("Speed", 0.0f);
        myAnimator.SetBool("liftYarnBall", false);
        myAnimator.SetBool("throwYarnBall", true);
        yarnBall.GetComponent<YarnBallController>().SetpUpThrowAgainstPlayer();
        myAnimator.SetBool("throwYarnBall", false);
    }

    public void PlayDeathRoutine()
    {
        myAnimator.SetTrigger("isDead");

        if (yarnBall != null)
            yarnBall.GetComponent<YarnBallController>().SetFadeAway();
    }
}
