using Godot;
using System.Collections.Generic;

public partial class SkeletonSpawner : Node2D
{
	[Export]
	public PackedScene SkeletonScene; // Εδώ θα σύρεις το Skeleton.tscn

	[Export]
	public int SkeletonsPerDoor = 3; // 3 σκελετοί ανά πόρτα (3 x 3 = 9 συνολικά)

	private List<Marker2D> _doors = new();
	private List<int> _spawnedPerDoor = new();
	private Timer _timer;

	public override void _Ready()
	{
		// Βρίσκει αυτόματα τις 3 πόρτες (Marker2D) και μηδενίζει τον μετρητή της καθεμιάς
		foreach (Node child in GetChildren())
		{
			if (child is Marker2D door)
			{
				_doors.Add(door);
				_spawnedPerDoor.Add(0);
			}
		}

		_timer = GetNode<Timer>("Timer");
		_timer.Timeout += OnSpawnTimerTimeout;
	}

	private void OnSpawnTimerTimeout()
	{
		if (SkeletonScene == null || _doors.Count == 0)
			return;

		// Μαζεύει μόνο όσες πόρτες έχουν βγάλει λιγότερους από 3 σκελετούς
		List<int> availableDoorIndices = new();
		for (int i = 0; i < _doors.Count; i++)
		{
			if (_spawnedPerDoor[i] < SkeletonsPerDoor)
			{
				availableDoorIndices.Add(i);
			}
		}

		// Αν και οι 3 πόρτες έβγαλαν από 3 σκελετούς (σύνολο 9), σταματάει το Timer
		if (availableDoorIndices.Count == 0)
		{
			_timer.Stop();
			return;
		}

		// Διαλέγει τυχαία μία από τις διαθέσιμες πόρτες και αυξάνει τον μετρητή της
		int randomPick = GD.RandRange(0, availableDoorIndices.Count - 1);
		int chosenDoorIndex = availableDoorIndices[randomPick];
		_spawnedPerDoor[chosenDoorIndex]++;

		// Δημιουργεί τον σκελετό στη θέση της πόρτας
		Node2D skeleton = SkeletonScene.Instantiate<Node2D>();
		skeleton.GlobalPosition = _doors[chosenDoorIndex].GlobalPosition;

		GetParent().AddChild(skeleton);
	}
}
