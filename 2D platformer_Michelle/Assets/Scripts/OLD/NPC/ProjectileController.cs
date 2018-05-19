using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour {
    public float speed;

    public PlayerController player;
    public GameObject projectileEffect;
   // public GameObject enemyDeathEffect;

    //public int pointsForKill;

    public float rotationSpeed;

    public int damageToGive;

	// Use this for initialization
	void Start () {
        player = FindObjectOfType<PlayerController>();

        if (player.transform.localScale.x < 0)
        {
            speed = -speed;
            rotationSpeed = -rotationSpeed;
        }
	}
	
	// Update is called once per frame
	void Update () {
        GetComponent<Rigidbody2D>().velocity = new Vector2(speed, GetComponent<Rigidbody2D>().velocity.y);
        GetComponent<Rigidbody2D>().angularVelocity = rotationSpeed;

	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            //Instantiate(enemyDeathEffect, other.transform.position, other.transform.rotation);
            //Destroy(other.gameObject);
            //ScoreManager.AddPoints(pointsForKill);

            other.GetComponent<EnemyHealthManager>().giveDamage(damageToGive);
        }

        Destroy(gameObject);
        Instantiate(projectileEffect, transform.position, transform.rotation);

    }
}
