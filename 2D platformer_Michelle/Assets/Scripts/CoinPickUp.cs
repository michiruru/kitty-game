using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickUp : MonoBehaviour {

    public int pointsToAdd;

    public AudioSource coinSound;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() == null)
            return;
        ScoreManager.AddPoints(pointsToAdd);
        coinSound.Play();
        Destroy(gameObject);
    }
		
	
}
