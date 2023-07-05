using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject _managers;
    private TimeManager _timeManager;
    private Rigidbody rb;
    public Camera playerCamera;

    private float horizontalInput;
    private float verticalInput;

    public float movementSpeed = 5f;
    public float sprintSpeed = 10f;
    private float movementOriginalSpeed = 5f;
    public float jumpForce = 5f;
    public float gravity;
    public float rotationSpeed = 5f;

    public float fallTime = 0f;
    public int fallShields = 3;

    public bool isJumping = false;
    private bool isPaused = false;
    private bool isDead = false;
    private bool canInteract = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        _timeManager = _managers.GetComponent<TimeManager>();

        movementOriginalSpeed = movementSpeed;
    }

    private void FixedUpdate()
    {
        if (!isPaused || !isDead)
        {
            Move();
        }
    }

    private void Update()
    {
        if (!isPaused || !isDead)
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");

            if (Input.GetButton("Sprint"))
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

            // Jump input
            if (Input.GetButtonDown("Jump") && !isJumping)
            {
                Jump();
            }

            if (Input.GetButton("Fire1"))
            {
                _timeManager.DoSlowMoTime();
            }
            else
            {
                _timeManager.StopSlowMoTime();
            }

            if (Input.GetButtonDown("Interact") && canInteract == true)
            {
                Interact();
            }
        }
    }

    private void Move()
    {
        // Calculate movement direction relative to the camera
        Vector3 cameraForward = playerCamera.transform.forward;
        Vector3 cameraRight = playerCamera.transform.right;
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Calculate movement vector
        Vector3 movement = cameraForward * verticalInput + cameraRight * horizontalInput;
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

    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isJumping = true;
    }

    private void Interact()
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
