using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    // Serializes Parameters
    [Header("Movement")]
    [SerializeField] int movementSpeed = 1;
    [SerializeField] float freezeInputCooldown = 1f;
    [Header("Raycast")]
    [SerializeField] float raycastRange = 1f;
    [SerializeField] DialogueController dialogueController;

    // Cached References
    Rigidbody2D myRigidbody2D;
    Animator myAnimator;

    // Cached Movement Variables
    private float horizontalInput;
    private float verticalInput;
    private Vector2 movement;
    private bool isInputFrozen;
    public bool IsInputFrozen { get { return isInputFrozen; } set { isInputFrozen = value; } }
    public float GetFreezeInputCooldown {  get { return freezeInputCooldown; } }

    // Cached Animation Variables
    private Vector2 lookDirection = new Vector2(0, -1);
    public Vector2 GetLookDirection { get { return lookDirection; } }

    // Cached Raycast Variables
    private bool isEngagedInConversation;
    public bool GetConversationStatus { get { return isEngagedInConversation; } }
    RaycastHit2D npcRaycastHit2D;
    NpcDialogueTrigger npcDialogueTrigger;

    private void Awake()
    {
        GetAccessToComponents();
        SetDefaultBoolStates();
    }

    private void GetAccessToComponents()
    {
        myRigidbody2D = GetComponent<Rigidbody2D>();
        myAnimator = GetComponentInChildren<Animator>();
        //dialogueController = GetComponent<DialogueController>();
    }

    private void SetDefaultBoolStates()
    {
        isInputFrozen = false;
        isEngagedInConversation = false;
    }

    void Update()
    {
        if (!isInputFrozen && !isEngagedInConversation)
        {
            GetMovementInput();
            ProcessPlayerAnimations();
        }
        InteractWithNpcs();
    }

    private void GetMovementInput()
    {
        // Get player input in both Horizontal and Vertical axes
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        // Save player input in a Vector2
        movement = new Vector2(horizontalInput, verticalInput);
    }

    private void ProcessPlayerAnimations()
    {
        /*
        The function Approximately compares two float points values and return true if they are similar.
        Because float points are imprecise, float numbers comparison might not return the desired outcome.
        Thus, using the Approximately function to compare a variable to 0.0f is an accurate way to check 
        if the variable's value is low enough to be considered zero.
        The code bellow checks if movement in the horizontal OR vertical axis are NOT equal to zero.
        If they have any value other than 0, then the code will run the condition.
        */
        if (!Mathf.Approximately(movement.x, 0.0f) || !Mathf.Approximately(movement.y, 0.0f))
        {
            lookDirection.Set(movement.x, movement.y);
            lookDirection.Normalize();
        }
        myAnimator.SetFloat("Look X", lookDirection.x);
        myAnimator.SetFloat("Look Y", lookDirection.y);
        myAnimator.SetFloat("Speed", movement.magnitude);
    }

    private void InteractWithNpcs()
    {
        if (Input.GetKeyDown(KeyCode.Comma) && !dialogueController.IsConversationActive)
        {
            npcRaycastHit2D = CastDialogueRaycast();

            if (npcRaycastHit2D.collider != null)
            {
                isEngagedInConversation = true;
                npcDialogueTrigger = npcRaycastHit2D.collider.gameObject.
                    GetComponent<NpcDialogueTrigger>();
                npcDialogueTrigger.DisplayDialogue();
                npcDialogueTrigger.TriggerDialogue();
                
            }
        }
        else if (Input.GetKeyDown(KeyCode.Comma) && dialogueController.IsConversationActive)
        {
            dialogueController.DisplayNextSentence();
        }
    }

    private RaycastHit2D CastDialogueRaycast()
    {
         RaycastHit2D _raycastHit2D = Physics2D.Raycast(myRigidbody2D.position + Vector2.up * 0.2f,
            lookDirection, raycastRange, LayerMask.GetMask("NPC"));
        return _raycastHit2D;
    }

    public void FinishInteractionWithNpcs()
    {
        isEngagedInConversation = false;
        npcDialogueTrigger.HideDialogue();
    }

    private void FixedUpdate()
    {
        // TODO: check if this if condition is really needed
        if (!isInputFrozen)
        {
            ProcessMovementInput();
        }
        /*
        myRigidbody2D = GetComponent<Rigidbody2D>();
        myRigidbody2D.MovePosition(myRigidbody2D.position + movementSpeed * Vector2.down * Time.fixedDeltaTime);
        */
    }

    private void ProcessMovementInput()
    {
        // Save player position in a Vector2
        Vector2 position = myRigidbody2D.position;
        // Create a new Vector2 with the player current position and the player movement input
        position += movement * movementSpeed * Time.fixedDeltaTime;
        // Pass the final Vector2 to the Rigidbody component
        myRigidbody2D.MovePosition(position);
    }

    public void CallFreezeInputCoroutine()
    {
        StartCoroutine("FreezeInput");
    }

    IEnumerator FreezeInput()
    {
        // Freeze input
        isInputFrozen = true;

        yield return new WaitForSeconds(freezeInputCooldown);

        // Unfreeze input
        isInputFrozen = false;
    }
}
