using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombatController : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] float bombCooldown = 1f;
    [Header("Passive Damage")]
    [SerializeField] int passiveDamage = -1;    // Damage dealt is always negative (positive for Healing)

    // Cached References
    EnemyHealthController enemyHealthController;
    EnemySpriteAnimation spriteAnimation;
    Rigidbody2D myRigidbody;

    // Cached Attack Variables
    private bool canAttack;
    private bool isAttacking;
    private float bombCooldownTimer;


    // Properties
    public int PassiveDamage { get { return passiveDamage; } }
    public bool IsAttacking { get { return isAttacking; } set { isAttacking = value; } }


    private void Awake()
    {
        GetAccessToComponents();
        SetUpVariables();
    }

    private void GetAccessToComponents()
    {
        enemyHealthController = GetComponent<EnemyHealthController>();
        myRigidbody = GetComponent<Rigidbody2D>();
        spriteAnimation = GetComponentInChildren<EnemySpriteAnimation>();
    }

    private void SetUpVariables()
    {
        canAttack = false;
        isAttacking = false;
        bombCooldownTimer = bombCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        HandleAttackCooldown();
        ShootBomb();
    }

    private void HandleAttackCooldown()
    {
        if (!isAttacking)
        {
            if (bombCooldownTimer <= 0)
            {
                canAttack = true;
            }
            else
            {
                canAttack = false;
                bombCooldownTimer -= Time.deltaTime;
            } 
        }
    }

    private void ShootBomb()
    {
        if (canAttack && !isAttacking && !enemyHealthController.IsInvincible)
        {
            spriteAnimation.InstantiateBomb();
            bombCooldownTimer = bombCooldown; 
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            enemyHealthController.ChangeEnemyHealth(collision.gameObject.GetComponent<PlayerCombatController>().GetPassiveDamage);
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        }
    }
}
