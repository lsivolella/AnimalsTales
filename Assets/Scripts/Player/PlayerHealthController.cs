using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthController : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] HealthBarUI healthBar;
    [SerializeField] int maxHealth = 1;
    [Header("Invincibility")]
    [SerializeField] float invincibleCooldown = 1f;
    [SerializeField] float blinkSpeed = 1f;
    [SerializeField] [Range(0, 1)] float alphaFactor;

    // Cached References
    Rigidbody2D myRigidbody;
    Animator myAnimator;
    PlayerMovementController playerMovementController;
    PlayerCombatController playerCombatController;
    SpriteRenderer spriteRenderer;

    // Cached Health Variables
    public int GetCurrentHealth { get { return currentHealth; } }
    public int GetMaxHealth { get { return maxHealth; } }
    private int currentHealth;

    // Cached Invincibility Variables
    private float invincibleCooldownTimer;
    public bool GetInvincibleStatus { get { return isInvincible; } }
    private bool isInvincible = false;
    private Color originalColor;

    // Start is called before the first frame update
    private void Awake()
    {
        GetAccessToComponents();
        SetPlayerHealth();
        SetDefaultBoolStates();
        GetOriginalSpriteColor();
    }

    private void GetAccessToComponents()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponentInChildren<Animator>();
        playerMovementController = GetComponent<PlayerMovementController>();
        playerCombatController = GetComponent<PlayerCombatController>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void SetPlayerHealth()
    {
        currentHealth = maxHealth;
    }

    private void SetDefaultBoolStates()
    {
        isInvincible = false;
    }

    private void GetOriginalSpriteColor()
    {
        originalColor = spriteRenderer.color;
    }

    // Update is called once per frame
    void Update()
    {
        CheckInvincibilityState();
        StartCoroutine(BlinkSpriteDuringInvincibility());
    }

    private void CheckInvincibilityState()
    {
        if (isInvincible)
        {
            invincibleCooldownTimer -= Time.deltaTime;
            if (invincibleCooldownTimer < 0)
            {
                isInvincible = false;
            }
        }
    }

    IEnumerator BlinkSpriteDuringInvincibility()
    {
        if (isInvincible)
        {
            while (invincibleCooldownTimer > 0)
            {
                spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f * alphaFactor);
                yield return new WaitForSeconds(blinkSpeed);
                spriteRenderer.color = originalColor;
                yield return new WaitForSeconds(blinkSpeed);
            }
        }
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
            {
                return;
            }
            else
            {
                isInvincible = true;
                invincibleCooldownTimer = invincibleCooldown;
            }
        }
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        HealthBarUI.Instance.SetValue(currentHealth / (float)maxHealth);
        //healthBar.SetValue(currentHealth / (float)maxHealth);
        if (currentHealth <= 0)
        {
            PlayDeathRoutine();
        }
    }

    /// <summary>
    /// This method is responsible for handling other methods in the Standard Damage Routine.
    /// The Standard Damage Routine includes: Health Penalty, Input Blockage, Knockback Displacement and Screen Shake.
    /// </summary>
    /// <param name="damageAmount">The amount of damage the player will suffer.</param>
    /// /// <param name="playerPosition">The Vector2 position of the player when damage was dealt.</param>
    /// <param name="objectPosition">The Vector2 position of the object when damage was dealt.</param>
        public void StandardDamageRoutine(int damageAmount, Vector2 playerPosition, Vector2 objectPosition)
    {
        if (!isInvincible)
        {
            // Damage Player
            ChangeHealth(damageAmount);
            // Knockback Player
            playerCombatController.PlayKnockbackRoutine(playerPosition, objectPosition);
            // Camera Shake
            CameraShake.Instance.CallShakeCoroutine(); 
        }
    }

    /// <summary>
    /// This method is responsible for handling other methods in the Static Damage Routine.
    /// The Static Damage Routine includes: Health Penalty.
    /// </summary>
    /// <param name="damageAmount">The amount of damage the player will suffer.</param>
    public void StaticDamageRoutine(int damageAmount)
    {
        if (!isInvincible)
        {
            // Damage Player
            ChangeHealth(damageAmount);
        }
    }

    private void PlayDeathRoutine()
    {
        playerMovementController.IsInputFrozen = true;
        myAnimator.SetTrigger("isDead");
        myRigidbody.constraints = RigidbodyConstraints2D.FreezePosition;
        // TODO Open Dead player pop-up
        // TODO Choose to relive in "sanctuary" or pay gold to revive in place
    }
}
