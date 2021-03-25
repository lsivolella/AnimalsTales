using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static GameMaster;

public class NpcDialogueTrigger : DialogueTrigger
{
    GameMaster gameMaster;

    protected override void GetAccessToComponents()
    {
        base.GetAccessToComponents();
        gameMaster = GameMaster.instance;
    }

    protected override void CallStartDialogue()
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
}
