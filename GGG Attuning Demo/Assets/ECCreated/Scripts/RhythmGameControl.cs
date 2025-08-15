using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class RhythmGameControl : MonoBehaviour
{
    public delegate void HitAccuracyEvent(HitAccuracyType accuracy);
    public static event HitAccuracyEvent OnHitAccuracy;


    /// <summary>
    /// Assests used for the Rhythm Minigame
    /// </summary>
    [SerializeField] public GameObject miniGame;
    [SerializeField] public Image targetCircle;
    [SerializeField] public Image metroCircle;
    [SerializeField] public Image hitCircle;
    [SerializeField] public TMP_Text beatCounterText;
    [SerializeField] GameObject missIndicator;
    [SerializeField] GameObject beatSpawnPosition;


    //GameManager gm;
    ScoreManager scoreManager;
    CircleCollider2D hitCol;
    private RectTransform spwnRect;
    private RectTransform failRect;
    [SerializeField] private RectTransform targetZoneRect; 

    /// <summary>
    /// Variables for the Rhythm Minigame
    /// </summary>
    [SerializeField] public int beatCount = 0;
    [SerializeField] float bpm = 120;
    float bps;
    float secsPerBeat;
    public float beatInterval;
    [SerializeField] float hitCircleMovementDuration = 1f;
    float perfectHitTolerance = 25f;
    [SerializeField] float goodHitTolerance = 50f;

    private Vector3 startPos;
    private Vector3 endPos;
    private Coroutine currentHitCircleMovement;
    private Coroutine currentBeatIncrementCoroutine;

    /// <summary>
    /// Audio sources for the audio assist
    /// </summary>
    [Header("Audio Assist")]
    [SerializeField] private AudioSource metronomeAudioSource;
    [SerializeField] private AudioClip metronomeClickSound;

    private WaitForSeconds cachedWaitForBeatInterval;

    public enum HitAccuracyType { None, Perfect, Good, Miss }
    private HitAccuracyType lastHitAccuracy = HitAccuracyType.None;

    private void Start()
    {
        //gm = gameObject.GetComponent<GameManager>();
        scoreManager = gameObject.GetComponent<ScoreManager>();

        hitCol = hitCircle.GetComponent<CircleCollider2D>();
        spwnRect = beatSpawnPosition.GetComponent<RectTransform>();
        failRect = missIndicator.GetComponent<RectTransform>();

        //some basic math to convert the input BPM to a value that the minigame can use
        bps = bpm / 60;
        secsPerBeat = 1 / bps;
        beatInterval = secsPerBeat;

        cachedWaitForBeatInterval = new WaitForSeconds(beatInterval);

        if(metronomeAudioSource == null)
        {
            metronomeAudioSource= GetComponent<AudioSource>();
            if(metronomeAudioSource == null)
            {
                metronomeAudioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        metronomeAudioSource.playOnAwake = false;
        metronomeAudioSource.loop = false;

        startPos = spwnRect.localPosition;
        endPos = targetZoneRect.localPosition;

        hitCircle.rectTransform.position = startPos;
        hitCircle.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        //starts the beatcounter and pulsing
        currentBeatIncrementCoroutine = StartCoroutine(IncrementBeat());
        
    }

    private void OnDisable()
    {
        if(currentBeatIncrementCoroutine != null)
        {
            StopCoroutine(currentBeatIncrementCoroutine);
        }
    }

    public void HandleHitInput()
    {
        if (!hitCircle.gameObject.activeInHierarchy) return; // Only process hits if a circle is active

        float distanceToTarget = Vector3.Distance(hitCircle.rectTransform.localPosition, targetZoneRect.localPosition);

        if (distanceToTarget <= perfectHitTolerance)
        {
            lastHitAccuracy = HitAccuracyType.Perfect;
            Debug.Log("Perfect Hit!");
            // Award XP or score for perfect hit
            OnHitAccuracy?.Invoke(HitAccuracyType.Perfect); // Notify GameManager via event
        }
        else if (distanceToTarget <= goodHitTolerance)
        {
            lastHitAccuracy = HitAccuracyType.Good;
            Debug.Log("Good Hit!");
            // Award XP or score for good hit
            OnHitAccuracy?.Invoke(HitAccuracyType.Good);
        }
        else
        {
            lastHitAccuracy = HitAccuracyType.Miss;
            Debug.Log("Miss!");
            // Penalize for miss or provide feedback
            OnHitAccuracy?.Invoke(HitAccuracyType.Miss);
        }
        ResetHitCircleState();
    }

    public void ResetValues()
    {
        beatCount = 1;
        if(beatCounterText != null) beatCounterText.text = beatCount.ToString();
        ResetHitCircleState();
        miniGame.SetActive(false);
        

        if(currentHitCircleMovement != null)
        {
            StopCoroutine(currentHitCircleMovement);
        }
        Debug.Log("Values Reset");

        if(currentBeatIncrementCoroutine != null)
        {
            StopCoroutine (currentBeatIncrementCoroutine);
        }
    }

    public void StartHitCircleForBeat()
    {
        hitCircle.gameObject.SetActive(true);
        hitCircle.rectTransform.localPosition = startPos;
        if(currentHitCircleMovement != null)
        {
            StopCoroutine(currentHitCircleMovement);
        }
        currentHitCircleMovement = StartCoroutine(MoveHitCircleToTarget(hitCircleMovementDuration));
    }

    private IEnumerator MoveHitCircleToTarget(float duration)
    {
        float timer = 0f;
        while(timer < duration)
        {
            hitCircle.rectTransform.localPosition = Vector3.Lerp(startPos,endPos, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        hitCircle.rectTransform.localPosition = endPos;

        if(lastHitAccuracy == HitAccuracyType.None)
        {
            OnHitAccuracy?.Invoke(HitAccuracyType.Miss);
        }
        lastHitAccuracy = HitAccuracyType.None;
        ResetHitCircleState();
    }

    private void ResetHitCircleState()
    {
        hitCircle.gameObject.SetActive(false);
        hitCircle.rectTransform.localPosition = startPos;
    }

    /// <summary>
    /// This method is simply for incrementing the beat counter from 1 to 4 as well as 
    /// adjusting the alpha values of the text and the outer circle to make it appear
    /// as if they are pulsing.
    /// </summary>
    public IEnumerator IncrementBeat()
    {
        while (true)
        {
            if (miniGame.activeInHierarchy)
            {
                beatCount++;
                metroCircle.canvasRenderer.SetAlpha(1f);
                beatCounterText.canvasRenderer.SetAlpha(1f);

                if (beatCount > 4)
                {
                    beatCount = 1;
                }

                if (beatCounterText != null)
                {
                    beatCounterText.text = beatCount.ToString();
                    metroCircle.CrossFadeAlpha(0.0f, secsPerBeat, false);
                    beatCounterText.CrossFadeAlpha(0.0f, secsPerBeat, false);

                    if(metronomeAudioSource != null && metronomeClickSound != null)
                    {
                        metronomeAudioSource.PlayOneShot(metronomeClickSound);
                    }

                    if(hitCircle.gameObject.activeInHierarchy && lastHitAccuracy == HitAccuracyType.None)
                    {
                        OnHitAccuracy?.Invoke(HitAccuracyType.Miss);
                    } 
                    lastHitAccuracy = HitAccuracyType.None;

                    StartHitCircleForBeat();
                    
                }
            }
            yield return cachedWaitForBeatInterval;
        }
    }
}
