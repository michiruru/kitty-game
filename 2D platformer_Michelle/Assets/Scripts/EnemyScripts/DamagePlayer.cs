using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayer : MonoBehaviour {
    public float damageAmount;
    public bool detectingPlayer;

    private Player player;

	// Use this for initialization
	void Start () {
        player = FindObjectOfType<Player>();
		
	}
	
	// Update is called once per frame
	void Update () {
       if(player.bisHiding == true)
        {
            detectingPlayer = false;
        }

       if(player.bisHiding == false)
        {
            detectingPlayer = true;
        }


    }

    void OnCollisionEnter2D(Collision2D other)
    {
         if (other.collider.tag == "Player")
        {

            if (!detectingPlayer)
            {
                Physics2D.IgnoreLayerCollision(13, 9, false);

            }

            if (detectingPlayer)
            {
                Physics2D.IgnoreLayerCollision(13, 9, true);

                other.gameObject.GetComponent<Player>().HurtPlayer(damageAmount);
                other.gameObject.GetComponent<Player>().Knockback(25, 5);

            }

        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            other.gameObject.GetComponent<Player>().HurtPlayer(damageAmount);
            
        }
    }
}
