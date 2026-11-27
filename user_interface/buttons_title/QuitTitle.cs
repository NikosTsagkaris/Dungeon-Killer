using Godot;
using System;

public partial class QuitTitle : Button
{
	public override void _Ready()
	{
		Pressed += () => GetTree().Quit();
	}
}
