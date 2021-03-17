using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameMaster;

public class NpcAnimationController : MonoBehaviour
{
    Animator myAnimator;
    GameMaster gameMaster;

    private void Start()
    {
        GetAccessToComponents();
        CheckHatStatus();
    }

    private void GetAccessToComponents()
    {
        myAnimator = GetComponent<Animator>();
        gameMaster = GameMaster.instance;
    }

    private void CheckHatStatus()
    {
        if (gameMaster.hatStatus == HatStatus.Delivered)
        {
            myAnimator.SetBool("hatDelivered", true);
            Debug.Log(gameMaster.hatStatus);
        }
        else
            myAnimator.SetBool("hatDelivered", false);
    }

    public void DoHatDance()
    {
        myAnimator.SetTrigger("hatDance");
        myAnimator.SetBool("hatDelivered", true);
    }

    private void Update()
    {
        DefineSpritePosition();
    }

    private void DefineSpritePosition()
    {
        Vector2 relativePositionToPlayer = (Vector2)transform.position - gameMaster.PlayerPosition;

        if (relativePositionToPlayer.y > 0)          // Player is to the bellow
        {
            myAnimator.SetFloat("Look X", 0.0f);
            myAnimator.SetFloat("Look Y", -1.0f);
        }
        else if (relativePositionToPlayer.x < 0)     // Player is to the right
        {
            myAnimator.SetFloat("Look X", 1.0f);
            myAnimator.SetFloat("Look Y", 0.0f);
        }
        else if (relativePositionToPlayer.x > 0)     // Player is to the left
        {
            myAnimator.SetFloat("Look X", -1.0f);
            myAnimator.SetFloat("Look Y", 0.0f);
        }
    }
}
