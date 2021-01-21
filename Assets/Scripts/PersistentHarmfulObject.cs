using UnityEngine;

public class PersistentHarmfulObject : MonoBehaviour
{
    [Header("Passive Damage")]
    [SerializeField] int passiveDamage = -1;

    public int PassiveDamage { get { return passiveDamage; } }

    /*
    private void OnTriggerStay2D(Collider2D other)
    {
        PlayerHealthController playerHealthController = other.GetComponent<PlayerHealthController>();
        if (playerHealthController)
        {
            playerHealthController.ChangeHealth(healthAmount);
        }
        // Knock player back 
    }
    */

    /*
    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerHealthController playerHealthController = collision.gameObject.GetComponent<PlayerHealthController>();
        PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
        if (playerHealthController && playerController)
        {
            playerHealthController.ChangeHealth(healthAmount);
            playerController.PlayKnockbackRoutine(collision.gameObject.transform.position, collision.otherCollider.transform.position);

        }
    }
    */
}
