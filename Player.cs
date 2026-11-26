using Godot;

public partial class Player : CharacterBody2D
{
	[Export]
	public float Speed = 200.0f;

	private Sprite2D _sprite;

	public override void _Ready()
	{
		// Αν χρησιμοποιείς απλό Sprite2D αντί για AnimatedSprite2D, άλλαξε τον τύπο σε Sprite2D
		_sprite = GetNode<Sprite2D>("Sprite2D");
	}

	public override void _PhysicsProcess(double delta)
	{
		// Διαβάζει τα WASD και κανονικοποιεί τη διαγώνια κίνηση ώστε να μην τρέχει πιο γρήγορα
		Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");

		Velocity = direction * Speed;
		MoveAndSlide();

		// Γυρίζει το sprite αριστερά/δεξιά ανάλογα με την κατεύθυνση
		if (direction.X != 0)
		{
			_sprite.FlipH = direction.X < 0;
		}
	}
}
