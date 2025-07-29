using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private GameObject attuneMinigame;
    [SerializeField] InputActionAsset playerControls;

    private InputAction interactAction;

    private ActionCircleRotate actionCircleRotate;
    private bool withinRadius = false;

    // Start is called before the first frame update
    void Start()
    {
        actionCircleRotate = attuneMinigame.GetComponent<ActionCircleRotate>();
        interactAction = playerControls.FindActionMap("Player").FindAction("Interact");
    }

    private void OnEnable()
    {
        interactAction.Enable();
    }

    private void OnDisable()
    {
        interactAction.Disable();
    }

    private void Update()
    {
        HandleInteract();
    }

    void HandleInteract()
    {
        if (withinRadius && !actionCircleRotate.attuned)
        {
            if (interactAction.triggered)
            {
                attuneMinigame.SetActive(true);
                actionCircleRotate.isRotating = true;
                actionCircleRotate.rotatorRB.freezeRotation = false;
            }
        }
        else 
        { 
            withinRadius = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("AttunementRadius"))
        {
            withinRadius = true;
        }
        else if (other.CompareTag("AttunementRadius") && actionCircleRotate.attuned)
        {
            withinRadius = false;
        }
    }
}