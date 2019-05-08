using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawn : MonoBehaviour {

    public GameObject projectile;

   private float projectileTime;
   public float projectileTimerCounter;

	// Use this for initialization
	void Start () {

        projectileTime = projectileTimerCounter;
    }
	
	// Update is called once per frame
	void Update () {

        projectileTimerCounter -= Time.deltaTime;

        if (projectileTimerCounter <= 0)
        {
            Spawn();
            projectileTimerCounter = projectileTime;
        }

    }

    void Spawn()
    {
        Instantiate(projectile, transform.position, transform.rotation);

    }


}
