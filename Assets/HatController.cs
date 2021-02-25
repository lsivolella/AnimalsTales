using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatController : MonoBehaviour
{
    [SerializeField] Vector2 yPadding = new Vector2(0, -2f);
    [SerializeField] float displacementDuration = 1f;
    [SerializeField] GameObject hatSprite;

    // Cached References
    Collider2D myCollider;

    //Cached Variables
    private Vector2 initialPosition;
    private Vector2 finalPosition;
    private float displacementTimer;
    private float displacementCompleted;

    private Vector2 hatSpriteInitialPosition;


    private void Awake()
    {
        GetAccessToComponents();
    }

    private void GetAccessToComponents()
    {
        myCollider = GetComponent<Collider2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        myCollider.enabled = false;
        initialPosition = transform.position;
        finalPosition = (Vector2)transform.position + yPadding;
        displacementTimer = 0;
        hatSpriteInitialPosition = hatSprite.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (displacementTimer < displacementDuration)
        {
            displacementTimer += Time.deltaTime;
            displacementCompleted = displacementTimer / displacementDuration;
            Vector2 newPosition = Vector2.Lerp(initialPosition, finalPosition, displacementCompleted);
            transform.position = newPosition;



            Vector2 newHatSpritePosition = Vector2.Lerp(hatSpriteInitialPosition, Vector2.zero, displacementCompleted);

            hatSprite.transform.localPosition = newHatSpritePosition;
        }
        else if (displacementTimer >= displacementDuration)
        {
            myCollider.enabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HatPickUp();
    }

    private void HatPickUp()
    {
        // Add Hat to "Inventory"?
        gameObject.SetActive(false);
    }
}
