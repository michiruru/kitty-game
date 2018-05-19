using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseMaxHealth : MonoBehaviour {
   public int upgradeCount;
    private HeathManager healthManager;

	// Use this for initialization
	void Start () {
        healthManager = FindObjectOfType<HeathManager>();
	}
	
	// Update is called once per frame
	void Update () {

	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            healthManager.healthUpgradeCount++;
            Destroy(gameObject);
        }
    }
}
