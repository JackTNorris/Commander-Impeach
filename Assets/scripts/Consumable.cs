using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class Consumable : MonoBehaviour {

    public static Consumable Instance;


    Collider2D consumable;

    Vector3 prevPos;
    Vector3 newPos;

    void Awake()
    {
        Instance = this;    //initializes the game
    }

    void OnEnable()
    {
        //TapController.OnConsumed += OnConsumed;
    }

    void OnDisable()
    {
        //TapController.OnConsumed -= OnConsumed;
    }
    void Start()
    {
        //consumable = GetComponent<Collider2D>();
        newPos.Set(0, 100, 0);
    }
    





  
    
	// Use this for initialization

}
