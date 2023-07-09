using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject _managers;
    private Rigidbody rb;
    public Camera playerCamera;

    public Transform groundCheck;
    public Transform ledgeCheckRayTransform;
    public float rayCheckDist;

    public float movementSpeed = 5f;
    public float sprintSpeed = 10f;
    private float movementOriginalSpeed = 5f;
    public float jumpForce = 5f;
    public float gravity;
    public float lowGravity;
    private float originalGravity;
    public float rotationSpeed = 5f;
    public float groundCheckDistance = .5f;

    public float fallTime = 0f;
    public int fallShields = 3;

    public bool isSprinting { get; set; } = false;
    public bool isGrounded = false;
    public bool isLedgeHanging = false;
    public bool isPaused = false;
    public bool isLowGravityBuffActive = false;
    public bool isDead = false;
    public bool canInteract = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        movementOriginalSpeed = movementSpeed;
        originalGravity = gravity;
    }

    private void Update()
    {
        if (!isPaused || !isDead)
        {
            isGrounded = CheckGround();

            if (isSprinting)
            {
                movementSpeed = sprintSpeed;
            } else
            {
                movementSpeed = movementOriginalSpeed;
            }

            if (!isGrounded && !isLedgeHanging)
            {
                fallTime += Time.deltaTime;
                DetectLedge();
            }

            if (isLowGravityBuffActive)
            {
                gravity = lowGravity;
            }
            else
            {
                gravity = originalGravity;
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

    private void DetectLedge()
    {
        Ray ledgeCheckRay = new Ray(ledgeCheckRayTransform.position, ledgeCheckRayTransform.forward);
        Vector3 standingPointBonusVector = (transform.forward * 0.6f) + (Vector3.up * 0.5f);

        RaycastHit ledgeHit;

        if (Physics.Raycast(ledgeCheckRay, out ledgeHit, rayCheckDist))
        {
            if (ledgeHit.collider.CompareTag("Ledge"))
            {
                Debug.Log("Ledge found!");

                ClimbLedge(ledgeHit.point + standingPointBonusVector);
            }
        }

        Debug.DrawRay(ledgeCheckRay.origin, ledgeCheckRay.direction * rayCheckDist, Color.red);
    }

    private void ClimbLedge(Vector3 newPos)
    {
        isPaused = true;
        isLedgeHanging = true;
        rb.isKinematic = true;
        fallTime = 0;

        StartCoroutine(MovePlayerToLedge(newPos));
    }

    private IEnumerator MovePlayerToLedge(Vector3 _newPos)
    {
        yield return new WaitForSeconds(2);
        transform.position = _newPos;
        yield return new WaitForSeconds(1);
        rb.isKinematic = false;
        isLedgeHanging = false;
        isPaused = false;
    }

    private bool CheckGround()
    {
       
        // Perform a raycast or a raycast-like check to detect the ground
        RaycastHit hit;
        if (Physics.Raycast(groundCheck.position, Vector3.down, out hit, groundCheckDistance))
        {
            // The object is grounded
            if (hit.collider.CompareTag("Ground"))
            {
                if (fallTime > 8f)
                {
                    TakeDamage(4);
                }
                else if (fallTime > 6f)
                {
                    TakeDamage(3);
                }
                else if (fallTime > 4f)
                {
                    TakeDamage(2);
                }
                else if (fallTime > 2f)
                {
                    TakeDamage(1);
                }

                fallTime = 0f;             
                return true;
            }
            else
            {
                return false;
            }
        }

        // The object is not grounded
        return false;
    }

    public void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
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
            }
            else
            {
                fallShields -= _damageTaken;
            }
        }
        else
        {
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
