using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TextPrompts : MonoBehaviour
{
    public TextAsset theText;
    // public GameObject image;

    public int startLine;
    public int endingLine;

    public TextBoxManager theTextBox;

    public bool requireButtonPress;
    private bool waitForPress;

    public bool destroyWhenActivated;

    public PlayerController player;



    // Use this for initialization
    void Start()
    {
        theTextBox = FindObjectOfType<TextBoxManager>();
        player = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (waitForPress && Input.GetKeyDown(KeyCode.C))
        {
            theTextBox.stopPlayerMovement = true;
         
            theTextBox.ReloadScript(theText);
            theTextBox.currentLine = startLine;
            theTextBox.endLine = endingLine;
            theTextBox.EnableTextBox();

            if (destroyWhenActivated)
            {
                Destroy(gameObject);
            }



        }

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //image.GetComponent<SpriteRenderer>().enabled = true;

        if (other.name == "Player")
        {
            if (requireButtonPress)
            {
                waitForPress = true;
                return;
            }


            theTextBox.ReloadScript(theText);
            theTextBox.currentLine = startLine;
            theTextBox.endLine = endingLine;
            theTextBox.EnableTextBox();

            if (destroyWhenActivated)
            {
                Destroy(gameObject);
            }


        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        //  image.GetComponent<SpriteRenderer>().enabled = false;
        {
            if (other.name == "Player")
            {
                waitForPress = false;
            }
        }
    }
}
