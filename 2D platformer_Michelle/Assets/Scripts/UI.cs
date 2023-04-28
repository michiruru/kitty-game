using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour {
    public static Player player;

    public GameObject[] HealthHearts;
    public static GameObject[] HeartsArray;
    public static float fHealthArraySize;
    public static float fHealthAccessibleHearts;
    public static float fHeartValue;

	// Use this for initialization
	void Start () {
        fHeartValue = 20f;  // each heart is worth n units of health

        player = FindObjectOfType<Player>();

        fHealthArraySize = HealthHearts.Length;
        HeartsArray = HealthHearts;

        UpdateNumberOfHearts();
        UpdateHealth(1f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static void UpdateHealth(float percentagehealth)
    {

        float numberOfHeart = Mathf.Ceil(percentagehealth * fHealthAccessibleHearts);
        Debug.Log(numberOfHeart);

        for (int i = 0; i < fHealthArraySize; i++)  // removing all hearts
        {
            HeartsArray[i].SetActive(false);
        }

        for (int i = 0; i < fHealthAccessibleHearts; i++)   // set available hearts to grey
        {
            HeartsArray[i].SetActive(true);
            HeartsArray[i].GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1);
        }

        for (int i = 0; i < numberOfHeart; i++) // set the correct hearts to red
        {
            HeartsArray[i].GetComponent<Image>().color = new Color(1, 0, 0, 1);
        }


    }

    public static void UpdateNumberOfHearts()
    {
        fHealthAccessibleHearts = Mathf.Floor(player.fPlayerMaxHealth / fHeartValue);
    }
}
