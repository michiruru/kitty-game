using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
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
    #endregion

    #region GameObjects
    private Controller2D controller;
    private Animator anim;
    private Renderer playerSprite;
    private BoxCollider2D playerCollider;
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
    public float fDashCooldown;     // timer for cooldown between dashes
    public float fDashSpeed = 25f; //25f
    private float dashTimeCounter;
    public float dashTime = 0.2f; //0.2f how long the dash will last for
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

    #region
    [Header("Knockback")]
    public float invinsibilityTime;
    public float invinsibilityTimeCounter;
    #endregion

    #region Hide
    [Header("Hiding")]
    public bool bisHiding;
    public float hideTime;
    public float hideTimeCounter;
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
        playerSprite = GetComponentInChildren<Renderer>();
        fGravity = -(2 * fMaxJumpHeight) / Mathf.Pow(fTimeToJumpApex, 2);
        fMaxJumpVelocity = Mathf.Abs(fGravity) * fTimeToJumpApex;
        fMinJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(fGravity) * fMinJumpHeight);
        gravityStore = fGravity;
        playerCollider = GetComponentInChildren<BoxCollider2D>();

        animGrounded = true;

        fPlayerMaxHealth = 80f;
        fPlayerCurrentHealth = 80f;
    }

    private void Update()
    {
        ResetSkills();
        CalculateTimers();
        CheckDeath();
        CalculateVelocity();

        CheckCollision();

        if (bCanWallClimb) { HandleWallSliding(); }

        CalculateAnimBools();

        if (bOnLadder) { LadderClimb(); }

        #region Dash code
        if (bIsDashing)
        {
            vVelocity.x = fDashSpeed;
            vVelocity.y = 0;

            if (animTravelLeft)
            {
                vVelocity.x = fDashSpeed * -1;
                vVelocity.y = 0;
            }
        }

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
            vVelocity.x = fMoveSpeed/2;
            if (animTravelLeft)
            {
                vVelocity.x = fMoveSpeed/-2;
            }

        }

        if (attackTimeCounter <= 0)
        {
            bisAttacking = false;
            iAttackCount = 0;
        }


        if (hurtTimeCounter > 0)
        {
            hurtTimeCounter -= Time.deltaTime;
        }

        if (hurtTimeCounter <= 0)
        {
            bisHurt = false;
        }

        if (hideTimeCounter > 0)
        {
            hideTimeCounter -= Time.deltaTime;
        }

        if (hideTimeCounter < 0)
        {
            bisHiding = false;
            playerSprite.material.color = new Color(playerSprite.material.color.r, playerSprite.material.color.g, playerSprite.material.color.b, 1f);

            //Physics2D.IgnoreLayerCollision(9, 13, false);

        }

        if (dashTimeCounter >= 0)
        {
            dashTimeCounter -= Time.deltaTime;
        }

        if (dashTimeCounter< 0)
        {
            bIsDashing = false;
            animDash = false;
        }

        if(invinsibilityTimeCounter > 0)
        {
            Physics2D.IgnoreLayerCollision(9, 13, true);
            //playerCollider.isTrigger = true;
            invinsibilityTimeCounter -= Time.deltaTime;

        }
        if (invinsibilityTimeCounter <= 0)
        {
            //playerCollider.isTrigger = false;
           Physics2D.IgnoreLayerCollision(9, 13, false); //this turns off collisions always, how can I turn them back on without relying on enemy code??

        }
    }

    private void CheckDeath()
    {
        /*if (controller.transform.position.y < -10)
        {
            Respawn(PlayerCheckpoint);
        }*/

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
        { anim.SetBool("falling", true); }
        else
        { anim.SetBool("falling", false); }

        if (animGrounded)
        { anim.SetBool("grounded", true); }
        else
        { anim.SetBool("grounded", false); }

        if (animOnWall)
        { anim.SetBool("onWall", true); }
        else
        { anim.SetBool("onWall", false); }

        if (animWallClimb)
        { anim.SetBool("wallClimb", true); }
        else
        { anim.SetBool("wallClimb", false); }

        if (animDash)
        { anim.SetBool("dash", true); }
        else
        { anim.SetBool("dash", false); }

        if (animOnLadder)
        { anim.SetBool("onLadder", true); }
        else
        { anim.SetBool("onLadder", false); }

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
            dashTimeCounter = dashTime;    // set dash timer
            bIsDashing = true;         // state dashing
            animDash = true;
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
        climbVelocityY = climbSpeed * Input.GetAxisRaw("Vertical");
        vVelocity = new Vector2(climbVelocityX, climbVelocityY);

        if (climbVelocityY > 0)
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
        attackTimeCounter = attackTime;
        bisAttacking = true;


        if (bisAttacking)
        {
            comboTimeCounter = comboTime;
            iAttackCount++;

            if (iAttackCount > 2)
            {
                attackTimeCounter = attackTime;
            }
        
        }

    }

    public void Hide()
    {
        bisHiding = true;
        if (bisHiding)
        {
            Physics2D.IgnoreLayerCollision(9, 13, true);

            hideTimeCounter = hideTime;
            playerSprite.material.color = new Color(playerSprite.material.color.r, playerSprite.material.color.g, playerSprite.material.color.b, 0.5f);
            
        }
    }

    public void Knockback(float knockbackAmountX, float knockbackAmountY)
    {
        vVelocity.x = knockbackAmountX * -1;
        vVelocity.y = knockbackAmountY;
        //Physics2D.IgnoreLayerCollision(9, 13, true);

        if (animTravelLeft)
        {
            vVelocity.x = knockbackAmountX;
            vVelocity.y = knockbackAmountY;
            //Physics2D.IgnoreLayerCollision(9, 13, true);

        }

        invinsibilityTimeCounter = invinsibilityTime;
        
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