using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [SerializeField] int arrowDamage = -1;    // Damage dealt is always negative (positive for Healing)
    [SerializeField] float arrowSpeed = 1f;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.down * arrowSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealthController>().StandardDamageRoutine(arrowDamage, 
                collision.transform.position, collision.otherCollider.transform.position);
            Destroy(gameObject);
        }
    }
}
