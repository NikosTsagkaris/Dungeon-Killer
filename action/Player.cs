using Godot;

public partial class Player : CharacterBody2D
{
	[Export]
	public float Speed = 200.0f;

	[Export]
	public int MaxHealth = 100;

	private int _currentHealth;
	private Sprite2D _sprite;
	private Sprite2D _crosshair;
	private Area2D _hurtbox;
	private ProgressBar _healthBar;

	private double _walkTime = 0.0;
	private bool _canTakeDamage = true;

	public override void _Ready()
	{
		_currentHealth = MaxHealth;
		_sprite = GetNode<Sprite2D>("Sprite2D");
		_hurtbox = GetNode<Area2D>("Hurtbox");

		// Αν έχεις βάλει το Crosshair node
		if (HasNode("Crosshair"))
		{
			_crosshair = GetNode<Sprite2D>("Crosshair");
			Input.MouseMode = Input.MouseModeEnum.Hidden;
		}

		// Βρίσκει το HealthBar μέσα από το UI CanvasLayer της σκηνής
		_healthBar = GetTree().Root.FindChild("HealthBar", true, false) as ProgressBar;
		if (_healthBar != null)
		{
			_healthBar.MaxValue = MaxHealth;
			_healthBar.Value = _currentHealth;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		// Κίνηση WASD
		Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		Velocity = direction * Speed;
		MoveAndSlide();

		// Κίνηση Crosshair & στροφή προς το ποντίκι
		Vector2 mousePos = GetGlobalMousePosition();
		if (_crosshair != null)
			_crosshair.GlobalPosition = mousePos;

		_sprite.FlipH = mousePos.X < GlobalPosition.X;

		// Animation περπατήματος
		if (direction != Vector2.Zero)
		{
			_walkTime += delta * 15.0;
			_sprite.Rotation = Mathf.Sin((float)_walkTime) * 0.12f;
			_sprite.Position = new Vector2(0, -Mathf.Abs(Mathf.Sin((float)_walkTime)) * 4.0f);
		}
		else
		{
			_walkTime = 0.0;
			_sprite.Rotation = 0.0f;
			_sprite.Position = Vector2.Zero;
		}

		// Έλεγχος αν κάποιος σκελετός ακουμπάει τον Player
		if (_canTakeDamage)
		{
			foreach (Node2D body in _hurtbox.GetOverlappingBodies())
			{
				if (body.IsInGroup("enemies"))
				{
					TakeDamage(5); // Κόβει 20 HP (5 χτυπήματα μέχρι το 0)
					break;
				}
			}
		}
	}

	private async void TakeDamage(int amount)
	{
		_canTakeDamage = false;
		_currentHealth = Mathf.Max(0, _currentHealth - amount);

		if (_healthBar != null)
			_healthBar.Value = _currentHealth;

		// Αναβοσβήνει κόκκινος ο παίκτης για 1 δευτερόλεπτο
		_sprite.Modulate = new Color(1.0f, 0.3f, 0.3f);
		await ToSignal(GetTree().CreateTimer(1.0), SceneTreeTimer.SignalName.Timeout);

		if (!IsInstanceValid(this))
			return;

		_sprite.Modulate = Colors.White;
		_canTakeDamage = true;

		// Αν μηδενιστεί η ζωή, κάνει restart την πίστα
		if (_currentHealth <= 0)
		{
			GetTree().ReloadCurrentScene();
		}
	}
}
