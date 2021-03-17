using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameMaster;

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
    GameMaster gameMaster;

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
        gameMaster = GameMaster.instance;
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
        if (!gameMaster.HasMadeExchange)
        {
            currentHealth = maxHealth;
            isInvincible = false;
            isAlive = true;
            gameMaster.IsBossAlive = true;
            gameMaster.BossHealth = currentHealth;
            gameMaster.HasMadeExchange = true;
        }
        else if (gameMaster.HasMadeExchange && gameMaster.IsBossAlive)
        {
            currentHealth = gameMaster.BossHealth;
            UpdateHealthBarUI();
            isAlive = true;
        }
        else if (gameMaster.HasMadeExchange && !gameMaster.IsBossAlive)
        {
            UpdateHealthBarUI();
            isAlive = false;
            myAnimator.SetTrigger("beginDead");
            transform.position = gameMaster.BossDeathPosition;
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
                gameMaster.BossHealth = currentHealth;
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
        gameMaster.IsBossAlive = false;
        gameMaster.BossDeathPosition = transform.position;
        reddishCatMovementController.CanMove = false;
        reddishCatMovementController.IsMoving = false;
    }

    private void DropHat()
    {
        // TODO: orange cat sprite with the hat
        // TODO: new dialogue when the hat is given to the orange cat and another one for after that
        Instantiate(hat, transform.position, Quaternion.identity);
        gameMaster.hatStatus = HatStatus.AtFlor;
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
            Debug.Log("Kill me!");
        }
    }

    private void KillCat()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            ChangeEnemyHealth(-currentHealth);
    }
}
