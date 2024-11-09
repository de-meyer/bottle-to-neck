using System;
using System.Collections;
using System.Collections.Generic;
using System.Media;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
  [SerializeField] private AudioSource musicPlayer;
  [SerializeField] private int afterMuffleSoundPercent;
  [SerializeField] private AudioListener audioListener;
  [SerializeField] private int trialLeniencyAmount;
  [SerializeField] private AudioSource victorySoundPlayer;

  private int currentScore;

  // real life measurements
  const int volumeWhenFull = 41;                               // Milliliters
  const int maxBottleLevel = 330;                              // Milliliters
  const int volumeWhenEmpty = volumeWhenFull + maxBottleLevel; // Milliliters
  const float frequencyWhenFull = 1376;                        // Hertz
  const float frequencyWhenEmpty = 172;                        // Hertz

  // these result in a total amount of 5.28-8.25, so 6-9 sips
  const int averageSipSize = 50;                               // Milliliters
  const float maxSipNoise = .25f;

  public List<int> frequencies;

  void Start()
  {
    frequencies = GenerateFrequencies();
    string log = "Frequencies: ";
    foreach (int frequency in frequencies)
    {
      log += frequency + ", ";
    }
    Debug.Log(log);
  }

  List<int> GenerateFrequencies()
  {
    int bottleLevel = maxBottleLevel;

    // add sips as long as bottle is not empty
    List<int> sips = new List<int>();
    while (bottleLevel > 0)
    {
      float sipNoise = UnityEngine.Random.Range(-maxSipNoise, maxSipNoise);
      // last sip might be bigger than possible due to remaining volume, but that gets
      // corrected in the next step
      // TODO: remember to not actually trigger a turn for the last sip since it will
      // always be for the empty bottle
      int sipSize = Mathf.FloorToInt(averageSipSize * (1 + sipNoise));
      sips.Add(sipSize);
      bottleLevel -= sipSize;
    }

    // // Log values of single sips
    // string log = "Sips: ";
    // foreach (int sip in sips)
    // {
    //   log += sip + ", ";
    // }
    // Debug.Log(log);

    // calculate volumes from sips
    int airVolume = volumeWhenFull;
    List<int> airVolumesAfterEachSip = new List<int>();
    foreach (int sip in sips)
    {
      if (airVolume + sip > volumeWhenEmpty)
      {
        airVolume = volumeWhenEmpty;
      }
      else
      {
        airVolume += sip;
      }
      airVolumesAfterEachSip.Add(airVolume);
    }

    // // log airVolumesAfterEachSip
    // log = "Volumes after each sip: ";
    // foreach (int volume in airVolumesAfterEachSip)
    // {
    //   log += volume + ", ";
    // }
    // Debug.Log(log);


    // translate volume to frequency
    List<int> frequencies = new List<int>();
    foreach (int volume in airVolumesAfterEachSip)
    {
      int frequency = TranslateVolumeToFrequency(volume);
      frequencies.Add(frequency);
    }

    // // log frequencies
    // log = "Frequencies: ";
    // foreach (float frequency in frequencies)
    // {
    //   log += frequency + ", ";
    // }
    // Debug.Log(log);

    return frequencies;
  }

  int TranslateVolumeToFrequency(int volume)
  {

    int frequency = Mathf.FloorToInt(frequencyWhenEmpty + (volume - volumeWhenEmpty) * (frequencyWhenFull - frequencyWhenEmpty) / (volumeWhenFull - volumeWhenEmpty));

    return frequency;
  }


  /** idealValue is the Hz value we are looking for,
  @return the score on a scale from 1 to 100
  */
  public void TriggerTrialPhase(int idealValue, int recLenghInSec)
  {
    Debug.Log("StartedTrialPhase");
    StartCoroutine(TrialPhaseCoroutine(idealValue, recLenghInSec));
  }

  public int getScore()
  {
    return currentScore;
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
    //Debug.Log("Closest Value: " + actualValue);
    float idealVolume = TranslateFrequencyToVolume(idealValue);
    float actualVolume = TranslateFrequencyToVolume(actualValue);
    float calcScore = 0f;
    Debug.Log("DifferenceVolume: " + Math.Abs(idealVolume - actualVolume));
    calcScore = Math.Abs(idealVolume - actualVolume) / 300;
    calcScore *= 100;
    int score = (int)Math.Round(100 - calcScore);
    return score;

  }

  void playVictorySound(int goalPitch)
  {
    Debug.Log("Victoryyyyy " + 233.0f / goalPitch);
    victorySoundPlayer.pitch = 233.0f / goalPitch;
    victorySoundPlayer.Play();
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

  // returns the volume or air (including the bottleneck of 41ml) resulting in 41ml - 371ml
  float TranslateFrequencyToVolume(float frequency)
  {
    float volume = volumeWhenEmpty + (frequency - frequencyWhenEmpty) * (volumeWhenFull - volumeWhenEmpty) / (frequencyWhenFull - frequencyWhenEmpty);

    return volume;
  }
  private IEnumerator TrialPhaseCoroutine(int idealValue, int recLenghInSec)
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
    yield return new WaitForSeconds(recLenghInSec);

    // Restore the old volume
    musicPlayer.volume = oldVolume;

    // Stop recording
    audioListener.StopRecording();

    currentScore = calculateScore(audioListener.getHzScores(), idealValue);

    Debug.Log("End Trigger Trial Phase " + currentScore);
  }
}
