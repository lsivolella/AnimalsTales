using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombController : MonoBehaviour
{
    [Header ("Arc Animation")]
    [SerializeField] float[] arcDuration = new float[3];
    [SerializeField] float bombSpeed = 1f;
    [Header("Bomb Detonation")]
    [SerializeField] public Transform colliderLeftPivot;
    [SerializeField] public Transform colliderUpPivot;
    [SerializeField] public Transform colliderRightPivot;
    [SerializeField] public Transform colliderDownPivot;
    [SerializeField] public float colliderRange = 1f;

    // Cached References
    Collider2D myCollider;
    Animator myAnimator;
    Rigidbody2D myRigidbody2D;

    private Vector2 liftedPosition;
    private Vector2 landedPosition;
    private bool startMoving = false;
    private bool travelBack = false;
    private float draftedArcDuration;
    private float arcDurationMeter;
    private float percentageCompleted;

    public bool StartMoving { set { startMoving = value; } }
    public bool TravelBack { set { travelBack = value; } }

    private void Awake()
    {
        GetAccessToComponents();
        SetDefaultVariables();
    }

    private void GetAccessToComponents()
    {
        myCollider = GetComponent<Collider2D>();
        myAnimator = GetComponentInChildren<Animator>();
        myRigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void SetDefaultVariables()
    {
        // Prevents bomb form colliding with enemy before it is thorwn
        myCollider.enabled = false;
        // Get a reference to where te bomb was thrown from
        liftedPosition = transform.position;
        // Arc animation variables
        int randomIndex = Random.Range(0, 3);
        draftedArcDuration = arcDuration[randomIndex];
    }

    // Update is called once per frame
    void Update()
    {
        ThrowAgainstPlayer();
        ManageArcAnimation();
        ThrowAgainstCaster();
    }

    private void ManageArcAnimation()
    {
        if (percentageCompleted > 1)
        {
            myCollider.enabled = true;
            startMoving = false;
            travelBack = false;
            ResetArcProgress();
        }
    }

    private void ResetArcProgress()
    {
        arcDurationMeter = 0;
        percentageCompleted = 0;
    }

    private void ThrowAgainstPlayer()
    {
        // This is initiated right when the cat throw the bomb towards the player
        if (startMoving)
        {
            // Catch the position where the bomb was thrown
            landedPosition = transform.position;
            Vector2 position = transform.position;
            position += Vector2.down * Time.deltaTime * bombSpeed;
            transform.position = position;
            ControllArcProgress();

            if (percentageCompleted >= 1f)
            {
                myAnimator.SetTrigger("detonation");
            }
        }
    }

    public void ThrowAgainstCaster()
    {
        if (travelBack)
        {
            Vector2 startPositionAdjusted = new Vector2(liftedPosition.x, liftedPosition.y + 0.5f);
            Vector2 positionVector = Vector2.Lerp(landedPosition, startPositionAdjusted, percentageCompleted);
            ControllArcProgress();
            myRigidbody2D.MovePosition(positionVector);
        }
    }

    private void ControllArcProgress()
    {
        arcDurationMeter += Time.deltaTime;
        percentageCompleted = arcDurationMeter / draftedArcDuration;
        PlayArcAnimation(percentageCompleted);
    }

    private void PlayArcAnimation(float percentageCompleted)
    {
        float maximumDisplacement = 0.6f;
        var hightIncrease = Mathf.Sin(percentageCompleted * Mathf.PI) * maximumDisplacement;
        Vector2 bombSpritePosition = new Vector2(0, hightIncrease);
        transform.GetChild(0).GetComponent<Transform>().transform.localPosition = bombSpritePosition;
    }

    public void SelfDestruct()
    {
        Destroy(gameObject);
    }
}
