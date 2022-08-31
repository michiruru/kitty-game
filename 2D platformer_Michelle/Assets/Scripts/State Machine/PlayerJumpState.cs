using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }


    public override void EnterState() {
        Debug.Log("HELLO FROM JUMP");
        Jump();

       


    }

    public override void UpdateState() {
        CheckSwitchStates();

    }

    public override void ExitState()
    {
        

    }

    public override void CheckSwitchStates() {
        if (_ctx.Controller.collisions.below) { SwitchStates(_factory.Grounded()); }


    }

    public override void InitializeSubState() { }

    public void Jump()
    {
        _ctx.YVelocity = _ctx.MaxJumpVelocity;

       }

    public void OnJumpInputDown()
    {
        _ctx.Animator.Play("jump");
        if (_ctx.JumpLateTimer < _ctx.JumpLateLeniency) // a normal jump
        {
           // _ctx.IsJumping = true;
            _ctx.YVelocity = _ctx.MaxJumpVelocity;
            _ctx.JumpCount++;

            _ctx.JumpLateTimer = _ctx.JumpLateLeniency + 1f;    // override timer (will reset on next grounding)
            //bHasDoubleJumped = false;
            //Debug.Log("normal jump");
        }

        if (/*_ctx.CanDoubleJump &&*/ (_ctx.JumpCount < _ctx.MaxJumpCount)) // double jump
        {
           // _ctx.IsJumping = true;
            _ctx.YVelocity = _ctx.MaxJumpVelocity;
            //_ctx.HasDoubleJumped = true;
           // _ctx.JumpCount++;
            //Debug.Log("double jump");
        }
    }

    public void OnJumpInputUp()
    {
        if (_ctx.YVelocity > _ctx.MinJumpVelocity)
        {
            _ctx.YVelocity = _ctx.MinJumpVelocity;
        }

    }
}
