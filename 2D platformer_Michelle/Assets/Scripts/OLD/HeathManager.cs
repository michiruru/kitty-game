using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeathManager : MonoBehaviour {
    public static int playerHealth;
    public int maxPlayerHealth;

    public int healthUpgradeCount;
    //Text text;

    public Slider healthBar;

    private LevelManager levelManager;

    public bool isDead;

   // private LifeManager lifeSystem;


    // Use this for initialization
    void Start () {
        //text = GetComponent<Text>();
        healthBar = GetComponent<Slider>();
        playerHealth = PlayerPrefs.GetInt("PlayerCurrentHealth");
        maxPlayerHealth = PlayerPrefs.GetInt("PlayerMaxHealth");
        levelManager = FindObjectOfType<LevelManager>();

        isDead = false;
        //lifeSystem = FindObjectOfType<LifeManager>();
    }
	
	// Update is called once per frame
	void Update () {
		if (playerHealth <= 0 && !isDead)
        {
            playerHealth = 0;
            isDead = true;
            levelManager.RespawnPlayer();
           // lifeSystem.TakeLife();
         }

        if (playerHealth > maxPlayerHealth)
        {
            playerHealth = maxPlayerHealth;        }

        // text.text = "" + playerHealth;
        healthBar.value = playerHealth;

        if (healthUpgradeCount == 4)
        {
            UpgradeHealth();
            ResetHealthUpgrade();
        }
    }

    public static void HurtPlayer (int damageToGive)
    {
        playerHealth -= damageToGive;
        PlayerPrefs.SetInt("PlayerCurrentHealth", playerHealth);
    }

    public void FullHealth()
    {
        playerHealth = PlayerPrefs.GetInt("PlayerMaxHealth");

        PlayerPrefs.SetInt("PlayerCurrentHealth", playerHealth);

    }

    public void UpgradeHealth()
    {
        maxPlayerHealth ++;
    }

    public void ResetHealthUpgrade()
    {
        healthUpgradeCount = 0;

    }
}
