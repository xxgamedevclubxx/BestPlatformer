using System;
using Godot;

public partial class Player : CharacterBody2D
{
    enum MoveStates : int {
        Idle,
        Move,
        Attack,
        Damage
    }

    enum JumpStates: int {
        Grounded,
        JumpFall,
        DoubleJump,
        WallCling
    }

    MoveStates moveState = MoveStates.Idle;
    JumpStates jumpState = JumpStates.JumpFall;

    public override void _Ready()
    {
        this.Velocity = new Vector2(0, 0);
    }

    public override void _Process(double delta)
    {
        

        if (Input.IsActionJustPressed("ui_right")) {
            this.Velocity.X = 50;
        }

        MoveAndSlide();
    }
}