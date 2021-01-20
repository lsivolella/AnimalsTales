using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReddishMinionCombatController : MonoBehaviour
{
    // Serialized Parameters
    [Header("Attack")]
    [SerializeField] float attackCooldown = 1f;

    // Cached References
    ReddishMinionAnimationController reddishMinionAnimationController;
    Rigidbody2D myRigidbody;

    // Cached Attack Variables
    private bool canAttack;
    private bool isAttacking;
    private float attackCooldownTimer;

    // Properties
    public bool IsAttacking { get { return isAttacking; } set { isAttacking = value; } }

    // Start is called before the first frame update
    void Start()
    {
        GetAccessToComponents();
        SetUpVariables();
    }

    private void GetAccessToComponents()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        reddishMinionAnimationController = GetComponentInChildren<ReddishMinionAnimationController>();
    }

    private void SetUpVariables()
    {
        canAttack = false;
        isAttacking = false;
        attackCooldownTimer = attackCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        HandleAttackCooldown();
    }

    private void HandleAttackCooldown()
    {
        if (!isAttacking)
        {
            if (attackCooldownTimer <= 0)
            {
                canAttack = true;
                ShootYarnBall();
            }
            else
            {
                canAttack = false;
                attackCooldownTimer -= Time.deltaTime;
            }
        }
    }

    private void ShootYarnBall()
    {
        if (canAttack && !isAttacking)
        {
            reddishMinionAnimationController.PrepareYarnBall();
            attackCooldownTimer = attackCooldown;
        }
    }
}
