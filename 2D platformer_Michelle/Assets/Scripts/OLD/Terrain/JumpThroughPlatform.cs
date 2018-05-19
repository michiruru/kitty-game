using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpThroughPlatform : MonoBehaviour {
    private BoxCollider2D boxColl;
    private PlayerController player;
    public BoxCollider2D passThrough;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        boxColl = gameObject.GetComponent<BoxCollider2D>();
    }

    void Update()
    {
 
        if (Input.GetButtonDown("Jump") && player.grounded)
        {
            boxColl.isTrigger = true;
            passThrough.isTrigger = true;
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        boxColl.isTrigger = false;
        passThrough.isTrigger = false;
    }
}
