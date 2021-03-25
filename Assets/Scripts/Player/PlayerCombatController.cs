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
    [Header("Crossing Wall Prevention")]
    [SerializeField] float skinWidth = 0.1f;

    // Cached References
    PlayerHealthController playerHealthController;
    PlayerMovementController playerMovementController;
    Animator myAnimator;
    Rigidbody2D myRigidbody;
    Collider2D myCollider;

    // Cached Attack Variables
    private float scratchCooldownTimer;
    public int GetPassiveDamage { get { return passiveDamage; } }

    // Cached Knockback Variables
    private float timeKnockbackStarted;
    private float timeSinceKnockbackStarted;
    private bool doKnockback;
    private Vector2 knockbackStartPosition;
    private Vector2 knockbackEndPosition;

    // Cached Crossing Walls Prevention Variables
    private float minimumExtent;
    private float partialExtent;
    private float squareMinimumExtent;
    private Vector2 previousPosition;


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
        myRigidbody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
    }

    private void SetDefaultVariables()
    {
        doKnockback = false;
        // The folliwng three variables refer to the Crossing Wall Prevention
        minimumExtent = Mathf.Min(myCollider.bounds.extents.x, myCollider.bounds.extents.y);
        partialExtent = minimumExtent * (1.0f - skinWidth);
        squareMinimumExtent = Mathf.Pow(minimumExtent, 2f);
        previousPosition = myRigidbody.position;
    }


    // Update is called once per frame
    void Update()
    {
        GetAttackInput();
    }

    private void GetAttackInput()
    {
        if (!playerMovementController.IsInputFrozen && !playerMovementController.IsEngagedInConversation && !SceneController.instance.CinematicsOn)
        {
            if (scratchCooldownTimer <= 0)
            {
                if (Input.GetKeyDown(KeyCode.O) && !playerMovementController.IsInputFrozen)
                {
                    // Reset any velocity residual values. They may occur when the Player colliders with another body.
                    myRigidbody.velocity = Vector2.zero;
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
            target.GetComponent<ReddishCatHealthController>().ChangeEnemyHealth(scratchDamage);

            // Freeze Player Input
            playerMovementController.CallFreezeInputCoroutine();

            // Call CameraShake
            CameraShake.instance.CallShakeCoroutine();
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
            playerHealthController.StandardDamageRoutine(
                collision.gameObject.GetComponent<ReddishCatCombatController>().PassiveDamage,
                collision.otherCollider.transform.position,
                collision.gameObject.transform.position);
        }
        else if (collision.gameObject.CompareTag("HarmfullObject"))
        {
            playerHealthController.StandardDamageRoutine(
                collision.gameObject.GetComponent<PersistentHarmfulObject>().PassiveDamage,
                collision.otherCollider.transform.position,
                collision.gameObject.transform.position);
        }
        else if (collision.gameObject.CompareTag("Objects") && doKnockback)
        {
            knockbackEndPosition = transform.position;
            //Debug.Log("Start Position: " + knockbackStartPosition);
            //Debug.Log("End Position: " + knockbackEndPosition);
            //Debug.Log("Player Position Upon Inpact: " + transform.position);
            //Debug.Log(collision.GetContact(0).normal);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Fragment"))
        {
            playerHealthController.StaticDamageRoutine(collision.gameObject.GetComponent<FragmentController>().PassiveDamage);
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
            myRigidbody.position = PerformKnockbackDisplacement(knockbackStartPosition, knockbackEndPosition,
                timeKnockbackStarted, knockbackDuration);

            if (timeSinceKnockbackStarted >= knockbackDuration)
            {
                ResetKnockBack();
            }
        }
    }

    /// <summary>
    /// Responsible for setting up the variables used in the knockback routine. First one to be called.
    /// </summary>
    /// <param name="playerPosition"></param>
    /// <param name="objectPosition"></param>
    public void PlayKnockbackRoutine(Vector2 playerPosition, Vector2 objectPosition)
    {
        if (!doKnockback)
        {
            // Freeze Player Input
            playerMovementController.CallFreezeInputCoroutine();
            // Define Direction of Displacement Vector2
            knockbackStartPosition = playerPosition;
            Vector2 directionOfKnockback = playerPosition - objectPosition;
            knockbackEndPosition = playerPosition + (directionOfKnockback.normalized * knockbackForce);
            knockbackEndPosition = CrossingWallPrevention();
            // Initialize knockback timer
            timeKnockbackStarted = Time.time;
            // Allow knockback displacement
            doKnockback = true;
            // Trigger knockback animation
            myAnimator.SetBool("doKnockback", true);
        }
    }

    private Vector2 PerformKnockbackDisplacement(Vector2 start, Vector2 end, float timeStarted, float duration)
    {
        // Update time meter
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
        // Calculate the height based on the knockback displacement completition
        float zIncrease = Mathf.Sin(percentageCompleted * Mathf.PI) * maximumDisplacement;
        // Create a new Vector2 to store the result
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

    private Vector2 CrossingWallPrevention()
    {
        RaycastHit2D linecast = Physics2D.Linecast(knockbackStartPosition, knockbackEndPosition, LayerMask.GetMask("Walls"));
        Vector2 newKnockbackEndPosition = knockbackEndPosition;
        if (linecast.point != Vector2.zero)
        {
            Vector2 knockbackDirectionNormalized = (knockbackEndPosition - knockbackStartPosition).normalized;

            newKnockbackEndPosition = new Vector2(linecast.point.x + (myCollider.bounds.extents.x * 2 * -1f * Mathf.Sign(knockbackDirectionNormalized.x)), 
                linecast.point.y + (myCollider.bounds.extents.y * 2 * -1f * Mathf.Sign(knockbackDirectionNormalized.y)));

        }
        //Debug.Log("Bounds: " + myCollider.bounds.center.ToString("f3"));
        //Debug.Log("Extents: " + myCollider.bounds.extents.ToString("f3"));
        //Debug.Log("Origin" + knockbackStartPosition.ToString("f3"));
        //Debug.Log("Destination" + knockbackEndPosition.ToString("f3"));
        //Debug.Log("Contact Point:" + linecast.point.ToString("f3"));
        //Debug.Log("New Knockback End Position: " + newKnockbackEndPosition.ToString("f3"));
        //Debug.Log("Knockback Direction: " + (knockbackEndPosition - knockbackStartPosition).normalized.ToString("f3"));

        return newKnockbackEndPosition;
    }
}
