using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Minigame enable, rotation scripting etc.
    /// </summary>

    [Header("Minigame Assets")]
    [SerializeField] private GameObject minigame;
    [SerializeField] private GameObject rotatePointer;
    [SerializeField] private GameObject goodHitArea;
    [SerializeField] private GameObject perfectHitArea;
    [SerializeField] private GameObject missHitArea;
    [SerializeField] public Rigidbody rotatorRB;
    [SerializeField] InputActionAsset playerControls;

    [Header("Minigame Variables")]
    private InputAction actionAction;
    private float anglesPerSecond = 90.0f;
    public bool isRotating = true;
    public bool attuneComplete = false;

    /// <summary>
    /// Attunement levels and xp handling
    /// </summary>

    [Header("XP Bar Assets")]
    [SerializeField] Image levelBar;
    [SerializeField] Text levelNumber;

    [Header("Player Levels and XP Variables")]
    [SerializeField] int playerAttuneLevel;
    [SerializeField] float playerCurrentXP;
    [SerializeField] float xpToNextLevel;

    float playerStartXP;
    float xpToFirstLevel;

    [Header("Animal Levels and Variables")]
    
    [SerializeField] int animalLevel;
    float startleValue;

    /// <summary>
    /// Individual Animal Attunement Check Variables
    /// </summary>

    [Header("Animal Attunement Check Variables")]
    [SerializeField] PlayerInteract pi;
    [SerializeField] public bool chickenAttuned = false;
    [SerializeField] public bool dogAttuned;
    [SerializeField] public bool penguinAttuned;
    [SerializeField] public bool catAttuned;
    [SerializeField] public bool deerAttuned;
    [SerializeField] public bool horseAttuned;
    [SerializeField] public bool tigerAttuned;


    /// <summary>
    /// Awake, Start, Update, OnEnable and OnDisable methods
    /// </summary>
    private void Awake()
    {
        //sets players current xp and level to 0
        playerAttuneLevel = 0;
        playerCurrentXP = 0;
    }

    void Start()
    {
        //minigame variables to set on start
        actionAction = playerControls.FindActionMap("Player").FindAction("Action");


        //Randomly sets the rotation for the good/perfect hit areas
        Vector3 ranRotation = goodHitArea.transform.eulerAngles;
        ranRotation.z = Random.Range(0, 360);
        goodHitArea.transform.eulerAngles = ranRotation;


        //attune level variables to set on start
        xpToFirstLevel = 100;
        playerStartXP = 0;
    }

    private void OnEnable()
    {
        actionAction.Enable();
    }

    private void OnDisable()
    {
        actionAction.Disable();
    }

    private void Update()
    {
        //minigame methods
        OnStopSkillRotator();

        //attune check methods
        AttunementStatus();

        //attune level methods
        HandlePlayerLevels();
        HandleAnimalLevels();

    }


    /// <summary>
    /// this method is for rotating the pointer in the minigame but only if the minigame is active 
    /// it then checks if the left mouse button has been clicked and will freeze stop the pointer 
    /// from rotating. it then activates the above coroutine which just waits a second before 
    /// setting the minigame to inactive again.
    /// </summary>
    void OnStopSkillRotator()
    {
        if (minigame.activeInHierarchy && isRotating)
        {
            //rotates the pointer constantly
            Vector3 rotation = rotatorRB.transform.eulerAngles;
            rotation.z -= Time.deltaTime * anglesPerSecond;
            rotatorRB.transform.eulerAngles = rotation;

            if (actionAction.triggered)
            {
                isRotating = false;
                rotatorRB.freezeRotation = true;
                playerCurrentXP += 50;
                attuneComplete = true;
                StartCoroutine(ActiveObject());
            }
        }
    }

    IEnumerator ActiveObject()
    {
        yield return new WaitForSeconds(1f);
        minigame.SetActive(false);
    }


    /// <summary>
    /// this method sets and holds the players attunement level as well as the xp they have earned and
    /// will update the onscreen xp bar with the progress to the next level. 
    /// currently this does not do much in play mode due to no sources of xp being setup yet.
    /// </summary>
    void HandlePlayerLevels()
    {
        xpToNextLevel = xpToFirstLevel;

        float i = Mathf.InverseLerp(playerStartXP, xpToNextLevel, playerCurrentXP);
        levelBar.fillAmount = i;

        if (i == 1)
        {
            levelBar.fillAmount = 0;
            playerCurrentXP = 0;
            xpToNextLevel = xpToFirstLevel += 50;
            playerAttuneLevel++;
            levelNumber.text = playerAttuneLevel.ToString();
        }
    }


    /// <summary>
    /// similarly to the player levels, this method will set and adjust the animal attune levels
    /// and base them upon the players level but also allows for a minimum level if the player
    /// is not level 3 or higher.
    /// </summary>
    void HandleAnimalLevels()
    {
        if (playerAttuneLevel <= 1)
        {
            animalLevel = 1;
        }
        else if (playerAttuneLevel >= 2)
        {
            animalLevel = playerAttuneLevel + Random.Range(-2, 2);
        }
    }


    /// <summary>
    /// individual animal attunement checks
    /// </summary>
    public void AttunementStatus()
    {
        if (attuneComplete)
        {
            if (pi.inChickenRadius && !chickenAttuned)
            {
                chickenAttuned = true;
                attuneComplete = false;
            }

            if (pi.inDogRadius && !dogAttuned)
            {
                dogAttuned = true;
                attuneComplete = false;
            }

            if (pi.inPenguinRadius && !penguinAttuned)
            {
                penguinAttuned = true;
                attuneComplete = false;
            }

            if (pi.inCatRadius && !catAttuned)
            {
                catAttuned = true;
                attuneComplete = false;
            }

            if (pi.inDeerRadius && !deerAttuned)
            {
                deerAttuned = true;
                attuneComplete = false;
            }

            if (pi.inHorseRadius && !horseAttuned)
            {
                horseAttuned = true;
                attuneComplete = false;
            }

            if (pi.inTigerRadius && !tigerAttuned)
            {
                tigerAttuned = true;
                attuneComplete = false;
            }
        }
    }
}
