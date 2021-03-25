using TMPro;
using UnityEngine;
using static GameMaster;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField]
    GameObject dialogueBoxCanvas;
    [SerializeField] 
    protected TextMeshProUGUI headerText;
    [SerializeField] 
    protected TextMeshProUGUI bodyText;
    [SerializeField]
    protected Animator dialogueAnimator;
    [SerializeField]
    protected DialogueContainer[] dialogueContainer;

    protected DialogueController dialogueController;

    protected void Awake()
    {
        GetAccessToComponents();
        HideDialogue();
    }
    protected virtual void GetAccessToComponents()
    {
        dialogueController = FindObjectOfType<DialogueController>();
    }

    public void TriggerDialogue()
    {
        headerText.text = null;
        bodyText.text = null;
        DisplayDialogue();
        Invoke("CallStartDialogue", 0.3f);

    }

    protected virtual void CallStartDialogue()
    {
        dialogueController.StartDialogue(dialogueContainer[0], headerText, bodyText, this.gameObject);
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
