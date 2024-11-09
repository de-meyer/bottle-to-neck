using System.Collections;
using UnityEngine;
using TMPro;

public class GameLoop : MonoBehaviour
{
    public enum GameState { Start, Listening, Recording, Scoring, Idle, GameOver }
    public GameState currentState = GameState.Start;

    [SerializeField] private Animator charAnimatorP1;
    [SerializeField] private Animator charAnimatorP2;
    [SerializeField] private GameObject startCanvas;
    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private TextMeshProUGUI textfield;
    [SerializeField] private TextMeshProUGUI scoreTextfieldPlayer1;
    [SerializeField] private TextMeshProUGUI scoreTextFieldPlayer2;
    
    [SerializeField] private GameManager gameManager;

    public int currentFrequency;
    private bool player1Recorded = false;
    private bool player2Recorded = false;
    private int points1 = 0;
    private int points2 = 0;
    private int score1 = 0;
    private int score2 = 0;
    private int rounds;
    private int currentRound = 0;

    public void StartGame()
    {
        startCanvas.gameObject.SetActive(false);
        gameCanvas.gameObject.SetActive(true);
        StartNewRound();
    }

    public void PlayFrequencySound()
    {
        //PlaySound
        StartCoroutine(PlayAnimationAfterSeconds("isDrinking", 2));
    }

    private IEnumerator PlayAnimationAfterSeconds(string animation, int seconds)
    {
        yield return new WaitForSeconds(seconds);
        charAnimatorP1.SetBool(animation, true);
        charAnimatorP2.SetBool(animation, true);
    }

    private void StartNewRound()
    {
        //Check if game is finished
        //else set new frequency
        player1Recorded = false;
        player2Recorded = false;
        currentRound++;
    }

    private void SetNewFrequency(int freq)
    {
        
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
                else { StartCoroutine(RecordAndScore(2)); player2Recorded = true ; }
                break;
            default:
                break;
        }
    }

    private IEnumerator RecordAndScore(int player)
    {
        Debug.Log("Recording");
        //Play Countdown
        int recordingDuration = 3;
        
        gameManager.TriggerTrialPhase(currentFrequency, recordingDuration);
        yield return new WaitForSeconds(recordingDuration + 1); //Maybe remove +1
        
        if      (player == 1) { points1 = gameManager.getScore(); player1Recorded = true ; } 
        else if (player == 2) { points2 = gameManager.getScore(); player2Recorded = true ;} 
        
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

    public void BottleToNeck(int winner)
    {
        charAnimatorP2.SetBool("punch", true);
        charAnimatorP1.SetBool("stunned", true);
    }
    
    public IEnumerator GameRound()
    {
        //textfield.text = "Drink until you hit this frequency";
        
        
        
        //charAnimator.SetBool(IsDrinking, false);
        
        //Play sound thats needed
        //Start timer 5 sec
        //Record for 5 sec
        //scoring

        yield return null;
    }
}
