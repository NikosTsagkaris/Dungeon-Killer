using Godot;
using System;

public partial class PlayButton : Button
{
	public override void _Ready() {
		
		Pressed += () => GetTree().ChangeSceneToFile("res://scene.tscn");
		
	}
}
