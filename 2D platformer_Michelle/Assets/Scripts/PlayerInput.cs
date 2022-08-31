using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour
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

    public bool canUseDashAbility;
    public bool dashAbilityUsed;
    public float dashAbilityCooldownTime;
    private float dashAbilityCooldownCounter;

    public bool canUseHideAbility;
    public bool hideAbilityUsed;
    public float hideAbilityCooldownTime;
    private float hideAbilityCooldownCounter;

    public bool canAttack;
    public bool attackUsed;
    public float attackCooldownTime;
    private float attackCooldownCounter;

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

        if (!disableInput)
        {
            Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            player.SetDirectionalInput(directionalInput);

            if (!disableJump)
            {

                if (Input.GetButtonDown("Jump"))
                {
                    player.OnJumpInputDown();
                }

               if (Input.GetButtonUp("Jump"))
                {
                    player.OnJumpInputUp();
                }

            }

            if (canUseDashAbility)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    player.Dash();
                    dashAbilityUsed = true;
                    AbilityCooldown();
                }
            }

            if (canAttack)
            {
                if (Input.GetButtonDown("Dash"))
                {

                    player.Attack();
                    attackUsed = true;
                    AbilityCooldown();


                }
            }
            if (canUseHideAbility)
            {

                if (Input.GetButtonDown("Fire2"))

                {
                    player.Hide();
                    hideAbilityUsed = true;
                    AbilityCooldown();

                }
            }

            
            if (!canUseDashAbility|| !canUseHideAbility ||!canAttack) {

                return;
            }
                
            

           
        }

        //DEBUG KEY INPUTS REGION
        if (Input.GetButtonDown("Reset"))
        {
            //Reset sequence
            for (int i = 0; i < sSequence.Length; i++)
            {
                sSequence[i] = "NA";
            }

            iSequenceLocation = 0;
            bLookForSequence = true;
            fSequenceNextTime = Time.time + Time.deltaTime;

            //player.Respawn(new Vector3(0,0,0));
        }

        if (Input.GetButtonDown("HurtPlayer"))
        {
            player.HurtPlayer(10f);
        }

        if (Input.GetButtonDown("HealPlayer"))
        {
            player.HurtPlayer(-10f);
        }

        if (Input.GetButtonDown("GainHeart"))
        {
            player.ChangePlayerHealth(20f);
        }

        if (Input.GetButtonDown("LoseHeart"))
        {
            player.ChangePlayerHealth(-20f);
        }

        if (bLookForSequence && Time.time > fSequenceNextTime)
        {
            if (Input.anyKeyDown)
            {
                sSequence[iSequenceLocation] = Input.inputString;
                iSequenceLocation++;
                fSequenceNextTime = Time.time + Time.deltaTime;
            }

            if (iSequenceLocation >= sSequence.Length)
            {
                //Debug.Log("sequence full");
                bLookForSequence = false;

                DoRespawn();
            }
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

        if(dashAbilityCooldownCounter > 0)
        {
            dashAbilityCooldownCounter -= Time.deltaTime;
                    }

        if(dashAbilityCooldownCounter <= 0)
        {
            canUseDashAbility = true;
            dashAbilityUsed = false;
        }

        if (hideAbilityCooldownCounter > 0)
        {
            hideAbilityCooldownCounter -= Time.deltaTime;
        }

        if (hideAbilityCooldownCounter <= 0)
        {
            canUseHideAbility = true;
            hideAbilityUsed = false;
        }

        if(attackCooldownCounter > 0)
        {
            attackCooldownCounter -= Time.deltaTime;
        }

        if(attackCooldownCounter <= 0)
        {
            canAttack = true;
            attackUsed = false;
        }
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

    public void AbilityCooldown()
    {
        if (dashAbilityUsed)
        {
        canUseDashAbility = false;
            dashAbilityCooldownCounter = dashAbilityCooldownTime;
        }

        if (hideAbilityUsed)
        {
            canUseHideAbility = false;
            hideAbilityCooldownCounter = hideAbilityCooldownTime;
        }

        if (attackUsed)
        {
            canAttack = false;
            attackCooldownCounter = attackCooldownTime;
        }
    }
    

}