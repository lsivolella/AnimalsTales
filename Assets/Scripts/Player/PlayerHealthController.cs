using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameMaster;

public class PlayerHealthController : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] HealthBarUI healthBar;
    [SerializeField] int maxHealth = 1;
    [Header("Invincibility")]
    [SerializeField] float invincibleCooldown = 1f;
    [SerializeField] float blinkSpeed = 1f;
    [SerializeField] [Range(0, 1)] float alphaFactor;
    [Header("Inventory")]
    [SerializeField] InventoryController inventoryController; 

    // Cached References
    PlayerMovementController playerMovementController;
    PlayerCombatController playerCombatController;
    Rigidbody2D myRigidbody;
    Animator myAnimator;
    SpriteRenderer spriteRenderer;
    Collider2D myCollider;
    GameMaster gameMaster;
    SceneController sceneController;

    // Cached Health Variables
    private int currentHealth;

    // Cached Invincibility Variables
    private float invincibleCooldownTimer;
    public bool GetInvincibleStatus { get { return isInvincible; } }
    private bool isInvincible = false;
    private Color originalColor;

    // Cached Inventory Variabless

    // Start is called before the first frame update
    private void Awake()
    {
        GetAccessToComponents();
        SetUpDefaultVariables();
        GetOriginalSpriteColor();
    }

    private void GetAccessToComponents()
    {
        playerMovementController = GetComponent<PlayerMovementController>();
        playerCombatController = GetComponent<PlayerCombatController>();
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        myCollider = GetComponent<Collider2D>();
        gameMaster = GameMaster.instance;
        sceneController = SceneController.instance;
    }

    private void SetUpDefaultVariables()
    {
        if (!gameMaster.PlayerHealthSetUp)
        {
            currentHealth = maxHealth;
            gameMaster.hatStatus = HatStatus.NotDropped;
            gameMaster.PlayerHealth = currentHealth;
            gameMaster.PlayerMaxHealth = maxHealth;
            gameMaster.PlayerHealthSetUp = true;
        }
        else if (gameMaster.PlayerHealthSetUp)
        {
            currentHealth = gameMaster.PlayerHealth;
        }
        isInvincible = false;
    }

    private void GetOriginalSpriteColor()
    {
        originalColor = spriteRenderer.color;
    }

    private void Start()
    {
        UpdateHealthBarUI(); 
    }

    public void UpdateHealthBarUI()
    {
        HealthBarUI.instance.SetValue(currentHealth / (float)maxHealth);
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
        gameMaster.PlayerHealth = currentHealth;
        UpdateHealthBarUI();
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
            CameraShake.instance.CallShakeCoroutine(); 
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
        myCollider.enabled = false;
        myRigidbody.constraints = RigidbodyConstraints2D.FreezePosition;
        myAnimator.SetTrigger("isDead");
        gameMaster.ResetGame();
        Invoke("CallLoadMenuScene", 6f);
    }

    private void CallLoadMenuScene()
    {
        sceneController.LoadMainMenu();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Hat"))
        {
            gameMaster.hatStatus = HatStatus.Inventory;
            inventoryController.ActivateHat();
        }
    }
}
