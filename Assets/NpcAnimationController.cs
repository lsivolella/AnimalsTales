using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcAnimationController : MonoBehaviour
{
    Animator myAnimator;
    GameMaster gameMaster;

    private void Start()
    {
        GetAccessToComponents();
        SetSpriteToDownPosition();
    }

    private void GetAccessToComponents()
    {
        myAnimator = GetComponent<Animator>();
        gameMaster = GameMaster.instance;
    }

    private void SetSpriteToDownPosition()
    {
        myAnimator.SetFloat("Look X", 0.0f);
        myAnimator.SetFloat("Look Y", -1.0f);
    }

    public void DoHatDance()
    {
        myAnimator.SetTrigger("hatDance");
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
