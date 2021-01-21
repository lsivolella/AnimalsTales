using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentController : MonoBehaviour
{
    [SerializeField] Sprite[] fragmentsSprite;
    [SerializeField] float fragmentSpeed = 1f;
    [SerializeField] float fragmentDuration = 1f;
    [SerializeField] float fadeSpeed = 1f;
    [SerializeField] int passiveDamage = -1;

    // Cached References
    SpriteRenderer spriteRenderer;

    // Cached Fade Variables
    private float fragmentDurationTimer;
    private float fadeSpeedTimer;

    // Cached Movement and Arc Variables
    private float timeMovementStarted;
    private float timeSinceMovementStarted;
    private float movementDuration = 0.5f;
    private bool canMove = false;
    private Vector2 originPosition;
    private Vector2 destinationPosition;

    // Properties
    public int PassiveDamage { get { return passiveDamage; } }

    // Start is called before the first frame update
    void Start()
    {
        GetAccessToComponents();
        SelectRandomSprite();
        SetFadeTimer();
    }

    private void GetAccessToComponents()
    {
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    private void SelectRandomSprite()
    {
        int randomIndex = Random.Range(0, 4);
        spriteRenderer.sprite = fragmentsSprite[randomIndex];
    }

    public void SetFragmentMovement(Vector2 destination)
    {
        // Set origin Vector2
        originPosition = transform.position;
        // Set destination Vector2
        destinationPosition = destination;
        // Allow movement
        canMove = true;
        // Initialize movement timer
        timeMovementStarted = Time.time;
    }

    private void SetFadeTimer()
    {
        fragmentDurationTimer = fragmentDuration;
        fadeSpeedTimer = fadeSpeed;
        Invoke("SelfDestruct", fragmentDuration + fadeSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        fragmentDurationTimer -= Time.deltaTime;

        if (fragmentDurationTimer <= 0)
        {
            fadeSpeedTimer -= Time.deltaTime;

            float alphaFactor = fadeSpeedTimer / fadeSpeed;
            spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f * alphaFactor);
        }

        if (canMove)
        {
            // transform.position = Vector2.MoveTowards(transform.position, movementDirection, projectileSpeed * Time.deltaTime);
            transform.position = PerformDisplacement(originPosition, destinationPosition, timeMovementStarted, movementDuration);
        }
    }

    private Vector2 PerformDisplacement(Vector2 origin, Vector2 destination, float timeStarted, float duration)
    {
        // Update time meter
        timeSinceMovementStarted = Time.time - timeMovementStarted;
        // use the time meter to measure completion
        float percentageCompleted = timeSinceMovementStarted / duration;
        // Interpolate between the Origin and Destination Vectors
        var result = Vector2.Lerp(origin, destination, percentageCompleted);
        // Call the arc animation
        PlayArcAnimation(percentageCompleted);
        // Stop all movement when movement is completed
        if (percentageCompleted > 1f)
        {
            canMove = false;
        }
        // Return the interpolation Vector2
        return result;
    }

    private void PlayArcAnimation(float percentageCompleted)
    {
        // Calculate the height based on the movement displacement completition
        float zIncrease = Mathf.Sin(percentageCompleted * Mathf.PI) * 0.3f;
        // Create a new Vector2 to store the result
        Vector2 fragmentSpritePosition = new Vector2(0, zIncrease);
        // Apply the Vector2 to the Fragment Sprite child component
        transform.GetChild(0).GetComponent<Transform>().transform.localPosition = fragmentSpritePosition;

    }

    private void SelfDestruct()
    {
        Destroy(gameObject);
    }
}
