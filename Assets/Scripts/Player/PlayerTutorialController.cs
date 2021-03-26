using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTutorialController : MonoBehaviour
{
    // Serialized Parameters
    [Header("Movement")]
    [SerializeField]
    int movementSpeed = 1;
    [SerializeField]
    float freezeInputCooldown = 1f;
    [Header("Attack")]
    [SerializeField]
    float scratchCooldown = 1f;
    [Header("Dialogue Raycast")]
    [SerializeField] float raycastRange = 1f;
    [SerializeField] DialogueController dialogueController;

    // Cached References
    Rigidbody2D myRigidbody;
    Animator myAnimator;

    // Cached Movement Variables
    private float horizontalInput;
    private float verticalInput;
    private Vector2 movement;
    private bool isInputFrozen = false;

    // Cached Animation Variables
    private Vector2 lookDirection = new Vector2(0, -1);

    // Cached Attack Variables
    private float scratchCooldownTimer;

    // Cached Raycast Variables
    private bool isEngagedInConversation;
    RaycastHit2D npcRaycastHit;
    DialogueTrigger dialogueTrigger;

    private void Awake()
    {
        GetAccessToComponents();
    }

    private void GetAccessToComponents()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        GetMovementInput();
        ProcessPlayerAnimations();
        GetAttackInput();
        InteractWithNpcs();
    }

    private void GetMovementInput()
    {
        if (!isInputFrozen)
        {
            movement = Vector2.zero;
            // Get player input in both Horizontal and Vertical axes
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");
            // Save player input in a Vector2
            movement = new Vector2(horizontalInput, verticalInput); 
        }
    }

    private void ProcessPlayerAnimations()
    {
        if (!isInputFrozen)
        {
            if (!Mathf.Approximately(movement.x, 0.0f) || !Mathf.Approximately(movement.y, 0.0f))
            {
                lookDirection.Set(movement.x, movement.y);
                lookDirection.Normalize();
            }
            myAnimator.SetFloat("Look X", lookDirection.x);
            myAnimator.SetFloat("Look Y", lookDirection.y);
            myAnimator.SetFloat("Speed", movement.magnitude); 
        }
    }

    private void GetAttackInput()
    {
        if (scratchCooldownTimer <= 0)
        {
            if (Input.GetKeyDown(KeyCode.O) && !isInputFrozen)
            {
                // Reset any velocity residual values. They may occur when the Player colliders with another body.
                myRigidbody.velocity = Vector2.zero;
                CallFreezeInputCoroutine();
                ScratchAttack();
                scratchCooldownTimer = scratchCooldown;
            }
        }
        else
        {
            scratchCooldownTimer -= Time.deltaTime;
        }
    }

    private void ScratchAttack()
    {
        myAnimator.SetTrigger("doAttack");
    }

    private void InteractWithNpcs()
    {
        if (Input.GetKeyDown(KeyCode.P) && !dialogueController.IsConversationActive)
        {
            npcRaycastHit = CastDialogueRaycast();;

            if (npcRaycastHit.collider != null)
            {
                myAnimator.SetFloat("Speed", 0);
                isEngagedInConversation = true;
                dialogueTrigger = npcRaycastHit.collider.gameObject.
                    GetComponent<DialogueTrigger>();
                dialogueTrigger.TriggerDialogue();

            }
        }
        else if (Input.GetKeyDown(KeyCode.P) && dialogueController.IsConversationActive)
        {
            dialogueController.DisplayNextSentence();
        }
    }

    private RaycastHit2D CastDialogueRaycast()
    {
        RaycastHit2D _raycastHit2D = Physics2D.Raycast(myRigidbody.position + Vector2.up * 0.2f,
           lookDirection, raycastRange, LayerMask.GetMask("NPC"));
        return _raycastHit2D;
    }

    public void FinishInteractionWithNpcs(GameObject interlocutor)
    {
        isEngagedInConversation = false;
        dialogueTrigger.HideDialogue();
    }


    private void FixedUpdate()
    {
        ProcessMovementInput();
    }

    private void ProcessMovementInput()
    {
        if (!isInputFrozen)
        {
            // Save player position in a Vector2
            Vector2 position = myRigidbody.position;
            // Create a new Vector2 with the player current position and the normalized player movement input (normalized to prevent faster diagonal movement)
            position += movement.normalized * movementSpeed * Time.fixedDeltaTime;
            // Pass the final Vector2 to the Rigidbody component
            myRigidbody.MovePosition(position); 
        }
    }

    public void CallFreezeInputCoroutine()
    {
        StartCoroutine("FreezeInput");
    }

    IEnumerator FreezeInput()
    {
        // Freeze input
        isInputFrozen = true;
        // Time.timeScale = 0.1f;

        yield return new WaitForSeconds(freezeInputCooldown);

        // Unfreeze input
        isInputFrozen = false;
        Time.timeScale = 1f;
    }
}
