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
    [Header("Loot")]
    [SerializeField] GameObject hat;
    [Header("Minions")]
    [SerializeField] List<GameObject> minions;

    // Cached References
    ReddishCatMovementController reddishCatMovementController;
    SpriteRenderer spriteRenderer;
    Animator myAnimator;

    // Cached Health Variables
    private int currentHealth;
    private Vector3 healthBarScale;
    private float healthPercentage;
    private bool isAlive;
    public bool IsAlive { get { return isAlive; } }

    // Cached Invincibility Variables
    private float invincibleCooldownTimer;
    private bool isInvincible = false;
    public bool IsInvincible { get { return isInvincible; } }
    private Color originalColor;

    private void Awake()
    {
        GetAccessToComponents();
        SetHealthBarUI();
    }

    private void GetAccessToComponents()
    {
        reddishCatMovementController = GetComponent<ReddishCatMovementController>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        myAnimator = GetComponentInChildren<Animator>();
    }

    private void SetHealthBarUI()
    {
        healthBarScale = fullHealthBar.localScale;
        healthPercentage = healthBarScale.x / maxHealth;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetDefaultVariables();
        GetOriginalSpriteColor();
    }

    private void SetDefaultVariables()
    {
        if (!GameMaster.instance.HasMadeExchange)
        {
            currentHealth = maxHealth;
            isInvincible = false;
            isAlive = true;
            GameMaster.instance.IsBossAlive = true;
            GameMaster.instance.BossHealth = currentHealth;
            GameMaster.instance.HasMadeExchange = true;
        }
        else if (GameMaster.instance.HasMadeExchange && GameMaster.instance.IsBossAlive)
        {
            currentHealth = GameMaster.instance.BossHealth;
            UpdateHealthBarUI();
            isAlive = true;
        }
        else if (GameMaster.instance.HasMadeExchange && !GameMaster.instance.IsBossAlive)
        {
            UpdateHealthBarUI();
            isAlive = false;
            myAnimator.SetTrigger("beginDead");
            transform.position = GameMaster.instance.BossDeathPosition;
        }
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
        KillCat();
    }

    private void CheckInvincibilityState()
    {
        if (isInvincible && isAlive)
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
        if (isInvincible && isAlive)
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
        if (isAlive)
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
                GameMaster.instance.BossHealth = currentHealth;
            }
            if (currentHealth <= 0)
            {
                SetDeadState();
                PlayDeathRoutine();
                KillMinions();
                Invoke("DropHat", 1f);
            } 
        }
    }

    private void UpdateHealthBarUI()
    {
        healthBarScale.x = healthPercentage * currentHealth;
        fullHealthBar.localScale = healthBarScale;
    }

    private void SetDeadState()
    {
        isAlive = false;
        GameMaster.instance.IsBossAlive = false;
        GameMaster.instance.BossDeathPosition = transform.position;
        reddishCatMovementController.CanMove = false;
        reddishCatMovementController.IsMoving = false;
    }

    private void DropHat()
    {
        // TODO: call hat animation: it is going to spawn at the place where the redish cat is and then slowly fall down to a place the player can pick it up
        // TODO: UI square to indicate the player has the hat in his inventory
        // TODO: orange cat sprite with the hat
        // TODO: new dialogue when the hat is given to the orange cat and another one for after that
        // TODO: make sure entering the cave after black cat is defeat wont spawn him again
        Instantiate(hat, transform.position, Quaternion.identity);
    }

    private void PlayDeathRoutine()
    {
        myAnimator.SetTrigger("isDead");
    }

    private void KillMinions()
    {
        foreach (GameObject minion in minions)
        {
            minion.GetComponentInChildren<ReddishMinionAnimationController>().PlayDeathRoutine();
            minion.GetComponent<ReddishMinionMovementController>().StoreDeadBodyPosition();
        }
    }

    private void KillCat()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            ChangeEnemyHealth(-currentHealth);
    }
}
