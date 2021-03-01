using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombAnimationController : MonoBehaviour
{
    BombController bombController;
    SpriteRenderer spriteRenderer;
    Animator myAnimator;

    private void Awake()
    {
        GetAccessToComponents();
    }

    private void GetAccessToComponents()
    {
        bombController = GetComponentInParent<BombController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        myAnimator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        
    }

    public void PrepareDetonation()
    {
        myAnimator.SetBool("holdDetonation", false);
        myAnimator.SetTrigger("prepareDetonation");
    }

    public void HoldDetonation()
    {
        myAnimator.SetBool("holdDetonation", true);
    }

    private void Detonate()
    {
        // Player can safelly hit back all bombs thrown in current animation speed
        // If detonation occurs faster, the challege could be greater
        // And if bombs reset their detonation count mid air, then the player will have a greater challenge damaging the enemy
        // Because the enemy can move away from it

        // Hide Shadow Sprite
        bombController.transform.GetChild(1).gameObject.SetActive(false);
        // Initiate detonation animation
        myAnimator.SetTrigger("detonate");
        // Call Bomb damage funcrtion
        bombController.ApplyDamage();
    }

    private void CallSelfDestruct()
    {
        bombController.SelfDestruct();
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 colliderOffset = GetComponentInParent<CapsuleCollider2D>().offset;
        Vector2 centeredPivotPoint = new Vector2(transform.position.x + colliderOffset.x, transform.position.y + colliderOffset.y);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(centeredPivotPoint, GetComponentInParent<BombController>().explosionRadius);
    }

    public void InstantDetonation()
    {
        myAnimator.SetTrigger("instantDetonation");
    }

}
