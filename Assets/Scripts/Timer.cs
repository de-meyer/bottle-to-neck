using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Data;

public class Timer : MonoBehaviour
{
  [SerializeField] private AudioManager audioManager;
  public AudioSource audioSource;
  public TextMeshProUGUI timeText;
  public bool timerIsRunning = false;
  public int duration = 3;
  int timeCounter;

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
    timeCounter = duration;
    timeText.text = duration.ToString();

    // translate frequency to pitch
    foreach (float frequency in gameManager.frequencies)
    {
      // random pitch between 1 and 8
      float pitch = frequency / frequencyWhenEmpty;
      pitches.Add(pitch);
    }
    InvokeRepeating("DisplayTime", 0, 1);
  }


  void DisplayTime()
  {
    Debug.Log("seconds: " + timeCounter);
    if (timeCounter == 0)
    {
      Debug.Log("Stopping Timer");
      timeText.text = string.Empty;
      PlayPitchedBottleBlowSound();
      CancelInvoke("DisplayTime");
      return;
    }

    timeText.text = timeCounter.ToString();
    audioManager.PlaySound(10 + timeCounter);

    timeCounter--;
  }

  private void PlayPitchedBottleBlowSound()
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
