using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NpcDialogueTrigger : MonoBehaviour
{
    [SerializeField] GameObject dialogueBoxCanvas;
    [SerializeField] TextMeshProUGUI headerText;
    [SerializeField] TextMeshProUGUI bodyText;
    [SerializeField] Animator dialogueAnimator;
    [SerializeField] DialogueContainer dialogueContainer;


    DialogueController dialogueController;

    private void Awake()
    {
        GetAccessToComponents();
        HideDialogue();
    }
    private void GetAccessToComponents()
    {
        dialogueController = FindObjectOfType<DialogueController>();
    }

    public void TriggerDialogue()
    {
        headerText.text = null;
        bodyText.text = null;
        Invoke("CallStartDialogue", 0.3f);
    }

    private void CallStartDialogue()
    {
        dialogueController.StartDialogue(dialogueContainer, headerText, bodyText);
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
