using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [Header("References")]
    [SerializeField] private GunController gunController;
    [SerializeField] private Transform cameraPosition;
    [SerializeField] private Transform feet;
    [HideInInspector] public Rigidbody rb;
    private CapsuleCollider collider;

    [Header("Movement")]
    [SerializeField][Range(0, 100)] private float walkSpeed;
    [SerializeField][Range(0, 100)] private float sprintSpeed;
    [SerializeField][Range(0, 100)] private float crouchSpeed;
    [SerializeField][Range(0, 100)] private float ADSSpeed;
    [SerializeField][Range(0, 100)] private float ADSCrouchSpeed;
    [SerializeField][Range(0, 100)] private float airSpeed;
    [HideInInspector] public Vector3 movementDirection;
    private float currentSpeed;

    [Header("Looking")]
    [SerializeField][Range(0, 100)] private float xSensitivity;
    [SerializeField][Range(0, 100)] private float ySensitivity;
    [SerializeField][Range(0, 90)] private float topLookClamp;
    [SerializeField][Range(0, 90)] private float bottomLookClamp;
    [Space]
    [SerializeField] private bool invertLookX;
    [SerializeField] private bool invertLookY;
    [HideInInspector] public Vector2 mouseInput;
    private float xRotation;
    private float yRotation;

    [Header("Sprinting")]
    [HideInInspector] public bool isSprinting;

    [Header("Jumping")]
    [SerializeField][Range(0, 50)] private float jumpHeight;
    [SerializeField][Range(0, 10)] private float airMultiplier;

    [Header("Crouching")]
    [SerializeField][Range(0, 10)] private float crouchHeight;
    [HideInInspector] public bool isCrouching;
    private float initialScale;
    private float standHeight;

    [Header("Ground Check")]
    [SerializeField][Range(0, 10)] private float groundCheckRadius;
    [SerializeField] private LayerMask environmentMask;
    [HideInInspector] public bool isGrounded;

    [Header("Drag Control")]
    [SerializeField][Range(0, 10)] private float groundDrag;

    private void Start() {

        collider = GetComponent<CapsuleCollider>();

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        initialScale = transform.localScale.y;
        standHeight = collider.height;

    }

    private void Update() {

        isGrounded = Physics.CheckSphere(feet.position, groundCheckRadius, environmentMask);

        cameraPosition.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        transform.rotation = Quaternion.Euler(0, yRotation, 0);

        ControlSpeed();

        if (isGrounded) {

            rb.drag = groundDrag;

        } else {

            rb.drag = 0;

        }
    }

    private void FixedUpdate() {

        rb.AddForce(movementDirection.normalized * currentSpeed, ForceMode.Force);

    }

    public void Move(Vector2 input) {

        movementDirection = transform.forward * input.y + transform.right * input.x;

        if (isGrounded && isSprinting && !isCrouching && !gunController.isADS) {

            currentSpeed = sprintSpeed * 10f;

        } else if (isGrounded && !isSprinting && !isCrouching && !gunController.isADS) {

            currentSpeed = walkSpeed * 10f;

        } else if (isGrounded && isCrouching && !gunController.isADS) {

            currentSpeed = crouchSpeed * 10f;

        } else if (isGrounded && !isCrouching && gunController.isADS) {

            currentSpeed = ADSSpeed * 10f;

        } else if (isGrounded && isCrouching && gunController.isADS) {

            currentSpeed = ADSCrouchSpeed * 10f;

        } else if (!isGrounded) {

            currentSpeed = airSpeed * airMultiplier * 10f;

        }
    }

    public void Look(Vector2 input) {

        float mouseX = input.x * xSensitivity / 100;
        float mouseY = input.y * ySensitivity / 100;

        mouseInput = new Vector2(mouseX, mouseY);

        if (invertLookX) {

            yRotation -= mouseX;

        } else {

            yRotation += mouseX;

        }

        if (invertLookY) {

            xRotation += mouseY;

        } else {

            xRotation -= mouseY;

        }

        xRotation = Mathf.Clamp(xRotation, -bottomLookClamp, topLookClamp);

    }

    private void ControlSpeed() {

        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVelocity.magnitude > currentSpeed) {

            Vector3 limitedVelocity = flatVelocity.normalized * currentSpeed;

            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);

        }
    }

    public void StartSprint() {

        if (isGrounded) {

            isSprinting = true;

        }
    }

    public void StopSprint() {

        isSprinting = false;

    }

    public void Jump() {

        if (isCrouching) {

            Crouch();
            return;

        }

        if (isGrounded) {

            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            rb.AddForce(transform.up * jumpHeight, ForceMode.Impulse);

        }
    }

    public void Crouch() {

        if (isGrounded) {

            isCrouching = !isCrouching;

            if (isCrouching) {

                collider.height = crouchHeight;
                transform.localScale = new Vector3(transform.localScale.x, crouchHeight, transform.localScale.z);

            } else {

                collider.height = standHeight;
                transform.localScale = new Vector3(transform.localScale.x, initialScale, transform.localScale.z);

            }
        }
    }
}
