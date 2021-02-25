using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombController : MonoBehaviour
{
    [Header("General")]
    [SerializeField] CapsuleCollider2D myCollider;
    [Header("Arc Animation")]
    [SerializeField] float minimumThrowDistance = 1f;
    [SerializeField] float maximumThrowDistance = 1f;
    [SerializeField] float minimumBombSpeed = 1f;
    [SerializeField] float maximumArcHight = 1f;
    [Header("Bomb Detonation")]
    [SerializeField] GameObject fragmentPrefab;
    [SerializeField] int bombDamage = -1;   // Damage dealt is always negative (positive for Healing)
    [SerializeField] public float explosionRadius = 1f;
    [Header("Fighting Area")]
    [Tooltip("Minimum world horizontal coordinate the Enemy may run up to.")]
    [SerializeField] float fightingArenaMinimumHorizontalPosition = 1f;
    [Tooltip("Maximum world horizontal coordinate the Enemy may run up to.")]
    [SerializeField] float fightingArenaMaximumHorizontalPosition = 1f;

    // Cached enum
    public enum BombState { NotThrown, FlyingToPlayer, GroundedAtPlayerArea, FlyingToEnemy, GroundedAtEnemyArea };
    BombState bombState;

    // Cached References
    BombAnimationController bombAnimationController;
    Rigidbody2D myRigidbody;
    RaycastHit2D searchForEnemyRaycast;


    // Cached Variables
    private Vector2 positionWhenThrownByEnemy;
    private Vector2 positionWhenLanded;
    private float randomlyPickedThrowDistance;
    private float timeWhenWasThrown = 0f;
    private float distanceTraveled = 0f;
    private float percentualDistanceTravaled;
    private float bombSpeed;

    // Properties
    public BombState BombStateGet { get { return bombState; } }
    public Vector2 PositionWhenLanded { get { return positionWhenLanded; } }


    private void Awake()
    {
        GetAccessToComponents();
        SetDefaultVariables();
    }

    private void GetAccessToComponents()
    {
        bombAnimationController = GetComponentInChildren<BombAnimationController>();
        myCollider = GetComponent<CapsuleCollider2D>();
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    private void SetDefaultVariables()
    {
        // Prevent Bomb from colliding with Enemy before it is thrown
        myCollider.enabled = false;
        // Select a random distance for the Bomb to be thrown at
        randomlyPickedThrowDistance = Random.Range(minimumThrowDistance, maximumThrowDistance);
        // Modify the speed according to the distance the Bomb will travel
        bombSpeed = minimumBombSpeed * randomlyPickedThrowDistance / minimumThrowDistance;
        // Set the Bomb initial state
        bombState = BombState.NotThrown;
    }

    public void SetUpThrowAgainstPlayer()
    {
        positionWhenThrownByEnemy = transform.position;
        positionWhenLanded = positionWhenThrownByEnemy + Vector2.down * randomlyPickedThrowDistance;
        // Get a Time reference for the moment the action begun
        timeWhenWasThrown = Time.time;
        bombState = BombState.FlyingToPlayer;
    }

    public void SetUpThrowAgainstCaster()
    {
        // Adjusts the start position so that the Bomb falls in the same y coordinate
        // as the enemy, when thrown back by the player, blocking his horizontal movements
        positionWhenThrownByEnemy += Vector2.down * 0.5f;
        // Check if the Enemy in in front of the Bomb and, if positive, return his position
        Vector2 _newPositionWhenThrownByEnemy = LaunchRaycast(positionWhenThrownByEnemy);
        // Select the adaquete destination Vector2 for the Bomb
        if (_newPositionWhenThrownByEnemy != Vector2.zero)
        {
            positionWhenThrownByEnemy = _newPositionWhenThrownByEnemy;
        }
        // Get a Time reference for the moment the action begun
        timeWhenWasThrown = Time.time;
        // travelBack = true;
        bombState = BombState.FlyingToEnemy;
        // Suspend Bomb detonation
        bombAnimationController.HoldDetonation();
    }

    private void FixedUpdate()
    {
        ControllBombDisplacementProgress();
        ThrowAgainstPlayer();
        ThrowAgainstCaster();
    }

    private void ControllBombDisplacementProgress()
    {
        if (bombState == BombState.FlyingToPlayer || bombState == BombState.FlyingToEnemy)
        {
            distanceTraveled = bombSpeed * (Time.time - timeWhenWasThrown);
            percentualDistanceTravaled = distanceTraveled / randomlyPickedThrowDistance;
            PlayArcAnimation();

            if (percentualDistanceTravaled >= 1f)
            {
                myCollider.enabled = true;
                bombAnimationController.PrepareDetonation();
                ResetArcProgress();

                if (bombState == BombState.FlyingToPlayer) bombState = BombState.GroundedAtPlayerArea;
                else if (bombState == BombState.FlyingToEnemy) bombState = BombState.GroundedAtEnemyArea;
            }
        }
    }

    private void ResetArcProgress()
    {
        distanceTraveled = 0;
        timeWhenWasThrown = 0;
        percentualDistanceTravaled = 0;
    }

    private void ThrowAgainstPlayer()
    {
        // This is initiated when the cat throw the bomb towards the player
        if (bombState == BombState.FlyingToPlayer)
        {
            Vector2 currentPosition = Vector2.Lerp(positionWhenThrownByEnemy, positionWhenLanded, percentualDistanceTravaled);
            myRigidbody.MovePosition(currentPosition);
        }
    }

    public void ThrowAgainstCaster()
    {
        // This is initiated when the player successfully hit the bomb from the bottom
        if (bombState == BombState.FlyingToEnemy)
        {
            // Vector2 _positionWhenThrownByEnemyAdjusted = new Vector2(positionWhenThrownByEnemy.x, positionWhenThrownByEnemy.y - 0.5f);
            Vector2 _positionWhenThrownByEnemyAdjusted = new Vector2(positionWhenThrownByEnemy.x, positionWhenThrownByEnemy.y);
            Vector2 _positionVector = Vector2.Lerp(positionWhenLanded, _positionWhenThrownByEnemyAdjusted, percentualDistanceTravaled);
            myRigidbody.MovePosition(_positionVector);
        }
    }

    private void PlayArcAnimation()
    {
        if (bombState == BombState.FlyingToPlayer)
        {
            float arcStartPercent = (0.5f / maximumArcHight) * 0.5f;
            float arcPercent = arcStartPercent + Mathf.Lerp(1 - arcStartPercent, 0 - arcStartPercent, percentualDistanceTravaled);
            var hightIncrease = Mathf.Sin(arcPercent * Mathf.PI) * maximumArcHight;
            Vector2 bombSpritePosition = new Vector2(0, hightIncrease);
            transform.GetChild(0).GetComponent<Transform>().transform.localPosition = bombSpritePosition;
        }
        else if (bombState == BombState.FlyingToEnemy)
        {
            var hightIncrease = Mathf.Sin(percentualDistanceTravaled * Mathf.PI) * maximumArcHight;
            Vector2 bombSpritePosition = new Vector2(0, hightIncrease);
            transform.GetChild(0).GetComponent<Transform>().transform.localPosition = bombSpritePosition;
        }
    }

    private Vector2 LaunchRaycast(Vector2 positionWhenThrownByEnemy)
    {
        if (bombState == BombState.GroundedAtPlayerArea)
        {
            searchForEnemyRaycast = CastSearchForEnemyRaycast();

            if (searchForEnemyRaycast.collider != null)
            {
                // Get Enemy Sprite Width
                float _enemySpriteWidthInUnits = searchForEnemyRaycast.collider.GetComponent<Transform>().
                    GetChild(0).GetComponent<SpriteRenderer>().sprite.bounds.size.x;
                // Get Bomb Sprite Width
                float _bombSpriteWidthInUnits = gameObject.GetComponent<Transform>().
                    GetChild(0).GetComponent<SpriteRenderer>().sprite.bounds.size.x;
                // Create a padding that places the Bomb just beside the Enemy with an additional hard coded value to guarantee colliders don't intersect
                float _xPadding = _enemySpriteWidthInUnits / 2 + _bombSpriteWidthInUnits / 2 + 0.1f;
                // Determine wheater Bomb and Enemy are on the left or right side or the fighting arena
                // And define witch side should the padding be applied to
                // Left Side > Bomb goes to the right (no need to change padding)
                float centerPoint = (fightingArenaMaximumHorizontalPosition + fightingArenaMinimumHorizontalPosition) / 2f;
                if (transform.position.x >= centerPoint)
                {
                    // Right Side > Bomb goes to the left > Dominant Side
                    _xPadding *= -1;
                }
                // Apply the padding
                Vector2 _newpositionWhenThrownByEnemy = new Vector2(positionWhenThrownByEnemy.x + _xPadding, positionWhenThrownByEnemy.y);
                return _newpositionWhenThrownByEnemy;
            }
            else { return Vector2.zero; }
        }
        else { return Vector2.zero; }
    }

    private RaycastHit2D CastSearchForEnemyRaycast()
    {
        RaycastHit2D _raycastHit2D = Physics2D.Raycast(myRigidbody.position + Vector2.up * 0.2f,
            Vector2.up, 6f, LayerMask.GetMask("Enemy"));
        return _raycastHit2D;
    }

    public void ApplyDamage()
    {
        Vector2 centeredPivotPoint = new Vector2(transform.position.x + myCollider.offset.x, transform.position.y + myCollider.offset.y);
        Collider2D[] playerTarget = Physics2D.OverlapCircleAll(centeredPivotPoint, explosionRadius, LayerMask.GetMask("Player"));

        foreach (Collider2D target in playerTarget)
        {
            target.GetComponent<PlayerHealthController>().StandardDamageRoutine(bombDamage, target.gameObject.transform.position, centeredPivotPoint);
        }

        Collider2D[] enemyTarget = Physics2D.OverlapCircleAll(centeredPivotPoint, explosionRadius, LayerMask.GetMask("Enemy"));
        foreach (Collider2D target in enemyTarget)
        {
            target.GetComponent<ReddishCatHealthController>().ChangeEnemyHealth(bombDamage);
        }
    }

    public void SelfDestruct()
    {
        if (bombState == BombState.GroundedAtPlayerArea)
            InstantiateBombFragment();
        Destroy(gameObject);
    }

    private void InstantiateBombFragment()
    {
        float angle = Random.Range(0, 360f);
        float angleStep = 360f / 3;

        for (int i = 0; i < 3; i++)
        {
            float xDestination = Mathf.Sin((angle * Mathf.PI) / 180);
            float yDestination = Mathf.Cos((angle * Mathf.PI) / 180);

            Vector2 fragmentOrigin = new Vector2(transform.position.x + xDestination * 0.3f,
                transform.position.y + yDestination * 0.3f);
            Vector2 fragmentDestination = new Vector2(transform.position.x + xDestination * 0.5f, 
                transform.position.y + yDestination * 0.5f);


            GameObject fragment = Instantiate(fragmentPrefab, fragmentOrigin, Quaternion.identity);
            fragment.GetComponent<FragmentController>().SetFragmentMovement(fragmentDestination);

            angle += angleStep;
        }
    }
}