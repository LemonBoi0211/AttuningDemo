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
    [SerializeField] public GameObject miniGame;
    [SerializeField] public Image targetCircle;
    [SerializeField] public Image metroCircle;
    [SerializeField] public TMP_Text beatCounter;

    [SerializeField] public int beatCount = 0;
    [SerializeField] float bpm = 120;
    float bps;
    float secsPerBeat;
    float beatInterval;

    private void Start()
    {
        bps = bpm / 60;
        secsPerBeat = 1 / bps;
        beatInterval = secsPerBeat;

        StartCoroutine(IncrementBeat());

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
