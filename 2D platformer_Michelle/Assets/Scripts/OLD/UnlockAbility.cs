using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockAbility : MonoBehaviour {
    private PlayerController player;
  

	// Use this for initialization
	void Start () {
        player = FindObjectOfType<PlayerController>();
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Colliding");
        if (other.tag == "Player")
        {
            player.canDoubleJump = true;

            Destroy(gameObject);

        }
    }
}
