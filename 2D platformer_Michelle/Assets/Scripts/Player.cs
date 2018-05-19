﻿using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    public float maxJumpHeight = 4f;
    public float minJumpHeight = 1f;
    public float timeToJumpApex = .4f;
    private float accelerationTimeAirborne = .2f;
    private float accelerationTimeGrounded = .1f;
    public float moveSpeed = 6f;
	private float smoothTimeX;

    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;

    public bool canDoubleJump;
    private bool isDoubleJumping = false;

    public float wallSlideSpeedMax = 3f;
    public float wallStickTime = .25f;
    private float timeToWallUnstick;

    private float gravity;
    private float maxJumpVelocity;
    private float minJumpVelocity;
    private Vector3 velocity;
    private float velocityXSmoothing;

    private Controller2D controller;
    private Vector2 directionalInput;
    private bool wallSliding;
    private int wallDirX;
	private float disableInputTime = 0.0f;

	[Header ("Dash")]
	public bool canDash = true;
	private float nextDash;
	private float endDash;
	public float dashCooldown = 2;
	public float dashSpeed = 10f;
	public float dashTime = 0.75f;

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
    }

    private void Update()
    {
		DisableInput();
		CalculateVelocity();
        HandleWallSliding();

        CalculateAnimBools();

        controller.Move(velocity * Time.deltaTime, directionalInput);

        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0f;
        }


    }

	private void DisableInput()
	{
		if (disableInputTime > 0) {
			PlayerInput.disableInput = true;
			disableInputTime -= Time.deltaTime;
		} else {
			PlayerInput.disableInput = false;
		}
	}

    private void CalculateVelocity()
    {

		// currently the directionInput gives both a direction (implictly) and degree (for joystick controller)
		// however when the char changes direction, the char 'jumps' by ?MovementSpeed

		float targetVelocityX = directionalInput.x * moveSpeed;

		//velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below ? accelerationTimeGrounded : accelerationTimeAirborne));
		velocity.x = targetVelocityX;
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

        SetAnimator();
    }

    public void SetAnimator()
    {
        if (animTravelLeft)
        	{transform.localScale = new Vector3(-1f, 1f, 1f);}
        else
        	{transform.localScale = new Vector3(1f, 1f, 1f);}

        if (animTravelDown)
        	{anim.SetBool("falling", true);}
        else 
			{anim.SetBool("falling", false);}

        if (animGrounded)
        	{anim.SetBool("grounded", true);}
        else 
			{anim.SetBool("grounded", false);}

        if (animWallClimb)
			{anim.SetBool("wallClimb", true);}
        else
        	{anim.SetBool("wallClimb", false);}
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