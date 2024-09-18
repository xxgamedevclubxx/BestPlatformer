using Godot;
using System;

public partial class CharController : CharacterBody2D
{

	private RayCast2D rightRay,
		leftRay;


	private Timer coyoteTimer,
		jumpBufferTimer,
		wallJumpTimer,
		wallJumpCoyoteTimer;

	// Movement settings
	private const float Speed = 250.0f;
	private const float SlideSpeed = 1750.0f;
	private const float WallJumpHorizontalVelocity = 400.0f;
	private const float WallJumpVerticalVelocity = -400.0f;
	private const float Gravity = 800.0f;
	private const float WallSlideSpeed = 100.0f;
    // Variables equivalent to Unity's serialized fields
    [Export] private float JumpPower = 500f;
    [Export] private float max_speed = 200f;
    [Export] private float accel = 1000f;
    [Export] private float Deceleration = 1000f;
    [Export] private float FallAcceleration = 1000f;
    [Export] private float MaxFallSpeed = 1000f;
    [Export] private float GroundingForce = 10f;
    [Export] private float JumpEndEarlyGravityModifier = 2f;
    [Export] private float JumpBuffer = 0.1f;  // Jump buffer

    // Cached time variable
    private float _time;

	    // Input gathering
    private Vector2 _inputMove = Vector2.Zero;
    private bool _jumpDown;
    private bool _jumpHeld;


    // Player state variables
    private Vector2 _frameVelocity = Vector2.Zero;
    private bool _jumpToConsume;
    private bool _bufferedJumpUsable;
    private bool _endedJumpEarly;
    private float _timeJumpWasPressed;
    private float _frameLeftGrounded = float.MinValue;
    private bool _grounded;



	private bool isWallSliding,
		isJumpingFromWall;

	private RayCast2D lastRayColliding = null;

	private AnimatedSprite2D sprite2d;

	public override void _Ready()
	{
		
		sprite2d = GetNode<AnimatedSprite2D>("Sprite2D");
		GD.Print(sprite2d);
		rightRay = GetNode<RayCast2D>("rightRay");
        leftRay = GetNode<RayCast2D>("leftRay");
        coyoteTimer = GetNode<Timer>("coyoteTimer");
        jumpBufferTimer = GetNode<Timer>("jumpBufferTimer");
        wallJumpTimer = GetNode<Timer>("wallJumpTimer");
        wallJumpCoyoteTimer = GetNode<Timer>("wallJumpCoyoteTimer");

	}
	
	public override void _PhysicsProcess(double delta)
	{

		Vector2 velocity = Velocity;
		bool WallLeft = leftRay.IsColliding();
		bool WallRight = rightRay.IsColliding();

		// Animations
		if (Math.Abs(velocity.X) > 5)
			sprite2d.Play("Running");
		else
			sprite2d.Animation = "default";
 

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


			
		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y -= JumpPower;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		if (direction != Vector2.Zero)
		{
    		velocity.X =
			direction != Vector2.Zero && wallJumpTimer.IsStopped()
				? direction.X * Speed
				: Mathf.MoveToward(Velocity.X, 0, SlideSpeed * (float)delta);
            }
		
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		}

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
		} else {
            isJumpingFromWall = true;

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

		Velocity = velocity;

		MoveAndSlide();
		bool isLeft = velocity.X < 0;
		bool isRight = velocity.X > 0;
		if(isLeft){
			sprite2d.FlipH = true;
		}else if(isRight){
			sprite2d.FlipH = false;
		}
	}
}
