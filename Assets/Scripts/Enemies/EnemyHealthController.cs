using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthController : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] int maxHealth = 1;
    [SerializeField] GameObject HealthBarUI;
    [SerializeField] Transform fullHealthBar;
    [Header("Invincibility")]
    [SerializeField] float invincibleCooldown = 1f;
    [SerializeField] float blinkSpeed = 1f;
    [SerializeField] [Range(0, 1)] float alphaFactor;

    // Cached References
    Rigidbody2D myRigidbody2D;
    SpriteRenderer spriteRenderer;

    // Cached Health Variables
    public int GetCurrentHealth { get { return currentHealth; } }
    public int GetMaxHealth { get { return maxHealth; } }
    private int currentHealth;
    private Vector3 healthBarScale;
    private float healthPercentage;

    // Cached Invincibility Variables
    private float invincibleCooldownTimer;
    public bool GetInvincibleStatus { get { return isInvincible; } }
    private bool isInvincible = false;
    private Color originalColor;


    // Start is called before the first frame update
    void Start()
    {
        SetHealthBarUI();
        SetEnemyHealth();
        GetAccessToComponents();
        SetDefaultBoolStates();
        GetOriginalSpriteColor();
    }

    private void SetHealthBarUI()
    {
        healthBarScale = fullHealthBar.localScale;
        healthPercentage = healthBarScale.x / maxHealth;
    }

    private void SetEnemyHealth()
    {
        currentHealth = maxHealth;
    }

    private void GetAccessToComponents()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
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

    public void ChangeEnemyHealth(int amount)
    {
        if (isInvincible)
        {
            return;
        }
        else
        {
            isInvincible = true;
            invincibleCooldownTimer = invincibleCooldown;
            currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
            UpdateHealthBarUI();
        }
        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    private void UpdateHealthBarUI()
    {
        healthBarScale.x = healthPercentage * currentHealth;
        fullHealthBar.localScale = healthBarScale;
    }
}
