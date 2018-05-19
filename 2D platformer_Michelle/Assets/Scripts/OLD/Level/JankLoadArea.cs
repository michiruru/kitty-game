using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JankLoadArea : MonoBehaviour {
    public GameObject areaToLoad;
    public bool turnOn;
    public bool turnOff;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && turnOn)
        {
            areaToLoad.SetActive(true);
        }

        if (other.tag == "Player" && turnOff)
        {
            areaToLoad.SetActive(false);
        }
    }
}
