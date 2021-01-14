using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpriteAnimation : MonoBehaviour
{
    [Header("Bomb Attack")]
    [SerializeField] GameObject bombPrefab;
    [Header("Arrow Attack")]
    [SerializeField] GameObject arrowPrefab;


    // Cached References
    EnemyMovementController enemyMovementController;
    EnemyCombatController enemyCombatController;
    Animator myAnimator;
    GameObject newBomb;

    private void Awake()
    {
        GetAccessToComponents();
    }

    private void GetAccessToComponents()
    {
        enemyMovementController = GetComponentInParent<EnemyMovementController>();
        enemyCombatController = GetComponentInParent<EnemyCombatController>();
        myAnimator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PrepareBow()
    {
        myAnimator.SetTrigger("prepareBow");
        enemyMovementController.CanMove = false;
        enemyCombatController.IsAttacking = true;
    }

    public void PrepareBomb()
    {
        myAnimator.SetBool("pickBomb", true);
        enemyMovementController.CanMove = false;
        enemyCombatController.IsAttacking = true;

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
        enemyMovementController.CanMove = true;
        enemyCombatController.IsAttacking = false;
    }

    private void ShootArrow()
    {
        enemyMovementController.CanMove = true;
        enemyCombatController.IsAttacking = false;
        Vector2 startPosition = new Vector2(transform.position.x, transform.position.y + 0.3f);
        Instantiate(arrowPrefab, startPosition, Quaternion.identity);
    }
}
