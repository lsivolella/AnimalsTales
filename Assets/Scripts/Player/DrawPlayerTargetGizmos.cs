using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPlayerTargetGizmos : MonoBehaviour
{
    PlayerCombatController playerCombatController;

    private void Start()
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        playerCombatController = GetComponentInParent<PlayerCombatController>();
        Gizmos.DrawWireSphere(playerCombatController.scratchLeftPivot.position, playerCombatController.attackRange);
        Gizmos.DrawWireSphere(playerCombatController.scratchUpPivot.position, playerCombatController.attackRange);
        Gizmos.DrawWireSphere(playerCombatController.scratchRightPivot.position, playerCombatController.attackRange);
        Gizmos.DrawWireSphere(playerCombatController.scratchDownPivot.position, playerCombatController.attackRange);
    }
}
