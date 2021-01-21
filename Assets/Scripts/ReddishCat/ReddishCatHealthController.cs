using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReddishCatHealthController : MonoBehaviour
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
    private bool isInvincible = false;
    public bool IsInvincible { get { return isInvincible; } }
    private Color originalColor;


    // Start is called before the first frame update
    void Start()
    {
        SetHealthBarUI();
        SetEnemyHealth();
        GetAccessToComponents();
        SetDefaultVariables();
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

    private void SetDefaultVariables()
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
            currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
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

    private void DropHat()
    {
        // TODO: call hat animation: it is going to spawn at the place where the redish cat is and then slowly fall down to a place the player can pick it up
        // TODO: UI square to indicate the player has the hat in his inventory
        // TODO: orange cat sprite with the hat
        // TODO: new dialogue when the hat is given to the orange cat and another one for after that
        // TODO: scene transition when player hit trigger (to be created) taking player to the cave
        // TODO: transition to the way back... redish cat recovers all energy and has new dialogues
        // TODO: make sure entering the cave after black cat is defeat wont spawn him again
        // TODO: create a minion for the black cat that spawns randomly and attacks the player?
        // TODO: create cave scene
    }
}
