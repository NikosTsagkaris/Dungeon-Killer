using Godot;

public partial class Skeleton : CharacterBody2D
{
	[Export]
	public float Speed = 30.0f;

	[Export]
	public int MaxHits = 5;

	[Export]
	public float AttackRange = 60.0f;

	[Export]
	public float KnockbackStrength = 220.0f; // Πόσο πίσω πετάγεται στο χτύπημα

	[Export]
	public float KnockupHeight = 16.0f; // Πόσο ψηλά ανασηκώνεται (knockup)

	private int _currentHits;
	private Node2D _player;
	private Sprite2D _sprite;
	private Area2D _hitbox;
	private double _wobbleTime = 0.0;
	private bool _mouseHovering = false;

	private Vector2 _knockbackVelocity = Vector2.Zero;
	private float _knockupOffset = 0.0f;

	public override void _Ready()
	{
		_currentHits = MaxHits;
		_sprite = GetNode<Sprite2D>("Sprite2D");
		_hitbox = GetNode<Area2D>("Hitbox");
		_player = GetTree().GetFirstNodeInGroup("player") as Node2D;
		_wobbleTime = GD.RandRange(0.0, 10.0);

		_hitbox.MouseEntered += () => _mouseHovering = true;
		_hitbox.MouseExited += () => _mouseHovering = false;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_player == null || !IsInstanceValid(_player))
			return;

		float distanceToPlayer = GlobalPosition.DistanceTo(_player.GlobalPosition);
		Vector2 direction = (_player.GlobalPosition - GlobalPosition).Normalized();

		// Αν έχει δεχτεί χτύπημα, υπερισχύει προσωρινά το knockback αντί για το περπάτημα
		if (_knockbackVelocity.Length() > 10.0f)
		{
			Velocity = _knockbackVelocity;
			_knockbackVelocity = _knockbackVelocity.Lerp(Vector2.Zero, (float)delta * 12.0f);
		}
		else
		{
			_knockbackVelocity = Vector2.Zero;
			Velocity = direction * Speed;
		}

		MoveAndSlide();

		if (direction.X != 0)
			_sprite.FlipH = direction.X < 0;

		// Επαναφέρει ομαλά το ύψος του knockup στο 0
		_knockupOffset = Mathf.Lerp(_knockupOffset, 0.0f, (float)delta * 10.0f);

		// Περπάτημα + ύψος knockup μαζί
		_wobbleTime += delta * 9.0;
		_sprite.Rotation = Mathf.Sin((float)_wobbleTime) * 0.14f;
		float walkBounce = Mathf.Abs(Mathf.Sin((float)_wobbleTime)) * 3.0f;
		_sprite.Position = new Vector2(0, -(walkBounce + _knockupOffset));

		if (_mouseHovering && Input.IsActionJustPressed("attack") && distanceToPlayer <= AttackRange)
		{
			TakeHit();
		}
	}

	public async void TakeHit()
	{
		_currentHits--;

		// Εφαρμόζει το Knockup (ανασήκωμα) και το Knockback (σπρώξιμο μακριά από τον Player)
		_knockupOffset = KnockupHeight;
		if (_player != null && IsInstanceValid(_player))
		{
			Vector2 pushDir = (GlobalPosition - _player.GlobalPosition).Normalized();
			_knockbackVelocity = pushDir * KnockbackStrength;
		}

		_sprite.Modulate = new Color(1.0f, 0.3f, 0.3f);
		await ToSignal(GetTree().CreateTimer(0.1), SceneTreeTimer.SignalName.Timeout);

		if (!IsInstanceValid(this))
			return;

		_sprite.Modulate = Colors.White;

		if (_currentHits <= 0)
		{
			QueueFree();
		}
	}
}
