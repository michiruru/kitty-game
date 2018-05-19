using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    /* public PlayerController player;
     public bool isFollowing;

     public float xOffset;
     public float yOffset;
     // Use this for initialization
     void Start () {
         player = FindObjectOfType<PlayerController>();
         isFollowing = true;
     }

     // Update is called once per frame
     void Update () {
         if (isFollowing) {
             transform.position = new Vector3(player.transform.position.x + xOffset, player.transform.position.y + yOffset, transform.position.z);
         }
     }*/

    private PlayerController followTarget;
    private Vector3 targetPos;
    public float moveSpeed;
    public float xOffset;
    public float yOffset;
    
    // Use this for initialization
    void Start()
    {
        followTarget = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        targetPos = new Vector3(followTarget.transform.position.x + xOffset, followTarget.transform.position.y + yOffset, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPos, moveSpeed * Time.deltaTime);
    }
}
