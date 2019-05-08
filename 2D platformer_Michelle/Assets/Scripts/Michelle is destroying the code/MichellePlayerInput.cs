using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Player))]
public class MichellePlayerInput : MonoBehaviour
{
    private Player player;
    private float nextDashTime; // absolute time that the next dash can occur

    [HideInInspector] public static bool disableInput;
    public static float disableInputTime;
    [HideInInspector] public static bool disableJump;

    public GameObject[] spawnPoints;

    public bool bLookForSequence = false;
    private float fSequenceNextTime;
    public int iSequenceLocation;
    public string[] sSequence;

    public enum PlayerMoveState
    {
        IDLE,
        MOVE,
        JUMP,
        FALL,
    }

    public enum PlayerActionState
    {
        ATTACK,
        DASH
    }

    public PlayerMoveState playerMoveState;

    private void Start()
    {
        player = GetComponent<Player>();
        disableInput = false;
        disableJump = false;

        sSequence = new string[2];  // length of sequence input
    }

    private void Update()
    {
        CalculateTimers();
        CheckDisableInput();
        //CheckDisableJump();
     
            
            Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            player.SetDirectionalInput(directionalInput);
        

        if (Input.GetButtonDown("Jump"))
        {
            playerMoveState = PlayerMoveState.JUMP;
            player.OnJumpInputDown();
        }
        if (Input.GetButtonUp("Jump"))
        {
            playerMoveState = PlayerMoveState.FALL;
            player.OnJumpInputUp();
        }

        HandlePlayerState();

    }

    void HandlePlayerState()
    {
        switch (playerMoveState)
        {
            case PlayerMoveState.IDLE:
                player.GetComponentInChildren<Animator>().Play("idle");
                              
                break;
            case PlayerMoveState.MOVE:
                player.GetComponentInChildren<Animator>().Play("run");
            
                break;
            case PlayerMoveState.JUMP:
         

                player.GetComponentInChildren<Animator>().Play("jump");
                break;
            case PlayerMoveState.FALL:

          
                player.GetComponentInChildren<Animator>().Play("jump_fall");
                break;


        }
    }

    private void CheckDisableInput()
    {
        if (disableInputTime > 0)
        {
            disableInput = true;
            disableInputTime -= Time.deltaTime;
        }
        else { disableInput = false; }
    }

    private void CalculateTimers()
    {

    }

  private void DoRespawn()
    {
        string input = sSequence[0] + sSequence[1];

        switch (input)
        {
            case "00":
                player.Respawn(spawnPoints[0].transform.position);
                break;

            case "01":
                player.Respawn(spawnPoints[1].transform.position);
                break;

            case "02":
                player.Respawn(spawnPoints[2].transform.position);
                break;

            case "03":
                player.Respawn(spawnPoints[3].transform.position);
                break;

            case "04":
                player.Respawn(spawnPoints[4].transform.position);
                break;

            default:
                break;

        }
    }
    

}