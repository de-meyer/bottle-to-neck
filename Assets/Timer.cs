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
  float timeRemaining = 3;
  public bool timerIsRunning = false;

  private void Start()
  {
    // Starts the timer automatically
    Debug.Log("Timer with duration of " + duration + "s started.");
    timerIsRunning = true;
    timeRemaining = duration;
    timeText.text = duration.ToString();
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

    float emptyBottleFrequency = 172f;
    float fullBottleFrequency = 1376f;

    float randomFrequency = Random.Range(emptyBottleFrequency, fullBottleFrequency) + emptyBottleFrequency;
    // random pitch between 1 and 8
    float randomPitch = randomFrequency / emptyBottleFrequency;
    Debug.Log("Frequency set to " + randomFrequency + "Hz, pitch set to " + randomPitch);
    audioSource.pitch = randomPitch;
    Debug.Log("Pitch set to " + randomPitch);
    audioSource.Play();
  }
}
