using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCtalking : MonoBehaviour {
    private Animator anim;
    public ActivateTextAtLine textbox;

	// Use this for initialization
	void Start () {
        anim = GetComponentInChildren<Animator>();
        textbox = FindObjectOfType<ActivateTextAtLine>();
	}
	
	// Update is called once per frame
	void Update () {
        if (textbox.isTalking == true)
        {
            anim.SetBool("isTalking", true);
        }

        if (textbox.isTalking == false)
        {
            anim.SetBool("isTalking", false);
        }
    }

   

   
}
