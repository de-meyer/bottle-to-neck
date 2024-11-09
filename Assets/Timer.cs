using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Timer : MonoBehaviour
{
  public float duration = 3;
  public AudioSource audioSource;
  public TextMeshProUGUI timeText;
  float timeRemaining;
  public bool timerIsRunning = false;

  // real life measurements
  const int volumeWhenFull = 41;                               // Milliliters
  const int maxBottleLevel = 330;                              // Milliliters
  const int volumeWhenEmpty = volumeWhenFull + maxBottleLevel; // Milliliters
  const float frequencyWhenFull = 1376;                        // Hertz
  const float frequencyWhenEmpty = 172;                        // Hertz

  // these result in a total amount of 5.28-8.25, so 6-9 sips
  const int averageSipSize = 50;                               // Milliliters
  const float maxSipNoise = .25f;

  List<int> sips = new List<int>();

  private void Start()
  {
    // Starts the timer automatically
    Debug.Log("Timer with duration of " + duration + "s started.");
    timerIsRunning = true;
    timeRemaining = duration;
    timeText.text = duration.ToString();

    int bottleLevel = maxBottleLevel;

    // add sips as long as bottle is not empty
    while (bottleLevel > 0)
    {
      float sipNoise = Random.Range(-maxSipNoise, maxSipNoise);
      // last sip might be bigger than possible due to remaining volume, but that gets
      // corrected in the next step
      // TODO: remember to not actually trigger a turn for the last sip since it will
      // always be for the empty bottle
      int sipSize = Mathf.FloorToInt(averageSipSize * (1 + sipNoise));
      sips.Add(sipSize);
      bottleLevel -= sipSize;
    }

    // Log values of single sips
    string log = "Sips: ";
    foreach (int sip in sips)
    {
      log += sip + ", ";
    }
    Debug.Log(log);

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

    // log airVolumesAfterEachSip
    log = "Volumes after each sip: ";
    foreach (int volume in airVolumesAfterEachSip)
    {
      log += volume + ", ";
    }
    Debug.Log(log);


    // translate volume to frequency
    List<float> frequencies = new List<float>();
    foreach (int volume in airVolumesAfterEachSip)
    {
      float frequency = TranslateVolumeToFrequency(volume);
      frequencies.Add(frequency);
    }

    // log frequencies
    log = "Frequencies: ";
    foreach (float frequency in frequencies)
    {
      log += frequency + ", ";
    }
    Debug.Log(log);

    // translate frequency to pitch
    List<float> pitches = new List<float>();
    foreach (float frequency in frequencies)
    {
      // random pitch between 1 and 8
      float pitch = frequency / frequencyWhenEmpty;
      pitches.Add(pitch);
    }

    // log pitches
    log = "Pitches: ";
    foreach (float pitch in pitches)
    {
      log += pitch + ", ";
    }
    Debug.Log(log);
  }

  void Update()
  {
    if (timerIsRunning)
    {
      if (timeRemaining > 0)
      {
        timeRemaining -= Time.deltaTime;
        // update text mash input each full second
        if (timeRemaining % 1 < Time.deltaTime)
        {
          DisplayTime(timeRemaining);
        }
        if (timeRemaining <= 0)
        {
          PlaySound();
        }
      }
      else
      {
        Debug.Log("Time has run out!");
        timeRemaining = 0;
        timerIsRunning = false;
      }
    }
  }

  void DisplayTime(float timeToDisplay)
  {
    int seconds = Mathf.Max(Mathf.FloorToInt(timeToDisplay), 0);
    if (seconds > 0)
    {
      timeText.text = seconds.ToString();
    }
    else
    {
      timeText.text = "*toooot*";
    }
  }

  void PlaySound()
  {
    // 1f   pitch results in   172Hz
    // // 7.2f pitch results in 1.375Hz (only by testing not by math)
    // 18   pitch results in 1.376Hz

    float randomPitch = 1;
    audioSource.pitch = randomPitch;
    Debug.Log("Pitch set to " + randomPitch);
    audioSource.Play();
  }

  float TranslateVolumeToFrequency(int volume)
  {

    float frequency = frequencyWhenEmpty + (volume - volumeWhenEmpty) * (frequencyWhenFull - frequencyWhenEmpty) / (volumeWhenFull - volumeWhenEmpty);

    return frequency;
  }
}
