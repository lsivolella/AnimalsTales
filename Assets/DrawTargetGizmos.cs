using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawTargetGizmos : MonoBehaviour
{
    [SerializeField] PlayerCombatController playerCombatController1;
    [SerializeField] BombController bombController;
    PlayerCombatController playerCombatController;

    /*
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        playerCombatController = GetComponentInParent<PlayerCombatController>();
        Gizmos.DrawWireSphere(playerCombatController.scratchLeftPivot.position, playerCombatController.attackRange);
        Gizmos.DrawWireSphere(playerCombatController.scratchUpPivot.position, playerCombatController.attackRange);
        Gizmos.DrawWireSphere(playerCombatController.scratchRightPivot.position, playerCombatController.attackRange);
        Gizmos.DrawWireSphere(playerCombatController.scratchDownPivot.position, playerCombatController.attackRange);
    }
    */

    private void OnDrawGizmosSelected()
    {
        if (playerCombatController1 != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(playerCombatController1.scratchLeftPivot.position, playerCombatController1.attackRange);
            Gizmos.DrawWireSphere(playerCombatController1.scratchUpPivot.position, playerCombatController1.attackRange);
            Gizmos.DrawWireSphere(playerCombatController1.scratchRightPivot.position, playerCombatController1.attackRange);
            Gizmos.DrawWireSphere(playerCombatController1.scratchDownPivot.position, playerCombatController1.attackRange);
        }
        else if (bombController != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(bombController.colliderLeftPivot.position, bombController.colliderRange);
            Gizmos.DrawWireSphere(bombController.colliderUpPivot.position, bombController.colliderRange);
            Gizmos.DrawWireSphere(bombController.colliderRightPivot.position, bombController.colliderRange);
            Gizmos.DrawWireSphere(bombController.colliderDownPivot.position, bombController.colliderRange);
        }
    }
}
