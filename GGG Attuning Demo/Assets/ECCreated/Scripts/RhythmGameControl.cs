using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.UI.ContentSizeFitter;


public class RhythmGameControl : MonoBehaviour
{
    /// <summary>
    /// Assests used for the Rhythm Minigame
    /// </summary>
    [SerializeField] public GameObject miniGame;
    [SerializeField] public Image targetCircle;
    [SerializeField] public Image metroCircle;
    [SerializeField] public Image hitCircle;
    [SerializeField] public TMP_Text beatCounter;
    [SerializeField] GameObject targetFail;
    [SerializeField] GameObject spawnr;

    GameManager gm;
    CircleCollider2D hitCol;
    private RectTransform spwnRect;
    private RectTransform failRect;

    /// <summary>
    /// Variables for the Rhythm Minigame
    /// </summary>
    [SerializeField] public int beatCount = 0;
    [SerializeField] float bpm = 120;
    float bps;
    float secsPerBeat;
    public float beatInterval;

    private bool hitMissed;
    private Vector3 hitMove;


    private void Start()
    {
        gm = gameObject.GetComponent<GameManager>();
        hitCol = hitCircle.GetComponent<CircleCollider2D>();
        spwnRect = spawnr.GetComponent<RectTransform>();
        failRect = targetFail.GetComponent<RectTransform>();

        //some basic math to convert the input BPM to a value that the minigame can use
        bps = bpm / 60;
        secsPerBeat = 1 / bps;
        beatInterval = secsPerBeat;
    }

    private void OnEnable()
    {
        //starts the beatcounter and pulsing
        StartCoroutine(IncrementBeat());
        StartCoroutine(SpawnHitCircle());
    }

    public void ResetValues()
    {
        beatCount = 1;
        beatCounter.text = beatCount.ToString();
        hitCircle.rectTransform.localPosition = spwnRect.localPosition;
        miniGame.SetActive(false);
        gm.clicked = false;
        gm.hitHit = false;
        Debug.Log("Values Reset");
    }

    public IEnumerator SpawnHitCircle()
    {
        while (true)
        {
            if (miniGame.activeInHierarchy && !gm.clicked)
            {
                if (hitCol.OverlapPoint(failRect.position))
                {
                    hitMissed = true;
                    hitCircle.rectTransform.localPosition = spwnRect.localPosition;
                    hitCircle.canvasRenderer.SetAlpha(0f);
                    hitMissed = false;
                    //possibly startle animal or some kind of debuff?
                }
                else if (miniGame.activeInHierarchy && !hitMissed)
                {
                    HitMove();
                }
            }
            yield return null;
        }
    }

    void HitMove()
    {
        //beginning logic for moving the hit circles
        hitMove = Vector3.MoveTowards(hitCircle.rectTransform.position, failRect.position, beatInterval * Time.deltaTime);
        hitCircle.rectTransform.position = hitMove;
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
                beatCounter.canvasRenderer.SetAlpha(1f);

                if (beatCount > 4)
                {
                    beatCount = 1;
                }

                if (beatCounter != null)
                {
                    beatCounter.text = beatCount.ToString();
                    metroCircle.CrossFadeAlpha(0.0f, secsPerBeat, false);
                    beatCounter.CrossFadeAlpha(0.0f, secsPerBeat, false);
                }
            }
            yield return new WaitForSeconds(beatInterval);
        }
    }
}
