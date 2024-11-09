using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameLoop : MonoBehaviour
{
  #region variables 
  
  [SerializeField] private Animator charAnimatorP1;
  [SerializeField] private Animator charAnimatorP2;
  [SerializeField] private GameObject startCanvas;
  [SerializeField] private GameObject gameCanvas;
  [SerializeField] private GameObject gameOverCanvas;
  [SerializeField] private TextMeshProUGUI textfield;
  [SerializeField] private TextMeshProUGUI scoreTextfieldPlayer1;
  [SerializeField] private TextMeshProUGUI scoreTextFieldPlayer2;

  [SerializeField] private GameManager gameManager;
  [SerializeField] private AudioManager audioListener; 

  private int currentFrequency;
  private bool player1Recorded = false;
  private bool player2Recorded = false;
  private int points1 = 0;
  private int points2 = 0;
  private int score1 = 0;
  private int score2 = 0;
  private int rounds;
  private int currentRound = 0;
  private List<int> frequencies;
  
  #endregion

  #region public methods
  
  public void StartNewRound()
  {
    scoreTextfieldPlayer1.text = 0.ToString();
    scoreTextFieldPlayer2.text = 0.ToString();
    
    frequencies = gameManager.frequencies;
    rounds = frequencies.Count;

    currentFrequency = frequencies[currentRound];
    
    //startCanvas.gameObject.SetActive(false);
    //gameCanvas.gameObject.SetActive(true);
    
    if (currentRound == rounds) //Check if game is finished
    {
      if(score1 > score2) { BottleToNeck(1); }
      else { BottleToNeck(2); }
    }

    player1Recorded = false;
    player2Recorded = false;
    currentFrequency = frequencies[currentRound];
    currentRound++;
  }

  public void PlayFrequencySound()
  {
    //PlaySound
    StartCoroutine(PlayAnimationAfterSeconds("isDrinking", 2));
  }

  public void StartRecording(int player)
  {
    switch (player)
    {
      case 1:
        if (player1Recorded) { StartCoroutine(SetTextField("You already had your chance!", 3)); }
        else { StartCoroutine(RecordAndScore(1)); }
        break;
      case 2:
        if (player2Recorded) { StartCoroutine(SetTextField("You already had your chance!", 3)); }
        else { StartCoroutine(RecordAndScore(2)); player2Recorded = true; }
        break;
      default:
        break;
    }
  }
  
  #endregion

  #region private methods
  
  public void BottleToNeck(int winner)
  {
    if (winner == 1)
    {
      charAnimatorP1.SetBool("punch", true);
      charAnimatorP2.SetBool("stunned", true);
    }
    else
    {
      charAnimatorP2.SetBool("punch", true);
      charAnimatorP1.SetBool("stunned", true);
    }
  }
  
  #endregion
  
  #region Coroutines 
  
  private IEnumerator PlayAnimationAfterSeconds(string animation, int seconds)
  {
    yield return new WaitForSeconds(seconds);
    charAnimatorP1.SetBool(animation, true);
    charAnimatorP2.SetBool(animation, true);
  }

  private IEnumerator RecordAndScore(int player)
  {
    Debug.Log("Recording");
    //Play Countdown
    int recordingDuration = 3;

    gameManager.TriggerTrialPhase(currentFrequency, recordingDuration);
    yield return new WaitForSeconds(recordingDuration + 1); //Maybe remove +1

    if (player == 1) { points1 = gameManager.getScore(); player1Recorded = true; }
    else if (player == 2) { points2 = gameManager.getScore(); player2Recorded = true; }

    if (player1Recorded && player2Recorded)
    {
      if (points1 > points2) { StartCoroutine(SetTextField("Player One was closer and wins this round", 5)); }
      if (points2 > points1) { StartCoroutine(SetTextField("Player Two was closer and wins this round", 5)); }
      if (points2 == points1) { StartCoroutine(SetTextField("Draw!", 5)); }

      score1 += points1;
      score2 += points2;
      scoreTextfieldPlayer1.text = score1.ToString();
      scoreTextFieldPlayer2.text = score2.ToString();
      points1 = 0;
      points2 = 0;

      Debug.Log("Starting New Round");
      StartNewRound();
    }
    yield return null;
  }

  private IEnumerator SetTextField(string text, int seconds)
  {
    textfield.text = text;
    yield return new WaitForSeconds(seconds);
    textfield.text = "";
  }

  #endregion
}
