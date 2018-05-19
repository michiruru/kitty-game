using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour {

    public int damageToGive;



    void OnTriggerEnter2D(Collider2D other) {
        if(other.tag  == "Enemy")
        {
            other.GetComponent<EnemyHealthManager>().giveDamage(damageToGive);
        }
    }
}
