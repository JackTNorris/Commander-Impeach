using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpeachableOffenses : MonoBehaviour {
    public delegate string LabelDelegate();
    public static event LabelDelegate LabelName;
    public AudioSource[] china;
    public AudioSource[] putin;
    public AudioSource[] cnn;
    public AudioSource[] collusion;
    public AudioSource[] comey;
    public AudioSource[] immigrants;
    public AudioSource[] kkk;
    public AudioSource[] women;
    public AudioSource[] kimJongUn;
    public AudioSource[] fox;
    public AudioSource[] warren;
    public AudioSource[] hillary;
   // private AudioSource[] allAudio;
    public static ImpeachableOffenses Instance;
    GameManager game;
    // Use this for initialization
    void Start () {
        game = GameManager.Instance;
	}
    void Awake()
    {
        Instance = this;
       // allAudio = FindObjectsOfType(typeof(AudioSource)) as AudioSource[]; //finds all audio and stores it in allAudio array
    }
    void OnEnable()
    {
        
        TapController.OnPlayerDied += OnPlayerDied;
    }
    void OnDisable()
    {
        TapController.OnPlayerDied -= OnPlayerDied;

    }

    public void PlaySound(string name)
    {
        int rnd;
        if(name == "cnn")
        {
            rnd = Random.Range(0, cnn.Length);
            cnn[rnd].Play();
        }
        if(name == "china")
        {
            rnd = Random.Range(0, china.Length);
            china[rnd].Play();
        }
        if(name == "immigrants")
        {
            rnd = Random.Range(0, immigrants.Length);
            immigrants[rnd].Play();
        }
        if (name == "putin")
        {
            rnd = Random.Range(0, putin.Length);
            putin[rnd].Play();
        }
        if (name == "kkk")
        {
            rnd = Random.Range(0, kkk.Length);
            kkk[rnd].Play();
        }
        if(name == "nfl")
        {

        }
        if(name == "comey")
        {
            rnd = Random.Range(0, comey.Length);
            comey[rnd].Play();
        }
        if(name == "collusion")
        {
            rnd = Random.Range(0, collusion.Length);
            collusion[rnd].Play();
        }
        if(name == "women")
        {
            rnd = Random.Range(0, women.Length);
            women[rnd].Play();
        }
        if(name == "kim jong un")
        {
            rnd = Random.Range(0, kimJongUn.Length);
            kimJongUn[rnd].Play();
        }
        if(name == "fox")
        {
            rnd = Random.Range(0, fox.Length);
            fox[rnd].Play();
        }
        if(name == "warren")
        {
            rnd = Random.Range(0, warren.Length);
            warren[rnd].Play();
        }
        if(name == "hillary")
        {
            rnd = Random.Range(0, hillary.Length);
            hillary[rnd].Play();
        }
    }

    void OnPlayerDied()
    {
        //here as an array of the arrays of audio

        
         
    }
    // Update is called once per frame
    void Update () {

    }



}
