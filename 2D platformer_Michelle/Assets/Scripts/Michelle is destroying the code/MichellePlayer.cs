using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class MichellePlayer : MonoBehaviour
{
    public Vector3 PlayerCheckpoint = new Vector3(-16.54f, 4.35f, 0f);    // location of the player checkpoint

    //DECLARATIONS
    #region PlayerStats
    public float fPlayerMaxHealth;
    public float fPlayerCurrentHealth;
    #endregion

    #region SkillsBools
    public bool bCanDoubleJump;
    public bool bCanWallClimb;
    public bool bCanDash = true;
    public bool bCanDirDash = true;
    #endregion

    #region GameObjects
    private Controller2D controller;
    private Animator anim;
    #endregion

    #region EnviroPhysics
    [Header("Physics Vars")]
    public float fMoveSpeed = 6f;
    private float fGravity;
    private Vector3 vVelocity;
    private float fVelocityXSmoothing;
    private Vector2 vDirectionalInput;
    public string sColliderTagHoriz;
    public string sColliderTagVert;
    #endregion

    #region Jump
    [Header("Jump Vars")]
    public float fMaxJumpHeight = 4f;
    [HideInInspector] public float fMinJumpHeight = 1f;
    public float fTimeToJumpApex = .4f;
    private float fMaxJumpVelocity;
    private float fMinJumpVelocity;
    private float fAccelerationTimeAirborne = .2f;
    private float fAccelerationTimeGrounded = .1f;

    public float fJumpLateLeniency = 0.2f;
    private float fJumpLateTimer;         // timer to determine how long since !grounded

    public float fFallDelay;    // ~= 2* fJumpLateLeniency : this is the delay before turning on the falling animation (used to hide the flickering when going over small bumps)
    private float fFallDelayTimer;

    public bool bHasDoubleJumped = false;
    public bool bIsJumping;
    public int iJumpCount = 0;
    public int iMaxJumpCount = 2;
    #endregion

    #region WallClimb
    [Header("WallClimb Vars")]
    public Vector2 vWallJumpClimb;   //7.5, 16 values for the ?velocity that char gets on WallJumpClimb
    public Vector2 vWallJumpOff;     //8.5, 7 (as above)
    public Vector2 vWallLeap;        //18, 17
    private bool bIsWallSliding;
    private int iWallDirX;          // signed direction of wall
    [HideInInspector] public float fWallSlideSpeedMax = 3f;
    [HideInInspector] public float fWallStickTime = .25f;   //???
    private float fTimeToWallUnstick;   //???
    #endregion

    #region Dash vars
    [Header("Dash")]
    private bool bIsDashing = false;
    [HideInInspector] public float fNextDashTime;   // the time at which the next dash can occur (?)
    [HideInInspector] public float fDashDuration;    // timer for how long current dash has been running
    public float fDashCooldown;     // timer for cooldown between dashes
    [HideInInspector] public float fTimeSinceDashEnd;    // timer for leniency at end of dash
    public float fDashSpeed = 25f; //25f
    public float fDashTime = 0.2f; //0.2f how long the dash will last for
    public float fDashImpactLeniency = 0.2f;  // leniency at end of dash to allow for impact breaking of walls etc.
    #endregion

    #region Attack
    [Header("Attacks")]
    public bool bisAttacking;
    public float attackTime;
    private float attackTimeCounter;
    public bool bisHurt;
    public float hurtTime;
    private float hurtTimeCounter;

    public int iAttackCount = 0;
    public int iMaxAttackCount = 3;

    public float comboTime;
    private float comboTimeCounter;
    #endregion

    #region EnviroInteractions
    [Header("Ladder Climb")]
    public bool bOnLadder;
    public float climbSpeed;
    private float gravityStore;
    #endregion

    #region Animation
    [Header("AnimationBools")]
    
    public bool animTravelLeft;
     public bool animTravelDown;
     public bool animGrounded;
     public bool animDoubleJump;
     public bool animWallClimb;
     public bool animDash;
     public bool animFalse;
     public bool animOnLadder;
    public bool animOnWall;
    public bool animAttacking;
    public bool animHurt;
    #endregion


    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        controller = GetComponent<Controller2D>();
        fGravity = -(2 * fMaxJumpHeight) / Mathf.Pow(fTimeToJumpApex, 2);
        fMaxJumpVelocity = Mathf.Abs(fGravity) * fTimeToJumpApex;
        fMinJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(fGravity) * fMinJumpHeight);
        gravityStore = fGravity;

        animGrounded = true;

        fPlayerMaxHealth = 80f;
        fPlayerCurrentHealth = 80f;
    }

    private void Update()
    {
        ResetSkills();
        CalculateTimers();
        //CheckDeath();
        CalculateVelocity();

        CheckCollision();

       if (bCanWallClimb) { HandleWallSliding(); }



        CalculateAnimBools();


        if (bOnLadder) { LadderClimb(); }

        #region Dash code
        if (bIsDashing && fDashDuration < fDashTime)
        {
            float totalInput = Mathf.Abs(vDirectionalInput.x) + Mathf.Abs(vDirectionalInput.y);
            if (bCanDirDash && totalInput > 0.01)
            {
                vVelocity.x = fDashSpeed * vDirectionalInput.x / totalInput;
                vVelocity.y = fDashSpeed * vDirectionalInput.y / totalInput;

            }
            else
            {
                vVelocity.x = fDashSpeed * (animTravelLeft ? -1 : 1);
                vVelocity.y = 0;
            }


            fDashDuration += Time.deltaTime;
        }
        else if (bIsDashing && (fDashDuration >= fDashTime && fDashDuration < 1.1f * fDashTime))
        {
            //vVelocity.x = 0; // reimplementing this causes the player to 'rebound ' in the opposite x-direction
            vVelocity.y = 0;
            bIsDashing = false;
            animDash = false;
        }
        else
        { bIsDashing = false; animDash = false; } // just in case, catch and turn off the dash
        #endregion

        #region Apply MOVEMENT
        controller.Move(vVelocity * Time.deltaTime, vDirectionalInput);

        if (controller.collisions.above || controller.collisions.below)
        {
            vVelocity.y = 0f;
        }
        #endregion

        CalculateAnimFloats();

    }


    // CALLED FROM UPDATE INITIAL
    private void ResetSkills()
    {
        if (animGrounded || bIsWallSliding)
        {// || animGrounded && bHasDoubleJumped || animGrounded && iJumpCount > iMaxJumpCount) {
            //Debug.Log("grounded; reseting double jump"); 
            bHasDoubleJumped = false;
            iJumpCount = 0;
            fJumpLateTimer = 0;
            fFallDelayTimer = 0;
        }

    }

    private void CalculateTimers()
    {
        if (!animGrounded && fJumpLateTimer < fJumpLateLeniency) { fJumpLateTimer += Time.deltaTime; }

        if (fJumpLateTimer > fJumpLateLeniency)
        {   // once timer has been exceeded,
            bIsJumping = false;     //then turn off the 'jumping' animation. This means that the character anim will not look like it is falling for up to fJumpLeniency seconds (prevention of the falling look when going over tiny bumps)
        }

        if (!animGrounded && fFallDelayTimer < fFallDelay) { fFallDelayTimer += Time.deltaTime; }


        if (attackTimeCounter > 0)
        {
            attackTimeCounter -= Time.deltaTime;
        }

        if (attackTimeCounter <= 0)
        {
            bisAttacking = false;
        }

        if(comboTimeCounter > 0)
        {
            comboTimeCounter -= Time.deltaTime;
        }


        if (comboTimeCounter <= 0)
        {
            iAttackCount = 0;
            bisAttacking = false;
        }

        if (hurtTimeCounter > 0)
        {
            hurtTimeCounter -= Time.deltaTime;
        }

        if (hurtTimeCounter <= 0)
        {
            bisHurt = false;
        }
    }

    private void CheckDeath()
    {
        if (controller.transform.position.y < -10)
        {
            Respawn(PlayerCheckpoint);
        }

        if (fPlayerCurrentHealth <= 0)
        {
            Respawn(PlayerCheckpoint);
        }
    }

    private void CalculateVelocity()
    {
        float targetVelocityX = vDirectionalInput.x * fMoveSpeed;
        vVelocity.x = Mathf.SmoothDamp(vVelocity.x, targetVelocityX, ref fVelocityXSmoothing, (controller.collisions.below ? fAccelerationTimeGrounded : fAccelerationTimeAirborne));
        vVelocity.y += fGravity * Time.deltaTime;
    }

    private void CheckCollision()
    {
        sColliderTagHoriz = Controller2D.sCollisionWithHoriz;
        sColliderTagVert = Controller2D.sCollisionWithVert;
    }

    private void CalculateAnimBools()
    {
        animGrounded = controller.collisions.below;

        if (sColliderTagHoriz == "Wall")// && bCanWallClimb)
        { animOnWall = true; }
        else
        { animOnWall = false; }


        if (vVelocity.x < -0.5) { animTravelLeft = true; }
        if (vVelocity.x > 0.5) { animTravelLeft = false; }

        if (vVelocity.y < 0.1 && !animGrounded) { animTravelDown = true; }
        else { animTravelDown = false; }


        animDoubleJump = bHasDoubleJumped && !animGrounded;

        animWallClimb = bIsWallSliding;

        animOnLadder = bOnLadder;

        animAttacking = bisAttacking;

        animHurt = bisHurt;

        SetAnimator();
    }

    public void SetAnimator()
    {
        animFalse = false;
        anim.SetBool("disable", animFalse);

        if (animTravelLeft)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else
        {
            transform.localScale = new Vector3(1f, 1f, 1f);

        }

        if (animTravelDown)
        { anim.Play("Jump_fall"); }

        if (animGrounded)
        { anim.Play("idle"); }
        else
        { anim.Play("jump"); }

        if (animOnWall)
        { anim.Play("wallClimb"); }


        if (animWallClimb)
        { anim.Play("wallClimb"); }

        if (animDash)
        { anim.Play("dash"); }

        if (animOnLadder)
            { anim.Play("ladderClimb"); }

        if (bIsJumping)
        { anim.SetBool("jumping", true); }
        else
        { anim.SetBool("jumping", false); }

        if (animAttacking)
        {
            anim.SetBool("attacking", true);
        }
        else { anim.SetBool("attacking", false); }

        if (animHurt)
        {
            anim.SetBool("hurt", true);
        }
        else
        {
            anim.SetBool("hurt", false);
        }

    }

    private void CalculateAnimFloats()
    {
        anim.SetFloat("velocityX", Mathf.Abs(vVelocity.x));
        anim.SetFloat("velocityY", (vVelocity.y));

        anim.SetFloat("falltimerdelay", fFallDelayTimer - fFallDelay);

        anim.SetInteger("attackCount", iAttackCount);
    }

    // CALLED FROM UPDATE CONDITIONALLY
    private void HandleWallSliding()
    {
        iWallDirX = (controller.collisions.left) ? -1 : 1;
        bIsWallSliding = false;

        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && vVelocity.y < 0)
        {
            bIsWallSliding = true;

            if (vVelocity.y < -fWallSlideSpeedMax)
            {
                vVelocity.y = -fWallSlideSpeedMax;
            }

            if (fTimeToWallUnstick > 0f)
            {
                fVelocityXSmoothing = 0f;
                vVelocity.x = 0f;
                if (vDirectionalInput.x != iWallDirX && vDirectionalInput.x != 0f)
                {
                    fTimeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    fTimeToWallUnstick = fWallStickTime;
                }
            }
            else
            {
                fTimeToWallUnstick = fWallStickTime;
            }
        }
    }


    //EXTERNALLY CALLED FROM CONTROLLER
    public void SetDirectionalInput(Vector2 input)
    {
        vDirectionalInput = input;
    }

    public void OnJumpInputDown()
    {
        if (bIsWallSliding)    // wall sliding jump
        {
            bIsJumping = true;
            if (iWallDirX == vDirectionalInput.x)
            {
                vVelocity.x = -iWallDirX * vWallJumpClimb.x;
                vVelocity.y = vWallJumpClimb.y;
            }
            else if (vDirectionalInput.x == 0)
            {
                vVelocity.x = -iWallDirX * vWallJumpOff.x;
                vVelocity.y = vWallJumpOff.y;
            }
            else
            {
                vVelocity.x = -iWallDirX * vWallLeap.x;
                vVelocity.y = vWallLeap.y;
            }
            iJumpCount++;
            //bHasDoubleJumped = false;
        }

        if (fJumpLateTimer < fJumpLateLeniency) // a normal jump
        {
            //Debug.Log("normal jump");
            bIsJumping = true;
            vVelocity.y = fMaxJumpVelocity;
            iJumpCount++;

            fJumpLateTimer = fJumpLateLeniency + 1f;    // override timer (will reset on next grounding)
            fFallDelayTimer = fFallDelay + 1f;
            //bHasDoubleJumped = false;
            //Debug.Log("normal jump");
        }

        if (bCanDoubleJump && !controller.collisions.below && (iJumpCount < iMaxJumpCount) && !bIsWallSliding) // double jump
        {
            bIsJumping = true;
            vVelocity.y = fMaxJumpVelocity;
            bHasDoubleJumped = true;
            iJumpCount++;
            //Debug.Log("double jump");
        }
    }

    public void OnJumpInputUp()
    {
        if (vVelocity.y > fMinJumpVelocity)
        {
            vVelocity.y = fMinJumpVelocity;
        }
    }

    public void Dash()
    {
        if (bCanDash)
        {
            //Debug.Log("dashed");
            fDashDuration = 0.0f;    // reset the current dash timer
            fTimeSinceDashEnd = 0.0f;    // reset timer since dash end
            bIsDashing = true;         // state dashing
            animDash = true;
            PlayerInput.disableInputTime = fDashTime;
        }
    }

    public void Respawn(Vector3 vSpawnPosition)
    {
        transform.position = vSpawnPosition;
        vVelocity.x = vVelocity.y = 0;
    }

    public void LadderClimb()
    {
        float climbVelocityX;
        float climbVelocityY;

        fGravity = 0;
        climbVelocityX = climbSpeed * Input.GetAxisRaw("Horizontal");
        //vVelocity.x = 0;
        climbVelocityY = climbSpeed * Input.GetAxisRaw("Vertical");
        vVelocity = new Vector2(climbVelocityX, climbVelocityY);

        if(climbVelocityY > 0)
        {
            anim.speed = 1;
        }
        else
        {
            anim.speed = 0;
        }
    }

    public void Attack()
    {
        comboTimeCounter = comboTime;

        attackTimeCounter = attackTime;
            bisAttacking = true;
            iAttackCount++;

    }

    public void HurtPlayer(float amountHurtPlayer)
    {
        fPlayerCurrentHealth -= amountHurtPlayer;

        if (fPlayerCurrentHealth >= fPlayerMaxHealth) { fPlayerCurrentHealth = fPlayerMaxHealth; }

        if (fPlayerCurrentHealth <= 0f)
        {
            fPlayerCurrentHealth = 0f;
            //die
        }

        UI.UpdateHealth(fPlayerCurrentHealth / fPlayerMaxHealth);

        bisHurt = true;
        hurtTimeCounter = hurtTime;
    }

    public void ChangePlayerHealth(float amountIncreaseMaxHealth)
    {
        //collect n number of heart pieces ...
        fPlayerMaxHealth += amountIncreaseMaxHealth;    // increase max health
        UI.UpdateNumberOfHearts();                      // update number of accessible hearts

        fPlayerCurrentHealth = fPlayerMaxHealth;        // heal player to max
        UI.UpdateHealth(fPlayerCurrentHealth / fPlayerMaxHealth);   // update those hearts/health
    }






    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.transform.tag == "Ladder")
        {
            bOnLadder = true;
        }
    }

    public void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.transform.tag == "Ladder")
        {
            bOnLadder = false;
            fGravity = gravityStore;
        }
    }



    void Awake()
    {
#if UNITY_EDITOR
        //Time.timeScale = 0.4f;
        //QualitySettings.vSyncCount = 0;  // VSync must be disabled
        //Application.targetFrameRate = 10;
#endif
    }

}