using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Globalization;
using UnityEngine.Advertisements;

public class GameManager : MonoBehaviour {
    private AudioSource[] allAudio;
    public delegate void GameDelegate();
    public static event GameDelegate OnGameStarted;
    public static event GameDelegate OnGameOverConfirmed;

    static DateTime localDate = System.DateTime.Now;
    static DateTime termEnd = new System.DateTime(2021, 1, 20);
    static DateTime scoreDate = localDate;
    //string daysLeft = termEnd.Subtract(localDate).Days.ToString();

    static string date;

    // static TimeSpan span = termEnd.Subtract(localDate);
    int daysLeft = 0;//(termEnd - localDate).Days;
    public static GameManager Instance;
    public static int Player_Lives = 3;

    static bool dead = false;
    static bool impeached = false;
    public GameObject startPage;
    public GameObject gameOverPage;
    public GameObject countdownPage;
    public Text scoreText;
    public Text daysTitle;
    public Text impeachTitle;
    public Text daysLeftText;
    public Text deadTitle;
    public GameObject[] lives;
    public AudioSource[] dieSound;
    public GameObject trumpHeads;
    //public AudioSource gameAudio;
    enum PageState
    {
        None,
        Start,
        GameOver,
        Countdown
    }


    int score = 0;
    public static bool gameOver = false;
    static bool gameStarted = false;
    public bool GameOver {  get { return gameOver; } }
    public bool InMotion { get { return gameStarted; } }
    public void SetMotionStatus(bool gameStarted) { gameStarted = this; } //setting bool gameStarted


    void Awake()
    {
        allAudio = FindObjectsOfType(typeof(AudioSource)) as AudioSource[]; //finds all audio and stores it in allAudio array
        Instance = this;    //initializes the game
    }

    void OnEnable() //when object active, do these things
    {
        CountdownText.OnCountdownFinished += OnCountdownFinished;
        TapController.OnPlayerDied += OnPlayerDied;
        TapController.OnPlayerScored += OnPlayerScored;
        //scoreText.text = scoreDate.Month.ToString() + "/" + scoreDate.Day.ToString() + "/" + (scoreDate.Year - 2000).ToString();
    }

    void OnDisable()
    {
        CountdownText.OnCountdownFinished -= OnCountdownFinished;
        TapController.OnPlayerDied -= OnPlayerDied;
        TapController.OnPlayerScored -= OnPlayerScored;
    }


    void Update()
    {
        date = scoreDate.Month.ToString() + "/" + scoreDate.Day.ToString() + "/" + (scoreDate.Year - 2000).ToString();
        
        if (impeached)
        {
            impeached = false;

            daysLeft = 0;// (termEnd - localDate).Days;
            Player_Lives = 3;
            scoreDate = localDate;
        }
    }



    void OnCountdownFinished()
    {
        //gameAudio.Play();
        SetPageState(PageState.None);
        OnGameStarted(); //event sent to TapController
        //score = daysLeft;
        gameOver = false;
        gameStarted = true;
    }


    void OnPlayerDied()
    {
        foreach (AudioSource thing in allAudio)
        {
            thing.Stop(); //stops all audio
        }
        gameOver = true;
        int sound = Player_Lives - 1;
        lives[Player_Lives - 1].active = true;
        Player_Lives--;
        
        int savedScore = PlayerPrefs.GetInt("HighScore");
        if (daysLeft > savedScore && Player_Lives == 0)
        {
            PlayerPrefs.SetInt("HighScore", daysLeft);

        }
        if(Player_Lives == 0)
        {
            dead = true;
            impeached = true;
            if (Advertisement.IsReady("video"))
                Advertisement.Show("video");
            
        }
        switch(Player_Lives)
        {
            case 2:
                deadTitle.text = "  REALLY!?";
                break;
            case 1:
                deadTitle.text = " YOU'RE ON THIN ICE!";
                break;
            case 0:
                deadTitle.text = " IMPEACHED!";
                break;
            default:
                break;

        }
        dieSound[sound].Play();
        SetPageState(PageState.GameOver);
        
        //AudioSource[] audios = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
        //audios.Stop(0);
    }

    void OnPlayerScored()
    {

        scoreDate = scoreDate.AddDays(1);
        daysLeft++;
        //score--;
        //scoreText.text = "DAYS LEFT: " + score.ToString();
        scoreText.text = scoreDate.Month.ToString() + "/" + scoreDate.Day.ToString() + "/" + (scoreDate.Year - 2000).ToString();
        daysLeftText.text = daysLeft.ToString();
    }



    void SetPageState(PageState state)
    {
        switch (state)
        {
            case PageState.None:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                break;
            case PageState.Start:
                startPage.SetActive(true);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                break;
            case PageState.GameOver:
                startPage.SetActive(false);
                gameOverPage.SetActive(true);
                countdownPage.SetActive(false);
                break;
            case PageState.Countdown:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(true);
                break;
        }
    }

     public void ConfirmGameOver()
    {
        //activated when replay button is hit
        if(dead)
        {
            scoreText.text = null;
            daysTitle.text = null;
            daysLeftText.text = null;
            impeachTitle.text = null;
            OnGameOverConfirmed(); //event sent TapController
            SetPageState(PageState.Start);
            dead = false;
            for(int i = 0; i < lives.Length; i++)
            {
                lives[i].active = false;
            }
            trumpHeads.active = false;
        }

        else
        {
            OnGameOverConfirmed();

            SetPageState(PageState.Countdown);
            /*
            scoreText.text = null;
            daysTitle.text = null;
            daysLeftText.text = null;
            impeachTitle.text = null;
            OnGameOverConfirmed(); //event sent TapController
            SetPageState(PageState.Start);
            */
        }

        
        //scoreText.text = null;//scoreDate.Month.ToString() + "/" + scoreDate.Day.ToString() + "/" + (scoreDate.Year - 2000).ToString(); 
    }

    public void StartGame()
    {
        scoreText.text = scoreDate.Month.ToString() + "/" + scoreDate.Day.ToString() + "/" + (scoreDate.Year - 2000).ToString();        //activated when play button is hit
        impeachTitle.text = "Impeachment Date:";
        daysTitle.text = "Days Until Impeachment:";
        daysLeftText.text = daysLeft.ToString();
        SetPageState(PageState.Countdown);
        trumpHeads.active = true;
    }
}
