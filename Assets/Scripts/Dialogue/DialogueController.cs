using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueController : MonoBehaviour
{
    [SerializeField] PlayerMovementController playerMovementController;
    [SerializeField] float letterTypeSpeed = 1.0f;

    private TextMeshProUGUI dialogueHeader;
    private TextMeshProUGUI dialogueBody;
    private GameObject interlocutor;

    // Queue is a type of list that operates by FIFO (First In First Out)
    private Queue<string> sentences;
    private bool isConversationActive;
    public bool IsConversationActive { get { return isConversationActive; } 
                            private set { isConversationActive = false; } }

    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue(DialogueContainer dialogue, TextMeshProUGUI header, TextMeshProUGUI body, GameObject interlocutor)
    {
        sentences.Clear();
        isConversationActive = true;
        dialogueHeader = header;
        dialogueHeader.text = dialogue.name;
        dialogueBody = body;
        this.interlocutor = interlocutor;

        foreach (string sentence in dialogue.sentences)
        {
            // Add all the sentences in the dialogue to the queue
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence (string sentence)
    {
        dialogueBody.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueBody.text += letter;
            yield return new WaitForSeconds(letterTypeSpeed);
        }
    }

    private void EndDialogue()
    {
        isConversationActive = false;
        playerMovementController.FinishInteractionWithNpcs(interlocutor);
    }

    private void ClearDialogueBox()
    {
        dialogueHeader.text = null;
        dialogueBody.text = null;
    }

}
