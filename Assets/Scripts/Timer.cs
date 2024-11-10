using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
  public AudioSource audioSource;
  public TextMeshProUGUI timeText;
  public bool timerIsRunning = false;
  public float duration = 3;
  float timeRemaining;
  List<float> pitches = new List<float>();
  public int currentRound;

  [SerializeField] private Animator uiAnimation;

  //List<int> sips = new List<int>();
  // const float frequencyWhenFull = 1376;                     // Hertz
  const float frequencyWhenEmpty = 172;                        // Hertz

  public List<float> frequencies = new List<float> { 172, 344, 516, 688, 860, 1032, 1204, 1376 };
  private static readonly int IsRunning = Animator.StringToHash("isRunning");

  public void StartTimer()
  {
    // Starts the timer automatically
    uiAnimation.SetBool(IsRunning, true);
    timerIsRunning = true;
    timeRemaining = duration;
    timeText.text = duration.ToString();

    // translate frequency to pitch
    foreach (float frequency in frequencies)
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
          PlaySound(currentRound);
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
      timeText.text = string.Empty;
    }
  }

  private void PlaySound(int index)
  {
    // 1f   pitch results in   172Hz
    // // 7.2f pitch results in 1.375Hz (only by testing not by math)
    // 18   pitch results in 1.376Hz

    audioSource.pitch = pitches[index];
    Debug.Log("Pitch set to " + pitches[index]);
    audioSource.Play();
  }
}
