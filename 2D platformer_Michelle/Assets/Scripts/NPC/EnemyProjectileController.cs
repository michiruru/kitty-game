using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileController : MonoBehaviour {

    public float speed;

    public PlayerController player;

    public GameObject projectileEffect;

    public float rotationSpeed;

    public int damageToGive;

    // Use this for initialization
    void Start()
    {
        player = FindObjectOfType<PlayerController>();

        if (player.transform.position.x < transform.position.x)
        {
            speed = -speed;
            rotationSpeed = -rotationSpeed;
        }
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(speed, GetComponent<Rigidbody2D>().velocity.y);
        GetComponent<Rigidbody2D>().angularVelocity = rotationSpeed;

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "Player")
        {
            HeathManager.HurtPlayer(damageToGive);
        }



        Destroy(gameObject);
        Instantiate(projectileEffect, transform.position, transform.rotation);

    }
}
