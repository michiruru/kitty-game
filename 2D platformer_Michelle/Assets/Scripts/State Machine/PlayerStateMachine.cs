using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class PlayerStateMachine : MonoBehaviour
{

    public Vector3 PlayerCheckpoint = new Vector3(-16.54f, 4.35f, 0f);    // location of the player checkpoint

    //DECLARATIONS
    #region PlayerStats
    public float _playerMaxHealth;
    public float _playerCurrentHealth;
    #endregion

    #region SkillsBools
    public bool _canDoubleJump;
    public bool _canWallClimb;
    public bool _canDash = true;
    #endregion

    #region GameObjects
    private Controller2D _controller;
    private Animator _anim;
    private Renderer _playerSprite;
    private BoxCollider2D _playerCollider;
    #endregion

    #region EnviroPhysics
    [Header("Physics Vars")]
    public float _moveSpeed = 6f;
    private float _gravity;
    private Vector3 _velocity;
    private float _yVelocity;
    private float _velocityXSmoothing;
    private Vector2 _directionalInput;
    //public string _colliderTagHoriz;
    //public string _colliderTagVert;
    #endregion

    #region Jump
    [Header("Jump Vars")]
    public float _maxJumpHeight = 4f;
    [HideInInspector] public float _minJumpHeight = 1f;
    public float _timeToJumpApex = .4f;
    private float _maxJumpVelocity;
    private float _minJumpVelocity;
    private float _accelerationTimeAirborne = .2f;
    private float _accelerationTimeGrounded = .1f;

    public float _jumpLateLeniency = 0.2f;
    private float _jumpLateTimer;         // timer to determine how long since !grounded

    public float _fallDelay;    // ~= 2* fJumpLateLeniency : this is the delay before turning on the falling animation (used to hide the flickering when going over small bumps)
    private float _fallDelayTimer;

    public bool _hasDoubleJumped = false;
    public bool _isJumping;
    public int _jumpCount = 0;
    public int _maxJumpCount = 2;
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

    //state variable
    PlayerBaseState _currentState;
    PlayerStateFactory _states;

    //getters and setters
    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public Animator Animator { get { return _anim; } }
    public Controller2D Controller { get { return _controller; } set { _controller = value; } }

    public float YVelocity { get { return _velocity.y; } set { _velocity.y = value; } }
    public bool IsJumping { set { _isJumping = value; } }
    public bool CanDoubleJump { get { return _canDoubleJump; } }
    public bool HasDoubleJumped { set { _hasDoubleJumped = value; } }
    public float MaxJumpVelocity { get { return _maxJumpVelocity; } set { _maxJumpVelocity = value; } }
    public float MinJumpVelocity { get { return _minJumpVelocity; } set { _minJumpVelocity = value; } }
    public float JumpLateTimer { get { return _jumpLateTimer; } set { _jumpLateTimer = value; } }
    public float JumpLateLeniency { get { return _jumpLateLeniency; } set { _jumpLateLeniency = value; } }
    public int JumpCount { get { return _jumpCount; } set { _jumpCount = value; } }
    public int MaxJumpCount { get { return _maxJumpCount; } set { _maxJumpCount = value; } }
    public float FallDelayTimer { get { return _fallDelayTimer; } set { _fallDelayTimer = value; } }
    public float FallDelay { get { return _fallDelay; } set { _fallDelay = value; } }
    public float Gravity { get { return _gravity; } set { _gravity = value; } }
    public float MaxJumpHeight { get { return _maxJumpHeight; } set { _maxJumpHeight = value; } }
    public float TimeToJumpApex { get { return _timeToJumpApex; } set { _timeToJumpApex = value; } }



    void Awake()
    {
        _anim = GetComponentInChildren<Animator>();
        _controller = GetComponent<Controller2D>();
        _playerSprite = GetComponentInChildren<Renderer>();
        _playerCollider = GetComponentInChildren<BoxCollider2D>();

        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();


    }

    // Start is called before the first frame update
    void Start()
    {
        _gravity = -(2 * _maxJumpHeight) / Mathf.Pow(_timeToJumpApex, 2);
        _maxJumpVelocity = Mathf.Abs(_gravity) * _timeToJumpApex;
        _minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(_gravity) * _minJumpHeight);
        //_gravityStore = fGravity;
        //animGrounded = true;

        _playerMaxHealth = 80f;
        _playerCurrentHealth = 80f;
    }

    // Update is called once per frame
    void Update()
    {
        //ResetSkills();
        //CalculateTimers();
        CheckDeath();
        CalculateVelocity();

        //CheckCollision();

        #region Apply MOVEMENT
        _controller.Move(_velocity * Time.deltaTime, _directionalInput);

        if (_controller.collisions.above || _controller.collisions.below)
        {
            _velocity.y = 0f;
        }

        #endregion

        _currentState.UpdateState();

    }

    private void CheckDeath()
    {
        /*if (controller.transform.position.y < -10)
        {
            Respawn(PlayerCheckpoint);
        }*/

        if (_playerCurrentHealth <= 0)
        {
            Respawn(PlayerCheckpoint);
        }
    }

    private void CalculateVelocity()
    {
        float targetVelocityX = _directionalInput.x * _moveSpeed;
        _velocity.x = Mathf.SmoothDamp(_velocity.x, targetVelocityX, ref _velocityXSmoothing, (_controller.collisions.below ? _accelerationTimeGrounded : _accelerationTimeAirborne));
        _velocity.y += _gravity * Time.deltaTime;
    }

    /*private void CheckCollision()
    {
        _colliderTagHoriz = Controller2D.sCollisionWithHoriz;
        _colliderTagVert = Controller2D.sCollisionWithVert;
    }*/

    public void Respawn(Vector3 SpawnPosition)
    {
        transform.position = SpawnPosition;
        _velocity.x = _velocity.y = 0;
    }
}
