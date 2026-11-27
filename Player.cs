using Godot;

public partial class Player : CharacterBody2D
{
	[Export]
	public float Speed = 200.0f;

	private Sprite2D _sprite;
	private double _walkTime = 0.0;

	public override void _Ready()
	{
		_sprite = GetNode<Sprite2D>("Sprite2D");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");

		Velocity = direction * Speed;
		MoveAndSlide();

		if (direction != Vector2.Zero)
		{
			if (direction.X != 0)
			{
				_sprite.FlipH = direction.X < 0;
			}

			// Δημιουργεί ρυθμικό κούνημα αριστερά-δεξιά και μικρά βηματάκια πάνω-κάτω
			_walkTime += delta * 15.0;
			_sprite.Rotation = Mathf.Sin((float)_walkTime) * 0.12f;
			_sprite.Position = new Vector2(0, -Mathf.Abs(Mathf.Sin((float)_walkTime)) * 4.0f);
		}
		else
		{
			// Επαναφέρει τον παίκτη ίσιο όταν σταματάει
			_walkTime = 0.0;
			_sprite.Rotation = 0.0f;
			_sprite.Position = Vector2.Zero;
		}
	}
}
