using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour
{
    private Player player;
    private float nextDashTime;

    [HideInInspector]
    public static bool disableInput;

    private void Start()
    {
        player = GetComponent<Player>();
    }

    private void Update()
	{
		if (!disableInput) {
			Vector2 directionalInput = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
			player.SetDirectionalInput (directionalInput);

			if (Input.GetButtonDown ("Jump")) {
				player.OnJumpInputDown ();
			}

			if (Input.GetButtonUp ("Jump")) {
				player.OnJumpInputUp ();
			}

			if (Input.GetButtonDown ("Dash")) {
                if (nextDashTime <= Time.time)
                {
                    nextDashTime = Time.time + player.dashCooldown;
                    player.OnDash();    
                }

			}
		}
	}
}
