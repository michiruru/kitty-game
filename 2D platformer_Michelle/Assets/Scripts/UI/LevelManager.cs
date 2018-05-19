using UnityEngine;
using System.Collections;


public class LevelManager : MonoBehaviour {
    public GameObject currentCheckpoint;

    private PlayerController player;
    private HeathManager healthManager;

    public GameObject deathParticle;
    public GameObject respawnParticle;

    public int pointPenaltyOnDeath;

    public float respawnDelay;

    private CameraController mainCamera;

    private float gravityScore;
    

    void Start() {
        player = FindObjectOfType<PlayerController>();
        mainCamera = FindObjectOfType<CameraController>();
        healthManager = FindObjectOfType<HeathManager>();

    }

    // Update is called once per frame 
    void Update () { }

    public void RespawnPlayer() {
        StartCoroutine("RespawnPlayerCo"); }


public IEnumerator RespawnPlayerCo()
    {
        Instantiate(deathParticle, player.transform.position, player.transform.rotation);
        player.enabled = false;
        player.GetComponentInChildren<SpriteRenderer >().enabled = false;
        //mainCamera.isFollowing = false;
      //  gravityScore = player.GetComponent<Rigidbody2D>().gravityScale;
       // player.GetComponent<Rigidbody2D>().gravityScale = 0f;
       // player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        ScoreManager.AddPoints(-pointPenaltyOnDeath);
        Debug.Log ("Player Respawn");
        yield return new WaitForSeconds(respawnDelay);
        //  player.GetComponent<Rigidbody2D>().gravityScale = gravityScore;
        player.transform.parent = null;
        player.transform.position = currentCheckpoint.transform.position;
        player.enabled = true;
        player.GetComponentInChildren<SpriteRenderer>().enabled = true;
      //  mainCamera.isFollowing = true;
        Instantiate(respawnParticle, currentCheckpoint.transform.position, currentCheckpoint.transform.rotation);
        healthManager.FullHealth();
        healthManager.isDead = false;

    }


}
