using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private const int COIN_SCORE_AMOUNT = 5;
    public static GameManager Instance {set; get; }
    private bool isGameStarted = false;
    public bool IsDead{set; get;}
    private PlayerMotor motor;
    
    // UI and UI fields
    public Animator gameCanvas, menuAnim, diamondAnim;
    public Text scoreText, coinText, modifierText, hiscoreText;
    public float score, coinScore, modifierScore;
    private int lastScore;
    private Text deadScoreText, deadcoinText;

    //Death menu
    public Animator deathMenuAnim;


    private void Awake(){
        Instance = this;
        modifierScore = 1;
        motor = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMotor>();

        // modifierText.text = "X" + modifierScore.ToString("0.0");
        // coinText.text = coinScore.ToString("0");
        // scoreText.text = score.ToString("0");

        // hiscoreText.text = PlayerPrefs.GetInt("Hiscore").ToString();
    }

    private void Update(){
    if(MobileInput.Instance.Tap && !isGameStarted){
        isGameStarted = true;
        motor.StartRunning();
        // FindObjectOfType<GlacierSpawner>().IsScrolling = true;
        FindObjectOfType<CameraMotor>().IsMoving = true;
        // gameCanvas.SetTrigger("Show");
        menuAnim.SetTrigger("Hide");
    }

    if (isGameStarted && !IsDead){
        //Bump up the score
        scoreText.text = score.ToString("0");
        score += (Time.deltaTime * modifierScore);
        if (lastScore != (int)score){
            lastScore = (int)score;
            scoreText.text = score.ToString("0");
        }
    }
}

    public void GetCoin(){
        coinScore ++;
        coinText.text = coinScore.ToString("0");
        score += COIN_SCORE_AMOUNT;
        scoreText.text = score.ToString("0");
        diamondAnim.SetTrigger("Collect");
    }

    public void UpdateModifier(float modifierAmount){
        modifierScore = 1.0f + modifierAmount;
        modifierText.text = "X" + modifierScore.ToString("0.0");
    }

    public void OnPlayButton(){
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }

    public void OnDeath(){
        IsDead = true;
        deadScoreText.text = score.ToString("0");
        deadcoinText.text = coinScore.ToString("0");
        deathMenuAnim.SetTrigger("Dead");
        FindObjectOfType<GlacierSpawner>().IsScrolling = false;
        gameCanvas.SetTrigger("Hide");

        //Check if this is a highscore
        if(score > PlayerPrefs.GetInt("Hiscore"));
        {
            float s = score;
            if(s % 1 == 0)
                s += 1;
            PlayerPrefs.SetInt("Hiscore", (int)s);
        }
    }
}