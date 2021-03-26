using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTrigger : MonoBehaviour
{
    [SerializeField] GameObject boss;
    [SerializeField] List<GameObject> minions;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ActivateEnemies(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        DeactivateEnemies(collision);
    }

    private void ActivateEnemies(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            foreach (GameObject enemy in minions)
            {
                var minionCombatScript = enemy.GetComponent<ReddishMinionCombatController>();
                if (minionCombatScript != null)
                    minionCombatScript.OnHold = false;
            }

            var bossCombatScript = boss.GetComponent<ReddishCatCombatController>();
            var bossMovementScript = boss.GetComponent<ReddishCatMovementController>();
            if (bossCombatScript != null && bossMovementScript != null)
            {
                bossCombatScript.OnHold = false;
                bossMovementScript.GenerateRandomMovementVector();
            }
        }
    }

    private void DeactivateEnemies(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            foreach (GameObject enemy in minions)
            {
                var minionCombatScript = enemy.GetComponent<ReddishMinionCombatController>();
                if (minionCombatScript != null)
                    minionCombatScript.OnHold = true;
            }

            var bossCombatScript = boss.GetComponent<ReddishCatCombatController>();
            var bossMovementScript = boss.GetComponent<ReddishCatMovementController>();
            if (bossCombatScript != null && bossMovementScript != null)
            {
                bossCombatScript.OnHold = true;
                bossMovementScript.GenerateRandomMovementVector();
            }
        }
    }
}
