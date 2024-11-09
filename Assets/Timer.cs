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
      audioSource.Play();
    }
  }
}
