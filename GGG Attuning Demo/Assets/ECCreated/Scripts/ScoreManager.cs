using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [Header("Score Display")]
    [SerializeField] private TMP_Text scoreText;

    [Header("Score Values")]
    [SerializeField] private int currentScore = 0;
    [SerializeField] private int perfectHitPoints = 100;
    [SerializeField] private int goodHitPoints = 50;
    [SerializeField] private int missPenalty = -25; // Optional: penalize for misses

    [Header("Game End Conditions")]
    [SerializeField] private int scoreToWin = 500; // Target score to reach
    public delegate void GameEndEvent();
    public static event GameEndEvent OnGameWon; // Event to broadcast game won

    private void OnEnable()
    {
        // Subscribe to the OnHitAccuracy event from RhythmGameControl
        RhythmGameControl.OnHitAccuracy += HandleHitAccuracy;
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks and unexpected behavior
        RhythmGameControl.OnHitAccuracy -= HandleHitAccuracy;
    }

    private void Start()
    {
        UpdateScoreDisplay(); // Initialize the score display at the start
    }

    private void HandleHitAccuracy(RhythmGameControl.HitAccuracyType accuracy)
    {
        if (currentScore >= scoreToWin) return; // Prevent score updates after winning

        switch (accuracy)
        {
            case RhythmGameControl.HitAccuracyType.Perfect:
                AddScore(perfectHitPoints);
                Debug.Log("ScoreManager: Perfect hit detected, score updated!");
                break;
            case RhythmGameControl.HitAccuracyType.Good:
                AddScore(goodHitPoints);
                Debug.Log("ScoreManager: Good hit detected, score updated!");
                break;
            case RhythmGameControl.HitAccuracyType.Miss:
                AddScore(missPenalty);
                Debug.Log("ScoreManager: Miss detected, score updated with penalty!");
                break;
        }

        CheckForWinCondition(); // Check after each score update
    }

    public void AddScore(int points)
    {
        currentScore += points;
        currentScore = Mathf.Max(0, currentScore); // Ensure score doesn't go below zero
        UpdateScoreDisplay();
    }

    private void CheckForWinCondition()
    {
        if (currentScore >= scoreToWin)
        {
            Debug.Log("ScoreManager: Win condition met! Score: " + currentScore);
            // Broadcast the OnGameWon event
            OnGameWon?.Invoke();

            // You might want to disable further score increases after winning
            // For example, by unsubscribing or using a boolean flag
            RhythmGameControl.OnHitAccuracy -= HandleHitAccuracy; // Stop listening
        }
    }


    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore.ToString();
        }
        else
        {
            Debug.LogWarning("ScoreManager: scoreText is not assigned. Please assign the TMP_Text component in the Inspector.", this);
        }
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }

    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreDisplay();
    }
}
