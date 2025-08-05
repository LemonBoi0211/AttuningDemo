using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] public GameObject attuneMinigame;
    [SerializeField] InputActionAsset playerControls;

    private InputAction interactAction;

    public GameManager gm;

    [SerializeField] public bool withinRadius;
    [SerializeField] public bool inChickenRadius;
    [SerializeField] public bool inDogRadius;
    [SerializeField] public bool inPenguinRadius;
    [SerializeField] public bool inCatRadius;
    [SerializeField] public bool inDeerRadius;
    [SerializeField] public bool inHorseRadius;
    [SerializeField] public bool inTigerRadius;

    // Start is called before the first frame update
    void Start()
    {
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

    /// <summary>
    /// This Method allows the player to press 'e' and it will bring up the attuning minigame 
    /// however it only does it if they are within the attuning radius and if the animal 
    /// has not been already attuned too
    /// </summary>
    void HandleInteract()
    {
        if (interactAction.triggered && withinRadius)
        {
            if (!gm.attuneComplete)
            {
                //sets the minigame to active
                attuneMinigame.SetActive(true);
            }
            else
            {
                withinRadius = false;
            }
        }
        
    }

    /// <summary>
    /// this method is checking if the player is within the attuning radius and returns a boolean
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 6 && !gm.attuneComplete)
        {
            withinRadius = true;
        }

        if (other.CompareTag("chickenRadius") && !gm.chickenAttuned)
        {
            inChickenRadius = true;
        }

        if (other.CompareTag("dogRadius") && !gm.dogAttuned)
        {
            inDogRadius = true;
        }

        if (other.CompareTag("penguinRadius") && !gm.penguinAttuned)
        {
            inPenguinRadius = true;
        }

        if (other.CompareTag("catRadius") && !gm.catAttuned)
        {
            inCatRadius = true;
        }

        if (other.CompareTag("deerRadius") && !gm.deerAttuned)
        {
            inDeerRadius = true;
        }

        if (other.CompareTag("horseRadius") && !gm.horseAttuned)
        {
            inHorseRadius = true;
        }

        if (other.CompareTag("tigerRadius") && !gm.tigerAttuned)
        {
            inTigerRadius = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 6 || gm.attuneComplete)
        {
            withinRadius = false;
        }

        if(other.CompareTag("chickenRadius") || gm.chickenAttuned)
        {
            inChickenRadius = false;
        }

        if (other.CompareTag("dogRadius") || gm.dogAttuned)
        {
            inDogRadius = false;
        }

        if (other.CompareTag("penguinRadius") || gm.penguinAttuned)
        {
            inPenguinRadius = false;
        }

        if (other.CompareTag("catRadius") || gm.catAttuned)
        {
            inCatRadius = false;
        }

        if (other.CompareTag("deerRadius") || gm.deerAttuned)
        {
            inDeerRadius = false;
        }

        if (other.CompareTag("horseRadius") || gm.horseAttuned)
        {
            inHorseRadius = false;
        }

        if (other.CompareTag("tigerRadius") || gm.tigerAttuned)
        {
            inTigerRadius = false;
        }
    }
}