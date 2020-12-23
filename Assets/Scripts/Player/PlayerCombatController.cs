using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] int scratchDamage = -1;                  // Damage dealt is always negative (positive for Healing)
    [SerializeField] float scratchCooldown = 1f;
    [SerializeField] public float attackRange = 1f;
    [SerializeField] int passiveDamage = -1;                 // Damage dealt is always negative (positive for Healing)
    [SerializeField] public Transform scratchLeftPivot;
    [SerializeField] public Transform scratchUpPivot;
    [SerializeField] public Transform scratchRightPivot;
    [SerializeField] public Transform scratchDownPivot;
    [Header("Knockback")]
    [SerializeField] float knockbackDuration;
    [SerializeField] float knockbackForce = 1f;
    [SerializeField] GameObject shadowSprite;

    // Cached References
    PlayerHealthController playerHealthController;
    PlayerMovementController playerMovementController;
    Animator myAnimator;
    Rigidbody2D myRigidbody2D;
    RigidbodyConstraints2D defaultConstraints;

    // Cached Attack Variables
    private float scratchCooldownTimer;
    public int GetPassiveDamage { get { return passiveDamage; } }

    // Cached Knockback Variables
    private float timeKnockbackStarted;
    private float timeSinceKnockbackStarted;
    private bool doKnockback;
    private Vector2 knockbackStartPosition;
    private Vector2 knockbackEndPosition;


    private void Awake()
    {
        GetAccessToComponents();
        SetDefaultVariables();
    }

    private void GetAccessToComponents()
    {
        playerHealthController = GetComponent<PlayerHealthController>();
        playerMovementController = GetComponent<PlayerMovementController>();
        myAnimator = GetComponentInChildren<Animator>();
        myRigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void SetDefaultVariables()
    {
        doKnockback = false;
        defaultConstraints = myRigidbody2D.constraints;
    }


    // Update is called once per frame
    void Update()
    {
        GetAttackInput();
    }

    private void GetAttackInput()
    {
        if (!playerMovementController.IsInputFrozen && !playerMovementController.IsEngagedInConversation)
        {
            if (scratchCooldownTimer <= 0)
            {
                if (Input.GetKeyDown(KeyCode.Period) && !playerMovementController.IsInputFrozen)
                {
                    // Reset any velocity residual values. They may occur when the Player colliders with another body.
                    myRigidbody2D.velocity = Vector2.zero;
                    // myRigidbody2D.constraints = RigidbodyConstraints2D.FreezePosition;
                    playerMovementController.CallFreezeInputCoroutine();
                    ScratchAttack();
                    scratchCooldownTimer = scratchCooldown;
                    // myRigidbody2D.constraints = defaultConstraints;
                }
            }
            else
            {
                scratchCooldownTimer -= Time.deltaTime;
            }
        }
    }

    private void ScratchAttack()
    {
        myAnimator.SetTrigger("doAttack");

        // Since double keyboard entry keys brake the Vector2.direcion factor, new conditions were made to account for these situations.
        // Dominant Sprite animations guide these conditions.
        Transform _playerLookDirection;
        if (playerMovementController.GetLookDirection == Vector2.left ||
            playerMovementController.GetLookDirection == new Vector2(-0.7071068f, -0.7071068f) ||
            playerMovementController.GetLookDirection == new Vector2(-0.7071068f, 0.7071068f))
        {
            _playerLookDirection = scratchLeftPivot;
        }
        else if (playerMovementController.GetLookDirection == Vector2.up ||
                 playerMovementController.GetLookDirection == new Vector2(0.7071068f, 0.7071068f))
        {
            _playerLookDirection = scratchUpPivot;
        }
        else if (playerMovementController.GetLookDirection == Vector2.right ||
                 playerMovementController.GetLookDirection == new Vector2(0.7071068f, -0.7071068f))
        {
            _playerLookDirection = scratchRightPivot;
        }
        else if (playerMovementController.GetLookDirection == Vector2.down)
        {
            _playerLookDirection = scratchDownPivot;
        }
        else
        {
            _playerLookDirection = null;
        }

        Collider2D[] targets = Physics2D.OverlapCircleAll(_playerLookDirection.position, attackRange, LayerMask.GetMask("Enemy"));

        foreach (Collider2D target in targets)
        {
            // Damage Targets
            target.GetComponent<EnemyHealthController>().ChangeEnemyHealth(scratchDamage);

            // Freeze Player Input
            playerMovementController.CallFreezeInputCoroutine();

            // Call CameraShake
            CameraShake.Instance.CallShakeCoroutine();
        }

        Collider2D[] bombTargets = Physics2D.OverlapCircleAll(scratchUpPivot.position, attackRange, LayerMask.GetMask("Bomb"));

        foreach (Collider2D bombTarget in bombTargets)
        {
            // Prevent from hitting the Bomb in any other situation
            if (bombTarget.GetComponent<BombController>().BombStateGet == BombController.BombState.GroundedAtPlayerArea)
            {
                bombTarget.GetComponent<BombController>().SetUpThrowAgainstCaster();
            }
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Damage Player
            playerHealthController.ChangeHealth(collision.gameObject.GetComponent<EnemyCombatController>().PassiveDamage);

            // Do Knockback
            PlayKnockbackRoutine(collision.otherCollider.transform.position,
                collision.gameObject.transform.position);

            // Call CameraShake
            CameraShake.Instance.CallShakeCoroutine();
        }
        else if (collision.gameObject.CompareTag("HarmfullObject"))
        {
            // Damage Player
            //playerHealthController.ChangeHealth(collision.gameObject.GetComponent<PersistentHarmfulObject>().GetPassiveDamage);
                       
            // collision.otherCollider = Player
            // collision.gameObject = Whom Player Collided with
            // Do Knockback
            PlayKnockbackRoutine(collision.otherCollider.transform.position,
                collision.gameObject.transform.position);

            // Call CameraShake
            CameraShake.Instance.CallShakeCoroutine();
        }
        else if (collision.gameObject.CompareTag("Bomb"))
        {
            if (collision.gameObject.GetComponent<BombController>().BombStateGet == BombController.BombState.FlyingToPlayer)
            {
                Debug.Log("Touch bomb");
                // myRigidbody2D.MovePosition(collision.gameObject.GetComponent<BombController>().EndPosition + Vector2.down);
            }
        }
    }

    private void FixedUpdate()
    {
        ProcessKnockback();
    }

    private void ProcessKnockback()
    {
        if (doKnockback)
        {
            myRigidbody2D.position = PerformKnockbackDisplacement(knockbackStartPosition, knockbackEndPosition,
                timeKnockbackStarted, knockbackDuration);

            if (timeSinceKnockbackStarted >= knockbackDuration)
            {
                ResetKnockBack();
            }
        }
    }

    public void PlayKnockbackRoutine(Vector2 playerPosition, Vector2 objectPosition)
    {
        // Freeze Player Input
        playerMovementController.CallFreezeInputCoroutine();
        // Define Direction of Displacement Vector2
        knockbackStartPosition = playerPosition;
        Vector2 _directionOfKnockback = playerPosition - objectPosition;
        knockbackEndPosition = playerPosition + (_directionOfKnockback.normalized * knockbackForce);
        // Initialize knockback timer
        timeKnockbackStarted = Time.time;
        // Allow for knockback displacement
        doKnockback = true;
        // Trigger knockback animation
        myAnimator.SetBool("doKnockback", true);
    }

    private Vector2 PerformKnockbackDisplacement(Vector2 start, Vector2 end, float timeStarted, float duration)
    {
        // Create a time meter
        timeSinceKnockbackStarted = Time.time - timeStarted;
        // Use the time meter to measure completion
        float percentageCompleted = timeSinceKnockbackStarted / duration;
        // Interpolate between the Start and End Vectors
        var result = Vector2.Lerp(start, end, percentageCompleted);
        // Call the knockback arc animation
        PlayKnockbackArcAnimation(percentageCompleted);
        // Pass the interpolation Vector2 to the function that does the object displacement
        return result;
    }

    private void PlayKnockbackArcAnimation(float percentageCompleted)
    {
        // Set the maximum height in the arc the sprite is going to be deslocated
        float maximumDisplacement = 0.2f;
        // Calculate the height based on the knockbac displacement completition
        var zIncrease = Mathf.Sin(percentageCompleted * Mathf.PI) * maximumDisplacement;
        // Create a new Vector2 to store the value
        Vector2 playerSpritePosition = new Vector2(0, zIncrease);
        // Apply the Vector2 to the Player Sprite child component
        transform.GetChild(0).GetComponent<Transform>().transform.localPosition = playerSpritePosition;
    }

    private void ResetKnockBack()
    {
        // Stop knockback animation
        myAnimator.SetBool("doKnockback", false);
        // Finish knockback routine access
        doKnockback = false;
    }
}
