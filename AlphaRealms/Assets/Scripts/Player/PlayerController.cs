using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [Header("References")]
    [SerializeField] private Transform cameraPosition;
    [SerializeField] private Transform feet;
    private Rigidbody rb;

    [Header("Movement")]
    [SerializeField][Range(0, 50)] private float walkSpeed;
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
    [SerializeField][Range(0, 50)] private float sprintSpeed;
    [HideInInspector] public bool isSprinting;

    [Header("Jumping")]
    [SerializeField][Range(0, 20)] private float jumpHeight;
    [SerializeField][Range(0, 3)] private float airMultiplier;

    [Header("Headbobbing")]
    [SerializeField] private float walkBobSpeed;
    [SerializeField] private float walkBobAmount;
    [SerializeField] private float sprintBobSpeed;
    [SerializeField] private float sprintBobAmount;
    private float defaultYPosition;
    private float timer;

    [Header("Ground Check")]
    [SerializeField][Range(0, 3)] private float groundCheckRadius;
    [SerializeField] private LayerMask environmentMask;
    private bool isGrounded;

    [Header("Drag Control")]
    [SerializeField][Range(0, 10)] private float groundDrag;

    private void Start() {

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        defaultYPosition = cameraPosition.transform.localPosition.y;

    }

    private void Update() {

        isGrounded = Physics.CheckSphere(feet.position, groundCheckRadius, environmentMask);

        cameraPosition.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        transform.rotation = Quaternion.Euler(0, yRotation, 0);

        ControlSpeed();

        HandleHeadbob();

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

        if (isGrounded && isSprinting) {

            currentSpeed = sprintSpeed * 10f;

        } else if (isGrounded && !isSprinting) {

            currentSpeed = walkSpeed * 10f;

        } else if (!isGrounded) {

            currentSpeed = walkSpeed * 10f * airMultiplier;

        }
    }

    public void Look(Vector2 input) {

        this.mouseInput = input;

        float mouseX = input.x * xSensitivity * Time.smoothDeltaTime;
        float mouseY = input.y * ySensitivity * Time.smoothDeltaTime;

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

        if (isGrounded) {

            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            rb.AddForce(transform.up * jumpHeight, ForceMode.Impulse);

        }
    }

    private void HandleHeadbob() {

        if (isGrounded) {

            if (rb.velocity.x > 0.1f || rb.velocity.z > 0.1f) {

                timer += (isSprinting ? sprintBobSpeed : walkBobSpeed) * Time.deltaTime;
                cameraPosition.localPosition = new Vector3(
                    cameraPosition.transform.localPosition.x,
                    defaultYPosition + Mathf.Sin(timer) * (isSprinting ? sprintBobAmount : walkBobAmount),
                    cameraPosition.transform.localPosition.z);

            }
        }
    }
}
