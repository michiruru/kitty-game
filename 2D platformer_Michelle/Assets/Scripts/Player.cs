using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    // Inputs
    private Controller2D controller;
    private Vector2 directionalInput;
    private Vector3 velocity;
    private float velocityXSmoothing;

    public float moveSpeed = 6f;
    private float smoothTimeX;

    // Jumping vars
    [HideInInspector]
    public Vector2 wallJumpClimb;
    [HideInInspector]
    public Vector2 wallJumpOff;
    [HideInInspector]
    public Vector2 wallLeap;
    [HideInInspector]
    public float maxJumpHeight = 4f;
    [HideInInspector]
    public float minJumpHeight = 1f;
    [HideInInspector]
    public float timeToJumpApex = .5f;

    public bool canDoubleJump;
    private bool isDoubleJumping = false;
    public bool isGrounded = true;

    private float gravity;
    private float maxJumpVelocity;
    private float minJumpVelocity;


    // Wall sliding
    private bool wallSliding;
    private int wallDirX;
    public float wallSlideSpeedMax = 3f;
    public float wallStickTime = .25f;
    private float timeToWallUnstick;
    // Disable input timer
    private float disableInputTime = 0.0f;

    [Header("Dash")]
    public bool canDash = true;
    private bool dashing = false;
    private float nextDash;
    private float dashDuration;
    public float dashCooldown;
    private float dashSpeed = 25f;
    private float dashTime = 0.2f;

    [Header("AnimationBools")]
    public bool animTravelLeft;
    public bool animTravelDown;
    public bool animGrounded;
    public bool animDoubleJump;
    public bool animWallClimb;
    public bool animDash;

    private Animator anim;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        controller = GetComponent<Controller2D>();
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

        animGrounded = true;
    }

    private void Update()
    {
        DisableInput();
        CheckDeath();
        CalculateVelocity();
        HandleWallSliding();

        CalculateAnimBools();

        // dash code override
        if (dashing && dashDuration < dashTime)
        {
            velocity.x = dashSpeed * (animTravelLeft ? -1 : 1);
            velocity.y = 0; // no arc (of the covenant nor otherwise)
            dashDuration += Time.deltaTime;
        }
        else
        { dashing = false; animDash = false; }
        // dash code end

        controller.Move(velocity * Time.deltaTime, directionalInput);


        if (controller.collisions.above || controller.collisions.below)
        { velocity.y = 0f; }
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
        if(controller.transform.position.y < -10){
            velocity = new Vector3(0, 0, 0);
            controller.transform.position = new Vector3(-3.8f, -2.29f, -1f);
        }
    }

    private void CalculateVelocity()
    {
        float targetVelocityX = directionalInput.x * moveSpeed;

        isGrounded = controller.collisions.below;

        if (isGrounded)
        {
            velocity.x = targetVelocityX;
        }else{
            if (Mathf.Abs(directionalInput.x)>0.2){    // if you are wanting to actually stop your character with a partial or full controller input
                velocity.x = Mathf.Abs(velocity.x) * directionalInput.x;
            }
        }
        velocity.y += gravity * Time.deltaTime;

        anim.SetFloat("velocityX", Mathf.Abs(velocity.x));
        anim.SetFloat("velocityY", Mathf.Abs(velocity.y));
    }

    // *** ANIMATIONS *** //

    private void CalculateAnimBools()
    {
        animGrounded = controller.collisions.below;

        if (velocity.x < -0.5) { animTravelLeft = true; }
        if (velocity.x > 0.5) { animTravelLeft = false; }

        if (velocity.y < 0 && !animGrounded) { animTravelDown = true; }
        else { animTravelDown = false; }


        animDoubleJump = isDoubleJumping && !animGrounded;

        animWallClimb = wallSliding;

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
    }

    // *** SPECIFIC MOTION SUBROUTINES *** //
    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }

    public void OnJumpInputDown()
    {
        if (wallSliding)
        {
            if (wallDirX == directionalInput.x)
            {
                velocity.x = -wallDirX * wallJumpClimb.x;
                velocity.y = wallJumpClimb.y;
            }
            else if (directionalInput.x == 0)
            {
                velocity.x = -wallDirX * wallJumpOff.x;
                velocity.y = wallJumpOff.y;
            }
            else
            {
                velocity.x = -wallDirX * wallLeap.x;
                velocity.y = wallLeap.y;
            }
            isDoubleJumping = false;
        }
        if (controller.collisions.below)
        {
            velocity.y = maxJumpVelocity;
            isDoubleJumping = false;
        }
        if (canDoubleJump && !controller.collisions.below && !isDoubleJumping && !wallSliding)
        {
            velocity.y = maxJumpVelocity;
            isDoubleJumping = true;
        }
    }

    public void OnJumpInputUp()
    {
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
    }

    public void OnDash()
    {
        //Debug.Log("dashed");
        dashDuration = 0.0f;    // reset the current dash timer
        dashing = true;         // state dashing
        animDash = true;
        disableInputTime = dashTime;
    }

    private void HandleWallSliding()
    {
        wallDirX = (controller.collisions.left) ? -1 : 1;
        wallSliding = false;
        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0)
        {
            wallSliding = true;

            if (velocity.y < -wallSlideSpeedMax)
            {
                velocity.y = -wallSlideSpeedMax;
            }

            if (timeToWallUnstick > 0f)
            {
                velocityXSmoothing = 0f;
                velocity.x = 0f;
                if (directionalInput.x != wallDirX && directionalInput.x != 0f)
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
}
