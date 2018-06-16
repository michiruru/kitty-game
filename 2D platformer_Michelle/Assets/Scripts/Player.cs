using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    //DECLARATIONS
    Vector3 vSpawnPoint;

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
    public float fJumpLateTimer;         // timer to determine how long since !grounded

    public bool bCanDoubleJump;
    public bool bHasDoubleJumped = false;
    public int iJumpCount = 0;
    public int iMaxJumpCount = 2;
    #endregion

    #region WallClimb
    [Header("WallClimb Vars")]
    public bool canWallJump;
    public Vector2 wallJumpClimb;   //7.5, 16
    public Vector2 wallJumpOff;     //8.5, 7
    public Vector2 wallLeap;        //18, 17
    private bool wallSliding;
    private int wallDirX;
    [HideInInspector] public float wallSlideSpeedMax = 3f;
    [HideInInspector] public float wallStickTime = .25f;
    private float timeToWallUnstick;
    #endregion

    #region Dash vars
    [Header("Dash")]
    public bool bCanDash = true;
    public bool bCanDirDash = true;
    private bool bIsDashing = false;
    [HideInInspector] public float fNextDashTime;
    [HideInInspector] public float fDashDuration;    // timer for how long current dash has been running
    public float fDashCooldown;     // timer for cooldown between dashes
    [HideInInspector] public float fTimeSinceDashEnd;    // timer for leniency at end of dash
    public float fDashSpeed = 25f; //25f
    public float fDashTime = 0.2f; //0.2f how long the dash will last for
    public float fDashImpactLeniency = 0.2f;  // leniency at end of dash to allow for impact breaking of walls etc.
    #endregion

    #region Animation
    [Header("AnimationBools")]
    [HideInInspector]
    public bool animTravelLeft;
    [HideInInspector] public bool animTravelDown;
    public bool animGrounded;
    [HideInInspector] public bool animDoubleJump;
    [HideInInspector] public bool animWallClimb;
    public bool animDash;
    #endregion

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        controller = GetComponent<Controller2D>();
        fGravity = -(2 * fMaxJumpHeight) / Mathf.Pow(fTimeToJumpApex, 2);
        fMaxJumpVelocity = Mathf.Abs(fGravity) * fTimeToJumpApex;
        fMinJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(fGravity) * fMinJumpHeight);

        vSpawnPoint = transform.position;
    }

    private void Update()
    {
        ResetSkills();
        CalculateTimers();
        //CheckDeath();
        CalculateVelocity();

        if (canWallJump) { HandleWallSliding(); }

        CalculateAnimBools();

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

    }

    // CALLED FROM UPDATE INITIAL
    private void ResetSkills()
    {
        if (animGrounded || wallSliding)
        {// || animGrounded && bHasDoubleJumped || animGrounded && iJumpCount > iMaxJumpCount) {
            //Debug.Log("grounded; reseting double jump"); 
            bHasDoubleJumped = false;
            iJumpCount = 0;

            fJumpLateTimer = 0;
        }

    }

    private void CalculateTimers()
    {
        if (!animGrounded && fJumpLateTimer < fJumpLateLeniency) { fJumpLateTimer += Time.deltaTime; }
    }

    private void CheckDeath()
    {
        if (controller.transform.position.y < -10)
        {
            Respawn();
        }
    }

    private void CalculateVelocity()
    {
        float targetVelocityX = vDirectionalInput.x * fMoveSpeed;
        vVelocity.x = Mathf.SmoothDamp(vVelocity.x, targetVelocityX, ref fVelocityXSmoothing, (controller.collisions.below ? fAccelerationTimeGrounded : fAccelerationTimeAirborne));
        vVelocity.y += fGravity * Time.deltaTime;

        anim.SetFloat("velocityX", Mathf.Abs(vVelocity.x));
        anim.SetFloat("velocityY", Mathf.Abs(vVelocity.y));

    }

    private void CalculateAnimBools()
    {
        animGrounded = controller.collisions.below;

        if (vVelocity.x < -0.5) { animTravelLeft = true; }
        if (vVelocity.x > 0.5) { animTravelLeft = false; }

        if (vVelocity.y < 0 && !animGrounded) { animTravelDown = true; }
        else { animTravelDown = false; }


        animDoubleJump = bHasDoubleJumped && !animGrounded;

        animWallClimb = wallSliding;

        SetAnimator();
    }

    public void SetAnimator()
    {
        if (animTravelLeft)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else
        {
            transform.localScale = new Vector3(1f, 1f, 1f);

        }

        if (animTravelDown)
        {
            anim.SetBool("falling", true);
        }
        else { anim.SetBool("falling", false); }

        if (animGrounded)
        {
            anim.SetBool("grounded", true);
        }
        else
        {
            anim.SetBool("grounded", false);
        }

        if (animWallClimb)
        {
            anim.SetBool("wallClimb", true);
        }
        else
        {
            anim.SetBool("wallClimb", false);
        }

        if (animDash)
        {
            anim.SetBool("dash", true);
        }
        else
        {
            anim.SetBool("dash", false);
        }
    }


    // CALLED FROM UPDATE CONDITIONALLY
    private void HandleWallSliding()
    {
        wallDirX = (controller.collisions.left) ? -1 : 1;
        wallSliding = false;
        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && vVelocity.y < 0)
        {
            wallSliding = true;

            if (vVelocity.y < -wallSlideSpeedMax)
            {
                vVelocity.y = -wallSlideSpeedMax;
            }

            if (timeToWallUnstick > 0f)
            {
                fVelocityXSmoothing = 0f;
                vVelocity.x = 0f;
                if (vDirectionalInput.x != wallDirX && vDirectionalInput.x != 0f)
                {
                    timeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }
            }
            else
            {
                timeToWallUnstick = wallStickTime;
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
        if (wallSliding)    // wall sliding jump
        {
            if (wallDirX == vDirectionalInput.x)
            {
                vVelocity.x = -wallDirX * wallJumpClimb.x;
                vVelocity.y = wallJumpClimb.y;
            }
            else if (vDirectionalInput.x == 0)
            {
                vVelocity.x = -wallDirX * wallJumpOff.x;
                vVelocity.y = wallJumpOff.y;
            }
            else
            {
                vVelocity.x = -wallDirX * wallLeap.x;
                vVelocity.y = wallLeap.y;
            }
            iJumpCount++;
            //bHasDoubleJumped = false;
        }
        if (fJumpLateTimer < fJumpLateLeniency) // a normal jump
        {
            vVelocity.y = fMaxJumpVelocity;
            iJumpCount++;

            fJumpLateTimer = fJumpLateLeniency + 1f;    // override timer (will reset on next grounding)
            //bHasDoubleJumped = false;
            //Debug.Log("normal jump");
        }
        if (bCanDoubleJump && !controller.collisions.below && (iJumpCount < iMaxJumpCount) && !wallSliding) // double jump
        {
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
        //Debug.Log("dashed");
        fDashDuration = 0.0f;    // reset the current dash timer
        fTimeSinceDashEnd = 0.0f;    // reset timer since dash end
        bIsDashing = true;         // state dashing
        animDash = true;
        PlayerInput.disableInputTime = fDashTime;
    }

    public void Respawn()
    {
        transform.position = vSpawnPoint;
        vVelocity.x = vVelocity.y = 0;
    }






}