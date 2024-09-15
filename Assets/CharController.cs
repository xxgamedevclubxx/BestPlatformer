using System;
using Godot;

public partial class CharController : CharacterBody2D
{
    [Export]
    private RayCast2D rightRay,
        leftRay;

    [Export]
    private Timer coyoteTimer,
        jumpBufferTimer,
        wallJumpTimer,
        wallJumpCoyoteTimer;

    // Movement settings
    private const float Speed = 250.0f;
    private const float SlideSpeed = 1750.0f;
    private const float JumpVelocity = -400.0f;
    private const float WallJumpHorizontalVelocity = 400.0f;
    private const float WallJumpVerticalVelocity = -400.0f;
    private const float Gravity = 800.0f;
    private const float WallSlideSpeed = 100.0f;

    private bool isWallSliding,
        isJumpingFromWall;

    private RayCast2D lastRayColliding = null;

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;
        bool WallLeft = leftRay.IsColliding();
        bool WallRight = rightRay.IsColliding();

        if (WallLeft)
        {
            lastRayColliding = leftRay;
        }
        else if (WallRight)
        {
            lastRayColliding = rightRay;
        }

        // Add the gravity.
        if (!IsOnFloor() && !isWallSliding)
        {
            velocity += GetGravity() * (float)delta;
        }

        //Wall sliding
        if (IsOnWall() && velocity.Y > 0 && !IsOnFloor())
        {
            isWallSliding = true;
            velocity.Y = WallSlideSpeed;
        }
        else
        {
            isWallSliding = false;
        }

        // Handle horizontal movement
        Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        velocity.X =
            direction != Vector2.Zero && wallJumpTimer.IsStopped()
                ? direction.X * Speed
                : Mathf.MoveToward(Velocity.X, 0, SlideSpeed * (float)delta);

        //Handle Wall Jump, timer included so the jump away from the wall is not cut too short.
        if (Input.IsActionJustPressed("ui_accept"))
        {
            // Floor jump condition
            if (IsOnFloor() || coyoteTimer.TimeLeft > 0.0)
            {
                velocity.Y = JumpVelocity;
            }
            // Wall jump condition
            else if (lastRayColliding != null && (IsOnWall() || wallJumpCoyoteTimer.TimeLeft > 0.0))
            {
                isJumpingFromWall = true;
                wallJumpTimer.Start();
                velocity.Y = WallJumpVerticalVelocity;
                if (lastRayColliding == leftRay)
                {
                    velocity.X = WallJumpHorizontalVelocity;
                }
                else
                {
                    velocity.X = -WallJumpHorizontalVelocity;
                }

                lastRayColliding = null;
            }
        }

        if (IsOnFloor())
        {
            isJumpingFromWall = false;
        }

        Velocity = velocity;
        bool wasOnFloor = IsOnFloor();
        bool wasOnWall = IsOnWall();
        MoveAndSlide();

        // Start coyote timer if the player just left a ledge
        if (wasOnFloor && !IsOnFloor() && velocity.Y >= 0)
        {
            coyoteTimer.Start();
        }
        if (wasOnWall && !IsOnWall())
        {
            wallJumpCoyoteTimer.Start();
        }
    }
}
