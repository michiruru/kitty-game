using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour {
    public float speed;

    public float rotationSpeed;

    private Rigidbody2D myRigidbody2D;
    // Use this for initialization
    void Start () {
        myRigidbody2D = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        myRigidbody2D.velocity = new Vector2(speed, myRigidbody2D.velocity.y);

        myRigidbody2D.angularVelocity = rotationSpeed;
    }

   void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
