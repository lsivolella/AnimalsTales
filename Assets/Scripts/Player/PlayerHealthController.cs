using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthController : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] HealhBarUI healthBar;
    [SerializeField] int maxHealth = 1;
    [Header("Invincibility")]
    [SerializeField] float invincibleCooldown = 1f;
    [SerializeField] float blinkSpeed = 1f;
    [SerializeField] [Range(0, 1)] float alphaFactor;

    // Cached References
    Rigidbody2D myRigidbody2D;
    Animator myAnimator;
    PlayerMovementController playerMovementController;
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
        myRigidbody2D = GetComponent<Rigidbody2D>();
        myAnimator = GetComponentInChildren<Animator>();
        playerMovementController = GetComponent<PlayerMovementController>();
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
        HealhBarUI.Instance.SetValue(currentHealth / (float)maxHealth);
        //healthBar.SetValue(currentHealth / (float)maxHealth);
        if (currentHealth <= 0)
        {
            PlayDeathRoutine();
        }
    }

    private void PlayDeathRoutine()
    {
        playerMovementController.IsInputFrozen = true;
        myAnimator.SetTrigger("isDead");
        myRigidbody2D.constraints = RigidbodyConstraints2D.FreezePosition;
        // TODO Open Dead player pop-up
        // TODO Choose to relive in "sanctuary" or pay gold to revive in place
    }
}
