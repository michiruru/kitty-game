using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour
{
    private Player player;
    private float nextDashTime;
    
    [HideInInspector]
    public static bool disableInput;
    public static bool disableJump;

    private void Start()
    {
        player = GetComponent<Player>();
        disableInput = false;
        disableJump = false;
    }

    private void Update()
    {
        if (!disableInput)
        {
            Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            player.SetDirectionalInput(directionalInput);

            if (!disableJump)
            {
                if (Input.GetButtonDown("Jump"))
                {
                    if (player.bCanDoubleJump && !player.bIsGrounded && !player.bIsDoubleJumping)   // if ready, do DoubleJump
                    {
                        player.DoubleJump();
                    }
                    else
                    {
                        player.fJumpRejumpEarlyTime = player.fJumpEarlyLeniency;
                        player.bCallJump = true;    // call a jump
                    }
                }

                if (Input.GetButtonUp("Jump"))
                { player.OnJumpInputUp(); } // ensure that jump is AT LEAST the minimum velocity.y

                if (Input.GetButtonDown("Dash"))
                {
                    if (nextDashTime <= Time.time)
                    {
                        nextDashTime = Time.time + player.fDashCooldown;
                        player.OnDash();
                    }
                }
            }
        }
    }
}
