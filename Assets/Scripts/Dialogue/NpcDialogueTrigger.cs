using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static GameMaster;

public class NpcDialogueTrigger : MonoBehaviour
{
    [SerializeField] GameObject dialogueBoxCanvas;
    [SerializeField] TextMeshProUGUI headerText;
    [SerializeField] TextMeshProUGUI bodyText;
    [SerializeField] Animator dialogueAnimator;
    [SerializeField] DialogueContainer[] dialogueContainer;

    DialogueController dialogueController;
    GameMaster gameMaster;


    private void Awake()
    {
        GetAccessToComponents();
        HideDialogue();
    }
    private void GetAccessToComponents()
    {
        dialogueController = FindObjectOfType<DialogueController>();
        gameMaster = GameMaster.instance;
    }

    private void Start()
    {
        SetStartVariables();
    }

    private void SetStartVariables()
    {
        // Access the GameMaster and check the Hat status
        // If not in inventory: state = lostHat
        // If in inventory: transition to foundHat during conversation. 
        // This information must be passed to the GameMaster so that it will be persistent.
        // The status change will be triggered by the player when he talks to the orange cat with the hat
        // Remember to keep the Orange cat methods in thids script
    }

    public void TriggerDialogue()
    {
        headerText.text = null;
        bodyText.text = null;
        DisplayDialogue();
        Invoke("CallStartDialogue", 0.3f);

    }

    private void CallStartDialogue()
    {
        int dialogueToDisplay = 0;
        if (gameMaster.hatStatus == HatStatus.NotDropped || gameMaster.hatStatus == HatStatus.AtFlor)
            dialogueToDisplay = 0;
        else if (gameMaster.hatStatus == HatStatus.Inventory)
            dialogueToDisplay = 1;
        else if (gameMaster.hatStatus == HatStatus.Delivered)
            dialogueToDisplay = 2;

        dialogueController.StartDialogue(dialogueContainer[dialogueToDisplay], headerText, bodyText, this.gameObject);
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayDialogue()
    {
        dialogueBoxCanvas.SetActive(true);
        dialogueAnimator.SetBool("isOpen", true);
    }

    public void HideDialogue()
    {
        dialogueBoxCanvas.SetActive(false);
        dialogueAnimator.SetBool("isOpen", false);
    }


}
