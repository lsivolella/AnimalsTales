using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombSpriteAnimation : MonoBehaviour
{
    BombController bombController;
    Animator myAnimator;

    private void Awake()
    {
        GetAccessToComponents();
    }

    private void GetAccessToComponents()
    {
        bombController = GetComponentInParent<BombController>();
        myAnimator = GetComponentInChildren<Animator>();
    }

    private void Detonate()
    {
        // Hold on the detonation if player hit it back?
        // Some bombs are just exploding mid-air
        // Also, player can safelly hit back all bombs thrown in current speed
        // If detonation occurs faster, the challege could be greater
        // And if bombs reset their detonation count mid air, then the player will have a greater challenge damaging the enemy
        // Because the enemy can move away from it
        bombController.SelfDestruct();
        Debug.Log("BOOM");

        // TODO: detonation animation (fire expeling)
        // TODO: detonation must trigger colliders action (they can only hit once per bomb)
        // TODO: the bomb sprite must be hidden, while the explosion takes over (and the colliders can act)
        // TODO: after that, all the bomb object can be destroyed
        // TODO: using the invoke method might do the job
        // Invoke("BombDestruction", explosionDuration)
    }
}
