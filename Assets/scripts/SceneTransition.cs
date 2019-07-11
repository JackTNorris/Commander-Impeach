using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour {

    // Use this for initialization
    GameManager game;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            ChangeToHomeScreen();
        }
    }


    void Start()
    {
        //game = GameManager.Instance;
    }
    public void ChangeToGame()
    {
        GameManager.gameOver = true;
        SceneManager.LoadScene(1);
    }
    
    public void ChangeToHomeScreen()
    {
        SceneManager.LoadScene(0);
        //game.SetMotionStatus(false);
    }
    public void ChangeToInfoScreen()
    {
        SceneManager.LoadScene(2);
    }

    public void OnClickPrivacyPolicy()
    {
        Application.OpenURL("https://commander-impeach.flycricket.io/privacy.html");
    }
}
