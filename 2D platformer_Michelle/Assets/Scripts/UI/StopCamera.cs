using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopCamera : MonoBehaviour {
    public CameraController camera;
	// Use this for initialization

        void Start ()
    {
        camera = FindObjectOfType<CameraController>();
    }

    void OnTriggerEnter2D (Collider2D other)
    {
        if (other.tag == "Player")
            Debug.Log("Colliding");
        //  camera.isFollowing = false;
        camera.moveSpeed = 0;
    }

    void OnTriggerExit2D (Collider2D other)
    {
        if (other.tag == "Player")
            Debug.Log("Colliding");
        //  camera.isFollowing = true;
        camera.moveSpeed = 5;
    }
}
