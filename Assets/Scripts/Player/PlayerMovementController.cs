using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameMaster;

public class PlayerMovementController : MonoBehaviour
{
    // Serialized Parameters
    [Header("Movement")]
    [SerializeField] int movementSpeed = 1;
    [SerializeField] float freezeInputCooldown = 1f;
    [Header("Raycast")]
    [SerializeField] float raycastRange = 1f;
    [SerializeField] DialogueController dialogueController;
    [Header("Inventory")]
    [SerializeField] InventoryController inventoryController;

    // Cached References
    Rigidbody2D myRigidbody;
    Animator myAnimator;
    GameMaster gameMaster;

    // Cached Movement Variables
    private float horizontalInput;
    private float verticalInput;
    private Vector2 movement;
    private bool isInputFrozen;
    public bool IsInputFrozen { get { return isInputFrozen; } set { isInputFrozen = value; } }
    public float FreezeInputCooldown {  get { return freezeInputCooldown; } }

    // Cached Animation Variables
    private Vector2 lookDirection = new Vector2(0, -1);
    public Vector2 GetLookDirection { get { return lookDirection; } }

    // Cached Raycast Variables
    private bool isEngagedInConversation;
    public bool IsEngagedInConversation { get { return isEngagedInConversation; } }
    RaycastHit2D npcRaycastHit;
    DialogueTrigger dialogueTrigger;

    private void Awake()
    {
        GetAccessToComponents();
        SetDefaultBoolStates();
    }

    private void GetAccessToComponents()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponentInChildren<Animator>();
        gameMaster = GameMaster.instance;
    }

    private void SetDefaultBoolStates()
    {
        isInputFrozen = false;
        isEngagedInConversation = false;
    }

    void Update()
    {
        if (!isInputFrozen && !isEngagedInConversation && !SceneController.instance.CinematicsOn)
        {
            GetMovementInput();
            ProcessPlayerAnimations();
        }
        if (SceneController.instance.CinematicsOn)
            isEngagedInConversation = true;
        InteractWithNpcs();
        PassPlayerPositionToGameMaster();
    }

    private void GetMovementInput()
    {
        movement = Vector2.zero;
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
        if (Input.GetKeyDown(KeyCode.P) && !dialogueController.IsConversationActive)
        {
            npcRaycastHit = CastDialogueRaycast();

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

    private void PassPlayerPositionToGameMaster()
    {
        gameMaster.PlayerPosition = transform.position;
    }

    private RaycastHit2D CastDialogueRaycast()
    {
         RaycastHit2D _raycastHit2D = Physics2D.Raycast(myRigidbody.position + Vector2.up * 0.2f,
            lookDirection, raycastRange, LayerMask.GetMask("NPC"));
        return _raycastHit2D;
    }

    public void FinishInteractionWithNpcs(GameObject interlocutor)
    {
        if (!SceneController.instance.CinematicsOn)
        {
            isEngagedInConversation = false;
            dialogueTrigger.HideDialogue();
            DeliverHat(interlocutor);
        }
        else if (SceneController.instance.CinematicsOn)
        {
            interlocutor.GetComponent<DialogueTrigger>().HideDialogue();
            FindObjectOfType<TimelineController>().StartTimeline();
        }
    }

    private void DeliverHat(GameObject interlocutor)
    {
        if (interlocutor.name == "NPC - Orange Cat" && gameMaster.hatStatus == HatStatus.Inventory)
        {
            interlocutor.GetComponent<NpcAnimationController>().DoHatDance();
            gameMaster.hatStatus = HatStatus.Delivered;
            inventoryController.DeactivateHat();
        }
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

    public void ResumeFromCinematics()
    {
        isEngagedInConversation = false;
    }
}
