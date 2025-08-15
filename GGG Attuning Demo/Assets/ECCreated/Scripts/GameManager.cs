using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Minigame enable, player action
    /// </summary>
    [Header("Minigame Assets")]
    [SerializeField] private GameObject minigame;
    [SerializeField] InputActionAsset playerControls;

    [Header("Minigame Variables")]
    private InputAction actionAction;
    private RhythmGameControl rhythmControl;
    public bool attuneComplete = false;

    /// <summary>
    /// Attunement levels and xp handling
    /// </summary>
    [Header("XP Bar Assets")]
    [SerializeField] Image levelBar;
    [SerializeField] Text levelNumber;

    [Header("Player Levels and XP Variables")]
    [SerializeField] int playerAttuneLevel;
    [SerializeField] public float playerCurrentXP;
    [SerializeField] float xpToNextLevel;

    float playerStartXP;
    float xpToFirstLevel;

    private ScoreManager scoreManager;

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
        actionAction = playerControls.FindActionMap("Player").FindAction("Action");

        actionAction.performed += HandleAction;

        //sets players current xp and level to 0
        playerAttuneLevel = 0;
        playerCurrentXP = 0;
    }

    void Start()
    {
        //minigame variables to set on start
        rhythmControl = gameObject.GetComponent<RhythmGameControl>();

        scoreManager = FindObjectOfType<ScoreManager>();

        //attune level variables to set on start
        xpToFirstLevel = 100;
        playerStartXP = 0;

        UpdateLevelUI();
    }

    private void OnEnable()
    {
        actionAction.Enable();
        RhythmGameControl.OnHitAccuracy += HandleHitAccuracyResult;
        ScoreManager.OnGameWon += HandleGameWon;
    }

    private void OnDisable()
    {
        actionAction.Disable();
        actionAction.performed -= HandleAction;
        RhythmGameControl.OnHitAccuracy -= HandleHitAccuracyResult;
        ScoreManager.OnGameWon -= HandleGameWon;
    }
    private void HandleHitAccuracyResult(RhythmGameControl.HitAccuracyType accuracy)
    {
        switch (accuracy)
        {
            case RhythmGameControl.HitAccuracyType.Perfect:
                AwardXP(50); // Award more XP for perfect hit
                Debug.Log("GameManager: Received Perfect Hit!");
                attuneComplete = true; // Set attuneComplete here as this is a successful action
                break;
            case RhythmGameControl.HitAccuracyType.Good:
                AwardXP(25); // Award less XP for good hit
                Debug.Log("GameManager: Received Good Hit!");
                attuneComplete = true; // Set attuneComplete here as this is a successful action
                break;
            case RhythmGameControl.HitAccuracyType.Miss:
                // Optionally, penalize or provide feedback for a miss
                Debug.Log("GameManager: Received Miss!");
                break;
        }
    }

    private void HandleGameWon()
    {
        Debug.Log("GameManager: Game Won! Stopping minigame.");
        // Deactivate the minigame when the score is reached
        // This will stop the beat incrementing coroutine in RhythmGameControl
        minigame.SetActive(false);

        // You might also want to:
        // - Display a win screen or message
        // - Load a new scene
        // - Play a winning sound effect
        // - Unlock new content (using attuneComplete, for example)
    }

    private void Update()
    {
        //attune check methods
        AttunementStatus();

        //attune level methods
        //HandlePlayerLevels();
        HandleAnimalLevels();
    }

    /// <summary>
    /// This method is to check whether the mingame is active in the hierarchy
    /// and will currently award xp and changes 
    /// a bool before waiting a second and setting itself to inactive again.
    /// </summary>
    public void HandleAction(InputAction.CallbackContext context)
    {
        if (minigame.activeInHierarchy)
        {
            rhythmControl.HandleHitInput();
        }
    }

    public void AwardXP(int xpAmount)
    {
        playerCurrentXP += xpAmount;
        Debug.Log($"Awarded {xpAmount} XP. Current XP: {playerCurrentXP}");
        HandlePlayerLevels(); // Check for level up after gaining XP
        attuneComplete = true; // Set attuneComplete here as this is a successful action
    }


    /// <summary>
    /// this method sets and holds the players attunement level as well as the xp they have earned and
    /// will update the onscreen xp bar with the progress to the next level. 
    /// currently this does not do much in play mode due to no sources of xp being setup yet.
    /// </summary>
    void HandlePlayerLevels()
    {
        xpToNextLevel = xpToFirstLevel + (playerAttuneLevel * 50);

        float fillAmount = Mathf.InverseLerp(playerStartXP, xpToNextLevel, playerCurrentXP);
        levelBar.fillAmount = fillAmount;

        if (playerCurrentXP >= xpToNextLevel)
        {
            levelBar.fillAmount = 0;
            playerCurrentXP -= xpToNextLevel;
            xpToNextLevel = xpToFirstLevel + ((playerAttuneLevel + 1) * 50);
            playerAttuneLevel++;
            UpdateLevelUI();
        }
    }

    void UpdateLevelUI()
    {
        levelNumber.text = playerAttuneLevel.ToString();
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
            if(animalLevel < 1) animalLevel = 1;
        }
    }


    /// <summary>
    /// individual animal attunement checks
    /// </summary>
    public void AttunementStatus()
    {
        if (attuneComplete)
        {
            bool attunedThisFrame = false;
            if(pi != null)
            {
                if (pi.inChickenRadius && !chickenAttuned)
                {
                    chickenAttuned = true;
                    attuneComplete = false;
                }

                else if (pi.inDogRadius && !dogAttuned)
                {
                    dogAttuned = true;
                    attuneComplete = false;
                }

                else if (pi.inPenguinRadius && !penguinAttuned)
                {
                    penguinAttuned = true;
                    attuneComplete = false;
                }

                else if (pi.inCatRadius && !catAttuned)
                {
                    catAttuned = true;
                    attuneComplete = false;
                }

                else if (pi.inDeerRadius && !deerAttuned)
                {
                    deerAttuned = true;
                    attuneComplete = false;
                }

                else if (pi.inHorseRadius && !horseAttuned)
                {
                    horseAttuned = true;
                    attuneComplete = false;
                }

                else if (pi.inTigerRadius && !tigerAttuned)
                {
                    tigerAttuned = true;
                    attuneComplete = false;
                }
            }
            if (attunedThisFrame)
            {
                attuneComplete = false;
            }
        }
    }
}
