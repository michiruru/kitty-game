using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerBaseState
{
    public PlayerGroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }

    public override void EnterState() {
        Debug.Log("HELLO FROM GROUNDED STATE");
        //_ctx.JumpCount = 0;
        _ctx.IsJumping = false;
        _ctx.Animator.Play("idle");

    }

    public override void UpdateState() {

        CheckSwitchStates();

    }

    public override void ExitState() { }

    public override void CheckSwitchStates() {
        // if players is grounded and jump is pressed, switch to jump state
        if (_ctx.Controller.collisions.below && Input.GetButtonDown("Jump"))
        {
            SwitchStates(_factory.Jump());
        }

     }

    public override void InitializeSubState() { }

}
