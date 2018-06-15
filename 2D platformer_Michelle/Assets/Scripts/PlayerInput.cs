using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour
{
    private Player player;
    private float nextDashTime; // absolute time that the next dash can occur

    [HideInInspector] public static bool disableInput;
    public static float disableInputTime;
    [HideInInspector] public static bool disableJump;

    private void Start()
    {
        player = GetComponent<Player>();
        disableInput = false;
        disableJump = false;
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
            }

            if (Input.GetButtonDown("Dash"))
            {
                if (nextDashTime <= Time.time)
                {
                    nextDashTime = Time.time + player.fDashCooldown;
                    player.Dash();
                }
            }
        }

        if (Input.GetButtonDown("Reset"))
        {
            player.Respawn();
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
        if (player.fTimeSinceDashEnd <= (player.fDashTime + player.fDashImpactLeniency)) { player.fTimeSinceDashEnd += Time.deltaTime; }    // fTimeSinceDashEnd resets when dash starts, hence run this timer until end of dash (fDashTime) + leniency (fDash...Leniency)
    }
}