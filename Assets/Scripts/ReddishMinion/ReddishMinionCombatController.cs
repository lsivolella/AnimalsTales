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

    // Cached Attack Variables
    private bool canAttack;
    private bool isAttacking;
    private float attackCooldownTimer;

    // Cached General Variables
    private bool onHold;

    // Properties
    public bool IsAttacking { get { return isAttacking; } set { isAttacking = value; } }
    public bool OnHold { get { return onHold; } set { onHold = value; } }

    // Start is called before the first frame update
    void Start()
    {
        GetAccessToComponents();
        SetUpVariables();
    }

    private void GetAccessToComponents()
    {
        reddishMinionAnimationController = GetComponentInChildren<ReddishMinionAnimationController>();
    }

    private void SetUpVariables()
    {
        onHold = true;
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
        if (!isAttacking && !SceneController.instance.CinematicsOn && !onHold && GameMaster.instance.IsBossAlive)
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
