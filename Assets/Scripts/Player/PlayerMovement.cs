using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public PlayerCamera playerCamera;
    public float rotationSpeed = 5f;
    public float moveSpeed = 5f;

    [Header("Rolling")]
    public float rollingTimeout = 5f;
    public float rollSpeed = 10f;

    private PlayerInputManager inputManager;
    private CapsuleCollider capsuleCollider;
    private Animator animator;
    private Rigidbody rb;
    
    private Vector3 targetDirection;
    private Quaternion targetRotation;
    private float accelleration = 0f;
    private float movementX;
    private float movementZ;

    private bool strafe;
    public bool Strafe
    {
        set { strafe = value; }
    }


    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();

        inputManager = GetComponent<PlayerInputManager>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        targetRotation = transform.rotation;
        targetDirection = transform.forward;
        strafe = false;
    }

    void Update()
    {
        strafe = playerCamera.FocusActive;

        MovementXZ();
    }

    private void FixedUpdate()
    {
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.fixedDeltaTime);

        rb.velocity = new Vector3(
            targetDirection.x * accelleration * moveSpeed,
            rb.velocity.y,
            targetDirection.z * accelleration * moveSpeed);
    }

    private void MovementXZ()
    {
        bool forwardPressed = inputManager.Forward > 0.01f && inputManager.canMove;
        bool backwardPressed = inputManager.Forward < -0.01f && inputManager.canMove;
        bool leftPressed = inputManager.Horizontal < -0.01f && inputManager.canMove;
        bool rightPressed = inputManager.Horizontal > 0.01f && inputManager.canMove;
        bool movementPressed = (Mathf.Abs(inputManager.Forward) > 0.01f || Mathf.Abs(inputManager.Horizontal) > 0.01f) && inputManager.canMove;

        Vector3 forward = playerCamera.transform.forward * inputManager.Forward;
        Vector3 horizontal = playerCamera.transform.right * inputManager.Horizontal;

        if (strafe)
        {
            // Strafe movement

            if (movementPressed)
            {
                targetDirection = (forward + horizontal).normalized;
                targetRotation = Quaternion.Euler(new Vector3(0, playerCamera.transform.rotation.eulerAngles.y, 0));
            }

            if (forwardPressed && movementZ < 1f)
                movementZ += Time.deltaTime * 2;
            if (backwardPressed && movementZ > -1f)
                movementZ -= Time.deltaTime * 2;
            if (rightPressed && movementX < 1f)
                movementX += Time.deltaTime * 2;
            if (leftPressed && movementX > -1f)
                movementX -= Time.deltaTime * 2;

            if (!forwardPressed && movementZ > 0f)
                movementZ -= Time.deltaTime * 2;
            if (!backwardPressed && movementZ < 0f)
                movementZ += Time.deltaTime * 2;
            if (!rightPressed && movementX > 0f)
                movementX -= Time.deltaTime * 2;
            if (!leftPressed && movementX < 0f)
                movementX += Time.deltaTime * 2;
        }
        else
        {
            // Base movement

            if (movementPressed)
            {
                targetDirection = (forward + horizontal).normalized;
                targetRotation = Quaternion.Euler(new Vector3(0, Quaternion.LookRotation(targetDirection, transform.up).eulerAngles.y, 0));
                if (movementZ < 1f)
                    movementZ += Time.deltaTime * 2;
            }
            else if (movementZ > 0.05f)
                movementZ -= Time.deltaTime * 2;
            else if (movementZ < -0.05f)
                movementZ += Time.deltaTime * 2;
            else
                movementZ = 0f;

            if (movementX > 0.05f)
                movementX -= Time.deltaTime * 2;
            else if (movementX < -0.05f)
                movementX += Time.deltaTime * 2;
            else
                movementX = 0f;
        }

        accelleration = (playerCamera.transform.forward * movementX + playerCamera.transform.right * movementZ).magnitude;

        animator.SetFloat("Velocity X", movementX);
        animator.SetFloat("Velocity Z", movementZ);
    }
}
