using System.Collections;
using UnityEngine;

public class GameLoop : MonoBehaviour
{

    public enum GameState { Start, Listening, Recording, Scoring, Idle, GameOver }
    public GameState currentState = GameState.Start;

    [SerializeField] private Animator charAnimatorP1;
    [SerializeField] private Animator charAnimatorP2;

    //Todo put in one game canvas
    [SerializeField] private GameObject startCanvas;
    [SerializeField] private GameObject gameCanvas;
    private static readonly int IsDrinking = Animator.StringToHash("IsDrinking");

    private void Start()
    {
        //charAnimator = GetComponent<Animator>();
    }

    
    public void StartGame()
    {
        startCanvas.gameObject.SetActive(false);
        gameCanvas.gameObject.SetActive(true);
    }

    public void StartRound()
    {
        StartCoroutine(nameof(GameRound));
    }

    public IEnumerator GameRound()
    {
        Debug.Log("In Coroutine");
        charAnimatorP1.SetBool("isDrinking", true);
        charAnimatorP2.SetBool("isDrinking", true);
        //charAnimator.SetTrigger("IsDrinking");

        yield return new WaitForSeconds(10);
        
        charAnimatorP1.SetBool("isDrinking", false);
        charAnimatorP2.SetBool("isDrinking", false);
        
        //charAnimator.SetBool(IsDrinking, false);
        
        //Play sound thats needed
        //Start timer 5 sec
        //Record for 5 sec
        //scoring
        //Idle
        
        yield return null;
    }
}
