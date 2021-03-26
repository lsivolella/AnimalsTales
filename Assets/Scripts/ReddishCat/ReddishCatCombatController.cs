using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReddishCatCombatController : MonoBehaviour
{
    // Serialized Parameters
    [Header("Attack")]
    [SerializeField] float attackCooldown = 1f;
    [SerializeField] [Range(0, 1)] float bombAttackProbability = 1f;
    [Header("Passive Damage")]
    [SerializeField] int passiveDamage = -1;    // Damage dealt is always negative (positive for Healing)

    // Cached References
    ReddishCatHealthController reddishCatHealthController;
    ReddishCatAnimationController reddishMinionAnimationController;
    Rigidbody2D myRigidbody;
    GameMaster gameMaster;

    // Cached Attack Variables
    private bool canAttack;
    private bool isAttacking;
    private float attackCooldownTimer;

    // Cached General Variables
    private bool onHold;


    // Properties
    public int PassiveDamage { get { return passiveDamage; } }
    public bool CanAttack { get { return canAttack; } set { canAttack = value; } }
    public bool IsAttacking { get { return isAttacking; } set { isAttacking = value; } }
    public bool OnHold { get { return onHold; } set { onHold = value; } }


    private void Awake()
    {
        GetAccessToComponents();
        SetUpDefaultVariables();
    }

    private void GetAccessToComponents()
    {
        reddishCatHealthController = GetComponent<ReddishCatHealthController>();
        reddishMinionAnimationController = GetComponentInChildren<ReddishCatAnimationController>();
        myRigidbody = GetComponent<Rigidbody2D>();
        gameMaster = GameMaster.instance;
    }

    private void SetUpDefaultVariables()
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
        if (!isAttacking && !onHold && !SceneController.instance.CinematicsOn && gameMaster.IsBossAlive)
        {
            if (attackCooldownTimer <= 0)
            {
                canAttack = true;
                SelectRandomAttack();
            }
            else
            {
                canAttack = false;
                attackCooldownTimer -= Time.deltaTime;
            }
        }
    }

    private void SelectRandomAttack()
    {
        float randomNumber = Random.Range(0f, 1f);
        if (randomNumber <= (1 - bombAttackProbability))
            ShootArrow();
        else
            ShootBomb();
        ShootBomb();
    }

    private void ShootArrow()
    {
        if (canAttack && !isAttacking && !reddishCatHealthController.IsInvincible)
        {
            reddishMinionAnimationController.PrepareBow();
            attackCooldownTimer = attackCooldown;
        }
    }

    private void ShootBomb()
    {
        if (canAttack && !isAttacking && !reddishCatHealthController.IsInvincible)
        {
            reddishMinionAnimationController.PrepareBomb();
            attackCooldownTimer = attackCooldown;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            reddishCatHealthController.ChangeEnemyHealth(collision.gameObject.GetComponent<PlayerCombatController>().GetPassiveDamage);
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        }
    }
}
