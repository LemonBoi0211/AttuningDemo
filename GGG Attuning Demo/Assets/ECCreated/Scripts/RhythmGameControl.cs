using System.Collections;
using System.Collections.Generic;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Build;


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

    GameManager gm;

    /// <summary>
    /// Variables for the Rhythm Minigame
    /// </summary>
    [SerializeField] public int beatCount = 0;
    [SerializeField] float bpm = 120;
    float bps;
    float secsPerBeat;
    float beatInterval;

    private Vector3 hitMove;
    Vector3 defaultPos;

    private void Start()
    {
        gm = gameObject.GetComponent<GameManager>();

        //some basic math to convert the input BPM to a value that the minigame can use
        bps = bpm / 60;
        secsPerBeat = 1 / bps;
        beatInterval = secsPerBeat;

        defaultPos = new Vector3 (0, 360, 0);
    }

    private void OnEnable()
    {
        //starts the beatcounter and pulsing
        StartCoroutine(IncrementBeat());
        StartCoroutine(HitMovement());
    }

    public void ResetValues()
    {
        hitCircle.rectTransform.anchoredPosition = defaultPos;
        beatCount = 1;
        beatCounter.text = beatCount.ToString();
        miniGame.SetActive(false);
        gm.clicked = false;
        Debug.Log("Values Reset");
    }

    public IEnumerator HitMovement()
    {
        while (true)
        {
            if (miniGame.activeInHierarchy)
            {
                //beginning logic for moving the hit circles
                hitMove = Vector3.MoveTowards(hitCircle.rectTransform.position, targetFail.transform.position, beatInterval * Time.deltaTime);
                hitCircle.rectTransform.position = hitMove;
            }
            yield return null;
        }
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
