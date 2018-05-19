using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBoxManager : MonoBehaviour {
    public GameObject textBox;

    public Text theText;

    public TextAsset textFile;
    public string[] textlines;

    public int currentLine;
    public int endLine;

    public PlayerController player;

    public bool isActive;

    public bool stopPlayerMovement;

    // Use this for initialization
    void Start()
    {
        player = FindObjectOfType<PlayerController>();

        if (textFile != null)
        {
            textlines = (textFile.text.Split('\n'));
        }

        if(endLine == 0)
        {
            endLine = textlines.Length - 1;
        }

        if (isActive)
        {
            EnableTextBox();
        }
        else
        {
            DisableTextBox();
        }
    }


    void Update()
    {
        if (!isActive)
        {
            return;
        }

        theText.text = textlines[currentLine];

        if(Input.GetButtonDown("Jump"))
        {
            currentLine += 1;
        }

        if(currentLine > endLine)
        {
            DisableTextBox();
        }
    }


    public void EnableTextBox()
    {
        textBox.SetActive(true);
        isActive = true;

        if(stopPlayerMovement)
        {
            player.canMove = false;
        }
    }

    public void DisableTextBox()
    {
        textBox.SetActive(false);
        isActive = false;


        if (stopPlayerMovement)
            { player.canMove = true;
        }
    }

    public void ReloadScript(TextAsset theText)
    {
      if(theText != null)
        {
            textlines = new string[1];
            textlines = (theText.text.Split('\n'));

        }
    }
}
