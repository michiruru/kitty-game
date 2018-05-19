using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallFallTrigger : MonoBehaviour {
    private Animator anim;
    public bool wallOpen;
	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        if (wallOpen)
        {
            anim.SetBool("Open", true);
        }

	}
}
