using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicPlayer;
    [SerializeField] private int afterMuffleSoundPercent;
    [SerializeField] private int trialLengthSec;
    [SerializeField] private AudioListener audioListener;
    [SerializeField] private int trialLeniencyAmount;
    //[SerializeField] private int trialLeniencyFrequency;
    [SerializeField] private Text scoreDisplay;
    
    
    /** idealValue is the Hz value we are looking for,
    @return the score on a scale from 1 to 100
    */
    public void TriggerTrialPhase(int idealValue)
    {
        Debug.Log("StartedTrialPhase");
        StartCoroutine(TrialPhaseCoroutine(idealValue));
        //return calculateScore(audioListener.getHzScores(), idealValue);
    }

    private int calculateScore(List<int> hzScores, int idealValue)
    {
        Dictionary<int, int> scoreMap = new Dictionary<int, int>();
        foreach (int number in hzScores)
        {
            if (!scoreMap.ContainsKey(number))
            {
                scoreMap.Add(number, 1);
            }
            else
            {
                scoreMap[number] += 1;
            }
        }
        //int actualValue = FindKeyWithClosestValue(scoreMap, idealValue);
        int actualValue = FindKeyWithHighestValue(scoreMap);
        Debug.Log("Closest Value: " + actualValue);
        int score = 100 - Math.Abs(idealValue-actualValue);
        if (score > 0)
        {
            return score;
        }
        return 0;
        
    }

    // Method to find the key associated with the closest value to the target
    private int FindKeyWithClosestValue(Dictionary<int, int> map, int targetValue)
    {
        int closestKey = -1; // Initial value for closestKey with an invalid placeholder key
        int smallestDifference = int.MaxValue; // Start with the largest possible difference

        // Iterate over each key-value pair in the dictionary
        foreach (var pair in map)
        {
            int key = pair.Key;
            int value = pair.Value;

            // Only consider keys with values higher than the trialLeniencyAmount
            if (value > trialLeniencyAmount)
            {
                // Calculate the absolute difference between the current value and the target
                int difference = Math.Abs(value - targetValue);

                // If the current difference is smaller than the smallest difference found so far, update
                if (difference < smallestDifference)
                {
                    smallestDifference = difference; // Update smallest difference
                    closestKey = key; // Update closest key
                }
            }
        }
        return closestKey; // Return the key with the closest value that meets the criteria
    }

    private int FindKeyWithHighestValue(Dictionary<int, int> map)
    {
        int keyWithHighestValue = -1; // Initial value for key with a placeholder
        int highestValue = int.MinValue; // Start with the smallest possible integer

        // Iterate over each key-value pair in the dictionary
        foreach (var pair in map)
        {
            int key = pair.Key;
            int value = pair.Value;

            // If the current value is greater than the highest value found so far, update
            if (value > highestValue)
            {
                highestValue = value; // Update highest value
                keyWithHighestValue = key; // Update key with highest value
            }
        }
        return keyWithHighestValue; // Return the key with the highest value
    }
    private IEnumerator TrialPhaseCoroutine(int idealValue)
    {
        Debug.Log("Trigger Trial Phase");
        float oldVolume = musicPlayer.volume;
        
        // Adjust the volume based on afterMuffleSoundPercent
        musicPlayer.volume *= 100f / afterMuffleSoundPercent;
        
        // Set volume to 0.5 as per earlier logic
        musicPlayer.volume = 0.5f;
        
        // Start recording
        audioListener.StartRecording();
        
        // Wait for trialLengthSec seconds
        yield return new WaitForSeconds(trialLengthSec);
        
        // Restore the old volume
        musicPlayer.volume = oldVolume;
        
        // Stop recording
        audioListener.StopRecording();

        scoreDisplay.text = calculateScore(audioListener.getHzScores(), idealValue).ToString();

        Debug.Log("End Trigger Trial Phase");
    }
}