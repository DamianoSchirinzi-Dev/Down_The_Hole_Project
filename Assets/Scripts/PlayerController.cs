using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject _managers;
    private Rigidbody rb;
    public Camera playerCamera;

    public float movementSpeed = 5f;
    public float sprintSpeed = 10f;
    private float movementOriginalSpeed = 5f;
    public float jumpForce = 5f;
    public float gravity;
    public float rotationSpeed = 5f;

    public float fallTime = 0f;
    public int fallShields = 3;

    public bool isSprinting { get; set; } = false;
    public bool isJumping = false;
    public bool isPaused = false;
    public bool isDead = false;
    public bool canInteract = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        movementOriginalSpeed = movementSpeed;
    }

    private void FixedUpdate()
    {
       
    }

    private void Update()
    {
        if (!isPaused || !isDead)
        {
            if (isSprinting)
            {
                movementSpeed = sprintSpeed;
            } else
            {
                movementSpeed = movementOriginalSpeed;
            }

            if(isJumping)
            {
                fallTime += Time.deltaTime;
            }
        }
    }

    public void Move(Vector2 userInput)
    {
        // Calculate movement direction relative to the camera
        Vector3 cameraForward = playerCamera.transform.forward;
        Vector3 cameraRight = playerCamera.transform.right;
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Calculate movement vector
        Vector3 movement = cameraForward * userInput.y + cameraRight * userInput.x;
        movement.Normalize();

        var additionGravity = gravity * Time.deltaTime;

        // Apply movement to the rigidbody
        rb.velocity = new Vector3(movement.x * movementSpeed, rb.velocity.y - additionGravity, movement.z * movementSpeed);

        if (movement != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    public void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isJumping = true;
    }

    public void Interact()
    {
        Debug.Log("The player just interacted with something!");
    }

    private void TakeDamage(int _damageTaken)
    {
        if(fallShields > 0)
        {
            Debug.Log("The player took damage and lost a shield");
            if(_damageTaken > fallShields)
            {
                fallShields = 0;

                Death();
            }
            else
            {
                fallShields -= _damageTaken;
            }
        }
        else
        {
            Death();
        }
    }

    private void Death()
    {
        Debug.Log("The player died.");

        isPaused = true;
        isDead = true;
        StartCoroutine(Reset(new Vector3(0, 5, 0)));
    }

    private IEnumerator Reset(Vector3 _resetPos)
    {
        Debug.Log("Resetting.");

        transform.position = _resetPos;

        yield return new WaitForSeconds(1);

        isPaused = false;
        isDead= false;
    }

    public void PausePlayerMovement()
    {
        isPaused = true;
    }

    public void UnpausePlayerMovement()
    {
        isPaused = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        // Reset jump state on collision with the ground
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Interact-able"))
        {
            if(fallTime > 8f)
            {
                TakeDamage(4);
            }
            else if( fallTime > 6f) 
            {
                TakeDamage(3);
            } else if( fallTime > 4f)
            {
                TakeDamage(2);
            } else if (fallTime > 2f)
            {
                TakeDamage(1);
            }

            isJumping = false;
            fallTime = 0f;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = true;
            fallTime = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interact-able"))
        {
            canInteract = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interact-able"))
        {
            canInteract = false;
        }
    }
}
