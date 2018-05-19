using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassThroughPlatform : MonoBehaviour {

    private BoxCollider2D boxColl;
    private PlayerController player;
    public BoxCollider2D jumpthroughBox;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        boxColl = gameObject.GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (Input.GetAxisRaw("Vertical") <0 )
        {
            boxColl.isTrigger = true;
            jumpthroughBox.isTrigger = true;
        }

     
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        boxColl.isTrigger = false;
        jumpthroughBox.isTrigger = false;

    }
}

