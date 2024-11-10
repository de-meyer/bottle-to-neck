using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
  [SerializeField] private AudioManager audioManager;
  public AudioSource audioSource;
  public TextMeshProUGUI timeText;
  public bool timerIsRunning = false;
  public float duration = 3;
  float timeRemaining;
  List<float> pitches = new List<float>();
  [SerializeField] private GameLoop gameLoop;

  [SerializeField] private Animator uiAnimation;

  const float frequencyWhenEmpty = 172;                        // Hertz

  [SerializeField] private GameManager gameManager;
  private static readonly int IsRunning = Animator.StringToHash("isRunning");

  public void StartTimer()
  {
    // Starts the timer automatically
    uiAnimation.SetBool(IsRunning, true);
    timerIsRunning = true;
    timeRemaining = duration;
    timeText.text = duration.ToString();

    // translate frequency to pitch
    foreach (float frequency in gameManager.frequencies)
    {
      // random pitch between 1 and 8
      float pitch = frequency / frequencyWhenEmpty;
      pitches.Add(pitch);
    }
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
          uiAnimation.SetBool(IsRunning, false);
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
    audioManager.PlaySound(10+seconds);
    if (seconds > 0)
    {
      timeText.text = seconds.ToString();
    }
    else
    {
      timeText.text = string.Empty;
    }
  }

  private void PlaySound()
  {
    // 1f   pitch results in   172Hz
    // // 7.2f pitch results in 1.375Hz (only by testing not by math)
    // 18   pitch results in 1.376Hz

    int index = gameLoop.currentRound;
    Debug.Log("Playing sound of round " + index
      + " with frequency " + gameManager.frequencies[index]
      + " and pitch " + pitches[index])
    ;
    audioSource.pitch = pitches[index];
    Debug.Log("Pitch set to " + pitches[index]);
    audioSource.Play();
  }
}
