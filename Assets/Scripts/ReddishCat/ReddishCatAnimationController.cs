using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReddishCatAnimationController : MonoBehaviour
{
    [Header("Bomb Attack")]
    [SerializeField] GameObject bombPrefab;
    [Header("Arrow Attack")]
    [SerializeField] GameObject arrowPrefab;


    // Cached References
    ReddishCatMovementController reddishCatMovementController;
    ReddishCatCombatController reddishCatCombatController;
    Animator myAnimator;
    GameObject newBomb;

    private void Awake()
    {
        GetAccessToComponents();
    }

    private void GetAccessToComponents()
    {
        reddishCatMovementController = GetComponentInParent<ReddishCatMovementController>();
        reddishCatCombatController = GetComponentInParent<ReddishCatCombatController>();
        myAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AnimateMovement(Vector2 movementDirection)
    {
        if (!Mathf.Approximately(movementDirection.x, 0.0f))
        {
            myAnimator.SetFloat("Look X", movementDirection.x);
            myAnimator.SetFloat("Speed", movementDirection.magnitude);
        }
    }

    public void PrepareBow()
    {
        myAnimator.SetTrigger("prepareBow");
        reddishCatMovementController.CanMove = false;
        reddishCatCombatController.IsAttacking = true;
    }

    public void PrepareBomb()
    {
        myAnimator.SetBool("pickBomb", true);
        reddishCatMovementController.CanMove = false;
        reddishCatCombatController.IsAttacking = true;

        Vector2 pickUpPosition = new Vector2(transform.position.x, transform.position.y - 0.35f); // pivot.y = 15.35f
        newBomb = Instantiate(bombPrefab, pickUpPosition, Quaternion.identity);

    }

    private void LiftBomb()
    {
        myAnimator.SetBool("pickBomb", false);
        myAnimator.SetBool("liftBomb", true);
        // TODO: fix this line after dealing with bomb arc
        // BombControllerTest newBomb = FindObjectOfType<BombControllerTest>();
        // newBomb.GetComponent<Transform>().position = new Vector2(transform.position.x, transform.position.y + 0.5f);
        newBomb.GetComponent<Rigidbody2D>().MovePosition(new Vector2(transform.position.x, transform.position.y + 0.5f));
    }

    private void ThrowBomb()
    {
        myAnimator.SetBool("liftBomb", false);
        // TODO: fix this line after dealing with bomb arc
        // BombController newBomb = FindObjectOfType<BombController>();
        newBomb.GetComponent<BombController>().SetUpThrowAgainstPlayer();
        reddishCatMovementController.CanMove = true;
        reddishCatCombatController.IsAttacking = false;
    }

    private void ShootArrow()
    {
        reddishCatMovementController.CanMove = true;
        reddishCatCombatController.IsAttacking = false;
        Vector2 startPosition = new Vector2(transform.position.x, transform.position.y + 0.3f);
        Instantiate(arrowPrefab, startPosition, Quaternion.identity);
    }
}
