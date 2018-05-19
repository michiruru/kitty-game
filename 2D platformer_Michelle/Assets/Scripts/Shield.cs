using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour {
    public float shieldCooldown;
    public bool shieldBroken;

    public GameObject shield;

    public int shieldHP;
    public int startShieldHP;
	// Use this for initialization
	void Start () {
        shieldBroken = false;
        shieldHP = startShieldHP;
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKey(KeyCode.Q))
        {
            shield.SetActive(true);

        }
        else
        {
            shield.SetActive(false);
        }

        if (shieldHP == 0)
        {
            shieldBroken = true;
        }
        if (shieldBroken)

        {
           
            shield.SetActive(false);

            shieldCooldown -= Time.deltaTime;
        }

        if (shieldCooldown <= 0)
        {
            shieldCooldown = 2;
            shieldBroken = false;
            ResetShieldHealth();

        }
    }

    void OnCollisionEnter2D (Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            
            shieldHP--;

        }
    }


    void ResetShieldHealth()
    {
        shieldHP = startShieldHP;
    }
}
