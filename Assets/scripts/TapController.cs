using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class TapController : MonoBehaviour
{

    public delegate void PlayerDelegate(); //delegates allow functions to be passed around like variables
    public delegate void ConsumerDelegate(Collider2D col);
    public static event ConsumerDelegate OnConsumed;
    public delegate void PassLabelDelegate(Collider2D col);
    public static event PassLabelDelegate OnPassLabel;
    

    public static event PlayerDelegate OnPlayerDied;
    public static event PlayerDelegate OnPlayerScored;
   // public static event PlayerDelegate OnPassLabel;

    public AudioSource gameAudio;
    //public AudioSource flapAudio;

    public float tapForce = 10;
    public float tiltSmooth = 5;
    public Vector3 startPos;
    //Vector3 consumablePos;
    bool gameStart = false;
    Rigidbody2D rigidbody;
    Quaternion downRotation;//quat = for rodation.
    Quaternion forwardRotation;

    bool gameInMotion = false;
    GameManager game;


    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        downRotation = Quaternion.Euler(0, 0, -60);
        forwardRotation = Quaternion.Euler(0, 0, 20);
        rigidbody.simulated = false;
        game = GameManager.Instance;
       
    }

    void OnCountdownFinished()
    {
        
        rigidbody.simulated = true;
       // gameInMotion = true;
    }


    void OnEnable()
    {
        GameManager.OnGameStarted += OnGameStarted;
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
       // Parallaxer.OnPassLabel += OnPassLabel;
    }

    void OnDisable()
    {
        GameManager.OnGameStarted -= OnGameStarted;
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
       // Parallaxer.OnPassLabel -= OnPassLabel;
    }

    void OnGameStarted()
    {

        gameAudio.Play();
        rigidbody.velocity = Vector3.zero;
        rigidbody.simulated = true; //applies gravity to trump
      
        //game.SetMotionStatus(true); //used to make sure trump don't move before everything begins to be in motion
    }

    void OnGameOverConfirmed()
    {
        
        transform.localPosition = startPos;
        transform.rotation = Quaternion.identity;
        game.SetMotionStatus(false);
    }



    void Update()
    {
        if (game.GameOver)
        {
            return;
        }
        if (game.InMotion)
        {
            if (Input.GetMouseButtonDown(0))//0 means left click
            {
                transform.rotation = forwardRotation;
                rigidbody.velocity = Vector3.zero;
                rigidbody.AddForce(Vector2.up * tapForce, ForceMode2D.Force);
                //flapAudio.Play();
            }

                transform.rotation = Quaternion.Lerp(transform.rotation, downRotation, tiltSmooth * Time.deltaTime);
        }

        
        

    }

    void OnTriggerEnter2D(Collider2D col)//on colision with another object, the object that you hit is sent to this function
    {
        if (col.gameObject.tag == "ScoreZone")//col means collilsion
        {
            //register a score event
            OnPlayerScored(); //event sent to GameManager;
            //OnPassLabel(); //event sent to parallaxer
            //play sound
        }

        if (col.gameObject.tag == "DeadZone")
        {
            gameAudio.Stop();
            rigidbody.simulated = false;
            //register a dead event
            OnPlayerDied();     //event sent to GameManager
            //play a sound
        }

        if (col.gameObject.tag == "Consumable")
        {
            //col.gameObject.transform.position = consumablePos;
            OnConsumed(col);
        }
        if (col.gameObject.tag == "Label")
        {
            OnPassLabel(col);
        }
    }
}
