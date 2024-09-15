using Godot;
using System;

public partial class CharController : CharacterBody2D
{

	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;
	private AnimatedSprite2D sprite2d;
	
	public override void _Ready()
	{
		sprite2d = GetNode<AnimatedSprite2D>("Sprite2D");
		GD.Print(sprite2d);
	}
	
	public override void _PhysicsProcess(double delta)
	{

		Vector2 velocity = Velocity;

		// Animations
		if (Math.Abs(velocity.X) > 5)
			sprite2d.Play("Running");
		else
			sprite2d.Animation = "default";
 


		// Add the gravity.
		if (!IsOnFloor())
		{
			sprite2d.Animation = "Jumping";
			velocity += GetGravity() * (float)delta;
		}
			
		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
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
