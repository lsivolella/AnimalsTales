using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YarnBallController : MonoBehaviour
{
    [SerializeField] float ballSpeed = 1f;
    [SerializeField] float rotationSpeed = 1f;

    // Cached References
    Rigidbody2D myRigidbody;

    private Vector2 movementDirection = new Vector2(1, 0);     // Positive is Right, Negative is Left
    //private Vector3 rotationDirection = new Vector3(0, 0, -1);       // Positive is Counter Clockwise, Negative is Clockwise
    private float rotationDirection = -1;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        ProcessMovementRotation();
    }

    private void ProcessMovementRotation()
    {
        myRigidbody.MovePosition(myRigidbody.position + movementDirection * ballSpeed * Time.fixedDeltaTime);
        myRigidbody.MoveRotation(myRigidbody.rotation + rotationDirection * rotationSpeed * Time.fixedDeltaTime);
        //transform.Rotate(rotationDirection * rotationSpeed * Time.fixedDeltaTime, Space.Self);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ChangeDirectionOfMovement(collision);

        if (collision.gameObject.CompareTag("Player"))
        {
            GetComponent<CircleCollider2D>().enabled = false;
        }
    }

    private void ChangeDirectionOfMovement(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Objects"))
        {
            movementDirection *= -1;
            rotationDirection *= -1;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GetComponent<CircleCollider2D>().enabled = true;
        }
    }
}
