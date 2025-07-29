using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] private float defaultSpeed = 3.0f;
    [SerializeField] private float sprintMultiplier = 2.0f;
    [SerializeField] private float crouchMultiplier = 0.5f;

    [Header("Jump Parameters")]
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float gravity = 9.81f;

    [Header("Crouch Parameters")]
    [SerializeField] private float originalHeight;
    [SerializeField] private Vector3 originalCenter;
    [SerializeField] private float crouchHeight;
    [SerializeField] private Vector3 crouchCenter;
    private bool isCrouching = false;

    [Header("Look Sensitivity")]
    [SerializeField] private float mouseSensitivity = 2.0f;
    //[SerializeField] private float upDownRange = 80.0f;

    [Header("Input Actions")]
    [SerializeField] InputActionAsset playerControls;

    private bool isMoving;
    private float nextStepTime;
    private GameObject moveReticle;
    private float verticalRotation;
    private Vector3 currentMovement = Vector3.zero;
    private CharacterController characterController;

    private float pushPower = 2.0f;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction crouchAction;
    private Vector2 moveInput;
    private Vector2 lookInput;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        moveReticle = GameObject.Find("MoveReticle");
        Cursor.lockState = CursorLockMode.Locked;

        originalHeight = characterController.height;
        originalCenter = characterController.center;
        crouchHeight = originalHeight / 2;

        moveAction = playerControls.FindActionMap("Player").FindAction("Move");
        lookAction = playerControls.FindActionMap("Player").FindAction("Look");
        jumpAction = playerControls.FindActionMap("Player").FindAction("Jump");
        sprintAction = playerControls.FindActionMap("Player").FindAction("Sprint");
        crouchAction = playerControls.FindActionMap("Player").FindAction("Crouch");

        moveAction.performed += context => moveInput = context.ReadValue<Vector2>();
        moveAction.canceled += context => moveInput = Vector2.zero;

        lookAction.performed += context => lookInput = context.ReadValue<Vector2>();
        lookAction.canceled += context => lookInput = Vector2.zero;
    }

    private void OnEnable()
    {
        moveAction.Enable();
        lookAction.Enable();
        jumpAction.Enable();
        sprintAction.Enable();
        crouchAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
        jumpAction.Disable();
        sprintAction.Disable();
        crouchAction.Disable();
    }

    void Update()
    {
        HandleMovement();
        HandleCrouching();
        HandleGravityAndJumping();
        HandleRotation();
    }

    void HandleMovement()
    {
        float speedMultiplier = sprintAction.ReadValue<float>() > 0 ? sprintMultiplier : 1f;

        float verticalSpeed = moveInput.y * defaultSpeed * speedMultiplier;
        float horizontalSpeed = moveInput.x * defaultSpeed * speedMultiplier;

        Vector3 horizontalMovement = new Vector3 (horizontalSpeed, 0, verticalSpeed);
        horizontalMovement = transform.rotation * horizontalMovement;

        HandleGravityAndJumping();

        currentMovement.x = horizontalMovement.x;
        currentMovement.z = horizontalMovement.z;

        characterController.Move(currentMovement * Time.deltaTime);

        isMoving = moveInput.y != 0 || moveInput.x != 0;
    }

    void HandleCrouching()
    {
        if (crouchAction.triggered)
        {
            
            if (!isCrouching)
            {
                characterController.height = crouchHeight;
                characterController.center = crouchCenter;
                defaultSpeed *= crouchMultiplier;
                isCrouching = true;
            }
            else
            {
                characterController.height = originalHeight;
                characterController.center = originalCenter;
                defaultSpeed /= crouchMultiplier;
                isCrouching = false;
            }
        }
        
    }

    void HandleGravityAndJumping()
    {
        if (characterController.isGrounded)
        {
            currentMovement.y = -0.5f;

            if (jumpAction.triggered)
            {
                currentMovement.y = jumpForce;
            }
        }
        else
        {
            currentMovement.y -= gravity * Time.deltaTime;
        }
    }

    void HandleRotation()
    {
        float mouseXRotation = lookInput.x * mouseSensitivity;
        transform.Rotate(0, mouseXRotation, 0);

        //verticalRotation -= lookInput.y * mouseSensitivity;
        //verticalRotation = Mathf.Clamp(verticalRotation, -upDownRange, upDownRange);
        //moveReticle.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }


    // this script pushes all rigidbodies that the character touches
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        // no rigidbody
        if (body == null || body.isKinematic)
        {
            return;
        }

        // We dont want to push objects below us
        if (hit.moveDirection.y < -0.3)
        {
            return;
        }

        // Calculate push direction from move direction,
        // we only push objects to the sides never up and down
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

        // If you know how fast your character is trying to move,
        // then you can also multiply the push velocity by that.

        // Apply the push
        body.velocity = pushDir * pushPower;
    }
}
