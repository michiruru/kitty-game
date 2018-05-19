using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    private float moveVelocity;
    public bool canMove;

    [Header("Jumping")]
    public float jumpHeight;
    private float timeInJump { get { return Time.time - timeStartedJumping; } }
    private float maxJumpTime = 0.33f;
    public bool jumpReleased = true;
    private float timeStartedJumping;

    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask whatIsGround;
    public bool grounded;

    private bool doubleJumped;
    public bool canDoubleJump;

    [Header ("Projectile Attack")]
    public Transform firePoint;
    public GameObject ninjaStar;

    public float shotDelay;
    private float shotDelayCounter;
    private bool canShoot;

    [Header ("Knockback")]
    private float knockback;
    public float knockbackLength;
    public float knockbackCount;
    public bool knockFromRight;

    private Rigidbody2D myRigidbody2D;
    private Animator anim;

    [Header ("Cat Transformation")]

    public bool canTransform;
    public bool transforming;

    public GameObject transformParticle;

    [Header ("Dash Attack")]
    public bool canDash;
    public float nextDash = 1;
    public float dashCooldown = 2;
    public float dashSpeed = 100f;
    public float dashTime = 0.25f;
    public float dashtimer;
    //public float endDashTime; //the system time at which the dash will end = Time.time + dashTime
    public bool dashing;

    [Header("Ladder Climb")]
    public bool onLadder;
    public float climbSpeed;
    public float climbVelocity;
    private float gravityStore;

    [Header("Wall Climb")]

    public bool wallSliding;
    public bool wallJumping;
    public Transform wallCheckPoint;
    public bool wallCheck;
    public LayerMask wallLayerMask;
    public float wallJumpXForce = 7.5f;
    public float wallJumpYForce = 12f;
    public bool disableHorizControl;
    public bool disableVertControl;
    public float wallJumpDuration = 0.1f;

    public bool facingRight;

    public int i;
    public bool buttonDown;

    [Header ("Melee attacks")]
    public float attackTime;
    private bool attacking;
    private float attackTimeCounter;

    public float specialChargeDuration;
    public float specialChargeTime;
    public float specialChargeCounter;
    public bool charging;
    public bool specialAttack;
    public ParticleSystem chargeEffect;

    [Header("Rope Swing")]
    public bool onRope;

    // Use this for initialization
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        myRigidbody2D = GetComponent<Rigidbody2D>();
        canDoubleJump = false;
        canMove = true;
        canShoot = true;
        disableHorizControl = false;
        buttonDown = false;

        gravityStore = myRigidbody2D.gravityScale;

        //Transform();
        facingRight = true;
    }

    void FixedUpdate()
    {
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
    }

    // Update is called once per frame
    void Update()
    {
        // state functions
        if (!canMove)
        {
            myRigidbody2D.velocity = Vector2.zero;
            return;
        }

        // movement input speed
        moveVelocity = moveSpeed * Input.GetAxisRaw("Horizontal");

        if (dashing)
        {
            Dash();
            myRigidbody2D.gravityScale = 0f;
            Invoke("dashReset", dashTime);
        }

        if (!dashing)
        {
            movePlayer(moveVelocity, myRigidbody2D.velocity.y);
        }

        // Wall jumping section (wall check)
        if (Physics2D.OverlapCircle(wallCheckPoint.position, 0.1f, wallLayerMask)) // if player is touching wall
        {
            myRigidbody2D.velocity = new Vector2(myRigidbody2D.velocity.x, -gravityStore * 0.7f); // set slow slide down
            wallSliding = true;
            anim.SetBool("WallCling", true);
            if (wallSliding && Input.GetButtonDown("Jump"))
            {
                wallJumping = true;
            }
        }

        if (wallJumping)
        {
            disableHorizControl = true;
            disableVertControl = true;
            Invoke("wallJumpReset", wallJumpDuration);
        }

        if (wallJumping && wallSliding)
        {
            WallJump();
        }

        if (!wallJumping && !dashing)
        {
            movePlayer(moveVelocity, myRigidbody2D.velocity.y);
        }

        // ladder
        if (onLadder)
        {
            myRigidbody2D.gravityScale = 0f;
            climbVelocity = climbSpeed * Input.GetAxisRaw("Vertical");
            myRigidbody2D.velocity = new Vector2(myRigidbody2D.velocity.x, climbVelocity);
            anim.SetBool("WallCling", true);
        }

        if (!onLadder && !dashing)
        {
            myRigidbody2D.gravityScale = gravityStore;
        }
        // end ladder


        if (grounded)
        {
            // reset stats and anim
            doubleJumped = false;
            wallSliding = false;
            anim.SetBool("WallCling", false);
            anim.SetBool("Grounded", grounded);
            anim.SetBool("Land", false);
            anim.ResetTrigger("Jump");
            // jumping
            if(Input.GetButton("Jump") && jumpReleased)
            {
                //myRigidbody2D.velocity = new Vector2(myRigidbody2D.velocity.x, jumpHeight*0.5f);
                //myRigidbody2D.AddForce(Vector2.up * 3, ForceMode2D.Impulse);
                Jump(GetComponent<Rigidbody2D>().velocity.x, jumpHeight);
                timeStartedJumping = Time.time;
            }
            if (Input.GetButtonUp("Jump"))
            {
                jumpReleased = true;
            }

            //if (Input.GetButtonDown("Jump"))
            //{
            //    buttonDownTime = Time.time;
            //    Debug.Log(buttonDownTime);
            //}

            //if (Input.GetButtonUp("Jump"))
            //{
            //    buttonHoldDuration = Time.time - buttonDownTime;

            //    if (buttonHoldDuration < 0.25)
            //    {
            //        jumpMult = 0.5f;
            //    }
            //    else if (buttonHoldDuration < 0.5)
            //    {
            //        jumpMult = 1;
            //    }
            //    else if (buttonHoldDuration < 2)
            //    {
            //        jumpMult = 2;
            //    }
            //    else if (buttonHoldDuration < 3)
            //    {
            //        jumpMult = 5;
            //    }

            //    Debug.Log(buttonHoldDuration + "; " + jumpMult);
            //    Jump(GetComponent<Rigidbody2D>().velocity.x, jumpHeight, jumpMult);
            //}
        }
        else if (!grounded)
        {
            // reset stats and anim
            //anim.SetBool("Grounded", false);

            // double jumping
            if (Input.GetButton("Jump") && timeInJump < maxJumpTime)
            {
                //myRigidbody2D.velocity = new Vector2(myRigidbody2D.velocity.x, jumpHeight*0.5f);
                //myRigidbody2D.AddForce(Vector2.up * 30f, ForceMode2D.Force);
                Jump(myRigidbody2D.velocity.x, jumpHeight);
                jumpReleased = false;
            } else
            {
                timeStartedJumping = 0;
            }

            if (Input.GetButtonUp("Jump"))
            {
                jumpReleased = true;
            }

            if (canDoubleJump == true)
            {
                DoubleJump();
            }
        }


        HandleLayers();

        // dash
        if (canDash == true && Input.GetAxis("RightTrigger") > 0 && Time.time > nextDash) //push B and cooldown has expired
        {
            dashing = true;
            nextDash = Time.time + dashCooldown;
        }

        //animation and heading set
        if (myRigidbody2D.velocity.x > 0 && !wallJumping)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
            facingRight = true;
        }

        if (myRigidbody2D.velocity.x < 0 && !wallJumping)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
            facingRight = false;
        }
        else
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }

        /*// transform
        if (canTransform == true && Input.GetButtonDown("Fire2"))
        {
            Instantiate(transformParticle, transform.position, transform.rotation);
            transforming = !transforming;
            Transform();
        }*/

        // attacks
        if (canShoot == true)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Instantiate(ninjaStar, firePoint.position, firePoint.rotation);
                shotDelayCounter = shotDelay;
            }

            if (Input.GetButton("Fire1"))
            {
                shotDelayCounter -= Time.deltaTime;
                if (shotDelayCounter <= 0)
                {
                    shotDelayCounter = shotDelay;
                    Instantiate(ninjaStar, firePoint.position, firePoint.rotation);
                }
            }
        }

        //melee attack

        if (Input.GetButtonDown("Fire3"))
        {
            attackTimeCounter = attackTime;
            attacking = true;

            
        }

        if (attacking)
        {
            myRigidbody2D.velocity = new Vector2(0, myRigidbody2D.velocity.y);
            anim.SetBool("Attack", true);

        }
        else
        {
            anim.SetBool("Attack", false);

        }

        if (attackTimeCounter > 0)
        {
            attackTimeCounter -= Time.deltaTime;
        }

    if (attackTimeCounter <= 0)
        {
            attacking = false;
        }

        //special melee attack
   
        if (Input.GetButton("Fire3"))
        {
            charging = true;
            specialChargeCounter += Time.deltaTime;
            specialChargeDuration = specialChargeTime;

        }

        if (Input.GetButtonUp("Fire3"))
        {
            charging = false;
        }

        if (charging)
        {
            chargeEffect.gameObject.SetActive(true);

            var main = chargeEffect.main;
            
            if (specialChargeCounter > 3)
            {
                
                main.startColor = new Color(0,0,1,1);
            }
            else
            {
                main.startColor = new Color(1, 190, 190, 1);
            }
        }
        else
        {
            chargeEffect.gameObject.SetActive(false);
        }

        if (Input.GetButtonUp("Fire3") && specialChargeCounter > 3)
        {
            charging = false;
            SpecialAttack();
        }

       
        if (specialAttack)
        {
            specialChargeDuration -= Time.deltaTime;
            specialChargeCounter = 0;
        }

        if(specialChargeDuration <= 0)
        {
            anim.SetBool("SpecialAttack", false);
            specialChargeDuration = 0;
            specialAttack = false;
        }
        
    
    }

    void movePlayer(float velocInX, float velocInY)
    {

        if (disableHorizControl)
        {
            velocInX = 0f;
        }

        if (disableVertControl)
        {
            velocInY = 0f;
        }

        if (knockbackCount <= 0)
        {
            myRigidbody2D.velocity = new Vector2(velocInX, velocInY); // myRigidbody2D.velocity.y);
        }
        else
        {
            if (knockFromRight)
                myRigidbody2D.velocity = new Vector2(-knockback, knockback);
            if (!knockFromRight)
                myRigidbody2D.velocity = new Vector2(knockback, knockback);
            knockbackCount -= Time.deltaTime;
        }

        if (myRigidbody2D.velocity.y < 0)
        {
            anim.SetBool("Land", true);
        }

        anim.SetFloat("Speed", Mathf.Abs(myRigidbody2D.velocity.x));
    }


    public void Jump(float velocX, float jumpHeightIn, float heightMult = 1)
    {
        myRigidbody2D.velocity = new Vector2(velocX, jumpHeight);
        //GetComponent<AudioSource>().Play();
        anim.SetTrigger("Jump");
    }

    public void DoubleJump()
    {
        if (Input.GetButtonDown("Jump") && !doubleJumped && !grounded)
        {
            Jump(GetComponent<Rigidbody2D>().velocity.x, jumpHeight);
            doubleJumped = true;
        }
    }

    /*public void Transform()
    {
        // Cat mode stats        

        if (transforming)
        {
            anim.SetBool("Tranform", true);
            moveSpeed = 10;
            jumpHeight = 10;
            canShoot = false;
           // canDash = true;
        }
        if (!transforming)
        {
            anim.SetBool("Tranform", false);
            moveSpeed = 7;
            jumpHeight = 15;
            canShoot = true;
            //canDash = false;
        }
    }*/

    private void dashReset()
    {
        anim.SetBool("Dashing", false);
        dashing = false;
        dashtimer = 0f;
    }

    private void wallJumpReset()
    {
        anim.SetBool("WallCling", false);
        wallJumping = false;
        disableHorizControl = false;
        disableVertControl = false;
    }

    private void Dash()
    {
        //Cat dash implement
        anim.SetBool("Dashing", true);

        if (facingRight)
        {
            movePlayer(dashSpeed, 0f);
            //Debug.Log("dash on right" + dashSpeed);
        }
        else if (!facingRight)
        {
            movePlayer(-dashSpeed, 0f);
        }
    }


    void WallJump()
    {
        if (facingRight) // player faces right on LEFT side of wall, hence XFroce is to the LEFT (negative)
        {
            Jump(-wallJumpXForce, wallJumpYForce);
        }

        if (!facingRight)
        {
            Jump(wallJumpXForce, wallJumpYForce);
        }
        //myRigidbody2D.AddForce(new Vector2(jumpPushForce, wallJumpForce));
        //if (!facingRight)
        //{
        //    myRigidbody2D.AddForce(new Vector2(-jumpPushForce, wallJumpForce));
        //}
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.tag == "Moving Platform")
        {
            transform.parent = other.transform;
        }

    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (other.transform.tag == "Moving Platform")
        {
            transform.parent = null;
        }

    
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Rope")
        {
            onRope = true;
            transform.parent = other.transform;


        }
    }
    void SpecialAttack()
    {
        specialAttack = true;
        anim.SetBool("SpecialAttack", true);

    }
  
    private void HandleLayers()
    {
        if (!grounded)
        {
            anim.SetLayerWeight(1, 1);
        }
        else
        {
            anim.SetLayerWeight(1, 0);
        }
    }

}
