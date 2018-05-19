using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthManager : MonoBehaviour {

    public int enemyHealth;

    public GameObject deathEffect;

    public GameObject coins;

    public int pointsOnDeath;

	// Use this for initialization
	void Start () {		
	}
	
	// Update is called once per frame
	void Update () {
        if (enemyHealth <= 0)
        {
            Instantiate(deathEffect, transform.position, transform.rotation);
            //ScoreManager.AddPoints(pointsOnDeath);
            Destroy(gameObject);
            Instantiate(coins, transform.position, transform.rotation);
        }
		
	}

    public void giveDamage(int damageToGive)
    {
        enemyHealth -= damageToGive;
        GetComponent<AudioSource>().Play();
    }
}
