using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.UI;

public class GameLoop : MonoBehaviour
{
  #region variables 

  [SerializeField] private Animator charAnimatorP1;
  [SerializeField] private Animator charAnimatorP2;
  //[SerializeField] private GameObject startCanvas;
  [SerializeField] private GameObject gameUI;
  [SerializeField] private GameObject recordButton1;
  [SerializeField] private GameObject recordButton2;
  [SerializeField] private GameObject gameOverCanvas;
  [SerializeField] private Button playFrequencyButton;

  [SerializeField] private TextMeshProUGUI textfield;
  [SerializeField] private TextMeshProUGUI scoreTextfieldPlayer1;
  [SerializeField] private TextMeshProUGUI scoreTextFieldPlayer2;
  [SerializeField] private TextMeshProUGUI recordButtonText1;
  [SerializeField] private TextMeshProUGUI recordButtonText2;
  [SerializeField] private GameManager gameManager;
  [SerializeField] private AudioManager audioListener;

  [SerializeField] private GameObject player1LeftHandBottle;
  [SerializeField] private GameObject player1RightHandBottle;
  [SerializeField] private GameObject player2LeftHandBottle;
  [SerializeField] private GameObject player2RightHandBottle;

  private int currentFrequency;
  private bool player1Recorded = false;
  private bool player2Recorded = false;
  private int points1 = 0;
  private int points2 = 0;
  private int score1 = 0;
  private int score2 = 0;
  private int rounds;
  public int currentRound = 0;
  private List<int> frequencies;
  private static readonly int Punch = Animator.StringToHash("punch");
  private static readonly int Stunned = Animator.StringToHash("stunned");

  #endregion

  #region public methods

  public void Restart()
  {
    charAnimatorP1.SetBool(Punch, false);
    charAnimatorP2.SetBool(Punch, false);
    charAnimatorP1.SetBool(Stunned, false);
    charAnimatorP2.SetBool(Stunned, false);
    player1LeftHandBottle.SetActive(true);
    player2LeftHandBottle.SetActive(true);
    player1RightHandBottle.SetActive(false);
    player2RightHandBottle.SetActive(false);

    gameOverCanvas.SetActive(false);
    gameUI.SetActive(true);
    points1 = 0;
    points2 = 0;
    score1 = 0;
    score2 = 0;
    currentRound = 0;
    frequencies = gameManager.GenerateFrequencies();
    // Log all frequencies
    rounds = frequencies.Count;
    Debug.Log("Frequencies: " + string.Join(", ", frequencies));
    StartNewRound();
  }

  // ReSharper disable Unity.PerformanceAnalysis
  public void StartNewRound()
  {
    recordButtonText1.text = "Record";
    recordButtonText2.text = "Record";
    recordButton1.SetActive(false);
    recordButton2.SetActive(false);
    recordButton1.GetComponent<Button>().interactable = true;
    recordButton2.GetComponent<Button>().interactable = true;

    scoreTextfieldPlayer1.text = score1.ToString();
    scoreTextFieldPlayer2.text = score2.ToString();

    currentFrequency = frequencies[currentRound];

    if (currentRound == rounds - 1) //Check if game is finished
    {
      if (score1 > score2) { BottleToNeck(1); }
      else { BottleToNeck(2); }
      return;
    }

    player1Recorded = false;
    player2Recorded = false;
    currentRound++;
  }

  public void PlayFrequencySound()
  {
    StartCoroutine(PlayAnimationAfterSeconds("isDrinking", 5));

    if (!player1Recorded && !player2Recorded)
    {
      recordButton1.SetActive(true);
      recordButton2.SetActive(true);
    }
  }

  public void StartRecording(int player)
  {
    switch (player)
    {
      case 1:
        if (player1Recorded)
        {
          StartCoroutine(SetTextField("You already had your chance!", 3));
        }
        else
        {
          StartCoroutine(RecordAndScore(1));
          recordButton1.GetComponent<Button>().interactable = false;
        }
        break;
      case 2:
        if (player2Recorded)
        {
          StartCoroutine(SetTextField("You already had your chance!", 3));
        }
        else
        {
          StartCoroutine(RecordAndScore(2)); player2Recorded = true;
          recordButton2.GetComponent<Button>().interactable = false;
        }
        break;
      default:
        break;
    }
  }

  public void BottleToNeck(int winner)
  {
    gameUI.gameObject.SetActive(false);
    gameOverCanvas.SetActive(true);
    if (winner == 1)
    {
      StartCoroutine(SetTextField("Congrats Player One!", 5));
      charAnimatorP1.SetBool(Punch, true);
      player1RightHandBottle.SetActive(true);
      player1LeftHandBottle.SetActive(false);
      StartCoroutine(WaitForStunned(2.1f, charAnimatorP2));

    }
    else
    {
      StartCoroutine(SetTextField("Congrats Player Two!", 5));
      charAnimatorP2.SetBool(Punch, true);
      player2RightHandBottle.SetActive(true);
      player2LeftHandBottle.SetActive(false);
      StartCoroutine(WaitForStunned(2.1f, charAnimatorP2));
    }
    audioListener.PlaySound(1);
  }

  public void CloseApplication()
  {
#if UNITY_EDITOR
    EditorApplication.isPlaying = false;
#endif
    Application.Quit();
  }

  #endregion

  #region Coroutines 

  private IEnumerator PlayAnimationAfterSeconds(string animation, int seconds)
  {
    playFrequencyButton.interactable = false;
    recordButton1.GetComponent<Button>().interactable = false;
    recordButton2.GetComponent<Button>().interactable = false;
    charAnimatorP1.SetBool(animation, true);
    charAnimatorP2.SetBool(animation, true);
    yield return new WaitForSeconds(seconds);
    charAnimatorP1.SetBool(animation, false);
    charAnimatorP2.SetBool(animation, false);
    playFrequencyButton.interactable = true;
    recordButton1.GetComponent<Button>().interactable = true;
    recordButton2.GetComponent<Button>().interactable = true;
  }

  private IEnumerator Wait(int seconds)
  {
    yield return new WaitForSeconds(seconds);
  }

  private IEnumerator WaitForStunned(float seconds, Animator character)
  {
    yield return new WaitForSeconds(seconds);
    character.SetBool(Stunned, true);
  }

  private IEnumerator RecordAndScore(int player)
  {
    StartCoroutine(SetTextField("Recording...", 3));
    int recordingDuration = 3;

    gameManager.TriggerTrialPhase(currentFrequency, recordingDuration);
    yield return new WaitForSeconds(recordingDuration);

    if (player == 1)
    {
      points1 = gameManager.getScore(); player1Recorded = true;
      recordButtonText1.text = points1.ToString();
    }

    if (player == 2)
    {
      points2 = gameManager.getScore(); player2Recorded = true;
      recordButtonText2.text = points2.ToString();
    }

    if (player1Recorded && player2Recorded)
    {
      if (points1 > points2) { StartCoroutine(SetTextField("Player One was closer and wins this round", 5)); }
      if (points2 > points1) { StartCoroutine(SetTextField("Player Two was closer and wins this round", 5)); }
      if (points2 == points1) { StartCoroutine(SetTextField("Draw!", 5)); }

      yield return new WaitForSeconds(5);

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
