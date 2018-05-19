using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class LifeManager : MonoBehaviour {

    //public int startingLives;
    private int lifeCounter;

    private Text theText;

    public GameObject gameOverScreen;

    public PlayerController player;

    public string mainMenu;

    public float waitAfterGameOver;

	// Use this for initialization
	void Start () {
        theText = GetComponent<Text>();

        lifeCounter = PlayerPrefs.GetInt("PlayerCurrentLives");

        player = FindObjectOfType<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {
       if (lifeCounter < 0)
        {
            gameOverScreen.SetActive(true);
            player.gameObject.SetActive(false);
        }

        theText.text = "x " + lifeCounter;

        if (gameOverScreen.activeSelf)
        {
            waitAfterGameOver -= Time.deltaTime;
        }

        if (waitAfterGameOver <0)
        {
            SceneManager.LoadScene("Start Menu");

        }

    }

    public void GiveLife()
    {
        lifeCounter++;
        PlayerPrefs.SetInt ("PlayerCurrentLives", lifeCounter);
    }

    public void TakeLife()
    {
        lifeCounter--;
        PlayerPrefs.SetInt("PlayerCurrentLives", lifeCounter);

    }
}
