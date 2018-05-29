using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    // DECLARATIONS
    #region // GameObjects
    private Controller2D controller;
    private Animator anim;
    #endregion

    #region // Physics vars
    [Header("Physics Vars")]
    [HideInInspector]
    public Vector2 vDirectionalInput;
    public Vector3 vVelocity;
    public float fMaxMoveSpeed = 6f;
    private float fGravity;
    private float fMaxJumpVelocity;
    private float fMinJumpVelocity;

    #endregion

    #region // Jumping vars
    [Header("Jumping")]
    [HideInInspector]
    public bool bCallJump = false;
    [HideInInspector] public Vector2 vWallJumpClimb;
    [HideInInspector] public Vector2 vWallJumpOff;
    [HideInInspector] public Vector2 vWallLeap;
    [HideInInspector] public float fMaxJumpHeight = 4f;
    [HideInInspector] public float fMinJumpHeight = 1f;
    [HideInInspector] public float fTimeToJumpApex = .5f;

    public float fJumpEarlyLeniency = 0.2f;  // amount of time you can early press a jump when airborne and still allow a jump at next grounded event
    public float fJumpLedgeLeniency = 0.2f;  // amount of time you can be off a platform but still allow a jump to occur
    [HideInInspector] public float fJumpRejumpEarlyTime;     // timer for early jump bounce leniency
    [HideInInspector] public float fJumpLedgeEarlyTime;        // timer for off-ledge jump leniency
    public bool bCanDoubleJump;
    private bool bIsDoubleJumping = false;
    private bool bIsGrounded = true;
    #endregion

    #region // Wall sliding
    private bool bIsWallSliding;
    private int iWallDirX;
    public float fWallSlideSpeedMax = 3f;
    public float fWallStickTime = .25f;
    private float fTimeToWallUnstick;
    // Disable input timer
    private float disableInputTime = 0.0f;
    #endregion

    #region // Dash vars
    [Header("Dash")]
    public bool bCanDash = true;
    public bool bCanDirDash = true;
    private bool bIsDashing = false;
    private float fNextDashTime;
    private float fDashDuration;
    public float fDashCooldown;
    private float fDashSpeed = 25f; //25f
    private float fDashTime = 0.2f; //0.2f
    #endregion

    #region // Animation bools
    [Header("AnimationBools")]
    public bool animTravelLeft;
    public bool animTravelDown;
    public bool animGrounded;
    public bool animDoubleJump;
    public bool animWallClimb;
    public bool animDash;
    #endregion

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        controller = GetComponent<Controller2D>();
        fGravity = -(2 * fMaxJumpHeight) / Mathf.Pow(fTimeToJumpApex, 2);
        fMaxJumpVelocity = Mathf.Abs(fGravity) * fTimeToJumpApex;
        fMinJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(fGravity) * fMinJumpHeight);

        animGrounded = true;
    }

    private void Update()
    {
        DisableInput();
        CalculateTimers();
        CheckDeath();
        CalculateVelocity();
        HandleWallSliding();

        CalculateAnimBools();

        #region // Dash code
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
                vVelocity.y = 0; // no arc (of the covenant nor otherwise)
            }
            fDashDuration += Time.deltaTime;
        }
        else
        { bIsDashing = false; animDash = false; }
        #endregion // dash code end

        #region // Apply MOVEMENT
        OnJumpInputDown();
        controller.Move(vVelocity * Time.deltaTime, vDirectionalInput);

        if (controller.collisions.above || controller.collisions.below)
        { vVelocity.y = 0f; }
        #endregion
    }

    private void CalculateTimers()
    {
        fJumpRejumpEarlyTime -= Time.deltaTime;    // leniency for press jump early before grounded
        fJumpLedgeEarlyTime -= Time.deltaTime;       // leniency for press jump when stepping off ledge

        if (controller.collisions.below) { fJumpLedgeEarlyTime = fJumpLedgeLeniency; }    // once grounded, set leniency time and start counting down
    }

    private void DisableInput()
    {
        if (disableInputTime > 0)
        {
            PlayerInput.disableInput = true;
            disableInputTime -= Time.deltaTime;
        }
        else
        {
            PlayerInput.disableInput = false;
        }
    }

    private void CheckDeath()
    {
        if (controller.transform.position.y < -10)
        {
            vVelocity = new Vector3(0, 0, 0);
            controller.transform.position = new Vector3(-3.8f, -2.29f, -1f);
        }
    }

    private void CalculateVelocity()
    {
        float targetVelocityX = vDirectionalInput.x * fMaxMoveSpeed;
        bIsGrounded = controller.collisions.below;

        vVelocity.x = targetVelocityX;

        if (bIsGrounded)
        {
            vVelocity.x = targetVelocityX;
        }
        else
        {
            if (Mathf.Abs(vDirectionalInput.x) > 0.2)
            {    // if you are wanting to actually stop your character with a partial or full controller input
                if (Mathf.Abs(vVelocity.x) < 0.01)
                { vVelocity.x = targetVelocityX; }  // if not moving horiz then take normal input
                else
                { vVelocity.x = Mathf.Abs(vVelocity.x) * vDirectionalInput.x; } // if moving horiz then maintain velocity and dampen with targetVelocity
            }
        }
        if (Mathf.Abs(vVelocity.x) > fMaxMoveSpeed) { vVelocity.x = Mathf.Sign(vDirectionalInput.x) * fMaxMoveSpeed; }



        vVelocity.y += fGravity * Time.deltaTime;

        anim.SetFloat("velocityX", Mathf.Abs(vVelocity.x));
        anim.SetFloat("velocityY", Mathf.Abs(vVelocity.y));
    }

    // *** ANIMATIONS *** //
    private void CalculateAnimBools()
    {
        animGrounded = controller.collisions.below;

        if (vVelocity.x < -0.5) { animTravelLeft = true; }
        if (vVelocity.x > 0.5) { animTravelLeft = false; }

        if (vVelocity.y < 0 && !animGrounded) { animTravelDown = true; }
        else { animTravelDown = false; }


        animDoubleJump = bIsDoubleJumping && !animGrounded;

        animWallClimb = bIsWallSliding;

        if (Time.time < 0.5f) { animGrounded = true; }  // force grounded anim while the character settles to ground

        SetAnimator();
    }

    public void SetAnimator()
    {
        if (animTravelLeft)
        { transform.localScale = new Vector3(-1f, 1f, 1f); }
        else
        { transform.localScale = new Vector3(1f, 1f, 1f); }

        if (animTravelDown)
        { anim.SetBool("falling", true); }
        else
        { anim.SetBool("falling", false); }

        if (animGrounded)
        { anim.SetBool("grounded", true); }
        else
        { anim.SetBool("grounded", false); }

        if (animWallClimb)
        { anim.SetBool("wallClimb", true); }
        else
        { anim.SetBool("wallClimb", false); }

        if (animDash)
        {
            anim.SetBool("dash", true);
        }
        else { anim.SetBool("dash", false); }
    }

    // *** SPECIFIC MOTION SUBROUTINES *** //
    public void SetDirectionalInput(Vector2 input)
    {
        vDirectionalInput = input;
    }

    public void OnJumpInputDown()
    {
        if (bIsWallSliding && bCallJump)
        {
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
            bIsDoubleJumping = false;
            bCallJump = false;  // end jump call
        }
        if ((fJumpRejumpEarlyTime > 0) && (fJumpLedgeEarlyTime > 0))    // did press within lenient times? (NO END JUMP CALL)
        {
            fJumpRejumpEarlyTime = 0;
            fJumpLedgeEarlyTime = 0;

            vVelocity.y = fMaxJumpVelocity;
            bIsDoubleJumping = false;
        }
        if (bCanDoubleJump && !controller.collisions.below && !bIsDoubleJumping && !bIsWallSliding && bCallJump)
        {
            vVelocity.y = fMaxJumpVelocity;
            bIsDoubleJumping = true;
            bCallJump = false;  // end jump call
            Debug.Log("doublejumop");
        }



    }

    public void OnJumpInputUp()
    {
        if (vVelocity.y > fMinJumpVelocity)
        {
            vVelocity.y = fMinJumpVelocity;
        }
    }

    public void OnDash()
    {
        //Debug.Log("dashed");
        fDashDuration = 0.0f;    // reset the current dash timer
        bIsDashing = true;         // state dashing
        animDash = true;
        disableInputTime = fDashTime;
    }

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
}
