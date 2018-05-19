using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCStats : MonoBehaviour {
    public int faction; //refers to the index of the FactionList (under MainHUD > NPCManager)
    public int playerRelationship;


	// Use this for initialization
	void Start () {
        playerRelationship = 50;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void changePlayerRelationship(int a)
    {
        playerRelationship = a;
    }
}
