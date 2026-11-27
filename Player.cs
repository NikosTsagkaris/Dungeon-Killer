using Godot;

public partial class Player : CharacterBody2D
{
	[Export]
	public float Speed = 200.0f;

	private AnimatedSprite2D _animSprite;

	public override void _Ready()
	{
		_animSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");

		Velocity = direction * Speed;
		MoveAndSlide();

		if (direction != Vector2.Zero)
		{
			// Παίζει το animation του περπατήματος όταν κινείται
			_animSprite.Play("walk");

			if (direction.X != 0)
			{
				_animSprite.FlipH = direction.X < 0;
			}
		}
		else
		{
			// Γυρίζει στο idle όταν σταματάει
			_animSprite.Play("idle");
		}
	}
}
